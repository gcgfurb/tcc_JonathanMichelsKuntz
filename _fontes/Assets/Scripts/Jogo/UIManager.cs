using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Objetos
    public Sprite[] vidas, baterias;
    public Image[] inventarioUI;
    public Image vidasImage, bateriaImage, dicaPuzzle, buggienImage, _backgroundImage;
    public Text pontosText, qntEnergiaText, qntTesourosText, tesouroFeedback, pontuacaoTotal, contadorTesouros;
    public TMP_InputField ifCodigo, _ifNumeros;
    public Toggle btnRecomecar, togglePainelComandos;
    public Button btnExecutar, btnPause, btnPainelSim, btnEnquantoMobile, btnConfirmarExclusao;
    public GameObject debug, gameOverImage, sucessoImage, pauseImage, dialog, panelBotoes, panelConfirmarExclusao, panelDica;
    public TextMeshProUGUI textoDebug, programarPlaceholder;
    public Dialog dialogPanel;
    public Sprite imagemVazia, dicaPuzzleAlavanca, dicaPuzzlePlacasPressao, btnExecutarPlay, btnExecutarPause;
    public RectTransform espacoLateral;

    public Furbot furbot;
    public UIManagerSucesso uiManagerSucesso;
    public GameManager gm;
    public Coroutine rotinaExecutar;
    [SerializeField]
    public Animator caixaDeTextoAnim, panelComandosAnim;
    [SerializeField]
    public TextMeshProUGUI textoFase;
    [SerializeField]
    public Text tituloLateral; // Novo
    [SerializeField]
    private PanelMobile _panelMobileScript;
    [SerializeField]
    public InterfaceTangivel _interfaceTangivel; // Novo

    private Analisador _analisador;

    public ArrayList listaAtualComandosStr = new ArrayList();
    public ArrayList ultimosComandosStr = new ArrayList();
    private bool isIf = false;
    
    //Integers
    public int tesouros;
    public float valorScrollBar;

    //Strings
    public string _textoLog, _textoDigitado;

    private bool _editandoCodigo;

    //Booleans
    public bool emDialogo, comandosAberto, permitidoDigitarComandos;

    private char[] _separadores = { ';', '{', '}' };

    private void Start()
    {
        //Novo -
        //Configura para aparecer opção de visualizar webcam
        _interfaceTangivel.gameObject.SetActive(LevelManager.isInterfaceTangivel);
        espacoLateral.gameObject.SetActive(!LevelManager.isInterfaceTangivel);
        if (LevelManager.isInterfaceTangivel)
        {
            tituloLateral.text = "Peças";
        }
        else
        {
            tituloLateral.text = "Código";
        }
        ////////////
        
        BuscarGameManager();
        btnPause.onClick.AddListener(delegate { MostrarPause(); });
        btnExecutar.onClick.AddListener(delegate
        {
            if (ifCodigo.text != "")
                ExecutarCodigo();
            if (Configuracoes.painelComandos)
                if (comandosAberto)
                    TogglePanelComandos();
        });

        BuscarFurbot();
        _analisador = furbot.GetComponent<Analisador>();
        ifCodigo.onValidateInput = ValidarCodigoFurbot;
        uiManagerSucesso = sucessoImage.GetComponent<UIManagerSucesso>();
        StartCoroutine(IniciarDialogo(2.0f));
        textoFase.text = SceneManager.GetActiveScene().name;
        AtualizarInventarioUI();
        dialogPanel.ChecarSeHaDialogo(); // Quando trocada a forma de usar chamar esse cara
        permitidoDigitarComandos = SceneManager.GetActiveScene().name.Contains(".") || SceneManager.GetActiveScene().name.Equals("FaseGerada") || !dialogPanel.haDialogoNessaFase;

        togglePainelComandos.onValueChanged.AddListener(delegate
        {
            Configuracoes.painelComandos = !Configuracoes.painelComandos;
            if (Configuracoes.painelComandos == true)
            {
                AtivarPainelComandos();
            }
            else
            {
                DesativarPainelComandos();
            }
        });

        // Quando trocada a forma de usar chamar esse cara
        if (Configuracoes.painelComandos)
        {
            AtivarPainelComandos();
        }
        else
        {
            DesativarPainelComandos();
            togglePainelComandos.SetIsOnWithoutNotify(false);
        }


    }

    public void ChecarEventoAposDialogo()
    {
        //Caso deseje fazer algo após o dialogo da fase se encerrar, faça aqui.
    }

    private void BuscarFurbot()
    {
        try
        {
            furbot = GameObject.Find("Furbot").GetComponent<Furbot>();
        }
        catch (System.NullReferenceException n) { furbot = GameObject.Find("Furbot").GetComponent<Furbot>(); }
    }

    private void BuscarGameManager()
    {
        if (!SceneManager.GetActiveScene().name.Equals("FaseGerada"))
        {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            gm.uiManager = GetComponent<UIManager>();
            if (!gm.enquanto && btnEnquantoMobile != null)
            {
                btnEnquantoMobile.interactable = false;
            }
        }
        else
        {
            gm = null;
        }
    }

    private void DesativarPainelComandos()
    {
        Configuracoes.painelComandos = false;
        StartCoroutine(ResetarCaixaDeTexto());
        ifCodigo.readOnly = false;
        panelBotoes.SetActive(false);
        programarPlaceholder.text = "Digite para programar no furbot...";
        btnConfirmarExclusao.onClick.RemoveAllListeners();
        ifCodigo.onEndEdit.RemoveAllListeners();
    }

    private IEnumerator ResetarCaixaDeTexto()
    {
        if (comandosAberto)
        {
            caixaDeTextoAnim.SetTrigger("Expandir");
            yield return new WaitForSeconds(0.3f);
        }
        _panelMobileScript.gameObject.SetActive(false);
        btnExecutar.gameObject.SetActive(true);
    }

    private void AtivarPainelComandos()
    {
        Configuracoes.painelComandos = true;
        programarPlaceholder.text = "Toque aqui para programar...";
        ifCodigo.readOnly = true;
        btnConfirmarExclusao.onClick.AddListener(ApagarTodosOsComandos);
        panelBotoes.SetActive(true);
        _panelMobileScript.gameObject.SetActive(true);
        //navDrawerSetup();

        ifCodigo.onEndEdit.AddListener(delegate
        {
            ifCodigo.readOnly = true;
            if (_editandoCodigo)
            {
                RefazerListaComandos();
            }
        });
    }

    public void TogglePanelComandos()
    {
        if (permitidoDigitarComandos && Configuracoes.painelComandos)
        {
            if (!comandosAberto)
            {
                if (!furbot.bateu && ifCodigo.interactable)
                {
                    comandosAberto = true;
                    caixaDeTextoAnim.SetTrigger("Fechar");
                    panelComandosAnim.SetTrigger("Abrir");
                    panelBotoes.GetComponent<Animator>().SetTrigger("Abrir");
                    btnExecutar.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!furbot.bateu && ifCodigo.interactable)
                {
                    comandosAberto = false;
                    caixaDeTextoAnim.SetTrigger("Expandir");
                    panelComandosAnim.SetTrigger("Fechar");
                    panelBotoes.GetComponent<Animator>().SetTrigger("Fechar");
                    btnExecutar.gameObject.SetActive(true);
                }
            }
        }
    }

    public IEnumerator ScrollarComandos()
    {
        yield return new WaitForSeconds(0.05f);
        ifCodigo.verticalScrollbar.SetValueWithoutNotify(1);
    }

    private void ApagarTodosOsComandos()
    {
        panelConfirmarExclusao.SetActive(false);
        isIf = false;
        ifCodigo.text = "";
        ifCodigo.readOnly = true;
        ultimosComandosStr = new ArrayList();
        //ultimosComandos = new ArrayList();
        _textoDigitado = "";

        //if (!_usandoNavDrawer)
        _panelMobileScript.VoltarPainelBase();
        /*
        else
        {
            TodosBotoesAtivados(true);
        }
        */
    }

    private void RefazerListaComandos()
    {
        ultimosComandosStr = new ArrayList();
        string[] textoDividido = ifCodigo.text.Split('\n');
        foreach (string cmd in textoDividido)
        {
            if (cmd.Contains("enquanto("))
            {
                ultimosComandosStr.Add("enquanto(");
                if (cmd.Contains("!"))
                {
                    ultimosComandosStr.Add("!");
                }
                if (cmd.Contains("eh("))
                {
                    ultimosComandosStr.Add("eh(");
                    if (cmd.Contains("caminho,"))
                    {
                        ultimosComandosStr.Add("caminho,");
                        if (cmd.Contains("direita))"))
                        {
                            ultimosComandosStr.Add("direita))");
                        }
                        else if (cmd.Contains("esquerda))"))
                        {
                            ultimosComandosStr.Add("esquerda))");
                        }
                        else if (cmd.Contains("acima))"))
                        {
                            ultimosComandosStr.Add("acima))");
                        }
                        else if (cmd.Contains("abaixo))"))
                        {
                            ultimosComandosStr.Add("abaixo))");
                        }
                        if (cmd.Contains("{"))
                        {
                            ultimosComandosStr.Add("{\n");
                        }
                    }
                }
            }
            else if (cmd.Contains("andar("))
            {
                ultimosComandosStr.Add("andar(");
                if (cmd.Contains("direita)"))
                {
                    ultimosComandosStr.Add("direita)");
                }
                else if (cmd.Contains("esquerda)"))
                {
                    ultimosComandosStr.Add("esquerda)");
                }
                else if (cmd.Contains("abaixo)"))
                {
                    ultimosComandosStr.Add("abaixo)");
                }
                else if (cmd.Contains("acima)"))
                {
                    ultimosComandosStr.Add("acima)");
                }
                if (cmd.Contains(";"))
                {
                    ultimosComandosStr.Add(";\n");
                }
            }
            else if (cmd.Contains("puxarAlavanca();"))
            {
                ultimosComandosStr.Add("puxarAlavanca()");
                ultimosComandosStr.Add(";\n");
            }
            else if (cmd.Contains("}"))
            {
                ultimosComandosStr.Add("}\n");
            }
        }
    }

    public void ConfirmaApagarTodosComandos()
    {
        panelConfirmarExclusao.SetActive(true);
    }

    public void EditarComandos()
    {
        if (ifCodigo.readOnly == true)
        {
            _editandoCodigo = true;
            ifCodigo.readOnly = false;
            ifCodigo.Select();
            _panelMobileScript.VoltarPainelBase();
            /*
            if (!_usandoNavDrawer)
            {
                _panelMobileScript.VoltarPainelBase();
            }
            */
        }
        else
        {
            _editandoCodigo = false;
            ifCodigo.readOnly = false;
        }
    }

    /*
        /// <summary>
        /// Botoes para cocatenar comandos, direcoes, terrenos, chaves e ponto e virgula mobile
        /// </summary>
        private enum MOBILE_CMD
        {
            cmdAndar, cmdEh, cmdEnquanto, cmdSe, cmdsenao, cmdEhVazio,
            dirAcima, dirDireita, dirEsquerda, dirAbaixo,
            terAgua, terAreia, terCaminho, terGrama, terAsfalto,
            outRb, outLb, outFinish, outNot
        }

        [SerializeField]
        private Button _btApagarTodosComandos, _btApagarUltimoComando, _btEditarComandos;

        private Button _btCmdAndar, _btCmdEh, _btCmdEnquanto, _btCmdSe, _btCmdsenao, _btCmdEhVazio;
        private Button _btDirAcima, _btDirDireita, _btDirEsquerda, _btDirAbaixo;
        private Button _btTerAgua, _btTerAreia, _btTerCaminho, _btTerGrama, _btTerAsfalto;
        private Button _btOutRB, _btOutLB, _btOutFinish, _btOutDiga, _btOutNot;
        private Button _btColar;
        private bool easyMode = false;

        public ArrayList ultimosComandos = new ArrayList();



        private void navDrawerSetup()
        {
            /// <summary>
            /// Listener para botoes mobile
            /// Comandos
            /// Direcoes
            /// Terrenos
            /// </summary>

            _usandoNavDrawer = true;
            string scene = SceneManager.GetActiveScene().name;
            string[] easyScenes = { "Fase 1", "Fase 2", "Fase 3", "Fase 4", "Fase 5" };
            for (int i = 0; i < easyScenes.Length; i++)
                if (scene.Equals(easyScenes[i]))
                    easyMode = true;

            _btApagarTodosComandos = btnPainelSim;
            _btApagarUltimoComando = GameObject.Find("btApagarUltimoComando").GetComponent<Button>();

            _btEditarComandos.onClick.AddListener(EditarComandos);
            _btApagarTodosComandos.onClick.AddListener(ConfirmaApagarTodosComandos);

            _btApagarUltimoComando.onClick.AddListener(() =>
            {
                if (ultimosComandosStr.Count > 0)
                {
                    ultimosComandosStr.RemoveAt(ultimosComandosStr.Count - 1);
                    ultimosComandos.RemoveAt(ultimosComandos.Count - 1);

                    if (ultimosComandosStr.Count == 0)
                    {
                        ifCodigo.text = "";
                        TodosBotoesAtivados(false);
                        _btCmdAndar.interactable = true;
                        _btCmdEnquanto.interactable = _gm.enquanto;
                        _btCmdSe.interactable = _gm.se;
                        _btOutFinish.interactable = false;
                        _btOutLB.interactable = false;
                        _btOutRB.interactable = false;
                        _btOutNot.interactable = false;

                        ultimosComandosStr = new ArrayList();
                        ultimosComandos = new ArrayList();
                    }
                    else
                    {
                        MOBILE_CMD cmd = (MOBILE_CMD)ultimosComandos[ultimosComandos.Count - 1];
                        HandleEasyMode(cmd);
                    }
                }
                TodosBotoesAtivados(false);
                _btCmdAndar.interactable = true;
            });

            _btCmdAndar = GameObject.Find("btCmdAndar").GetComponent<Button>();
            _btCmdAndar.interactable = true;
            _btCmdEh = GameObject.Find("btCmdEh").GetComponent<Button>();
            _btCmdEh.interactable = false;
            _btCmdEnquanto = GameObject.Find("btCmdEnquanto").GetComponent<Button>();
            _btCmdEnquanto.interactable = _gm.enquanto;
            _btCmdSe = GameObject.Find("btCmdSe").GetComponent<Button>();
            _btCmdSe.interactable = _gm.se;
            _btCmdsenao = GameObject.Find("btCmdCasoNao").GetComponent<Button>();
            _btCmdsenao.interactable = false; //Por enquanto não o utilizaremos
            _btCmdEhVazio = GameObject.Find("btCmdEhVazio").GetComponent<Button>();
            _btCmdEhVazio.interactable = false;


            _btCmdAndar.onClick.AddListener(() =>
            {
                string str = "andar(";
                MOBILE_CMD cmd = MOBILE_CMD.cmdAndar;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btCmdEh.onClick.AddListener(() =>
            {
                string str = "eh(";
                MOBILE_CMD cmd = MOBILE_CMD.cmdEh;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btCmdEnquanto.onClick.AddListener(() =>
            {
                string str = "enquanto(";
                MOBILE_CMD cmd = MOBILE_CMD.cmdEnquanto;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btCmdSe.onClick.AddListener(() =>
            {
                string str = "se(";
                MOBILE_CMD cmd = MOBILE_CMD.cmdSe;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btCmdsenao.onClick.AddListener(() =>
            {
                string str = "senao {\n";
                MOBILE_CMD cmd = MOBILE_CMD.cmdsenao;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btCmdEhVazio.onClick.AddListener(() =>
            {

                string str = "ehVazio(";
                MOBILE_CMD cmd = MOBILE_CMD.cmdEhVazio;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });


            _btDirAcima = GameObject.Find("btDirAcima").GetComponent<Button>();
            _btDirAcima.interactable = false;
            _btDirAbaixo = GameObject.Find("btDirAbaixo").GetComponent<Button>();
            _btDirAbaixo.interactable = false;
            _btDirDireita = GameObject.Find("btDirDireita").GetComponent<Button>();
            _btDirDireita.interactable = false;
            _btDirEsquerda = GameObject.Find("btDirEsquerda").GetComponent<Button>();
            _btDirEsquerda.interactable = false;

            _btDirAcima.onClick.AddListener(() =>
            {
                string str = (isIf ? "acima))" : "acima)");
                MOBILE_CMD cmd = MOBILE_CMD.dirAcima;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btDirAbaixo.onClick.AddListener(() =>
            {
                string str = (isIf ? "abaixo))" : "abaixo)");
                MOBILE_CMD cmd = MOBILE_CMD.dirAbaixo;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btDirDireita.onClick.AddListener(() =>
            {
                string str = (isIf ? "direita))" : "direita)");
                MOBILE_CMD cmd = MOBILE_CMD.dirDireita;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btDirEsquerda.onClick.AddListener(() =>
            {
                string str = (isIf ? "esquerda))" : "esquerda)");
                MOBILE_CMD cmd = MOBILE_CMD.dirEsquerda;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });

            _btTerAgua = GameObject.Find("btTerAgua").GetComponent<Button>();
            _btTerAgua.interactable = false;
            _btTerAreia = GameObject.Find("btTerAreia").GetComponent<Button>();
            _btTerAreia.interactable = false;
            _btTerGrama = GameObject.Find("btTerGrama").GetComponent<Button>();
            _btTerGrama.interactable = false;
            _btTerCaminho = GameObject.Find("btTerCaminho").GetComponent<Button>();
            _btTerCaminho.interactable = false;
            _btTerAsfalto = GameObject.Find("btTerAsfalto").GetComponent<Button>();
            _btTerAsfalto.interactable = false;

            _btTerAgua.onClick.AddListener(() =>
            {
                string str = "agua,";
                MOBILE_CMD cmd = MOBILE_CMD.terAgua;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btTerAreia.onClick.AddListener(() =>
            {
                string str = "areia,";
                MOBILE_CMD cmd = MOBILE_CMD.terAreia;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btTerGrama.onClick.AddListener(() =>
            {
                string str = "grama,";
                MOBILE_CMD cmd = MOBILE_CMD.terGrama;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btTerCaminho.onClick.AddListener(() =>
            {
                string str = "caminho,";
                MOBILE_CMD cmd = MOBILE_CMD.terCaminho;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btTerAsfalto.onClick.AddListener(() =>
            {
                string str = "asfalto,";
                MOBILE_CMD cmd = MOBILE_CMD.terAsfalto;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });

            _btOutLB = GameObject.Find("btOutLB").GetComponent<Button>();
            _btOutLB.interactable = false;
            _btOutRB = GameObject.Find("btOutRB").GetComponent<Button>();
            _btOutRB.interactable = false;
            _btOutFinish = GameObject.Find("btOutFinish").GetComponent<Button>();
            _btOutFinish.interactable = false;
            _btOutNot = GameObject.Find("btOutNot").GetComponent<Button>();
            _btOutNot.interactable = false;
            //_btOutDiga = GameObject.Find("btOutDiga").GetComponent<Button>();

            _btOutLB.onClick.AddListener(() =>
            {
                string str = "}\n";
                MOBILE_CMD cmd = MOBILE_CMD.outLb;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btOutRB.onClick.AddListener(() =>
            {
                string str = "){\n";
                MOBILE_CMD cmd = MOBILE_CMD.outRb;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btOutFinish.onClick.AddListener(() =>
            {
                string str = ";\n";
                MOBILE_CMD cmd = MOBILE_CMD.outFinish;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btOutNot.onClick.AddListener(() =>
            {
                string str = "!";
                MOBILE_CMD cmd = MOBILE_CMD.outNot;
                AddComandosALista(str, cmd);
                HandleEasyMode(cmd);
            });
            _btColar = GameObject.Find("BtColar").GetComponent<Button>();
            if (LevelManager.devMode)
            {
                _btColar.onClick.AddListener(() =>
                 {
                     ifCodigo.text = Gabarito.GetResultado(LevelManager.faseAtual)[0];
                 });
            }
            else
            {
                _btColar.gameObject.SetActive(false);
            }
            /*
            _btOutDiga.onClick.AddListener(() =>
            {
                string say = "";
                ifCodigo.text += "diga(\"" + say + "\");\n";

            });

            ifCodigo.readOnly = true;
            ifCodigo.enabled = false;

        }

        private void AddComandosALista(string str, MOBILE_CMD cmd)
        {
            ultimosComandosStr.Add(str);
            ultimosComandos.Add(cmd);
        }



        private void HandleEasyMode(MOBILE_CMD cmd)
        {
            ifCodigo.text = "";
            for (int i = 0; i < ultimosComandosStr.Count; i++)
                ifCodigo.text += ultimosComandosStr[i];

            TodosBotoesAtivados(false);
            bool state = true;
            bool RB = false;
            switch (cmd)
            {
                case MOBILE_CMD.cmdEh:
                    TodosBotoesAtivados(false);
                    _btTerAgua.interactable = true;
                    _btTerAreia.interactable = true;
                    _btTerCaminho.interactable = true;
                    _btTerGrama.interactable = true;
                    _btTerAsfalto.interactable = true;
                    _btOutRB.interactable = false;
                    _btOutNot.interactable = false;
                    break;
                case MOBILE_CMD.cmdEnquanto:
                case MOBILE_CMD.cmdSe:
                    isIf = true;
                    TodosBotoesAtivados(false);
                    _btCmdEh.interactable = true;
                    _btCmdEhVazio.interactable = true;
                    _btOutNot.interactable = true;
                    _btOutLB.interactable = false;
                    break;
                case MOBILE_CMD.outNot:
                    isIf = true;
                    TodosBotoesAtivados(false);
                    _btCmdEh.interactable = true;
                    _btCmdEhVazio.interactable = true;
                    _btOutNot.interactable = false;
                    break;
                case MOBILE_CMD.cmdsenao:
                    TodosBotoesAtivados(state);
                    break;
                case MOBILE_CMD.dirAcima:
                case MOBILE_CMD.dirEsquerda:
                case MOBILE_CMD.dirDireita:
                case MOBILE_CMD.dirAbaixo:
                    TodosBotoesAtivados(false);
                    if (isIf)
                    {
                        _btOutRB.interactable = true; // se for uma condicional habilita as chaves

                    }
                    else
                    {
                        _btOutFinish.interactable = true; // senão habilita o ponto e vírgula

                    }
                    isIf = false;
                    break;


                case MOBILE_CMD.terAgua:
                case MOBILE_CMD.terAreia:
                case MOBILE_CMD.terCaminho:
                case MOBILE_CMD.terGrama:
                case MOBILE_CMD.terAsfalto:
                    TodosBotoesAtivados(false);
                    _btDirAcima.interactable = true;
                    _btDirEsquerda.interactable = true;
                    _btDirDireita.interactable = true;
                    _btDirAbaixo.interactable = true;
                    _btOutRB.interactable = false;
                    _btOutFinish.interactable = false;
                    _btOutNot.interactable = false;
                    break;
                case MOBILE_CMD.cmdEhVazio:
                    isIf = true;
                    TodosBotoesAtivados(false);
                    _btDirAcima.interactable = true;
                    _btDirEsquerda.interactable = true;
                    _btDirDireita.interactable = true;
                    _btDirAbaixo.interactable = true;
                    _btOutRB.interactable = false;
                    _btOutFinish.interactable = false;
                    _btOutNot.interactable = false;
                    break;

                case MOBILE_CMD.outFinish:
                    TodosBotoesAtivados(false);
                    _btCmdAndar.interactable = true;
                    _btCmdEnquanto.interactable = _gm.enquanto;
                    _btCmdSe.interactable = _gm.se;
                    _btOutFinish.interactable = false;
                    _btOutLB.interactable = true;
                    break;
                case MOBILE_CMD.outLb:
                    TodosBotoesAtivados(false);
                    _btCmdAndar.interactable = true;
                    _btCmdEnquanto.interactable = _gm.enquanto;
                    _btCmdSe.interactable = _gm.se;
                    _btOutFinish.interactable = false;
                    _btOutLB.interactable = false;
                    RB = false;
                    break;
                case MOBILE_CMD.outRb:
                    RB = true;
                    _btOutLB.interactable = false;
                    _btOutRB.interactable = false;
                    _btCmdAndar.interactable = true;
                    _btCmdEnquanto.interactable = _gm.enquanto;
                    _btCmdSe.interactable = _gm.se;
                    _btOutFinish.interactable = false;
                    _btOutLB.interactable = false;
                    break;
                case MOBILE_CMD.cmdAndar:
                    TodosBotoesAtivados(false);
                    _btDirAcima.interactable = true;
                    _btDirEsquerda.interactable = true;
                    _btDirDireita.interactable = true;
                    _btDirAbaixo.interactable = true;
                    /*
                    _btOutLB.interactable = false;
                    _btOutRB.interactable = false;
                    _btOutNot.interactable = false;
                    _btOutFinish.interactable = false;
                    break;

                default:

                    TodosBotoesAtivados(false);
                    _btCmdAndar.interactable = true;
                    _btCmdEnquanto.interactable = _gm.enquanto;
                    _btCmdSe.interactable = _gm.se;
                    _btOutFinish.interactable = false;
                    _btOutLB.interactable = false;
                    _btOutRB.interactable = false;
                    _btOutNot.interactable = false;

                    break;
            }
        }

        private void TodosBotoesAtivados(bool state)
        {
            _btCmdAndar.interactable = state;
            _btCmdEh.interactable = state && (_gm.enquanto || _gm.se);
            _btCmdEnquanto.interactable = state && (_gm.enquanto);
            _btCmdSe.interactable = state && (_gm.se);
            _btCmdsenao.interactable = false && _gm.senao && _gm.senao; //por enquanto ele estará desabilitado
            _btCmdEhVazio.interactable = state && (_gm.enquanto || _gm.se);
            _btDirAcima.interactable = state;
            _btDirDireita.interactable = state;
            _btDirEsquerda.interactable = state;
            _btDirAbaixo.interactable = state;
            _btTerAgua.interactable = state && (_gm.enquanto || _gm.se);
            _btTerAreia.interactable = state && (_gm.enquanto || _gm.se);
            _btTerCaminho.interactable = state && (_gm.enquanto || _gm.se);
            _btTerGrama.interactable = state && (_gm.enquanto || _gm.se);
            _btTerAsfalto.interactable = state && (_gm.enquanto || _gm.se);
        }
        */

    private char ValidarCodigoFurbot(string text, int charIndex, char ch)
    {
        if (((int)ch >= 65 && (int)ch <= 90) || ((int)ch >= 97 && ch <= 122) || ch == '{' || ch == '}' || ch == ';' || ch == '(' || ch == ')' || ch == ',' || ch == '\n' || ch == '!')
        {
            return ch;
        }
        else
        {
            return (char)0x00;
        }
    }

    /// <summary>
    /// Atualiza na interface o número de vidas do robô.
    /// </summary>
    /// <param name="numeroVida">O parâmetro de entrada é referente a quantidade de vidas que o robô possui.</param>
    public void AtualizarVida(int numeroVida)
    {
        if (numeroVida >= 0 && numeroVida <= 4)
        {
            vidasImage.sprite = vidas[numeroVida];
        }
    }

    public void AtualizarQntTesouros(int tesouro)
    {
        qntTesourosText.text = ": " + tesouro;
    }

    /// <summary>
    /// Atualiza na interface a quantidade de energia que o robô contém. 
    /// </summary>
    /// <param name="energia">O parâmetro de entrada é referente a quantidade de energia que o robô possui.</param>
    public void AtualizarQntEnergia(int energia)
    {
        qntEnergiaText.text = ": " + energia + "%";
        if (energia <= 0)
        {
            bateriaImage.sprite = baterias[0];
        }
        else if (energia <= 25)
        {
            bateriaImage.sprite = baterias[1];
        }
        else if (energia <= 50)
        {
            bateriaImage.sprite = baterias[2];
        }
        else if (energia <= 75)
        {
            bateriaImage.sprite = baterias[3];
        }
        else if (energia <= 100)
        {
            bateriaImage.sprite = baterias[4];
        }
    }

    /// <summary>
    /// Atualiza na interface a pontuação do robô.
    /// </summary>
    /// <param name="pontos">O parâmetro de entrada é referente a quantidade de pontos que o robô acabou de ganhar.</param>
    public void AtualizarPontuacao()
    {
        pontosText.text = ": " + PontuacaoController.pontosFase;
    }

    /// <summary>
    /// Verifica se o botão Recomeçar está selecionado.
    /// </summary>
    /// <returns>Retorna true se o botão estiver selecionado e retorna false se não estiver selecionado.</returns>
    public bool Recomecar()
    {
        if (btnRecomecar.isOn)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Inicia corrotina para leitura do código digitado no campo de texto.
    /// </summary>
    public void ExecutarCodigo()
    {
        if (gm.personagemSelecionado.name.Contains("Furbot"))
        {

            if (!furbot.venceu && !furbot.emExecucao)
            {
                _analisador.IncrementarExecucoes();
                btnExecutar.GetComponentInChildren<Text>().text = "PARAR";
                btnExecutar.image.sprite = btnExecutarPause;
                btnRecomecar.interactable = false;
                ifCodigo.interactable = false;
                if (Configuracoes.painelComandos)
                    if (comandosAberto)
                    {
                        TogglePanelComandos();
                        permitidoDigitarComandos = false;
                    }
                rotinaExecutar = StartCoroutine(furbot.compilador.LerComandos(GetTexto().ToLower()));
            }
            else
            {
                if (!emDialogo)
                    RecuperarTexto();
                if (rotinaExecutar != null)
                {
                    StopCoroutine(rotinaExecutar);
                }
                btnExecutar.GetComponentInChildren<Text>().text = "EXECUTAR";
                btnExecutar.image.sprite = btnExecutarPlay;
                rotinaExecutar = null;
                furbot.emExecucao = false;
                btnRecomecar.interactable = true;
                ifCodigo.interactable = true;
            }
        }
        else
        {
            BuggienAndroid _buggien = gm.personagemSelecionado.GetComponent<BuggienAndroid>();
            if (!furbot.venceu && !_buggien.emExecucao)
            {
                btnExecutar.GetComponentInChildren<Text>().text = "PARAR";
                btnExecutar.image.sprite = btnExecutarPause;
                btnRecomecar.interactable = false;
                ifCodigo.interactable = false;
                if (Configuracoes.painelComandos)
                    if (comandosAberto)
                    {
                        TogglePanelComandos();
                        permitidoDigitarComandos = false;
                    }
                rotinaExecutar = StartCoroutine(_buggien.compilador.LerComandosBuggien(GetTexto().ToLower()));
            }
            else
            {
                if (!emDialogo)
                    // RecuperarTexto();
                    if (rotinaExecutar != null)
                    {
                        StopCoroutine(rotinaExecutar);
                    }
                btnExecutar.GetComponentInChildren<Text>().text = "EXECUTAR";
                btnExecutar.image.sprite = btnExecutarPlay;
                rotinaExecutar = null;
                _buggien.emExecucao = false;
                btnRecomecar.interactable = true;
                ifCodigo.interactable = true;
            }
        }
    }

    public void PararExecucao()
    {
        btnExecutar.GetComponentInChildren<Text>().text = "EXECUTAR";
        btnExecutar.image.sprite = btnExecutarPlay;
    }

    // Alterado para utilizar interface tangivel
    public string GetTexto()
    {
        if (!LevelManager.isInterfaceTangivel) {
            return ifCodigo.text.Replace("<b>", "").Replace("</b>", "");
        } else
        {
            return _interfaceTangivel.CodigoPecasLista();
        }
    }

    /// <summary>
    /// Destaca a linha que está sendo executada no texto que foi digitado no console.
    /// </summary>
    /// <param name="index">Linha que está sendo executada no momento.</param>
    /// <returns>O texto do console com a linha destacada.</returns>
    public string SetTextoDebug(int index)
    {
        string textoDebug = "";
        string[] textoDividido = _textoDigitado.Split('\n');
        foreach (string linha in textoDividido)
        {
            linha.Replace("<b>", "").Replace("</b>", "");
        }
        //valorScrollBar = ((index + 1f) * 100f / textoDividido.Length) * 0.01f;
        //Debug.Log(index + " / " + textoDividido.Length + " = " + ifCodigo.verticalScrollbar.value);
        try
        {
            textoDividido[index] = "<b>" + textoDividido[index] + "</b>";
        }
        catch (System.IndexOutOfRangeException)
        { }

        for (int i = GetMinimo(index, textoDividido.Length); i < GetMaximo(index, textoDividido.Length); i++)
        {
            textoDebug += textoDividido[i] + "\n";
        }
        AtualizarNumeros(furbot.compilador.GetIndex(), GetMinimo(index, textoDividido.Length), GetMaximo(index, textoDividido.Length));
        return textoDebug.Remove(textoDebug.Length - 1);
    }

    /// <summary>
    /// Mostra o Painel de Diálogo com o texto passado pelo parâmetro.
    /// </summary>
    /// <param name="texto">Texto a ser mostrado.</param>
    /// <returns>Retorna da corotina desfazendo tudo.</returns>
    public IEnumerator Diga(string texto)
    {
        //debug.SetActive(true);
        ifCodigo.readOnly = true;
        dialogPanel.MostrarTexto(Dialog_Char.FURBOT, texto);

        yield return new WaitForSeconds(3.0f);

        debug.SetActive(false);
#if !UNITY_ANDROID && !UNITY_IOS
        ifCodigo.readOnly = false;
#endif
    }

    public IEnumerator Diga(Dialog_Char personagem, string texto)
    {
        //debug.SetActive(true);
        ifCodigo.readOnly = true;
        dialogPanel.MostrarTexto(personagem, texto);

        yield return new WaitForSeconds(3.0f);

        debug.SetActive(false);
#if !UNITY_ANDROID && !UNITY_IOS
        ifCodigo.readOnly = false;
#endif
    }

    /// <summary>
    /// Destaca em vermelho a linha que foi passada pelo parâmetro.
    /// </summary>
    /// <param name="index">Linha a ser destacada.</param>
    /// <returns>Texto com a linha destacada.</returns>
    public string SetTextoErro(int index)
    {
        /*
        _furbot.isMoving = true;
        string textoDebug = "";
        string[] textoDividido = _textoDigitado.Split('\n');

        //valorScrollBar = ((index + 1f) * 100f / textoDividido.Length) * 0.01f;
        if (index != -1)
        {
            textoDividido[index] = "<b>" + textoDividido[index] + "</b>";

        }

        for (int i = 0; i < textoDividido.Length - 1; i++)
        {
            textoDebug += textoDividido[i] + "\n";
        }
        _furbot.isMoving = false;
        return textoDebug;
        */
        return index != -1 ? SetTextoDebug(index) : _textoDigitado;
    }

    /// <summary>
    /// Mostra qual linha deve-se começar o "for" pra aparecer no Painel de Debug. 
    /// </summary>
    /// <param name="i">Linha sendo lida no momento.</param>
    /// <returns>Linha em que se deve começar a ler.</returns>
    public int GetMinimo(int i, int lenght)
    {
        if (i < 15)
        {
            return 0;
        }
        else if (i + 15 > lenght)
        {
            if (lenght > 30)
            {
                return lenght - 31;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return i - 15;
        }
    }

    public void SalvarTexto()
    {
        _textoDigitado = ifCodigo.text.Replace("<b>", "").Replace("</b>", "");
    }

    public void RecuperarTexto()
    {
        ifCodigo.text = _textoDigitado.Replace("<b>", "").Replace("</b>", "");
    }

    /// <summary>
    /// Mostra qual é a linha em que a Leitura do "for" deve terminar de ler para não sair.
    /// da tela do Painel de Debug
    /// </summary>
    /// <param name="i">Linha sendo lida no momento.</param>
    /// <param name="lenght">Tamanho total do texto.</param>
    /// <returns></returns>
    public int GetMaximo(int i, int lenght)
    {
        if (i < 16)
        {
            if (lenght > 30)
            {
                return 31;
            }
            else return lenght;
        }
        else if (i > lenght - 16)
        {
            return lenght;
        }
        else return i + 16;
    }

    /// <summary>
    /// Setter do texto no Console.
    /// </summary>
    /// <param name="texto">Texto a ser colocado.</param>
    public void SetTexto(string texto)
    {
        ifCodigo.text = texto.Replace("<b>", "").Replace("</b>", "");
    }

    /// <summary>
    /// Esconde a tela de derrota do jogo.
    /// </summary>
    public void EsconderGameOver()
    {
        gameOverImage.SetActive(false);
    }

    /// <summary>
    /// Mostra a tela de derrota do jogo.
    /// </summary>
    public void MostrarGameOver()
    {
        gameOverImage.SetActive(true);
    }

    /// <summary>
    /// Mostra tela de vitória do jogo.
    /// </summary>
    public void MostrarSucesso()
    {
#if UNITY_ANDROID || UNITY_IOS
        GameObject navDrawer = GameObject.Find("navDrawer");
        if (navDrawer != null)
        {
            navDrawer.SetActive(false);
        }
#endif
        sucessoImage.SetActive(true);
        UIManagerSucesso uiSucesso = sucessoImage.GetComponent<UIManagerSucesso>();
        int tesourosColetados = furbot.GetQntTesouros();
        int diferenca = 0;
        int pontuacaoSobresalente;
        bool jaJogou;
        PontuacaoController.ChecaPontuacaoFase(tesourosColetados, out diferenca, out pontuacaoSobresalente, out jaJogou);
        if (gm != null)
        {
            if (jaJogou)
            {
                gm.pontuacaoTotal += pontuacaoSobresalente;
            }
            else
            {
                gm.pontuacaoTotal += PontuacaoController.pontosFase;
            }
        }
        pontuacaoTotal.text = "Pontuacao total: " + gm.pontuacaoTotal;
        contadorTesouros.text = tesourosColetados + "/" + ((int)(PontuacaoController.GetTotalTesouros() + gm.tesourosCenaAnterior));
        uiManagerSucesso.SetPontos(PontuacaoController.pontosFase);
        PontuacaoController.pontosFase = 0;
        if (jaJogou && diferenca > 0)
        {
            tesouroFeedback.text = "Nesta passagem você coletou " + diferenca + " tesouro(s) a mais!";
            tesouroFeedback.color = Color.green;
        }
        else if (jaJogou && diferenca < 0)
        {
            tesouroFeedback.text = "Nesta passagem você coletou " + (diferenca * -1) + " tesouro(s) a menos!";
            tesouroFeedback.color = Color.red;
        }
    }

    /// <summary>
    /// Mostra a tela de pause do jogo em momento de pause.
    /// </summary>
    public void MostrarPause()
    {
        bool condicao;
        if (gm.personagemSelecionado.GetComponent<BuggienAndroid>() != null)
        {
            condicao = gm.personagemSelecionado.GetComponent<BuggienAndroid>().emExecucao;
        }
        else
        {
            condicao = furbot.emExecucao;
        }

        if (condicao)
        {
            ExecutarCodigo();
        }
        pauseImage.SetActive(true);
    }

    /// <summary>
    /// Adiciona algum evento no Log com o horário atual.
    /// </summary>
    /// <param name="evento">Evento a ser passado.</param>
    public void AddLog(string evento)
    {
        _textoLog = _textoLog + "\n" + System.DateTime.Now.ToString(new System.Globalization.CultureInfo("en-GB"))
            + ":\n" + evento;
    }

    /// <summary>
    /// Getter do Log.
    /// </summary>
    /// <returns>Retorna o texto do Log formatado.</returns>
    public string GetLog()
    {
        return "\n\nLog:\n" + _textoLog;
    }

    /// <summary>
    /// Método para atualizar a interface do invetário com os itens atuais que o Furbot carrega.
    /// </summary>
    public void AtualizarInventarioUI()
    {
        int indice = 0;
        foreach (int id in gm.inventarioIDs)
        {
            GameObject go = gm.todosItensJogo[id];
            inventarioUI[indice].gameObject.tag = go.tag;
            inventarioUI[indice].sprite = go.GetComponent<SpriteRenderer>().sprite;
            indice++;
        }

        for (int i = gm.qtdeItensAtuaisInventario; i <= gm.inventarioIDs.Capacity - 1; i++)
        {
            inventarioUI[i].gameObject.tag = "Untagged";
            inventarioUI[i].sprite = imagemVazia;
        }
    }

    public void MostrarExplicação(int quantidade, string comando)
    {
        dialog.SetActive(true);
        dialogPanel.explicacao = true;
        dialogPanel.Explicar(quantidade, comando);
    }

    public IEnumerator IniciarDialogo(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        bool temDialogo = true;
        if (!SceneManager.GetActiveScene().name.Equals("FaseGerada"))
        {
            dialogPanel.explicacao = false;
            dialogPanel.ContarHistoria(out temDialogo);
            if (temDialogo)
            {
                dialog.SetActive(true);
            }
            else
            {
                ifCodigo.interactable = true;
                yield return null;
            }
        }
    }


    public void AtualizarNumeros()
    {
        if (!furbot.emExecucao)
        {
            int linhas = ifCodigo.text.Split('\n').Length;

            string textoNumeros = "";

            for (int i = 0; i < linhas; i++)
            {
                textoNumeros += (i + 1) + ":\n";
            }
            _ifNumeros.text = textoNumeros;
            if (ifCodigo.text.Equals(""))
            {
                _ifNumeros.text = "";
            }
        }
    }

    public void AtualizarNumeros(int index)
    {
        string textoNovo = "";
        string[] textoDividido = _ifNumeros.text.Split('\n');
        foreach (string linha in textoDividido)
        {
            linha.Replace("<b>", "").Replace("</b>", "");
        }
        try
        {
            textoDividido[index] = "<b>" + textoDividido[index] + "</b>";
        }
        catch (System.IndexOutOfRangeException i) { }
        foreach (string linha in textoDividido)
        {
            textoNovo += linha + "\n";
        }
        _ifNumeros.text = textoNovo;
    }

    public void AtualizarNumeros(int index, int minimo, int maximo)
    {
        int linhas = ifCodigo.text.Split('\n').Length;
        string textoNumeros = "";

        for (int i = minimo; i < maximo; i++)
        {
            if (i == index)
            {
                textoNumeros += "<b>" + (i + 1) + ":</b>\n";
            }
            else
            {
                textoNumeros += (i + 1) + ":\n";
            }
        }
        _ifNumeros.text = textoNumeros;

        if (linhas == 0)
        {
            _ifNumeros.text = "";
        }
    }

    public void ToggleDialog()
    {
        if (!furbot.emExecucao)
        {
            permitidoDigitarComandos = !dialog.gameObject.activeInHierarchy;
            btnExecutar.interactable = permitidoDigitarComandos;
            ifCodigo.interactable = permitidoDigitarComandos;
            if (ifCodigo.text.Length > 0)
            {
                RecuperarTexto();
            }
        }
    }

    public void ReiniciarFaseAtual()
    {
        gm.gameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VoltarParaSelecaoDeFases()
    {
        gm.gameOver = false;
        SceneManager.LoadScene("SelecaoDeFases");
    }

    /// <summary>
    /// Método para mostrar dica referente ao quebra-cabeça em questão.
    /// </summary>
    /// <param name="go">Objeto clicado que contém o componente EventTrigger</param>
    public void UsarItemInventario(GameObject go)
    {
        switch (go.tag.ToLower())
        {
            case "papiro01":
                dicaPuzzle.gameObject.SetActive(true);
                dicaPuzzle.sprite = dicaPuzzleAlavanca;
                break;
            case "papiro02":
                dicaPuzzle.gameObject.SetActive(true);
                dicaPuzzle.sprite = dicaPuzzlePlacasPressao;
                break;
        }
    }

    public void AlterarHubPorPersonagem(Dialog_Char personagem)
    {
        if (personagem == Dialog_Char.FURBOT)
        {
            vidasImage.gameObject.SetActive(true);
            buggienImage.gameObject.SetActive(false);
            AtualizarQntEnergia(furbot.energia);
            _backgroundImage.color = Color.white;
        }
        else
        {
            vidasImage.gameObject.SetActive(false);
            buggienImage.gameObject.SetActive(true);
            bateriaImage.sprite = baterias[4];
            qntEnergiaText.text = ": Ꝏ%";
            _backgroundImage.color = Color.green;
        }

    }

    /// <summary>
    /// Método para fechar a dica do puzzle.
    /// </summary>
    public void FecharDicaPuzzle()
    {
        dicaPuzzle.gameObject.SetActive(false);
    }

    public void AtivarDicaFase()
    {
        if (panelDica.activeSelf == true)
        {
            panelDica.SetActive(false);
        }
        else
        {
            panelDica.SetActive(true);
        }
    }
}
