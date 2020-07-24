using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    //Objetos
    private Coroutine _rotinaTexto;
    private UIManager _uiManager;

    //Objetos (SERIALIZED)
    [SerializeField]
    private GameObject _dialogPanel;
    [SerializeField]
    private Sprite[] _imagemPersonagens;
    [SerializeField]
    private TextMeshProUGUI _texto;
    [SerializeField]
    private SpriteRenderer _personagem;
    [SerializeField]
    private Image _setinha;

    private bool _faseGerada;
    public bool haDialogoNessaFase;

    //Floats
    private float _velocidadeTexto, _velocidadeTextoPadrao;

    //Strings
    public string[] _dialogoAtual;
    private string[] _dialogoFase1 = new string[] {"Está bem... Vamos com calma.",
                                                   "Vamos procurar por pistas que digam para qual caminho levaram a Sam.",
                                                   "Programe o Furbot até a pista!"};

    private string[] _dialogoFase2 = new string[] {"Vamos seguindo em frente.",
                                                   "Não tenha pressa. O importante agora é achar uma maneira de encontrar o esconderijo.",
                                                   "Programe com calma até outra pista."};

    private string[] _dialogoFase3 = new string[] {"Veja, parece que não temos escolha. Devemos utilizar as vitórias-régias para atravessar este lago.",
                                                   "Você sabia que a vitória-régia é a maior planta aquática do mundo?",
                                                   "Prossiga com a programação do Furbot, parece que há uma pista no outro lado do lago."};

    private string[] _dialogoFase4 = new string[] {"Parece que devemos dar a volta por essas rochas e ver o que encontramos.",
                                                   "Foco nas pistas...  Para onde eles foram???"};

    private string[] _dialogoFase5 = new string[] {"Faz muito tempo que estamos andando e não encontramos nenhuma evidência concreta de onde eles podem estar.",
                                                   "Neste local há muitos caminhos, deve ter alguma coisa que pode nos ajudar.",
                                                   "Programe o que for preciso para acharmos algo relevante."};

    private string[] _dialogoFase6 = new string[] { "Chegamos ao Egito! Acho que na pirâmide conseguiremos mais pistas.",
                                                    "Para entrar nela, temos que coletar os acessórios do faraó, vamos lá!" };

    private string[] _dialogoFase7 = new string[] { "Continue procurando, os faraós usavam inúmeros acessórios.",
                                                    "Devem estar por aqui em algum lugar." };

    private string[] _dialogoFase8 = new string[] { "Uhuul! Entramos na pirâmide!",
                                                    "Aqui dentro deve ter alguma pista sobre o lugar que os Buggiens estão se escondendo."};

    private string[] _dialogoFase9 = new string[] { "Veja! Esta sala contém algumas alavancas e números relacionados.",
                                                    "Se você quiser puxá-la, fique em frente a uma e utilize o comando 'puxarAlavanca();'.",
                                                    "E verifique o pergaminho no inventário, ele pode conter alguma dica."};

    private string[] _dialogoFase10 = new string[] { "UAAU!!! Esta é a sala do sarcófago, tem muito tesouro aqui dentro.",
                                                     "Provável que aqui esteja a última parte da mensagem dos papíros."};

    private string[] _dialogoFase11 = new string[] { "Chegamos na Sibéria, o lugar mais frio do mundo!",
                                                     "Precisamos pedir para alguém se ele viu a nave passando por aqui.",
                                                     "Acho que eu vi alguem ali na frente, vamos!"};

    private string[] _dialogoFase12 = new string[] { "Ele nos mandou vir pra cá, mas acho que a gente se perdeu.",
                                                     "Mas eu acho que tem outro esquimó ali na frente, vamos ver se ele sabe onde a nave está."};

    private string[] _dialogoFase13 = new string[] { "Poxa! Pena que ele não viu a nave, mas ele disse que o amigo dele viu.",
                                                     "De acordo com o esquimó, o amigo dele é pra estar logo ali na frente.",
                                                     "Vamos lá falar com ele!"};

    private string[] _dialogoFase14 = new string[] { "Estamos no caminho certo!",
                                                     "Só precisamos dar a volta por aqui e estaremos bem próximos da nave."};

    private string[] _dialogoFase15 = new string[] { "Nossa, esses buggiens sujam demais! Olha quanto lixo espalhado!",
                                                     "Tem um esquimó muito próximo da nave, vamos ver o que ele tá fazendo lá."};

    private string[] _dialogoFase16 = new string[] { "Pelo jeito aqui dentro está mais sujo que lá fora...",
                                                     "E pelo o que eu posso observar, esses canos devem transportar o combustível da nave.",
                                                     "Vamos ver se encontramos alguma válvula para fechar, assim essa nave não sairá do lugar"};

    private string[] _dialogoFase17 = new string[] { "Aqui deve ser um depósito de androides quebrados.",
                                                     "Cuide para não encostar neles."};

    private string[] _dialogoFase173 = new string[] { "Aqueles androides não parecem estar quebrados.",
                                                      "Talvez você consiga empurrar eles."};

    private string[] _dialogoFase18 = new string[] { "Aqui tem androides que podem ser reprogramados.",
                                                     "Clique ou toque neles para programá-los e leve-os para os botões."};

    private string[] _dialogoFase19 = new string[] { "Ali em cima tem mais uma válvula, precisamos pegá-la.",
                                                     "Mas não dá de chegar até lá.",
                                                     "Furbot, tente pisar naqueles botões vermelhos que nem os androides fizeram antes."};

    private string[] _dialogoFase20 = new string[] { "Chegamos! A Professora Sam deve de estar por aqui.",
                                                     "Mas em qual das portas será que ela está?",
                                                     "De qualquer maneira, primeiro precisamos encontrar a chave para tirar ela daquela jaula.",
                                                     "E tome cuidado para não esbarrar nesses ovos",
                                                     "O esquimó que estava perto da nave disse que viu um deles explodir!" };


    private string[] _dialogoFaseGerada = new string[] { "Não conheço essa região...",
                                                         "Mas confio em você para nos guiar."};

    private string[] _dialogoDefault = new string[] { "???" };

    private string[] _dialogoEnquanto = new string[] { "",
                                                       "Você encontrou a primeira sílaba de um novo comando para o furbot! Tente encontrar as próximas.",
                                                       "Você encontrou a segunda sílaba! Estamos perto de liberar este comando.",
                                                       "Você encontrou a ultima sílaba do comando de repetição 'enquanto'! " +
                                                       "Este comando serve para criar um laço de repetição de comandos " +
                                                       "EX: enquanto(eh(caminho,direita)){ andar(direita); }"};

    private string[] _dialogoSeSenao = new string[] { "",
                                                      "Veja, você encontrou uma peça de outra funcionalidade que Sam estava desenvolvendo, porém esta é só uma parte do comando.",
                                                      "Você encontrou mais uma parte do comando, estamos quase lá!",
                                                      "Parabéns, você encontrou todas as partes do comando! Ache a última pista e irei introduzir a nova funcionalidade do Furbot!"};

    //Integers
    public int indiceDialogo;
    private int _contadorSkip;

    //Booleans
    public bool explicacao;

    private void Awake()
    {
        _uiManager = GameObject.Find("CanvasInterface").GetComponent<UIManager>();
        _texto.text = "";
        InicializarDialogoAtual();
    }

    // Use this for initialization
    void Start()
    {
        _velocidadeTextoPadrao = 0.05f;
        _velocidadeTexto = _velocidadeTextoPadrao;
        _contadorSkip = 0;
        explicacao = false;
        if (SceneManager.GetActiveScene().name.Equals("FaseGerada"))
        {
            _faseGerada = true;
        }
    }

    private void Update()
    {
        if (_uiManager.emDialogo)
        {
            if (!_faseGerada)
            {
                if (indiceDialogo < _dialogoAtual.Length)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                    {
                        PularFala();
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) ||
                    Input.GetKeyDown(KeyCode.Space) ||
                    Input.GetKeyDown(KeyCode.Return) ||
                    Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    PularFala();
                }
            }
        }
    }
    private void OnEnable()
    {
        _uiManager.ToggleDialog();
    }
    private void OnDisable()
    {
        _uiManager.ToggleDialog();
    }

    public void InicializarDialogoAtual()
    {
        _dialogoAtual = null;
        switch (SceneManager.GetActiveScene().name)
        {
            case "Fase 1":
                _dialogoAtual = _dialogoFase1;
                break;
            case "Fase 2":
                _dialogoAtual = _dialogoFase2;
                break;
            case "Fase 3":
                _dialogoAtual = _dialogoFase3;
                break;
            case "Fase 4":
                _dialogoAtual = _dialogoFase4;
                break;
            case "Fase 5":
                _dialogoAtual = _dialogoFase5;
                break;
            case "Fase 6":
                _dialogoAtual = _dialogoFase6;
                break;
            case "Fase 7":
                _dialogoAtual = _dialogoFase7;
                break;
            case "Fase 8":
                _dialogoAtual = _dialogoFase8;
                break;
            case "Fase 9":
                _dialogoAtual = _dialogoFase9;
                break;
            case "Fase 10":
                _dialogoAtual = _dialogoFase10;
                break;
            case "Fase 11":
                _dialogoAtual = _dialogoFase11;
                break;
            case "Fase 12":
                _dialogoAtual = _dialogoFase12;
                break;
            case "Fase 13":
                _dialogoAtual = _dialogoFase13;
                break;
            case "Fase 14":
                _dialogoAtual = _dialogoFase14;
                break;
            case "Fase 15":
                _dialogoAtual = _dialogoFase15;
                break;
            case "Fase 16":
                _dialogoAtual = _dialogoFase16;
                break;
            case "Fase 17":
                _dialogoAtual = _dialogoFase17;
                break;
            case "Fase 17.3":
                _dialogoAtual = _dialogoFase173;
                break;
            case "Fase 18":
                _dialogoAtual = _dialogoFase18;
                break;
            case "Fase 19":
                _dialogoAtual = _dialogoFase19;
                break;
            case "Fase 20":
                _dialogoAtual = _dialogoFase20;
                break;
            case "FaseGerada":
                _dialogoAtual = new string[1];
                break;
            default:
                _dialogoAtual = _dialogoDefault;
                break;
        }
    }

    public void ContarHistoria(out bool temDialogo)
    {
        temDialogo = true;
        indiceDialogo = 0;
        InicializarDialogoAtual();
        if (_dialogoAtual != _dialogoDefault)
        {
            switch (SceneManager.GetActiveScene().name)
            {
                default:
                    MostrarTexto(Dialog_Char.S223, _dialogoAtual[indiceDialogo]);
                    break;
            }
        }
        else
        {
            temDialogo = false;
        }
    }

    public void ChecarSeHaDialogo()
    {
        if (LevelManager.isInterfaceTangivel)
        {
            haDialogoNessaFase = false;
        }
        else 
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Fase 1":
                case "Fase 2":
                case "Fase 3":
                case "Fase 4":
                case "Fase 5":
                case "Fase 6":
                case "Fase 7":
                case "Fase 8":
                case "Fase 9":
                case "Fase 10":
                case "Fase 11":
                case "Fase 12":
                case "Fase 13":
                case "Fase 14":
                case "Fase 15":
                case "Fase 16":
                case "Fase 17":
                case "Fase 17.3":
                case "Fase 18":
                case "Fase 19":
                case "Fase 20":
                case "FaseGerada":
                    haDialogoNessaFase = true;
                    break;
                default:
                    haDialogoNessaFase = false;
                    break;
            }
        }
    }

    public void Explicar(int quantidade, string comando)
    {
        indiceDialogo = 0;
        if (comando == "enquanto")
        {
            _dialogoAtual = _dialogoEnquanto;
        }
        else if (comando == "sesenao")
        {
            _dialogoAtual = _dialogoSeSenao;
        }
        _texto.text = _dialogoAtual[quantidade];
        Time.timeScale = 0;
    }

    public void MostrarTexto(Dialog_Char personagem, string texto)
    {
        _dialogPanel.SetActive(true);
        TrocarPersonagem(personagem);
        if (!explicacao)
        {
            _rotinaTexto = StartCoroutine(ReproduzirTexto(texto));
        }
        else
        {
            _texto.text = _dialogoAtual[indiceDialogo];
        }
        _uiManager.emDialogo = true;
    }

    public IEnumerator ReproduzirTexto(string falaAtual)
    {
        _texto.text = "";
        _setinha.gameObject.SetActive(false);
        int contadorDeChar = 0;
        while (contadorDeChar < falaAtual.Length)
        {
            _texto.text += falaAtual[contadorDeChar++];
            yield return new WaitForSeconds(_velocidadeTexto);
        }
        _setinha.gameObject.SetActive(true);
        _contadorSkip = 2;
    }

    public void PularFala()
    {
        if (++_contadorSkip == 1)
        {
            _velocidadeTexto = 0.005f;
        }
        else
        {
            if (explicacao == false)
            {
                indiceDialogo++;
                if ((indiceDialogo >= _dialogoAtual.Length) || _dialogoAtual == null)
                {
                    if (_rotinaTexto != null)
                    {
                        StopCoroutine(_rotinaTexto);
                    }
                    _uiManager.dialog.SetActive(false);
                    _uiManager.emDialogo = false;
                    indiceDialogo = 0;
                    _contadorSkip = 0;
                    _velocidadeTexto = _velocidadeTextoPadrao;
                    _dialogoAtual = new string[1];
                    _uiManager.ChecarEventoAposDialogo();
                }
                else
                {
                    if (_rotinaTexto != null)
                    {
                        StopCoroutine(_rotinaTexto);
                    }
                    _contadorSkip = 0;
                    _velocidadeTexto = _velocidadeTextoPadrao;
                    _rotinaTexto = StartCoroutine(ReproduzirTexto(_dialogoAtual[indiceDialogo]));
                }
            }
            else
            {
                Time.timeScale = 1;
                explicacao = false;
                _uiManager.dialog.SetActive(false);
                _dialogoAtual = _dialogoDefault;
                _contadorSkip = 0;
                _velocidadeTexto = _velocidadeTextoPadrao;
            }
        }
    }

    public void TrocarPersonagem(Dialog_Char personagem)
    {
        switch (personagem)
        {
            case Dialog_Char.BUGGIEN:
                _personagem.transform.localScale = new Vector3(150, 150, 0);
                _personagem.flipX = false;
                _personagem.sprite = _imagemPersonagens[0];
                break;
            case Dialog_Char.FURBOT:
                _personagem.transform.localScale = new Vector3(50, 50, 1);
                _personagem.flipX = true;
                _personagem.sprite = _imagemPersonagens[1];
                break;
            case Dialog_Char.IMPERADOR_BUGGIEN:
                _personagem.transform.localScale = new Vector3(30, 30, 1);
                _personagem.flipX = true;
                _personagem.sprite = _imagemPersonagens[2];
                break;
            case Dialog_Char.PROFESSORA:
                _personagem.transform.localScale = new Vector3(25, 25, 0);
                _personagem.flipX = true;
                _personagem.sprite = _imagemPersonagens[3];
                break;
            case Dialog_Char.S223:
                _personagem.transform.localScale = new Vector3(17, 17, 0);
                _personagem.flipX = true;
                _personagem.sprite = _imagemPersonagens[4];
                break;
        }
    }
}

public enum Dialog_Char
{
    PROFESSORA, FURBOT, S223, BUGGIEN, IMPERADOR_BUGGIEN
}
