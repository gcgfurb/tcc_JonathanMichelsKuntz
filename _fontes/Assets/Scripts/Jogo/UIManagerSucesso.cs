using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerSucesso : MonoBehaviour
{
    //Objetos
    public Button btnReiniciar, btnSair, btnProximo;
    public Text pontosText;
    private Furbot _furbot;
    private LevelManager _lm;

    public GameObject salvandoJogoDisplay, salvouJogoDisplay;

    void Start()
    {
        btnReiniciar.onClick.AddListener(delegate { Reiniciar(); });
        btnSair.onClick.AddListener(delegate { Sair(); });
        btnProximo.onClick.AddListener(delegate { ProximaFase(); });
        _lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        _furbot = GameObject.Find("Furbot").GetComponent<Furbot>();
    }

    /// <summary>
    /// Este método reinicia a fase atual do zero.
    /// </summary>
    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _furbot.Reiniciar();
    }

    /// <summary>
    /// Este método retorna para a tela de selecionar fase.
    /// </summary>
    public void Sair()
    {
        SceneManager.LoadScene("SelecaoDeFases");
        _furbot.Reiniciar();
    }

    /// <summary>
    /// Este método avança para a fase seguinte da história do jogo.
    /// </summary>
    public void ProximaFase()
    {
        LevelManager.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        LevelManager.faseAtual++;
        GameObject.Find("GameManager").GetComponent<GameManager>().ReiniciarContadores();
        PontuacaoController.pontosFase = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        _furbot.Reiniciar();
        if (LevelManager.ultimaFaseLiberada % 5 == 0)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().energia = 100;
        }
    }

    public void SalvouJogo()
    {
        salvandoJogoDisplay.SetActive(false);
        salvouJogoDisplay.SetActive(true);
        btnProximo.interactable = true;
        btnReiniciar.interactable = true;
        btnSair.interactable = true;
    }

    /// <summary>
    /// Este método configura a variável que mostra a pontuação após o término de uma fase.
    /// </summary>
    /// <param name="pontos"> Pontuação atual do jogo. </param>
    public void SetPontos(int pontos)
    {
        Debug.Log("Set PONTOS");
        pontosText.text = pontos + "";
    }

}
