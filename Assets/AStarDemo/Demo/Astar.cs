using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour {

    public GameObject prefab;
    public Transform empty;

    private Pos startPos;
    private Pos endPos;
    private Pos tmpPos;
    private Vector3[] neighbor;

    Dictionary<Vector3, Pos> mapDic = new Dictionary<Vector3, Pos>();
    Dictionary<Vector3, Pos> blockDic = new Dictionary<Vector3, Pos>();

    List<Pos> openList = new List<Pos>();
    List<Pos> closeList = new List<Pos>();

	void Start () {
        initMap();

        neighbor = new Vector3[8]
        {
            new Vector3(-1,0,0), new Vector3(0,0,-1), new Vector3(0,0,1), new Vector3(1,0,0),
            new Vector3(-1,0,-1), new Vector3(-1,0,1), new Vector3(1,0,-1), new Vector3(1,0,1)
        };

        startPos.h = h_Distance(startPos, endPos);
        startPos.g = 0;
        startPos.f = startPos.g + startPos.h;
        openList.Add(startPos);

        LookForPath();
    }

    void initMap()
    {
        for (int i = -5; i <= 5; i++)
        {
            GameObject g = Instantiate(prefab, new Vector3(i, 0, -5), Quaternion.identity);
            Pos p = new Pos(g.transform, g.transform.position);
            mapDic.Add(p.pos, p);
            blockDic.Add(p.pos, p);

            g = Instantiate(prefab, new Vector3(i, 0, 5), Quaternion.identity);
            p = new Pos(g.transform, g.transform.position);
            mapDic.Add(p.pos, p);
            blockDic.Add(p.pos, p);
        }

        for (int i = -4; i <= 4; i++)
        {
            GameObject g = Instantiate(prefab, new Vector3(-5, 0, i), Quaternion.identity);
            Pos p = new Pos(g.transform, g.transform.position);
            mapDic.Add(p.pos, p);
            blockDic.Add(p.pos, p);

            g = Instantiate(prefab, new Vector3(5, 0, i), Quaternion.identity);
            p = new Pos(g.transform, g.transform.position);
            mapDic.Add(p.pos, p);
            blockDic.Add(p.pos, p);
        }
        
        for (int i = -4; i < 5; i++)
        {
            for (int j = -4; j < 5; j++)
            {
                GameObject g = Instantiate(prefab, new Vector3(i, 0, j), Quaternion.identity);
                Pos p = new Pos(g.transform, g.transform.position);
                mapDic.Add(p.pos, p);
                if (i == 0 && j > -3 && j < 3)
                {
                    blockDic.Add(p.pos, p);
                }

                if (g.transform.position == new Vector3(-2, 0, 0))
                {
                    g.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    startPos = p;
                }

                if (g.transform.position == new Vector3(2, 0, 0))
                {
                    g.GetComponent<MeshRenderer>().material.color = Color.red;
                    endPos = p;
                }
            }
        }

        foreach (var item in blockDic)
        {
            item.Value.trans.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }

    private float neighbor_Dis(int i)
    {
        if (i > 4) return 1.4f;
        return 1;
    }

    private float h_Distance(Pos p1, Pos p2)
    {
        return Mathf.Abs(p2.pos.z - p1.pos.z) + Mathf.Abs(p2.pos.x - p1.pos.x);
    }

    private int Comp(Pos p1,Pos p2)
    {
        return p1.f.CompareTo(p2.f);
    }

    private void LookForPath()
    {
        Pos curPos;
        while (openList.Count != 0)
        {
            curPos = openList[0];
            openList.RemoveAt(0);
            if (curPos == endPos)
            {
                Pos p = curPos.parent;
                while (p != startPos)
                {
                    p.trans.GetComponent<MeshRenderer>().material.color = Color.blue;
                    p = p.parent;
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector3 tmpV3 = curPos.pos + neighbor[i];
                    if (blockDic.ContainsKey(tmpV3)) continue;

                    Pos tmpPos = mapDic[tmpV3];
                    float g = curPos.g + neighbor_Dis(i);
                    if ((openList.Contains(tmpPos) || closeList.Contains(tmpPos)) && tmpPos.g <= g)
                    {
                        continue;
                    }
                    else
                    {
                        tmpPos.parent = curPos;
                        tmpPos.g = g;
                        tmpPos.h = h_Distance(tmpPos, endPos);
                        tmpPos.f = tmpPos.g + tmpPos.h;

                        if (closeList.Contains(tmpPos)) closeList.Remove(tmpPos);

                        if (!openList.Contains(tmpPos)) openList.Add(tmpPos);

                        openList.Sort(Comp);
                    }
                }
            }
            closeList.Add(curPos);
        }
    }
}
