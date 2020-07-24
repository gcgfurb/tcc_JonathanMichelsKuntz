using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TelaDeAjuda : MonoBehaviour
{

    [SerializeField]
    private GameObject[] _todasTelasDeTutorial;
    private GameObject[] _telasDeTutorial;
    [SerializeField]
    private Button _btAvancar;
    [SerializeField]
    private Button _btVoltar;
    [SerializeField]
    private TextMeshProUGUI _txtPaginas;
    private GameManager _gm;

    private int _pgAtual;

    private void Awake()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _telasDeTutorial = TelasQuePodemSerMostradas();
    }

    // Use this for initialization
    void Start()
    {
        _pgAtual = 0;
        _btAvancar.onClick.AddListener(delegate { Avancar(); });
        _btVoltar.onClick.AddListener(delegate { Voltar(); });
        AlternarPgs(AttNr(_pgAtual));
        AttTexto(_pgAtual);
    }

    public void Avancar()
    {
        AlternarPgs(AttNr(_pgAtual + 1));

        AttTexto(_pgAtual);
    }

    public void Voltar()
    {

        AlternarPgs(AttNr(_pgAtual - 1));

        AttTexto(_pgAtual);
    }

    public void AttTexto(int pagina)
    {
        _txtPaginas.text = string.Format("{0} - {1}", _pgAtual + 1, _telasDeTutorial.Length);
    }

    public int AttNr(int nr)
    {
        if (nr >= 0 && nr < _telasDeTutorial.Length)
        {
            _pgAtual = nr;
        }
        return _pgAtual;
    }

    public GameObject[] TelasQuePodemSerMostradas()
    {
        List<GameObject> novaTela = new List<GameObject>();
        for (int i = 0; i < _todasTelasDeTutorial.Length; i++)
        {
            novaTela.Add(_todasTelasDeTutorial[i]);
        }
        if (!_gm.enquanto)
        {
            novaTela.Remove(_todasTelasDeTutorial[2]);
            novaTela.Remove(_todasTelasDeTutorial[3]);
        }
        if (!_gm.se)
        {
            novaTela.Remove(_todasTelasDeTutorial[4]);
        }
        return novaTela.ToArray();
    }
    public void AlternarPgs(int pagina)
    {
        for (int pg = 0; pg < _todasTelasDeTutorial.Length; pg++)
        {
            _todasTelasDeTutorial[pg].SetActive(pg == pagina);
        }
    }
}
