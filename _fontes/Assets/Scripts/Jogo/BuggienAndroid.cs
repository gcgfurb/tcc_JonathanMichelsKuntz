using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuggienAndroid : MonoBehaviour
{
    [SerializeField]
    private Estado estado;

    // Boolean
    public bool isMoving, bateu, isSelecionado;


    public Sprite desligado, ligado;
    public Sprite[] quebrado;
    SpriteRenderer sr;

    // GameObject (SERIALIZED)
    [SerializeField]
    private GameObject _animSeta, _pontoEnergiaPrefab, _pontoVidaPrefab2, _pontoVidaPrefab, _ponto500Prefab, _sensorDireitaObj,
        _sensorEsquerdaObj, _sensorAcimaObj, _sensorAbaixoObj, _android, _sensorLongaDistancia;
    [SerializeField]
    private Sprite[] _imagemBuggien;
    // Object
    private UIManager _uiManager;
    private GameManager _gameManager;
    private LevelManager _levelManager;
    private Tilemap _mapa;
    private Sensor _sensorDireita, _sensorEsquerda, _sensorAcima, _sensorAbaixo;
    private GameObject anim;
    public Compilador compilador;

    public ArrayList ultimosComandosStr = new ArrayList();

    // Vector3
    private Vector3 _posicaoAtual, _posicaoInicial, _posicaoInicialNaoConvertida;
    public Vector3 oldPosition;

    // Float
    private float _stepAtual;

    // string
    public string direcaoAtual, backupTexto;

    //bool
    public bool emExecucao;


    // Start is called before the first frame update
    void Start()
    {
        _stepAtual = 1f;
        sr = gameObject.GetComponent<SpriteRenderer>();
        if (GameObject.Find("GameManager") != null)
        {
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            _levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        }

        _mapa = GameObject.Find("TilemapMoldura").GetComponent<Tilemap>();
        _uiManager = GameObject.Find("CanvasInterface").GetComponent<UIManager>();
        isMoving = false;
        _posicaoInicial = ConverterPosMundoParaPosCell(transform.position);
        _posicaoInicialNaoConvertida = transform.position;
        _sensorDireita = _sensorDireitaObj.GetComponent<Sensor>();
        _sensorEsquerda = _sensorEsquerdaObj.GetComponent<Sensor>();
        _sensorAcima = _sensorAcimaObj.GetComponent<Sensor>();
        _sensorAbaixo = _sensorAbaixoObj.GetComponent<Sensor>();
        direcaoAtual = "Parado";
        compilador = GetComponent<Compilador>();
        Selecionar(false);

        switch (estado)
        {
            case Estado.Estatico:
                sr.sprite = quebrado[UnityEngine.Random.Range(0, quebrado.Length)];
                break;
            case Estado.Empurravel:
                sr.sprite = desligado;
                break;
            case Estado.Programavel:
                sr.sprite = ligado;
                break;
        }

        if (SceneManager.GetActiveScene().name.Equals("Fase 19.3") && _gameManager.buggienPassou == false)
        {
            this.gameObject.SetActive(false);
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
        return posCell + new Vector3(0, 0, 0);
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
                    break;
                case Direcao.ACIMA:
                    direcaoAtual = "Acima";
                    newPosition = _posicaoAtual + new Vector3(0, _stepAtual, 0);
                    break;
                case Direcao.DIREITA:
                    direcaoAtual = "Direita";
                    newPosition = _posicaoAtual + new Vector3(_stepAtual, 0, 0);
                    break;
                case Direcao.ESQUERDA:
                    direcaoAtual = "Esquerda";
                    newPosition = _posicaoAtual - new Vector3(_stepAtual, 0, 0);
                    break;
                default: return;// tratamento de exceções
            }
            StartCoroutine(Mover(_posicaoAtual, newPosition));
        }
    }
    public Estado GetEstado()
    {
        return estado;
    }

    public bool VaiBater(Direcao dir)
    {
        if (GetSensor(dir).obstaculo)
        {
            bateu = true;
            Bater(GetSensor(dir).ColetadoDoTrigger);
            _uiManager.ifCodigo.interactable = false;
            return true;
        }
        else return false;
    }

    private IEnumerator Mover(Vector3 originalPosition, Vector3 newPosition)
    {
        isMoving = true;
        float startTime = Time.time;
        float endTime = startTime + 0.5f;
        float moveTime = 0.5f;
        while (Time.time < endTime)
        {
            transform.position = Vector3.Lerp(originalPosition, newPosition, (Time.time - startTime) / moveTime);
            yield return null;
        }
        isMoving = false;
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


    private void OnMouseDown()
    {

        Furbot furbot = GameObject.Find("Furbot").GetComponent<Furbot>();


        if (!furbot.emExecucao)
        {
            if (estado == Estado.Programavel)
            {
                _sensorLongaDistancia.SetActive(true);
                bool furbotnaArea = _sensorLongaDistancia.GetComponent<SensorProgramacaoRemota>().furbotNaArea;
                if (FurbotNaArea())
                {
                    _sensorLongaDistancia.SetActive(false);
                    if (_gameManager.personagemSelecionado.name.Contains("Furbot"))
                    {
                        Furbot scriptFurbot = _gameManager.personagemSelecionado.GetComponent<Furbot>();
                        scriptFurbot.backupTexto = _uiManager.GetTexto();
                        Selecionar(true);
                        _uiManager.AlterarHubPorPersonagem(Dialog_Char.BUGGIEN);
                    }
                    else if (_gameManager.personagemSelecionado.name.Contains("AndroidBuggien"))
                    {
                        BuggienAndroid scriptBuggien = _gameManager.personagemSelecionado.GetComponent<BuggienAndroid>();
                        scriptBuggien.Selecionar(false);
                        scriptBuggien.backupTexto = _uiManager.GetTexto();
                        Selecionar(true);
                        _uiManager.AlterarHubPorPersonagem(Dialog_Char.BUGGIEN);

                    }

                    _gameManager.personagemSelecionado = gameObject;
#if UNITY_ANDROID
                    _uiManager.listaAtualComandosStr = ultimosComandosStr;
#endif
                    _uiManager.SetTexto(backupTexto);

                }
                else
                {
                    StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Precisamos chegar mais perto para programar ele"));
                    _sensorLongaDistancia.SetActive(false);

                }
            }
        }
    }
    public void VoltarProInicio()
    {
        transform.position = _posicaoInicialNaoConvertida;
    }

    public void Selecionar(bool estadoSelecionado)
    {
        _animSeta.SetActive(estadoSelecionado);
        isSelecionado = estadoSelecionado;
    }

    bool FurbotNaArea()
    {
        Vector3 furbot = ConverterPosMundoParaPosCell(GameObject.Find("Furbot").transform.position);
        Vector3 aBuggien = ConverterPosMundoParaPosCell(gameObject.transform.position);

        return (furbot.x <= aBuggien.x + 4 && furbot.x >= aBuggien.x - 4) && (furbot.y < aBuggien.y + 4 && furbot.y >= aBuggien.y - 4);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Furbot furbot = GameObject.Find("Furbot").GetComponent<Furbot>();
        switch (other.tag.ToLower())
        {
            case "gatilho":
                other.GetComponent<Gatilho>().Pressionar(true);
                break;
            case "tesouro":
                furbot.GetAnalisador().IncrementarTesouro();
                furbot.tesouros++;
                Instantiate(_ponto500Prefab, new Vector2(other.transform.position.x, other.transform.position.y), Quaternion.identity);
                /*if (!_faseGerada)
                {
                    _gameManager.AddItemColetavel(other.gameObject);
                    PontuacaoController.pontosFase += PontuacaoController.pontosPorTesouro;
                }*/
                other.gameObject.SetActive(false);
                _uiManager.AtualizarQntTesouros(furbot.tesouros);
                _uiManager.AtualizarPontuacao();
                _uiManager.AddLog("O Furbot coletou um Tesouro");
                break;
            case "energia":
                furbot.GetAnalisador().IncrementarEnergia();
                if (furbot.energia + 30 > 100)
                {
                    StartCoroutine(furbot.AdicionarEnergia(100 - furbot.energia));
                }
                else
                {
                    StartCoroutine(furbot.AdicionarEnergia(30));
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
                if (furbot.vidas < 3)
                {
                    furbot.GetAnalisador().IncrementarVida();
                    furbot.vidas++;
                    GameObject anim = Instantiate(_pontoVidaPrefab, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity, transform);
                    anim.transform.localScale = new Vector2(0.045f, 0.045f);
                    Destroy(anim, 2.0f);
                    _uiManager.AtualizarVida(furbot.vidas);
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
        }
    }

    public void Bater(string objeto)
    {
        int index = compilador.GetIndex();
        switch (objeto.ToLower())
        {

            case "laboratorio":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "Acho que não é por aqui!"));
                _uiManager.PararExecucao();
                break;

            case "pedra":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "Cuidado! Eu bati em uma pedra!"));
                _uiManager.PararExecucao();
                break;
           
            case "caixa":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "Cuidado! Eu bati em uma caixa!"));
                _uiManager.PararExecucao();
                break;

            case "arbusto":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "Cuidado! Eu bati em um arbusto!"));
                _uiManager.PararExecucao();
                break;

            case "lixo":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "Cuidado! Eu bati em um lixo!"));
                _uiManager.PararExecucao();
                break;

            case "meleca":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "Cuidado! Eu bati em uma meleca!"));
                _uiManager.PararExecucao();
                break;

            case "arvore":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "Cuidado! Eu bati em uma árvore!"));
                _uiManager.PararExecucao();
                break;

            case "paredepiramide":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "Acho que não é por aqui!"));
                _uiManager.PararExecucao();
                break;

            case "parede":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "AI!"));
                _uiManager.PararExecucao();
                break;

            case "agua":
                StartCoroutine(_uiManager.Diga(Dialog_Char.BUGGIEN, "Eu não sei nadar"));
                _uiManager.PararExecucao();
                break;

            case "buggien":
                //StartCoroutine(_uiManager.Diga(Dialog_Char.S223, "Ei! cuidado com os buggiens!"));
                //_uiManager.AddLog("O Buggien comandado pelo furbot bateu em outro Buggien");
                break;


        }
        _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(index);
        _uiManager.btnExecutar.GetComponentInChildren<Text>().text = "EXECUTAR";
    }


    public enum Estado
    {
        Estatico, Empurravel, Programavel
    }
}
