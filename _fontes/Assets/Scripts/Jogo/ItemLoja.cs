using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLoja : MonoBehaviour
{
    public Text nome;
    public int preco;

    private GameManager _gameManager;
    [SerializeField]
    private LojaManager _lm;

    // Use this for initialization
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void ComprarItem()
    {
        if (_gameManager.pontuacaoTotal >= preco)
        {
            switch (nome.text)
            {
                case "Energia":

                    if (_gameManager.energia < 100)
                    {
                        if (_gameManager.energia < 76)
                        {
                            _gameManager.energia += 25;
                        }
                        else
                        {
                            _gameManager.energia = 100;
                        }
                        break;
                    } else
                    {
                        return;
                    }
                case "Vida":
                    if (_gameManager.vida < 3)
                    {
                        _gameManager.vida++;
                    }
                    else
                    {
                        return;
                    }
                    break;
            }
            _gameManager.pontuacaoTotal -= preco;
            _lm.AtualizarPontos();
            Debug.Log("Item comprado com sucesso.");
        }
        else
        {
            Debug.Log("Pontos insuficientes.");
        }
    }
}
