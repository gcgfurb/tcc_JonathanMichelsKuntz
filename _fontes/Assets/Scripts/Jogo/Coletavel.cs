using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coletavel : MonoBehaviour
{
    //Booleans (SERIALIZED)
    [SerializeField]
    private bool _ehReutilizavel;

    public bool GetEhReutilizavel()
    {
        return this._ehReutilizavel;
    }
}
