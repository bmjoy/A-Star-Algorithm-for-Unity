using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pos{
    public Transform trans;
    public Vector3 pos;
    public Pos parent = null;

    public float g = 10000;
    public float h = 10000;

    public float f = 10000;

    public Pos(Transform t, Vector3 v)
    {
        trans = t;
        pos = v;
    }
}
