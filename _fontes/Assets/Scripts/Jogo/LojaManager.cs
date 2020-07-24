using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LojaManager : MonoBehaviour
{
    private GameManager _gm;

    [SerializeField]
    private Text pontosLoja;

    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pontosLoja.text = _gm.pontuacaoTotal.ToString();
    }

    public void AtualizarPontos()
    {
        pontosLoja.text = _gm.pontuacaoTotal.ToString();
    }
}
