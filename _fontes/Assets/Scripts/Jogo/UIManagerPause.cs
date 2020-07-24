using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerPause : MonoBehaviour
{
    //Objetos
    public Button btnReiniciar;
    public Button btnSair;
    public Button btnContinuar;
    public Button _btnSim;
    public Button _btnNao;
    public Button _btnNN;
    public Button _btnSS;
    public GameObject pauseImage, _painelPausaReiniciar, _painelPausaSair;
    private Furbot _furbot;
    private LevelManager _lm;

    void Start()
    {
        _lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        btnReiniciar.onClick.AddListener(delegate { Reiniciar(); });
        btnSair.onClick.AddListener(delegate { Sair(); });
        btnContinuar.onClick.AddListener(delegate { ContinuarFase(); });
        _furbot = GameObject.Find("Furbot").GetComponent<Furbot>();
    }

    /// <summary>
    /// Este método reinicia a fase atual do zero.
    /// </summary>
    public void Reiniciar()
    {
        _painelPausaReiniciar.SetActive(true);
        _btnSim.onClick.AddListener(delegate
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            _furbot.Reiniciar();
        });
        _btnNao.onClick.AddListener(delegate
        {
            _painelPausaReiniciar.SetActive(false);
           
        });

        Debug.Log("TesteReiniciar");
        
    }

    /// <summary>
    /// Este método retorna para a tela de selecionar fase.
    /// </summary>
    public void Sair()
    {
        //_lm.Reinstanciar();
        _painelPausaSair.SetActive(true);
        _btnSS.onClick.AddListener(delegate
        {
            _furbot.Reiniciar();

            SceneManager.LoadScene("SelecaoDeFases");
        });
        _btnNN.onClick.AddListener(delegate
        {
            _painelPausaSair.SetActive(false);
          
        });
    }

    /// <summary>
    /// Este método retorna ao último momento do jogo antes de tê-lo pausado.
    /// </summary>
    public void ContinuarFase()
    {
        pauseImage.SetActive(false);
        
    }

}
