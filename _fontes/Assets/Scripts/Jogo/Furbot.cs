using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Furbot : MonoBehaviour
{
    //Bools
    private bool _condicaoRetorno, _teclado, _faseGerada, _defCenarioAcima, _defCenarioAbaixo, _defCenarioEsq, _defCenarioDir;
    public bool isMoving, venceu, emExecucao, testando, usarAlavanca, bateu;

    [SerializeField]
    private bool _mainAspect;
    private bool _primeiroPasso;

    //Integers (SERIALIZED)
    [SerializeField]
    public int vidas, _energiaRessarcivel, _resW, _resH;
    public int energia, tesouros;

    //Floats
    private float _step, _step2; //Variavel que define o tamanho de cada passo do furbot
    [SerializeField]
    private float _stepAtual;

    //Strings
    private string objetoColididoNoFurbot, _defColetadoAcima, _defColetadoAbaixo, _defColetadoEsq, _defColetadoDir, _textoGabarito;
    public string direcaoAtual, terrenoAtual, backupTexto;

    //GameObjects (SERIALIZED)
    [SerializeField]
    private GameObject _pontoEnergiaPrefab, _pontoVidaPrefab2, _pontoVidaPrefab, _ponto500Prefab, _sensorDireitaObj, _sensorEsquerdaObj, _sensorAcimaObj, _sensorAbaixoObj;

    //Objetos
    private S223 _s223;
    private SpriteRenderer _spriteRenderer;
    private UIManager _uiManager;
    private GameManager _gameManager;
    private LevelManager _levelManager;
    public PuzzleManager puzzleManager;
    public GameObject alavancaAtual;
    public Compilador compilador;
    private Analisador _analisador;
    private Porta _porta;
    private Furbot_Animation _animation;
    private Tilemap _mapa;
    private Resolution _res;
    private Sensor _sensorDireita, _sensorEsquerda, _sensorAcima, _sensorAbaixo;

    // Vector3
    private Vector3 _posicaoAtual, _posicaoInicial;
    public Vector3 oldPosition;

    void Start()
    {
#if !UNITY_EDITOR //Calcula com base na resolucao o step do furbot quando estiver rodando em um dispositivo
        _res = Screen.currentResolution;
        //float aspect = (float)_res.width / _res.height;
        _resH = _res.height;
        _resW = _res.width;
        if (_resW == 1280 && _resH == 800)
        {
            _step = 0.92f;
            _step2 = _step - 0.02f;

        } else if (_resW == 2048 && _resH == 1536)
        {
            _step = 0.92f;
            _step2 = _step - 0.01f;
        }
        else
        {
            _mainAspect = true;
            _step = 1f;
        }
#else
        //Configurar manualmente o step quando estiver no editor
        /*_step = 1f;
        _mainAspect = true;
        testando = false;
        if (testando)
        {
            _mainAspect = false;
            _step = 1f;
            _step2 = _step - 0.01f;
        }*/
#endif
        _stepAtual = 1f;// _step;
        _uiManager = GameObject.Find("CanvasInterface").GetComponent<UIManager>();
        _primeiroPasso = true;
        if (GameObject.Find("GameManager") != null)
        {
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            _levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            vidas = _gameManager.vida;
            energia = _gameManager.energia;
            tesouros = _gameManager.contadorTesouro;
            _uiManager.AtualizarQntEnergia(_gameManager.energia);
            _uiManager.AtualizarVida(_gameManager.vida);
            _uiManager.AtualizarQntTesouros(_gameManager.contadorTesouro);
            _uiManager.AtualizarPontuacao();
            _gameManager.personagemSelecionado = gameObject;
        }
        else
        {
            _faseGerada = true;
            vidas = 3;
            energia = 100;
            _uiManager.AtualizarQntEnergia(energia);
            _uiManager.AtualizarVida(vidas);
        }
        oldPosition = new Vector3(this.GetComponent<Transform>().position.x, this.GetComponent<Transform>().position.y,
            this.GetComponent<Transform>().position.z);
        _analisador = GetComponent<Analisador>();
        _s223 = GameObject.Find("S-223").GetComponent<S223>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _mapa = GameObject.Find("TilemapMoldura").GetComponent<Tilemap>();
        _teclado = false;
        isMoving = false;
        venceu = false;
        _energiaRessarcivel = 0;
        _posicaoInicial = ConverterPosMundoParaPosCell(transform.position);
        _sensorDireita = _sensorDireitaObj.GetComponent<Sensor>();
        _sensorEsquerda = _sensorEsquerdaObj.GetComponent<Sensor>();
        _sensorAcima = _sensorAcimaObj.GetComponent<Sensor>();
        _sensorAbaixo = _sensorAbaixoObj.GetComponent<Sensor>();
        _defCenarioDir = _sensorDireita.obstaculo;
        _defCenarioEsq = _sensorEsquerda.obstaculo;
        _defCenarioAcima = _sensorAcima.obstaculo;
        _defCenarioAbaixo = _sensorAbaixo.obstaculo;
        _defColetadoDir = _sensorDireita.ColetadoDoTrigger;
        _defColetadoEsq = _sensorEsquerda.ColetadoDoTrigger;
        _defColetadoAcima = _sensorAcima.ColetadoDoTrigger;
        _defColetadoAbaixo = _sensorAbaixo.ColetadoDoTrigger;
        _animation = GetComponent<Furbot_Animation>();
        direcaoAtual = "Parado";
        _analisador.VarreduraDeMapa();
        _porta = null;
        emExecucao = false;
        usarAlavanca = false;
        if (GameObject.Find("PuzzleManager") != null)
        {
            puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        }
        PontuacaoController.SetTotalTesouros(_analisador.totalTesouros);
        compilador = GetComponent<Compilador>();
    }

    void Update()
    {
        if (!_uiManager.emDialogo)
        {
            //USO PARA TESTES//
            if (Input.GetKeyDown(KeyCode.F9) && LevelManager.devMode)
            {
                _teclado = true;
                _textoGabarito = "";
                Debug.Log("Teclado liberado");
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log(_analisador.GerarTexto());
            }
            if (_teclado && !isMoving)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    _uiManager.ifCodigo.text += "andar(DIREITA);\n";
                    _textoGabarito += "andar(direita);";
                    Andar(Direcao.DIREITA);
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    _uiManager.ifCodigo.text += "andar(ESQUERDA);\n";
                    _textoGabarito += "andar(esquerda);";
                    Andar(Direcao.ESQUERDA);
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _uiManager.ifCodigo.text += "andar(ACIMA);\n";
                    _textoGabarito += "andar(acima);";
                    Andar(Direcao.ACIMA);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _uiManager.ifCodigo.text += "andar(ABAIXO);\n";
                    _textoGabarito += "andar(abaixo);";
                    Andar(Direcao.ABAIXO);
                }

            }
        }

        /*if (isMoving && _uiManager.ifCodigo.verticalScrollbar.size < 1)
        {
            _uiManager.ifCodigo.verticalScrollbar.value = _uiManager.valorScrollBar;
        }*/
    }

    public Sensor GetSensor(Direcao dir)
    {
        switch (dir)
        {
            case Direcao.ABAIXO: return _sensorAbaixo;
            case Direcao.ACIMA: return _sensorAcima;
            case Direcao.DIREITA: return _sensorDireita;
            case Direcao.ESQUERDA: return _sensorEsquerda;
            default: throw new Exception("Sensor inválido");
        }
    }

    /// <summary>
    /// Método que converte a posição do espaço de mundo para posição da célula que o objeto se encontra.
    /// </summary>
    /// <param name="posMundo">Posição do espaço de mundo do objeto.</param>
    /// <returns></returns>
    public Vector3 ConverterPosMundoParaPosCell(Vector3 posMundo)
    {
        Vector3Int v = _mapa.WorldToCell(posMundo);
        Vector3 posCell = _mapa.GetCellCenterWorld(v);
        return posCell + new Vector3(0, 0.4f, 0f);
    }

    /// <summary>
    /// Movimenta o Furbot conforme a direção passada por parâmetro.
    /// </summary>
    /// <param name="direcao">Direção a qual o Furbot vai andar</param>
    public void Andar(Direcao direcao)
    {
        if (!isMoving && !_uiManager.emDialogo && !VaiBater(direcao))
        {
            _posicaoAtual = ConverterPosMundoParaPosCell(this.GetComponent<Collider2D>().bounds.center);

            Vector3 newPosition = Vector3.zero;
            switch (direcao)
            {
                case Direcao.ABAIXO:
                    direcaoAtual = "Abaixo";
                    newPosition = _posicaoAtual - new Vector3(0, _stepAtual, 0);
                    _animation.MoverAbaixo();
                    _s223.SetSprite(Direcao.ABAIXO);
                    break;
                case Direcao.ACIMA:
                    direcaoAtual = "Acima";
                    newPosition = _posicaoAtual + new Vector3(0, _stepAtual, 0);
                    _animation.MoverAcima();
                    _s223.SetSprite(Direcao.ACIMA);
                    break;
                case Direcao.DIREITA:
                    direcaoAtual = "Direita";
                    newPosition = _posicaoAtual + new Vector3(_stepAtual, 0, 0);
                    _animation.MoverDireita();
                    _s223.SetSprite(Direcao.DIREITA);
                    break;
                case Direcao.ESQUERDA:
                    direcaoAtual = "Esquerda";
                    newPosition = _posicaoAtual - new Vector3(_stepAtual, 0, 0);
                    _animation.MoverEsquerda();
                    _s223.SetSprite(Direcao.ESQUERDA);
                    break;
                default: return;// tratamento de exceções
            }
            StartCoroutine(Mover(_posicaoAtual, newPosition));
        }
    }

    public bool VaiBater(Direcao dir)
    {
        if (GetSensor(dir).obstaculo)
        {
            if (GetSensor(dir).ContemCollider("Buggien"))
            {
                BuggienAndroid buggien = GetSensor(dir).ObjColetado.GetComponent<BuggienAndroid>();
                if (buggien.GetEstado() == BuggienAndroid.Estado.Empurravel)
                {
                    if (!buggien.VaiBater(dir))
                    {
                        buggien.Andar(dir);
                        return false;

                    }
                    else
                    {
                        bateu = true;
                        Bater(GetSensor(dir).ColetadoDoTrigger);
                        _uiManager.ifCodigo.interactable = false;
                        return true;
                    }
                }
            }
            bateu = true;
            Bater(GetSensor(dir).ColetadoDoTrigger);
            _uiManager.ifCodigo.interactable = false;
            return true;
        }
        else return false;
    }

    public bool IsPrimeiroPasso()
    {
        return _primeiroPasso;
    }

    public void SetPrimeiroPasso(bool valor)
    {
        _primeiroPasso = valor;
    }

    public void SetPosicaoInicial(Vector3 posicao)
    {
        _posicaoInicial = posicao;
    }

    private IEnumerator Mover(Vector3 originalPosition, Vector3 newPosition)
    {
        if (terrenoAtual.Equals("caminho"))
        {
            DecrementarEnergia(1);
            _energiaRessarcivel++;
        }
        else
        {
            DecrementarEnergia(5);
        }
        isMoving = true;
        float startTime = Time.time;
        float endTime = startTime + 0.5f;
        float moveTime = 0.5f;
        while (Time.time < endTime)
        {
            transform.position = Vector3.Lerp(originalPosition, newPosition, (Time.time - startTime) / moveTime);
            yield return null;
        }
        _animation.Parar();
        isMoving = false;
    }



    public void Bater(string objeto)
    {
        int index = compilador.GetIndex();
        switch (objeto.ToLower())
        {

            case "laboratorio":
                DecrementarEnergia(25);
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Ei! Acho que não é por aí!"));
                _analisador.IncrementarLocaisIndevidos();
                energia += _energiaRessarcivel;
                _gameManager.energia = energia;
                _uiManager.PararExecucao();
                break;

            case "pedra":
                DecrementarEnergia(25);
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Cuidado! Você bateu em uma pedra!"));
                _analisador.IncrementarLocaisIndevidos();
                energia += _energiaRessarcivel;
                _gameManager.energia = energia;
                _uiManager.PararExecucao();
                break;

            case "arbusto":
                DecrementarEnergia(25);
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Cuidado! Você bateu em um arbusto!"));
                _analisador.IncrementarLocaisIndevidos();
                energia += _energiaRessarcivel;
                _gameManager.energia = energia;
                _uiManager.PararExecucao();
                break;

            case "arvore":
                DecrementarEnergia(25);
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Cuidado! Você bateu em uma árvore!"));
                _analisador.IncrementarLocaisIndevidos();
                energia += _energiaRessarcivel;
                _gameManager.energia = energia;
                _uiManager.PararExecucao();
                break;

            case "lixo":
                DecrementarEnergia(25);
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Cuidado! Você bateu em um lixo!"));
                _analisador.IncrementarLocaisIndevidos();
                energia += _energiaRessarcivel;
                _gameManager.energia = energia;
                _uiManager.PararExecucao();
                break;

            case "caixa":
                DecrementarEnergia(25);
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Cuidado! Você bateu em uma caixa!"));
                _analisador.IncrementarLocaisIndevidos();
                energia += _energiaRessarcivel;
                _gameManager.energia = energia;
                _uiManager.PararExecucao();
                break;

            case "meleca":
                DecrementarEnergia(25);
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Cuidado com a meleca!"));
                _analisador.IncrementarLocaisIndevidos();
                energia += _energiaRessarcivel;
                _gameManager.energia = energia;
                _uiManager.PararExecucao();
                break;

            case "paredepiramide":
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Ei! Acho que não é por aí!"));
                _uiManager.PararExecucao();
                break;

            case "parede":
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Ei! Acho que não é por aí!"));
                _uiManager.PararExecucao();
                break;

            case "agua":
                DecrementarVida();
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Fique longe da água! ela é perigosa para os seus circuitos."));
                StartCoroutine(MoverParaInicio());
                _analisador.IncrementarLocaisIndevidos();
                _gameManager.energia = energia;
                _uiManager.PararExecucao();
                break;

            case "buggien":
                DecrementarEnergia(25);
                StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Ei! cuidado com os buggiens!"));
                _analisador.IncrementarBatidas();
                _uiManager.AddLog("O Furbot bateu em um Buggien");
                break;

        }
        _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(index);


        _uiManager.btnExecutar.GetComponentInChildren<Text>().text = "EXECUTAR";
    }

    private void DecrementarEnergia(int energia)
    {
        if (this.energia - energia > 0)
        {
            this.energia -= energia;
            _uiManager.AtualizarQntEnergia(this.energia);
        }
        else
        {
            DecrementarVida();
        }
    }

    private void DecrementarVida()
    {
        if (vidas - 1 >= 0)
        {
            vidas--;
            _gameManager.vida = this.vidas;
            RecuperarEnergia();
            _gameManager.energia = this.energia;
        }
        else
        {
            GameOver();
        }
        _uiManager.AtualizarVida(vidas);
    }

    public string GetTextoGabarito()
    {
        return _textoGabarito;
    }


    public void AbrirCaixaDeErro(String message, out bool PodeLer)
    {
        PodeLer = false;
        //_uiManager.debug.SetActive(true);
        _uiManager.dialog.SetActive(true);
        _uiManager.dialogPanel.MostrarTexto(Dialog_Char.S223, message);
        Debug.Log(message);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag.ToLower())
        {
            case "caminho":
                terrenoAtual = "caminho";
                break;
            case "coletavel":
                _analisador.IncrementarColetavel();
                _gameManager.AddItemColetavel(other.gameObject);
                other.gameObject.SetActive(false);
                break;
            case "porta":
                _porta = other.gameObject.GetComponent<Porta>();
                if (_porta.estado.Equals("aberta"))
                {
                    DecrementarVida();
                    StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Fique longe da gosma! Ela é mais perigosa que a água."));
                    StartCoroutine(MoverParaInicio());
                    _analisador.IncrementarLocaisIndevidos();
                    _gameManager.energia = energia;
                    _uiManager.PararExecucao();
                }
                break;
            case "tesouro":
                _analisador.IncrementarTesouro();
                this.tesouros++;
                Instantiate(_ponto500Prefab, new Vector2(other.transform.position.x, other.transform.position.y), Quaternion.identity);
                if (!_faseGerada)
                {
                    _gameManager.AddItemColetavel(other.gameObject);
                    PontuacaoController.pontosFase += PontuacaoController.pontosPorTesouro;
                }
                other.gameObject.SetActive(false);
                _uiManager.AtualizarQntTesouros(tesouros);
                _uiManager.AtualizarPontuacao();
                _uiManager.AddLog("O Furbot coletou um Tesouro");
                break;
            case "energia":
                _analisador.IncrementarEnergia();
                if (this.energia + 30 > 100)
                {
                    StartCoroutine(AdicionarEnergia(100 - energia));
                }
                else
                {
                    StartCoroutine(AdicionarEnergia(30));
                    // GameObject anim = Instantiate(_pontoEnergiaPrefab, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity, transform);
                    // anim.transform.localScale = new Vector2(0.045f, 0.045f);
                    // Destroy(anim, 2.0f);
                }
                //_uiManager.AtualizarQntEnergia(this.energia);
                _gameManager.AddItemColetavel(other.gameObject);
                other.gameObject.SetActive(false);
                _uiManager.AddLog("O Furbot coletou Energia");
                break;
            case "vida":
                if (this.vidas < 3)
                {
                    _analisador.IncrementarVida();
                    this.vidas++;
                    GameObject anim = Instantiate(_pontoVidaPrefab, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity, transform);
                    anim.transform.localScale = new Vector2(0.045f, 0.045f);
                    Destroy(anim, 2.0f);
                    _uiManager.AtualizarVida(this.vidas);
                }
                else
                {
                    PontuacaoController.pontosFase += PontuacaoController.pontosPorVida;
                    _uiManager.AtualizarPontuacao();
                    GameObject anim = Instantiate(_pontoVidaPrefab2, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity, transform);
                    anim.transform.localScale = new Vector2(0.045f, 0.045f);
                    Destroy(anim, 2.0f);
                }
                _gameManager.AddItemColetavel(other.gameObject);
                other.gameObject.SetActive(false);
                _uiManager.AddLog("O Furbot coletou Vida");
                break;
            case "vitoria":
                _gameManager.AddItemColetavel(other.gameObject);
                PontuacaoController.pontosFase += PontuacaoController.pontosPorPista;
                _uiManager.AtualizarPontuacao();
                if (_gameManager.ChecarSucesso(other.tag))
                {
                    Sucesso();
                }
                Debug.Log(_uiManager.GetTexto());
                other.gameObject.SetActive(false);
                _analisador.SetCodigoExtenso(_textoGabarito);
                break;
            case "papiro01":
                _gameManager.AddItemColetavel(other.gameObject);
                _gameManager.AddItemInventario(0);
                PontuacaoController.pontosFase += PontuacaoController.pontosPorPista;
                _uiManager.AtualizarPontuacao();
                if (_gameManager.ChecarSucesso(other.tag))
                {
                    Sucesso();
                }
                Debug.Log(_uiManager.GetTexto());
                other.gameObject.SetActive(false);
                _analisador.SetCodigoExtenso(_textoGabarito);
                break;
            case "papiro02":
                _gameManager.AddItemColetavel(other.gameObject);
                _gameManager.AddItemInventario(1);
                PontuacaoController.pontosFase += PontuacaoController.pontosPorPista;
                _uiManager.AtualizarPontuacao();
                if (_gameManager.ChecarSucesso(other.tag))
                {
                    Sucesso();
                }
                Debug.Log(_uiManager.GetTexto());
                other.gameObject.SetActive(false);
                _analisador.SetCodigoExtenso(_textoGabarito);
                break;
            case "papiro03":
                _gameManager.AddItemColetavel(other.gameObject);
                _gameManager.AddItemInventario(12);
                PontuacaoController.pontosFase += PontuacaoController.pontosPorPista;
                _uiManager.AtualizarPontuacao();
                if (_gameManager.ChecarSucesso(other.tag))
                {
                    Sucesso();
                }
                Debug.Log(_uiManager.GetTexto());
                other.gameObject.SetActive(false);
                _analisador.SetCodigoExtenso(_textoGabarito);
                break;
            case "valvula":
                _gameManager.AddItemColetavel(other.gameObject);
                _gameManager.AddItemInventario(2);
                PontuacaoController.pontosFase += PontuacaoController.pontosPorPista;
                _uiManager.AtualizarPontuacao();
                Debug.Log(_uiManager.GetTexto());
                other.gameObject.SetActive(false);
                _analisador.SetCodigoExtenso(_textoGabarito);
                break;
            case "basevalvula":
                if (_gameManager.VerificarItemInventario(_gameManager.todosItensJogo[2]))
                {
                    GameObject valvula = other.transform.GetChild(0).gameObject as GameObject;
                    valvula.SetActive(true);
                    GameObject trocarCenaObj = GameObject.Find("TrocarCena") as GameObject;
                    trocarCenaObj.GetComponent<BoxCollider2D>().enabled = true;
                    trocarCenaObj.GetComponent<SpriteRenderer>().enabled = true;
                    _gameManager.RemoverItemInventario(2);
                }
                else
                {
                    Debug.Log("Você precisa ter uma válvula no inventário para encaixar neste local!");
                }
                break;
            case "chave":
                _gameManager.AddItemColetavel(other.gameObject);
                _gameManager.AddItemInventario(3);
                PontuacaoController.pontosFase += PontuacaoController.pontosPorPista;
                _uiManager.AtualizarPontuacao();
                GameObject.Find("TrocarCena").GetComponent<SpriteRenderer>().enabled = true;
                GameObject.Find("TrocarCena").GetComponent<BoxCollider2D>().enabled = true;
                break;
            case "silabasenquanto":
                //Cria a nova silaba
                Silaba silabaEnquanto = new Silaba(other.name);

                //Adiciona na lista a ser enviada para o servidor
                if (!_gameManager.silabasColetadas.Contains(silabaEnquanto))
                    _gameManager.silabasColetadas.Add(silabaEnquanto);

                /*Cria uma nova lista de silabas em formato de string para
                 * fazer verificação se contém todas as silabas para liberar o enquanto
                */
                List<string> silabasTeste = new List<string>();
                foreach (Silaba s in _gameManager.silabasColetadas)
                {
                    silabasTeste.Add(s.silaba);
                }

                //Faz a verificação
                if (silabasTeste.Contains("SilabaEn") &&
                    silabasTeste.Contains("SilabaQuan") &&
                    silabasTeste.Contains("SilabaTo"))
                {
                    //Tem todas as silabas para o enquanto
                    _gameManager.enquanto = true;

                    //Remove enquanto do inventário
                    _gameManager.RemoverItemInventario(4);
                    _gameManager.RemoverItemInventario(5);
                    _gameManager.RemoverItemInventario(6);
                }
                else
                {
                    //Adiciona sprite no inventário
                    switch (silabaEnquanto.silaba)
                    {
                        case "SilabaEn": _gameManager.AddItemInventario(4); break;
                        case "SilabaQuan": _gameManager.AddItemInventario(5); break;
                        case "SilabaTo": _gameManager.AddItemInventario(6); break;
                    }
                }

                MostrarExplicacao(other.gameObject.name);
                other.gameObject.SetActive(false);
                break;
            case "silabassesenao":
                //Cria a nova silaba
                Silaba silabaSeSenao = new Silaba(other.name);

                //Adiciona na lista a ser enviada para o servidor
                _gameManager.silabasColetadas.Add(silabaSeSenao);

                /*Cria uma nova lista de silabas em formato de string para
                 * fazer verificação se contém todas as silabas para liberar o se, senao
                */
                silabasTeste = new List<string>();
                foreach (Silaba s in _gameManager.silabasColetadas)
                {
                    silabasTeste.Add(s.silaba);
                }

                if (silabaSeSenao.silaba == "SilabaSe")
                {
                    _gameManager.se = true;
                }
                else if (silabaSeSenao.silaba == "SilabaNao")
                {
                    if (silabasTeste.Contains("SilabaNao") && silabasTeste.Contains("SilabaSe"))
                    {
                        _gameManager.senao = true;
                    }
                    else
                    {
                        _gameManager.AddItemInventario(8);
                    }
                }

                /*
                foreach (Silaba palavra in _gameManager.silabasNoServidor)
                {
                    if (palavra.silaba.Equals("SilabaSe"))
                    {
                        _gameManager.se = true;
                        _gameManager.AddItemInventario(7);
                    }
                    if (palavra.silaba.Equals("SilabaNao"))
                    {
                        _gameManager.senao = true;
                        _gameManager.AddItemInventario(8);
                    }
                    if (_gameManager.silabasNoServidor.Contains(new Silaba("SilabSe")) &&
                    _gameManager.silabasNoServidor.Contains(new Silaba("SilabaNao")))
                    {
                        _gameManager.RemoverItemInventario(7);
                        _gameManager.RemoverItemInventario(8);
                    }
                }
                */

                MostrarExplicacao(other.gameObject.name);
                other.gameObject.SetActive(false);
                break;
            case "placapressao":
                puzzleManager.PlacaPressionada(other.gameObject.GetComponent<Simbolo>().id);
                break;
            case "esquimo":
                _gameManager.AddItemColetavel(other.gameObject);
                if (_gameManager.ChecarSucesso(other.tag))
                {
                    Sucesso();
                }
                //Debug.Log(_uiManager.GetTexto());
                //_analisador.SetCodigoExtenso(_textoGabarito);
                break;
            case "gatilho":
                other.GetComponent<Gatilho>().Pressionar(true);
                break;
        }
    }

    public Analisador GetAnalisador()
    {
        return _analisador;
    }

    private void MostrarExplicacao(string objeto)
    {
        switch (objeto)
        {
            case "SilabaEn":
                _uiManager.MostrarExplicação(1, "enquanto"); break;
            case "SilabaQuan":
                _uiManager.MostrarExplicação(2, "enquanto"); break;
            case "SilabaTo":
                _uiManager.MostrarExplicação(3, "enquanto"); break;
            case "SilabaSe":

                if (!_gameManager.se)
                {
                    _uiManager.MostrarExplicação(1, "enquanto"); break;
                }
                else
                {
                    _uiManager.MostrarExplicação(2, "sesenao"); break;
                }

            case "SilabaNao":
                _uiManager.MostrarExplicação(3, "sesenao"); break;
            default: throw new System.NotImplementedException();
        }
    }

    private void OnMouseDown()
    {
        if (_gameManager.personagemSelecionado.name.Contains("Furbot"))
        {
            Furbot scriptFurbot = _gameManager.personagemSelecionado.GetComponent<Furbot>();
            scriptFurbot.backupTexto = _uiManager.GetTexto();
        }
        else if (_gameManager.personagemSelecionado.name.Contains("AndroidBuggien"))
        {
            BuggienAndroid scriptBuggien = _gameManager.personagemSelecionado.GetComponent<BuggienAndroid>();
            scriptBuggien.backupTexto = _uiManager.GetTexto();
            scriptBuggien.Selecionar(false);
            _uiManager.AlterarHubPorPersonagem(Dialog_Char.FURBOT);
        }
#if UNITY_ANDROID
        _uiManager.listaAtualComandosStr = _uiManager.ultimosComandosStr;
#endif
        _gameManager.personagemSelecionado = gameObject;
        _uiManager.SetTexto(this.backupTexto);
    }


    public IEnumerator AdicionarEnergia(int energiaAMais)
    {
        for (int i = energia; i < energia + energiaAMais; i++)
        {
            _uiManager.AtualizarQntEnergia(energia + energiaAMais);
            yield return new WaitForSeconds(0.01f);
        }
        this.energia += energiaAMais;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        switch (other.tag.ToLower())
        {
            case "caminho":
                terrenoAtual = "";
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        switch (other.tag.ToLower())
        {
            case "caminho":
                terrenoAtual = "caminho";
                break;
        }
    }


    /// <summary>
    /// Recupera um tanto de energia conforme a quantidade de vida.
    /// </summary>
    private void RecuperarEnergia()
    {
        switch (vidas)
        {
            case 3:
                energia = 100;
                break;
            case 2:
                energia = 75;
                break;
            case 1:
                energia = 50;
                break;
            case 0:
                energia = 25;
                break;
            default:
                energia = 0;
                break;
        }
        _uiManager.AtualizarQntEnergia(energia);
    }

    /// <summary>
    /// Configura as variáveis de fim de jogo, chamando métodos relacionados ao fim de jogo.
    /// </summary>
    private void GameOver()
    {
        if (!SceneManager.GetActiveScene().name.Equals("FaseGerada"))
        {
            _gameManager.gameOver = true;
            _gameManager.ReiniciarContadores();
            // Atribuindo valores após perder o jogo, valores temporários
            energia = 100;
            vidas = 3;
            _gameManager.vida = vidas;
            // Sao estes valores acima
            _uiManager.ifCodigo.text = "";
            _uiManager.MostrarGameOver();
            Destroy(this);
            _levelManager.GameOver();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// Reinicia os status do robô.
    /// </summary>
    public void Reiniciar()
    {
        PontuacaoController.pontosFase = 0;
        tesouros = 0;
        _gameManager.ReiniciarContadores();

    }

    /// <summary>
    /// Reinicia Valores de status e arrays de manipulação de itens do cenário do jogo.
    /// </summary>
    public void ReiniciarValores()
    {
        _sensorAcima.ColetadoDoTrigger = _defColetadoAcima;
        _sensorAbaixo.ColetadoDoTrigger = _defColetadoAbaixo;
        _sensorDireita.ColetadoDoTrigger = _defColetadoDir;
        _sensorEsquerda.ColetadoDoTrigger = _defColetadoEsq;
        _sensorAcima.obstaculo = _defCenarioAcima;
        _sensorAbaixo.obstaculo = _defCenarioAbaixo;
        _sensorDireita.obstaculo = _defCenarioDir;
        _sensorEsquerda.obstaculo = _defCenarioEsq;
        if (!SceneManager.GetActiveScene().name.Equals("FaseGerada"))
        {
            vidas = _gameManager.vida;
            energia = _gameManager.energia;
            tesouros = _gameManager.contadorTesouro;
            _gameManager.ReativarItensColetados();
            transform.position = _posicaoInicial;
            PontuacaoController.pontosFase = _gameManager.pontuacaoIntermediaria;
        }
        else
        {
            vidas = 3;
        }
        _energiaRessarcivel = 0;
        _uiManager.AtualizarVida(vidas);
        _uiManager.AtualizarQntEnergia(_gameManager.energia);
        _uiManager.AtualizarQntTesouros(tesouros);
        _uiManager.AtualizarPontuacao();

        //Para casos em que a fase contem um puzzle
        if (puzzleManager != null)
        {
            alavancaAtual = null;
            puzzleManager.ReiniciarPuzzle();
        }

        if (SceneManager.GetActiveScene().name.Equals("Fase 16.3")
            || SceneManager.GetActiveScene().name.Equals("Fase 17.3")
            || SceneManager.GetActiveScene().name.Equals("Fase 18.3")
            || SceneManager.GetActiveScene().name.Equals("Fase 19.3"))
        {
            GameObject baseValvula = GameObject.Find("Base_Valvula") as GameObject;
            GameObject valvula = baseValvula.transform.GetChild(0).gameObject as GameObject;
            valvula.SetActive(false);
            GameObject trocarCenaObj = GameObject.Find("TrocarCena") as GameObject;
            trocarCenaObj.GetComponent<BoxCollider2D>().enabled = false;
            trocarCenaObj.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// Configura variáveis de vitória do jogo, chamando métodos relacionados a vitória do jogo.
    /// </summary>
    private void Sucesso()
    {
        venceu = true;
        _gameManager.vida = vidas;
        _gameManager.energia = energia;
        _gameManager.inventarioIDsBackup = _gameManager.inventarioIDs;
        _analisador.SetLog(_uiManager.GetLog());
        _analisador.SetCodigoExtenso(_textoGabarito);
        _analisador.SetQtdLinhas(_uiManager.ifCodigo.text.Split('\n').Length);
        _levelManager.PassarDeFase();
        //_analisador.ExportarDados();
        _analisador.GerarLog();
        _uiManager.PararExecucao();
        Debug.Log(_analisador.GetCodigoExtenso());
        //Debug.Log(Gabarito.CalcularGabarito(LevelManager.faseAtual, _analisador.GetCodigoExtenso()));
        Debug.Log(_analisador.GerarTexto());
        StartCoroutine(DelaySucesso());
    }

    /// <summary>
    /// Retorna a quantidade de tesouros coletados.
    /// </summary>
    /// <returns> Retorna a quantidade de tesouros. </returns>
    public int GetQntTesouros()
    {
        return this.tesouros;
    }

    /// <summary>
    /// Corotina usada para delay da aparição da tela de sucesso;
    /// </summary>
    /// <returns>Retorna uma espera de 1.5 segundos.</returns>
    private IEnumerator DelaySucesso()
    {
        yield return new WaitForSeconds(1.5f);
        _uiManager.MostrarSucesso();
        yield break;
    }

    private IEnumerator MoverParaInicio()
    {
        Debug.Log("voltando ao inicio");
        Vector3 newPosition = new Vector3(this.GetComponent<Transform>().position.x, this.GetComponent<Transform>().position.y
           , this.GetComponent<Transform>().position.z);
        float startTime = Time.time;
        float endTime = startTime + 0.5f;
        float moveTime = 0.5f;
        while (Time.time < endTime)
        {
            this.transform.position = Vector3.Lerp(newPosition, oldPosition, (Time.time - startTime) / moveTime);
            yield return null;
        }
    }
}