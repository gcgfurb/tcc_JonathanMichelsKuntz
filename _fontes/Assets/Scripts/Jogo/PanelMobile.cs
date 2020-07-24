using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMobile : MonoBehaviour
{
    private UIManager _uiManager;
    private GameManager _gameManager;

    [SerializeField]
    private Button _btAndar, _btEnquanto, _btAcima, _btEsquerda, _btDireita, _btAbaixo,
        _btPontoVirgula, _btApagarUltimoComando, _btEh, _btCaminho, _btAbreChaves, _btFechaChaves, _btApagarTodosComandos, _btEditarComandos, _btNot, _btPuxarAlavanca;

    [SerializeField]
    private GameObject _panelAtual, _panelComandosBase, _panelPontoVirgula, _panelFechaChaves, _panelAbreChaves, _panelDirecoes, _panelEnquanto;

    private bool _isIf = false;
    private bool _not = false;
    private bool _enquanto;

    [SerializeField]
    private Scrollbar _scrollBar;

    void Start()
    {
        _uiManager = GameObject.Find("CanvasInterface").GetComponent<UIManager>();
        AdicionarListeners();
        _panelAtual = _panelComandosBase;
    }

    private void AdicionarListeners()
    {
        //COMANDOS BÁSICOS
        _btAndar.onClick.AddListener(() =>
        {
            string str = "andar(";
            AddComandosALista(str);
            _panelComandosBase.SetActive(false);
            _panelDirecoes.SetActive(true);
            _panelAtual = _panelDirecoes;
            InserirComando();
        });

        _btEnquanto.onClick.AddListener(() =>
        {

            string str = "enquanto(";
            AddComandosALista(str);
            _enquanto = true;
            InserirComando();
            _panelComandosBase.SetActive(false);
            _panelEnquanto.SetActive(true);
            _panelAtual = _panelEnquanto;
            _btEh.gameObject.SetActive(true);
        });

        if (LevelManager.faseAtual == 9)
        {
            _btPuxarAlavanca.gameObject.SetActive(true);
            _btPuxarAlavanca.onClick.AddListener(() =>
            {
                string str = "puxarAlavanca()";
                AddComandosALista(str);
                InserirComando();
                _panelAtual.SetActive(false);
                _panelPontoVirgula.SetActive(true);
                _panelAtual = _panelPontoVirgula;
            });
        }

        //DIREÇÕES
        _btAcima.onClick.AddListener(() =>
        {
            string str = (_isIf || _enquanto ? "acima))" : "acima)");

            AddComandosALista(str);
            InserirComando();
            VerificarProximoPanelDirecoes();
        });
        _btAbaixo.onClick.AddListener(() =>
        {
            string str = (_isIf || _enquanto ? "abaixo))" : "abaixo)");
            AddComandosALista(str);
            InserirComando();
            VerificarProximoPanelDirecoes();
        });
        _btDireita.onClick.AddListener(() =>
        {
            string str = (_isIf || _enquanto ? "direita))" : "direita)");
            AddComandosALista(str);
            InserirComando();
            VerificarProximoPanelDirecoes();
        });
        _btEsquerda.onClick.AddListener(() =>
        {
            string str = (_isIf || _enquanto ? "esquerda))" : "esquerda)");
            AddComandosALista(str);
            InserirComando();
            VerificarProximoPanelDirecoes();
        });

        //OPERADORES E TOKENS
        _btPontoVirgula.onClick.AddListener(() =>
        {
            string str = ";\n";
            AddComandosALista(str);
            InserirComando();
            _panelPontoVirgula.SetActive(false);
            _panelComandosBase.SetActive(true);
            _panelAtual = _panelComandosBase;
            StartCoroutine(_uiManager.ScrollarComandos());
        });

        _btEh.onClick.AddListener(() =>
        {
            string str = "eh(";
            AddComandosALista(str);
            InserirComando();
            _btEh.gameObject.SetActive(false);
            _btCaminho.gameObject.SetActive(true);
            _btNot.gameObject.SetActive(false);
        });

        _btNot.onClick.AddListener(() =>
        {
            _not = true;
            string str = "!";
            AddComandosALista(str);
            InserirComando();
            _btNot.gameObject.SetActive(false);
        });

        _btCaminho.onClick.AddListener(() =>
        {
            string str = "caminho,";
            AddComandosALista(str);
            InserirComando();
            _btCaminho.gameObject.SetActive(false);
            _panelEnquanto.SetActive(false);
            _panelDirecoes.SetActive(true);
            _btNot.gameObject.SetActive(true);
            _panelAtual = _panelDirecoes;
        });

        _btFechaChaves.onClick.AddListener(() =>
        {
            string str = "}\n";
            AddComandosALista(str);
            InserirComando();
            _btFechaChaves.gameObject.SetActive(false);
        });

        _btAbreChaves.onClick.AddListener(() =>
        {
            string str = "{\n";
            AddComandosALista(str);
            InserirComando();
            _panelAbreChaves.SetActive(false);
            _panelComandosBase.SetActive(true);
            _panelAtual = _panelComandosBase;
            _btFechaChaves.gameObject.SetActive(true);
            _enquanto = false;
        });


        //APAGAR
        _btApagarUltimoComando.onClick.AddListener(ApagarUltimoComando);
        _btApagarTodosComandos.onClick.AddListener(_uiManager.ConfirmaApagarTodosComandos);

        _btEditarComandos.onClick.AddListener(_uiManager.EditarComandos);

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _btEnquanto.gameObject.SetActive(_gameManager.enquanto);
    }

    private void ApagarUltimoComando()
    {
        if (_uiManager.listaAtualComandosStr.Count > 0)
        {
            VerificarComandoApagado((string)_uiManager.listaAtualComandosStr[_uiManager.listaAtualComandosStr.Count - 1]);

            _uiManager.listaAtualComandosStr.RemoveAt(_uiManager.listaAtualComandosStr.Count - 1);

            if (_uiManager.listaAtualComandosStr.Count == 0)
            {
                _uiManager.ifCodigo.text = "";

                _uiManager.listaAtualComandosStr = new ArrayList();
            }
            else
            {
                InserirComando();
            }
        }
    }

    public void VoltarPainelBase()
    {
        _panelAtual.SetActive(false);
        _panelComandosBase.SetActive(true);
        _panelAtual = _panelComandosBase;
    }

    private void InserirComando()
    {
        _uiManager.ifCodigo.text = "";
        for (int i = 0; i < _uiManager.listaAtualComandosStr.Count; i++)
            _uiManager.ifCodigo.text += _uiManager.listaAtualComandosStr[i];
    }

    private void VerificarProximoPanelDirecoes()
    {
        _panelDirecoes.SetActive(false);
        if (_enquanto)
        {
            _panelAbreChaves.SetActive(true);
            _panelAtual = _panelAbreChaves;
        }
        else
        {
            _panelPontoVirgula.SetActive(true);
            _panelAtual = _panelPontoVirgula;
        }
    }

    private void AddComandosALista(string str)
    {
        _uiManager.listaAtualComandosStr.Add(str);
    }

    private void VerificarComandoApagado(string comando)
    {
        if (comando.Equals("andar(")) { _panelAtual.SetActive(false); _panelComandosBase.SetActive(true); _panelAtual = _panelComandosBase; }
        else if (ehDirecao(comando)) { _panelAtual.SetActive(false); _panelDirecoes.SetActive(true); _panelAtual = _panelDirecoes; }
        else if (comando.Equals("enquanto(")) { _enquanto = false; _panelAtual.SetActive(false); _panelComandosBase.SetActive(true); _panelAtual = _panelComandosBase; }
        else if (comando.Equals("eh(")) { AtivarPainelEnquanto(true); if (!_not) { _btNot.gameObject.SetActive(true); } }
        else if (comando.Equals("caminho,")) { AtivarPainelEnquanto(false); _btNot.gameObject.SetActive(false); }
        else if (comando.Equals(";\n")) { _panelAtual.SetActive(false); _panelPontoVirgula.SetActive(true); _panelAtual = _panelPontoVirgula; }
        else if (comando.Equals("{\n")) { _panelAtual.SetActive(false); _panelAbreChaves.SetActive(true); _panelAtual = _panelAbreChaves; }
        else if (comando.Equals("}\n")) { _panelAtual.SetActive(false); _panelComandosBase.SetActive(true); _btFechaChaves.gameObject.SetActive(true); _panelAtual = _panelComandosBase; }
        else if (comando.Equals("!")) { _panelAtual.SetActive(false); _panelEnquanto.SetActive(true); _panelAtual = _panelEnquanto; _btNot.gameObject.SetActive(true); _not = false; }
        else if (comando.Equals("puxarAlavanca()")) { _panelAtual.SetActive(false); _panelComandosBase.SetActive(true); _panelAtual = _panelComandosBase; }
    }

    private void AtivarPainelEnquanto(bool btEh)
    {
        if (!_panelEnquanto.activeInHierarchy) { _panelAtual.SetActive(false); _panelEnquanto.SetActive(true); _panelAtual = _panelEnquanto; }
        _btEh.gameObject.SetActive(btEh);
        _btCaminho.gameObject.SetActive(!btEh);
    }

    private bool ehDirecao(string comando)
    {
        if (comando.Contains("esquerda") || comando.Contains("direita") || comando.Contains("acima") || comando.Contains("abaixo"))
        {
            if (comando.Contains("))"))
            {
                _enquanto = true;
            }
            return true;
        }
        return false;
    }
}