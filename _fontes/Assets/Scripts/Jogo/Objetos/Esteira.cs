using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Esteira : MonoBehaviour {

    [SerializeField]
    private bool isInverted;

    [SerializeField]
    private Sprite[] sprites;


    [SerializeField]
    private Direcao direcao;

	void Start () {
        SetDirecao(direcao);
	}

    public void SetEstado(bool estado)
    {
        isInverted = estado;
    }

    public bool GetEstado()
    {
        return isInverted;
    }

    public void SetDirecao(Direcao direcao)
    {
        switch (direcao)
        {
            case Direcao.DIREITA:
                this.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case Direcao.ESQUERDA:
                this.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case Direcao.ACIMA:
                this.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case Direcao.ABAIXO:
                this.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
        }
    }

    public Direcao InverterDirecao(bool isInverted)
    {
        this.isInverted = isInverted;
        if (isInverted)
        {
            switch (direcao)
            {
                case Direcao.DIREITA:
                    SetDirecao(Direcao.ESQUERDA);
                    return Direcao.ESQUERDA;
                case Direcao.ESQUERDA:
                    SetDirecao(Direcao.DIREITA);
                    return Direcao.DIREITA;
                case Direcao.ACIMA:
                    SetDirecao(Direcao.ABAIXO);
                    return Direcao.ABAIXO;
                case Direcao.ABAIXO:
                    SetDirecao(Direcao.ACIMA);
                    return Direcao.ACIMA;
                default: return direcao;
            }
        }
        else
        {
            SetDirecao(direcao);
            return direcao;
        }
        
    }

    public Direcao GetDirecao()
    {
        return InverterDirecao(isInverted);
    }
}
