using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Objetos
    private static GameManager _instance;

    private LevelManager _levelManager;

    public UIManager uiManager;

    public List<Silaba> silabasColetadas;

    public GameObject personagemSelecionado;

    //Integers
    public int energia, contadorTesouro, vida, pontuacaoTotal, tesourosCenaAnterior, pontuacaoIntermediaria, qtdeItensParaColetar;

    //Strings
    public string tagVitoria;
    public string codigoIntermediario;

    //Boolean
    public bool gameOver, enquanto, se, senao, jaJogouIglu, buggienPassou, fezTutorial1;

    //Integer
    public int qtdeItensAtuaisInventario;

    //Listas
    public List<GameObject> coletaveisColetados = new List<GameObject>();
    public List<int> inventarioIDsBackup = new List<int>();
    public List<int> inventarioIDs = new List<int>();


    // O ID do item está relacionado a posicao no vetor  
    // NOME     |     ID
    // papiro01 |     00
    // papiro02 |     01
    // valvula  |     02
    // chave    |     03
    // en       |     04
    // quan     |     05
    // to       |     06
    // se       |     07
    // não      |     08
    // farao01  |     09
    // farao02  |     10
    // farao03  |     11
    // papiro03 |     12

    public GameObject[] todosItensJogo;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    void Start()
    {
        Configuracoes.painelComandos = true;
        _levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        DontDestroyOnLoad(this);
        gameOver = false;

        vida = 3;
        energia = 100;
        pontuacaoTotal = 0;

        if (LevelManager.devMode)
        {
            enquanto = true;
        }

        qtdeItensParaColetar = 1;
        inventarioIDs.Capacity = 8;
        qtdeItensAtuaisInventario = inventarioIDs.Count;
        buggienPassou = false;
    }

    /// <summary>
    /// Verifica se o Furbot atingiu o objetivo para que a fase finalize.
    /// </summary>
    /// <returns>Retorna true caso alcançou o objetivo e false caso não alcançou o objetivo.</returns>
    public bool ChecarSucesso(string tagObj)
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Fase 7.2":
                RemoverItemInventario(9);
                RemoverItemInventario(10);
                RemoverItemInventario(11);
                qtdeItensParaColetar = 2;
                tagVitoria = tagObj;
                return ContemItemDeVitoria();
            case "Fase 8.2":
                qtdeItensParaColetar = 1;
                tagVitoria = tagObj;
                return ContemItemDeVitoria();
            case "Fase 9.2":
                RemoverItemInventario(0);
                qtdeItensParaColetar = 1;
                tagVitoria = tagObj;
                return ContemItemDeVitoria();
            case "Fase 10.2":
                RemoverItemInventario(1);
                RemoverItemInventario(12);
                qtdeItensParaColetar = 1;
                tagVitoria = tagObj;
                return ContemItemDeVitoria();
            case "Fase 11.2":
                qtdeItensParaColetar = 1;
                tagVitoria = tagObj;
                return ContemItemDeVitoria();
            case "Fase 12.2":
                qtdeItensParaColetar = 1;
                tagVitoria = tagObj;
                return ContemItemDeVitoria();
            case "Fase 13.2":
                qtdeItensParaColetar = 1;
                tagVitoria = tagObj;
                return ContemItemDeVitoria();
            case "Fase 14.2":
                qtdeItensParaColetar = 1;
                tagVitoria = tagObj;
                return ContemItemDeVitoria();
            case "Fase 15.2":
                qtdeItensParaColetar = 1;
                tagVitoria = tagObj;
                return ContemItemDeVitoria();
            default:
                LimparInventario();
                qtdeItensParaColetar = 1;
                tagVitoria = "Vitoria";
                return ContemItemDeVitoria();
        }
    }

    /// <summary>
    /// Verifica se entre os itens coletados há um item que confirme vitória 
    /// </summary>
    /// <returns>Retorna true se confirmar vitória, caso contrário retorna false.</returns>
    public bool ContemItemDeVitoria()
    {
        bool vitoria = false;
        int qtdeItensColetados = 0;
        foreach (GameObject g in coletaveisColetados)
        {
            if (g.tag == tagVitoria)
            {
                qtdeItensColetados++;
            }
        }

        if (qtdeItensParaColetar == qtdeItensColetados)
        {
            vitoria = true;
        }
        else
        {
            vitoria = false;
        }
        return vitoria;
    }

    /// <summary>
    /// Metodo para adicionar na lista de itens coletaveis todos os coletaveis coletados em uma fase.
    /// </summary>
    /// <param name="obj">Objeto que sera adicionado na lista</param>
    public void AddItemColetavel(GameObject obj)
    {
        coletaveisColetados.Add(obj);
    }

    /// <summary>
    /// Metodo para adicionar itens no inventário do Furbot.
    /// </summary>
    /// <param name="obj">Objeto que sera adicionado ao inventario</param>
    public void AddItemInventario(int id)
    {

        if (inventarioIDs.Capacity >= qtdeItensAtuaisInventario && !VerificarItemInventario(todosItensJogo[id]))
        {
            qtdeItensAtuaisInventario++;
            inventarioIDs.Add(id);
            uiManager.AtualizarInventarioUI();
        }
        else
        {
            StartCoroutine(uiManager.Diga(Dialog_Char.S223, "Você não pode carregar isto, seu inventário está cheio ou você já possui este item!"));
        }
    }
    /// <summary>
    /// Metodo para remover um item do inventario do Furbot.
    /// </summary>
    /// <param name="obj">Objeto que sera removido do inventario</param>
    public void RemoverItemInventario(int id)
    {
        if (qtdeItensAtuaisInventario > 0)
        {
            qtdeItensAtuaisInventario--;
            inventarioIDs.Remove(id);
            uiManager.AtualizarInventarioUI();
        }
    }

    /// <summary>
    /// Metodo mapa remover todos os itens que estao no inventario.
    /// </summary>
    /// <param name="limparTotalmente">Booleano para saber se a limpeza de listas deve ser feita nas listas de backup</param>
    public void LimparInventario()
    {
        inventarioIDs.Clear();
        inventarioIDs = inventarioIDsBackup;
        qtdeItensAtuaisInventario = inventarioIDsBackup.Count;
        uiManager.AtualizarInventarioUI();
    }

    /// <summary>
    /// Verifica se o Furbot já possui no inventário o item que ele acabou de coletar
    /// </summary>
    /// <param name="objAtualColetado">Objeto que acabou de coletar.</param>
    /// <returns></returns>
    public bool VerificarItemInventario(GameObject objAtualColetado)
    {
        foreach (int idObjeto in inventarioIDs)
        {
            if (todosItensJogo[idObjeto].tag.Equals(objAtualColetado.tag))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Método que ativa todos os itens coletados na execução de código anterior.
    /// </summary>
    public void ReativarItensColetados()
    {
        foreach (GameObject g in coletaveisColetados)
        {
            if (g != null)
            {
                g.SetActive(true);
            }
        }
        coletaveisColetados.Clear();
    }

    public void ReiniciarContadores()
    {
        contadorTesouro = 0;
        codigoIntermediario = "";
        this.coletaveisColetados.Clear();
    }

    public void Voltar()
    {
        SceneManager.LoadScene("MenuPrincipalCadastro");
    }
}
