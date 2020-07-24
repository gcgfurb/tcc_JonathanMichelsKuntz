using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocarCenaFase : MonoBehaviour
{
    public GameObject trocarCenaObjeto;
    public string nomeMiniGame;

    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = GameObject.Find("CanvasInterface").GetComponent<UIManager>();
        nomeMiniGame = "MiniGame_Pesca";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (other.tag == "Player")
        {
            Furbot furbot = GameObject.Find("Furbot").GetComponent<Furbot>();

            gm.vida = furbot.vidas;
            gm.energia = furbot.energia;
            gm.contadorTesouro = furbot.tesouros;
            //gm.codigoIntermediario = GameObject.Find("CanvasInterface").GetComponent<UIManager>().ifCodigo.text;
            gm.codigoIntermediario = other.GetComponent<Furbot>().GetTextoGabarito();
            furbot.GetComponent<Analisador>().SetCodigoExtenso(gm.codigoIntermediario);
            gm.tesourosCenaAnterior = PontuacaoController.GetTotalTesouros();
            gm.pontuacaoIntermediaria = PontuacaoController.pontosFase;
            LevelManager.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log(gm.codigoIntermediario);
            switch (SceneManager.GetActiveScene().name)
            {
                case "Fase 6":
                    SceneManager.LoadScene("Fase 6.2");
                    break;
                case "Fase 7":
                    SceneManager.LoadScene("Fase 7.2");
                    break;
                case "Fase 8":
                    SceneManager.LoadScene("Fase 8.2");
                    break;
                case "Fase 9":
                    SceneManager.LoadScene("Fase 9.2");
                    break;
                case "Fase 10":
                    SceneManager.LoadScene("Fase 10.2");
                    break;
                case "Fase 11":
                    SceneManager.LoadScene(nomeMiniGame);
                    break;
                case "Fase 12":
                    SceneManager.LoadScene(nomeMiniGame);
                    break;
                case "Fase 13":
                    SceneManager.LoadScene(nomeMiniGame);
                    break;
                case "Fase 14":
                    SceneManager.LoadScene(nomeMiniGame);
                    break;
                case "Fase 15":
                    SceneManager.LoadScene(nomeMiniGame);
                    break;
                case "Fase 16":
                    SceneManager.LoadScene("Fase 16.2");
                    break;
                case "Fase 16.2":
                    SceneManager.LoadScene("Fase 16.3");
                    break;
                case "Fase 16.3":
                    _uiManager.MostrarSucesso();
                    break;
                case "Fase 17":
                    SceneManager.LoadScene("Fase 17.2");
                    break;
                case "Fase 17.2":
                    SceneManager.LoadScene("Fase 17.3");
                    break;
                case "Fase 17.3":
                    _uiManager.MostrarSucesso();
                    break;
                case "Fase 18":
                    SceneManager.LoadScene("Fase 18.2");
                    break;
                case "Fase 18.2":
                    SceneManager.LoadScene("Fase 18.3");
                    break;
                case "Fase 18.3":
                    _uiManager.MostrarSucesso();
                    break;
                case "Fase 19":
                    SceneManager.LoadScene("Fase 19.2");
                    break;
                case "Fase 19.2":
                    SceneManager.LoadScene("Fase 19.3");
                    break;
                case "Fase 19.3":
                    _uiManager.MostrarSucesso();
                    break;
                case "Fase 20":
                    SceneManager.LoadScene("Fase 20.2");
                    break;
                case "Fase 20.2":
                    SceneManager.LoadScene("Fase 20.3");
                    break;
                case "Fase 20.3":
                    SceneManager.LoadScene("Fase 20.4");
                    break;
                case "Fase 20.4":
                    SceneManager.LoadScene("MiniGame_CaboDeGuerra");
                    break;
            }
        }
        else if (other.tag == "Buggien")
        {
            gm.buggienPassou = true;
            other.gameObject.SetActive(false);
        }
    }
}
