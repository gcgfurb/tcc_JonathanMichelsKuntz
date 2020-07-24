using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class Silaba
{
    public string silaba;

    public Silaba(string silaba)
    {
        this.silaba = silaba;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Silaba silaba = obj as Silaba;
        if (silaba == null) return false;
        else return Equals(silaba);
    }

    public bool Equals(Silaba outra)
    {
        if (outra.silaba == this.silaba)
        {
            return true;
        }
        return false;
    }
}