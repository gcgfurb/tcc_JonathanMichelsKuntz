using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipalOffline : MonoBehaviour
{

    //Objetos
    public Button btnIniciar, btnMenu, btnSair, gerador;

    void Start()
    {
#if !UNITY_WEBGL
        btnSair.onClick.AddListener(delegate { Sair(); });
#else
        btnSair.gameObject.SetActive(false);
#endif
        btnIniciar.onClick.AddListener(delegate { Iniciar(); });

        btnMenu.onClick.AddListener(delegate { Menu(); });

        gerador.onClick.AddListener(delegate { SceneManager.LoadScene("GeradorDeFases"); });
    }

    /// <summary>
    /// Este método carrega a cena de seleção de fases.
    /// </summary>
    private void Iniciar()
    {
        //#if UNITY_STANDALONE
        //SceneManager.LoadScene("Historia");
        //#else 
        string cenaCarrega = (LevelManager.isInterfaceTangivel ? "SelecaoDeFases" : "TutorialProjecao");
        SceneManager.LoadScene(cenaCarrega);
        //#endif
    }

    /// <summary>
    /// Este método carrega a cena do menu.
    /// </summary>
    private void Menu()
    { 
        SceneManager.LoadScene("Configuracoes");
    }

    /// <summary>
    /// Este método finaliza a execução do jogo.
    /// </summary>
    private void Sair()
    {
        Application.Quit();
    }
}
