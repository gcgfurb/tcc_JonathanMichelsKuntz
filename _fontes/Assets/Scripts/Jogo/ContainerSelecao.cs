using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerSelecao : MonoBehaviour
{
    //GameObjects
    [SerializeField]
    private GameObject[] _regioes, _fases1, _fases2, _fases3, _fases4;
    public GameObject[] circuloFases;

    //Componentes
    [SerializeField]
    private Sprite[] _imgsRegioes;

    [SerializeField]
    private Image _imagemRegiao;

    [SerializeField]
    private Sprite[] _fundoDesfocado;

    [SerializeField]
    public Button _btnProximaRegiao, _btnVoltaRegiao, _btnVoltarMenuLogin;

    [SerializeField]
    public GameObject loja;
    public GameObject mobileButtons;

    [SerializeField]
    public Text pontuacaoLoja;

    public GameObject[] GetRegioes()
    {
        return _regioes;
    }

    public GameObject[] GetFases(int regiao)
    {
        switch (regiao)
        {
            case 0:
                return _fases1;
            case 1:
                return _fases2;
            case 2:
                return _fases3;
            case 3:
                return _fases4;
            default:
                Debug.Log("Regiao nao existente");
                return null;
        }
    }

    public Image GetImagemComponent()
    {
        return _imagemRegiao;
    }

    public Sprite GetImagemRegiao(int regiao)
    {
        GameObject.Find("Fundo Desfocado").GetComponent<Image>().sprite = _fundoDesfocado[regiao];
        return _imgsRegioes[regiao];
    }
}
