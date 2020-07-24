using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //Booleans
    [SerializeField]
    private bool _isMoving, _mobile, _canMove, _primeiraVez, _podeTrocarRegiao, _devModeSetter;
    public bool miniGames, quizJogado, pescaJogado;
    public static bool devMode = true, isInterfaceTangivel = true;
    private static bool _carregouDados;

    //Integers (SERIALIZED)
    [SerializeField]
    private int _fasePisando, _regiao;
    public static int sceneIndex, ultimaFaseLiberada, faseAtual;

    //GameObjects (SERIALIZED)
    [SerializeField]
    private GameObject _furbot, _fases;
    [SerializeField]
    private GameObject[] _regioes;

    //Objetos (SERIALIZED)
    [SerializeField]
    private Text _faseDisplay;
    [SerializeField]
    private GameObject _mobileButtons;
    [SerializeField]
    private Button _btVoltaRegiao, _btProximaRegiao, _btVoltarMenuLogin;
    [SerializeField]
    private ContainerSelecao _containerReferencias;
    [SerializeField]
    private GameObject _loja;
#if UNITY_ANDROID || UNITY_IOS
    [SerializeField]
    private Button _btMoveLeft, _btMoveRight, _btOk;
#endif

    //Objetos
    private static LevelManager _instance; //Singleton
    private Image _imagemRegiao;
    private GameManager _gm;
    private SpriteRenderer _srFurbot;
    private Animator _fade;
    private Vector3 _newPosition, _oldPosition;
    private GameObject[] _circuloFases;
    private Animator _anim;
    private Furbot _furbott;
    public GameObject[] fases = new GameObject[1];
    public Text pontuacaoLoja;
    public string codigoIntermediario;
    public GameObject txtDevMode;

    public void Awake()
    {
        Application.targetFrameRate = 60;
        if (_devModeSetter)
        {
            devMode = true;
        }
        txtDevMode.SetActive(devMode);
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
        ReconstruirReferencias();
        VerificarFasesAcessiveis();
        _btVoltaRegiao.gameObject.SetActive(false);
        _canMove = true;
        if (ultimaFaseLiberada == 0)
        {
            StartCoroutine(PrimeiroPasso());
        }
        _btProximaRegiao.gameObject.SetActive(false);
#if UNITY_ANDROID
        VerificarSeHabilitaBotoes();
#endif
    }

    void Start()
    {
        isInterfaceTangivel = Convert.ToBoolean(PlayerPrefs.GetInt("isInterfaceTangivel"));
        DontDestroyOnLoad(this);
    }

    public int GetFasePisando()
    {
        return _fasePisando;
    }

    private IEnumerator PrimeiroPasso()
    {
        yield return new WaitForSeconds(0.5f);
        MoverProximaFase();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("SelecaoDeFases"))
        {
            if (_furbot != null)
            {
                _oldPosition = _furbot.transform.position;
                _newPosition = transform.position;

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    MoverProximaFase();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (_fasePisando != 0 && _regiao == 0 || _fasePisando != 0 && _regiao >= 1)
                    {
                        MoverFaseAnterior();
                    }

                }
                if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !_loja.activeSelf)
                {
                    if (_fasePisando < 1 && _regiao == 0)
                    {
                        _fade.SetTrigger("FadeOut");
                        StartCoroutine(EsperarFade("TutorialProjecao"));
                    }
                    else
                    {
                        CarregarFase(fases[_fasePisando].gameObject.name);
                    }
                }
            }
            else
            {
                ReconstruirReferencias();
            }
        }
    }

#if UNITY_ANDROID
    void VerificarSeHabilitaBotoes()
    {
        if (!devMode)
        {
            // _btMoveRight.interactable = _regiao == 0 ? (_fasePisando - 1 <= ultimaFaseLiberada || devMode):(_fasePisando + (_regiao * 5) <= ultimaFaseLiberada || devMode);
            //_btMoveLeft.interactable = _fasePisando > 0;
        }
    }
#endif

    private IEnumerator Mover(Vector3 oldPosition, Vector3 newPosition)
    {
        float startTime = Time.time;
        float endTime = startTime + 0.75f;
        float moveTime = 0.75f;
        while (Time.time < endTime)
        {
            _furbot.transform.position = Vector3.Lerp(oldPosition, newPosition, (Time.time - startTime) / moveTime);
            yield return null;
        }
        _anim.speed = 0;
        _anim.SetBool("Andar_Direita", false);
        _anim.SetBool("Andar_Esquerda", false);
    }

    private IEnumerator MoverCooldown()
    {
        yield return new WaitForSeconds(0.55f);
        _canMove = true;
    }

    public void MoverProximaFase()
    {
        if (_canMove)
        {
            if (_fasePisando + 1 < fases.Length)
            {
                _fasePisando++;
                faseAtual++;
                _newPosition = new Vector3(fases[_fasePisando].transform.position.x, fases[_fasePisando].transform.position.y, 0);
                _faseDisplay.text = fases[_fasePisando].gameObject.name;
                _anim.speed = 1;
#if UNITY_ANDROID
                //VerificarSeHabilitaBotoes();
#endif
                switch (_regiao)
                {
                    case 0:
                        if (_fasePisando < 4)
                        {
                            _anim.SetBool("Andar_Direita", true);
                        }
                        else
                        {
                            _anim.SetBool("Andar_Esquerda", true);
                        }
                        if (_fasePisando > 0)
                        {
                            _faseDisplay.text = fases[_fasePisando].gameObject.name;
                        }
                        else
                        {
                            _faseDisplay.text = "Laboratório";
                        }
                        break;
                    case 1:
                        if (_fasePisando == 4)
                        {

                            _anim.SetBool("Andar_Esquerda", true);
                        }
                        else
                        {
                            _anim.SetBool("Andar_Direita", true);

                        }
                        break;
                    case 2:
                        if (_fasePisando == 2)
                        {
                            _anim.SetBool("Andar_Direita", true);

                        }
                        if (_fasePisando == 3)
                        {

                            _anim.SetBool("Andar_Esquerda", true);
                        }
                        break;
                    case 3:
                        if (_fasePisando == 4)
                        {

                            _anim.SetBool("Andar_Esquerda", true);
                        }
                        else
                        {
                            _anim.SetBool("Andar_Direita", true);

                        }
                        break;
                    default:
                        break;
                }
                _isMoving = true;
                _canMove = false;
            }
        }
        if (_isMoving)
        {
            StartCoroutine(Mover(_oldPosition, _newPosition));
            StartCoroutine(MoverCooldown());
            _isMoving = false;
        }
    }

    public void MoverFaseAnterior()
    {
        if (_canMove)
            if (_fasePisando - 1 >= 0)
            {
                _anim.SetTrigger("Andar");
                _fasePisando--;
                faseAtual--;
                _newPosition = new Vector3(fases[_fasePisando].transform.position.x, fases[_fasePisando].transform.position.y, 0);
                _faseDisplay.text = fases[_fasePisando].gameObject.name;
                _anim.speed = 1;
#if UNITY_ANDROID
                //  VerificarSeHabilitaBotoes();
#endif
                switch (_regiao)
                {
                    case 0:
                        if (_fasePisando > 2)
                        {
                            _anim.SetBool("Andar_Direita", true);
                        }
                        else
                        {
                            _anim.SetBool("Andar_Esquerda", true);
                        }
                        if (_fasePisando > 0)
                        {
                            _faseDisplay.text = fases[_fasePisando].gameObject.name;
                        }
                        else
                        {
                            _faseDisplay.text = "Laboratório";
                        }
                        break;
                    case 1:
                        if (_fasePisando == 3)
                        {
                            _anim.SetBool("Andar_Direita", true);
                        }
                        else if (_fasePisando == 2)
                        {
                            _anim.SetBool("Andar_Esquerda", true);
                        }
                        else if (_fasePisando == 0)
                        {
                            _anim.SetBool("Andar_Esquerda", true);
                        }
                        break;
                    case 2:
                        if (_fasePisando == 3 || _fasePisando == 0 || _fasePisando == 2)
                        {
                            _anim.SetBool("Andar_Direita", true);
                        }
                        else
                        {
                            _anim.SetBool("Andar_Esquerda", true);
                        }
                        break;
                    case 3:
                        if (_fasePisando == 3)
                        {
                            _anim.SetBool("Andar_Direita", true);
                        }
                        else
                        {
                            _anim.SetBool("Andar_Esquerda", true);
                        }
                        break;
                    default:
                        break;
                }
                _isMoving = true;
                _canMove = false;
            }
            else
            {
                //_fasePisando = ultimaFaseLiberada % 5; tirar bug da selecao de fases
                _newPosition = new Vector3(fases[0].transform.position.x, fases[0].transform.position.y, 0);
                if (_regiao == 0)
                {
                    _faseDisplay.text = "Laboratório";
                }
                else
                {
                    _faseDisplay.text = fases[_fasePisando].gameObject.name;
                }
                _anim.SetBool("Andar_Esquerda", true);
                _isMoving = true;
            }
        if (_isMoving)
        {
            StartCoroutine(Mover(_oldPosition, _newPosition));
            StartCoroutine(MoverCooldown());
            _isMoving = false;
        }
    }

    public void OkButton()
    {
        CarregarFase(fases[_fasePisando].gameObject.name);
    }

    void CarregarFase(string fase)
    {
        if (_regiao == 0)
        {
            if (_fasePisando >= 1)
            {
                if (_fasePisando - 1 <= ultimaFaseLiberada || devMode)
                {
                    _fade.SetTrigger("FadeOut");
                    StartCoroutine(EsperarFade(fase));
                }
                else
                {
                    _faseDisplay.text = "Esta fase está bloqueada!";
                    Debug.Log("Esta fase está bloqueada!");
                }
            }
        }
        else
        {
            if (_fasePisando >= 0)
            {
                if (_fasePisando + (_regiao * 5) <= ultimaFaseLiberada || devMode)
                {
                    _fade.SetTrigger("FadeOut");
                    StartCoroutine(EsperarFade(fase));
                }
                else
                {
                    _faseDisplay.text = "Esta fase está bloqueada!";
                    Debug.Log("Esta fase está bloqueada!");
                }
            }
        }
    }

    private IEnumerator EsperarFade(string fase)
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(fase);
    }

    private IEnumerator FadeOutFadeIn(char acao)
    {
        _podeTrocarRegiao = false;
        _fade.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        if (acao == 'p')
        {
            ProximaRegiao();
        }
        else if (acao == 'v')
        {
            VoltaRegiao();
        }
        _fade.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1);
        _podeTrocarRegiao = true;
    }

    public void PassarDeFase()
    {
        if (faseAtual > ultimaFaseLiberada)
        {
            if ((ultimaFaseLiberada + 1 == 5) || (ultimaFaseLiberada + 1 == 10) || (ultimaFaseLiberada + 1 == 15))
            {
                QuizController.ReiniciarPerguntasSorteadas();
            }
            ultimaFaseLiberada++;
            _gm.ReiniciarContadores();
        }
    }



    public void CarregarMiniGame()
    {
        if (faseAtual <= 6)
            SceneManager.LoadScene("MiniGame_Quiz");
        else if (faseAtual <= 11)
            SceneManager.LoadScene("MiniGame_Lixo");
        else if (faseAtual <= 16)
            SceneManager.LoadScene("MiniGame_Iglu");
        else if (faseAtual <= 20)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        _furbott.Reiniciar();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("SelecaoDeFases"))
        {
            ReconstruirReferencias();
            _btProximaRegiao.gameObject.SetActive(false);
            _fade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
            _podeTrocarRegiao = true;
            _regiao = 0;
            if (ultimaFaseLiberada > 4)
            {
                if (ultimaFaseLiberada < 10)
                {
                    ProximaRegiao();
                }
                else if (ultimaFaseLiberada < 15)
                {
                    ProximaRegiao();
                    ProximaRegiao();
                }
                else if (ultimaFaseLiberada < 20)
                {
                    ProximaRegiao();
                    ProximaRegiao();
                    ProximaRegiao();
                }
            }
            if (devMode && _regiao <= 2)
            {
                _btProximaRegiao.gameObject.SetActive(true);
            }
            if (ultimaFaseLiberada > 0)
            {
                if (_regiao == 0)
                {
                    _fasePisando = ultimaFaseLiberada % 5 + 1;
                }
                else
                {
                    _fasePisando = ultimaFaseLiberada % 5;
                }
                _furbot.transform.SetPositionAndRotation(new Vector3(fases[_fasePisando].transform.position.x, fases[_fasePisando].transform.position.y), Quaternion.identity);
                _faseDisplay.text = fases[_fasePisando].gameObject.name;
                if (_regiao == 0)
                {
                    faseAtual = (5 * _regiao) + (_fasePisando);
                }
                else
                {
                    faseAtual = (5 * _regiao) + (_fasePisando + 1);
                }
            }
            else
            {
                _fasePisando = 0;
                StartCoroutine(PrimeiroPasso());
            }
            if (_regiao == 0)
                _btVoltaRegiao.gameObject.SetActive(false);
            VerificarFasesAcessiveis();
        }
    }

    private void ReconstruirReferencias()
    {
        _fases = GameObject.Find("Fases");
        _furbot = GameObject.Find("FurbotFases");
        _srFurbot = _furbot.GetComponent<SpriteRenderer>();
        _anim = _furbot.GetComponent<Animator>();
        _containerReferencias = GameObject.Find("Container").GetComponent<ContainerSelecao>();
        _regioes = _containerReferencias.GetRegioes();
        fases = _containerReferencias.GetFases(_regiao);
        _imagemRegiao = _containerReferencias.GetImagemComponent();
        _circuloFases = _containerReferencias.circuloFases;
        _btProximaRegiao = _containerReferencias._btnProximaRegiao;
        _btVoltaRegiao = _containerReferencias._btnVoltaRegiao;
        _btVoltarMenuLogin = _containerReferencias._btnVoltarMenuLogin;
        _loja = _containerReferencias.loja;
        pontuacaoLoja = _containerReferencias.pontuacaoLoja;
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _btProximaRegiao.onClick.AddListener(BtnProximaRegiao);
        _btVoltaRegiao.onClick.AddListener(BtnVoltaRegiao);
        _btVoltarMenuLogin.onClick.AddListener(_gm.Voltar);
        _fade = GameObject.Find("Fade").GetComponent<Animator>();
        _faseDisplay = GameObject.Find("TextoFase").GetComponent<Text>();
        _faseDisplay.text = "Selecione uma fase!";
        pontuacaoLoja.text = _gm.pontuacaoTotal.ToString();
#if UNITY_ANDROID
        if (_mobileButtons == null)
        {
            _mobileButtons = _containerReferencias.mobileButtons;
        }
        _mobileButtons.SetActive(true);
        _btMoveLeft = GameObject.Find("btMoveLeft").GetComponent<Button>();
        _btMoveRight = GameObject.Find("btMoveRight").GetComponent<Button>();
        _btOk = GameObject.Find("btOk").GetComponent<Button>();
        _btMoveLeft.onClick.AddListener(MoverFaseAnterior);
        _btMoveRight.onClick.AddListener(MoverProximaFase);
        _btOk.onClick.AddListener(OkButton);
#endif
    }

    private void VerificarFasesAcessiveis()
    {
        for (int i = 0; i <= ultimaFaseLiberada; i++)
        {
            _circuloFases[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
    }

    public void BtnProximaRegiao()
    {
        if (_podeTrocarRegiao)
        {
            EventSystem.current.SetSelectedGameObject(null, null);
            if (_regiao + 1 == 1)
            {
                faseAtual = 6;
            }
            else if (_regiao + 1 == 2)
            {
                faseAtual = 11;
            }
            else if (_regiao + 1 == 3)
            {
                faseAtual = 16;
            }
            StartCoroutine(FadeOutFadeIn('p'));

        }
    }

    private void ProximaRegiao()
    {
        if (_regiao + 1 < 4)
        {
            _regioes[_regiao++].SetActive(false);
            _regioes[_regiao].SetActive(true);
            if (_regiao > 0)
            {
                _fasePisando = 0;
                if (_regiao == 1)
                {
                    if (ultimaFaseLiberada > 9 || devMode)
                    {
                        _btProximaRegiao.gameObject.SetActive(true);
                    }
                    else
                    {
                        _btProximaRegiao.gameObject.SetActive(false);
                    }
                }
                else if (_regiao == 2)
                {
                    if (ultimaFaseLiberada > 14 || devMode)
                    {
                        _btProximaRegiao.gameObject.SetActive(true);
                    }
                    else
                    {
                        _btProximaRegiao.gameObject.SetActive(false);
                    }
                }
                else if (_regiao == 3)
                {
                    _btProximaRegiao.gameObject.SetActive(false);
                    _imagemRegiao.transform.localScale = new Vector3(-1, 1, 0);
                }
            }
            _imagemRegiao.sprite = _containerReferencias.GetImagemRegiao(_regiao);
            fases = _containerReferencias.GetFases(_regiao);
            _furbot.transform.SetPositionAndRotation(new Vector3(fases[0].transform.position.x, fases[0].transform.position.y, 0), Quaternion.identity);
            _faseDisplay.text = fases[_fasePisando].gameObject.name;
            _canMove = true;
            _btVoltaRegiao.gameObject.SetActive(true);
            _btVoltaRegiao.onClick.RemoveAllListeners();
            _btVoltaRegiao.onClick.AddListener(BtnVoltaRegiao);
            VerificarFasesAcessiveis();
        }
    }

    public void BtnVoltaRegiao()
    {
        if (_podeTrocarRegiao)
        {
            EventSystem.current.SetSelectedGameObject(null, null);
            StartCoroutine(FadeOutFadeIn('v'));
        }
    }

    private void VoltaRegiao()
    {
        if (_regiao - 1 >= 0)
        {
            Debug.Log("Regiao Anterior");
            _regioes[_regiao--].SetActive(false);
            _regioes[_regiao].SetActive(true);
            _imagemRegiao.sprite = _containerReferencias.GetImagemRegiao(_regiao);
            _imagemRegiao.transform.localScale = new Vector3(1, 1, 0);
            fases = _containerReferencias.GetFases(_regiao);
            _btProximaRegiao.gameObject.SetActive(true);
            _furbot.transform.SetPositionAndRotation(new Vector3(fases[0].transform.position.x, fases[0].transform.position.y, 0), Quaternion.identity);
            if (_regiao == 0)
            {
                _faseDisplay.text = "Laboratório";
                _btVoltaRegiao.gameObject.SetActive(false);
                faseAtual = 0;
                _fasePisando = 0;
            }
            else
            {
                if (_regiao == 1)
                {
                    faseAtual = 6;
                }
                else if (_regiao == 2)
                {
                    faseAtual = 11;
                }
                _fasePisando = 0;
                _faseDisplay.text = fases[_fasePisando].gameObject.name;
            }
            VerificarFasesAcessiveis();
            _canMove = true;
        }
    }

    public void GameOver()
    {
        _gm.ReiniciarContadores();
        switch (_regiao)
        {
            case 0:
                ultimaFaseLiberada = 0;
                _fasePisando = -1;
                return;
            case 1:
                ultimaFaseLiberada = 5;
                break;
            case 2:
                ultimaFaseLiberada = 10;
                break;
            case 3:
                ultimaFaseLiberada = 15;
                break;
        }
        _fasePisando = 0;
    }
}
