using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileException : Exception {

    Vector3 local;

    public TileException(Vector3 local)
    {
        this.local = local;
    }
    public Vector3 GetVector()
    {
        return local;
    }

}
