using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PontoDeFoco : MonoBehaviour
{
    //Floats
    [SerializeField]
    private float _tempoDeLeitura;

    public float GetTempoDeLeitura()
    {
        if (this._tempoDeLeitura != 0)
        {
            return this._tempoDeLeitura;
        }
        Debug.Log("Tempo de leitura para o ponto de foco " + this.gameObject.name + " nao foi definido! um valor padrao foi atribuido.");
        return 4.0f;
    }
}
