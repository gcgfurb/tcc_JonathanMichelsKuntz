using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _letra_prefab, _posicaoInicial, _sucessoImage, _derrotaImage, _tutorialImage, _maisPontosPrefab, _menosPontosPrefab, _canvas;
    [SerializeField]
    private GameObject[] _respostas;
    [SerializeField]
    private Text _perguntaComponent, _timerText, _pontuacaoText, _pontuacaoFinalText;
    [SerializeField]
    private Text[] _letras;
    [SerializeField]
    private float _timer;
    [SerializeField]
    private int _pontosPorAcerto;
    [SerializeField]
    private Text _pularTexto;
    [SerializeField]
    private Button _btnResetarLetras;
    [SerializeField]
    private Animator _fadeAnimator;

    private LevelManager _lm;

    private GameManager _gm;

    private bool _emJogo, _venceu;

    private List<int> _letrasOcupadas;

    private string _pergunta, _resposta;

    private int _acertos, _pontos;

    void Start()
    {
        _btnResetarLetras.onClick.AddListener(ResetarLetras);
        _pontos = 0;
        if (_pontosPorAcerto == 0)
        {
            _pontosPorAcerto = 20;
            Debug.Log("Pontuacao por acerto nao foi definida. Usando valor padrao (20)");
        }
        if (_timer == 0)
            _timer = 60;
        if (GameObject.Find("LevelManager") != null)
        {
            _lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (_lm != null)
            {
                if (!_lm.quizJogado)
                {
                    _lm.quizJogado = true;
                }
                else
                {
                    _tutorialImage.SetActive(false);
                    Começar();
                }
            }
        }
#if UNITY_IOS || UNITY_ANDROID 
        _pularTexto.text = "Toque na tela para começar!";
#else
        _pularTexto.text = "Pressione espaço ou enter para começar!";
#endif

        _venceu = false;
    }

    private void Update()
    {
        if (!_emJogo)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                _tutorialImage.GetComponent<Animator>().SetBool("SlideOut", true);
                Destroy(_tutorialImage, 1.5f);
                Começar();
            }
        }
    }

    private void ResetarLetras()
    {
        foreach (Text letra in _letras)
        {
            Quiz_letra letraQuiz = letra.GetComponent<Quiz_letra>();
            StartCoroutine(letraQuiz.MoverDeVolta());
        }
        _acertos = 0;
        _pontos = 0;
        _pontuacaoText.text = _pontos.ToString();
    }

    private void Começar()
    {
        _emJogo = true;
        StartCoroutine(StartTimer());
        _letrasOcupadas = new List<int>();
        _pergunta = QuizController.GerarPergunta(out _resposta);
        _perguntaComponent.text = _pergunta;
        int quantLetras = _resposta.Length;
        if (quantLetras > _respostas.Length)
        {
            Debug.Log("A resposta sorteada tem tamanho maior do que o suportado (" + _respostas.Length + ").");
            return;
        }
        bool atribuiu = false;
        for (int i = 0; i < quantLetras; i++)
        {
            while (!atribuiu)
            {
                int sorteio = Random.Range(0, _letras.Length);
                if (!_letrasOcupadas.Contains(sorteio))
                {
                    _letrasOcupadas.Add(sorteio);
                    _letras[sorteio].gameObject.SetActive(true);
                    _letras[sorteio].text += _resposta[i];
                    atribuiu = true;
                }
            }
            atribuiu = false;
            _respostas[i].gameObject.SetActive(true);
            _respostas[i].GetComponent<Quiz_Resposta>().respostaCorreta += _resposta[i];
        }
        int dummies = Random.Range(2, 7);
        for (int i = 0; i < dummies; i++)
        {
            if (_letrasOcupadas.Count < _letras.Length)
            {
                while (!atribuiu)
                {
                    int sorteio = Random.Range(0, _letras.Length);
                    if (!_letrasOcupadas.Contains(sorteio))
                    {
                        _letrasOcupadas.Add(sorteio);
                        _letras[sorteio].gameObject.SetActive(true);
                        char c = (char)('a' + Random.Range(0, 26));
                        _letras[sorteio].text += c;
                        atribuiu = true;
                    }
                }
                atribuiu = false;
            }
        }
    }

    private IEnumerator StartTimer()
    {
        while (_emJogo)
        {
            _timerText.text = "Tempo restante: " + (--_timer);
            yield return new WaitForSeconds(1.0f);
            if (_timer == 0 && !_venceu)
            {
                _derrotaImage.SetActive(true);
                _emJogo = false;
                StartCoroutine(WaitForFade());
            }
        }
    }

    public void IncrementarAcerto()
    {
        _pontos += _pontosPorAcerto;
        _pontuacaoText.text = _pontos.ToString();
        GameObject pontosFx = Instantiate(_maisPontosPrefab, _canvas.transform);
        pontosFx.GetComponent<Text>().text = "+" + _pontosPorAcerto + " pontos!";
        Destroy(pontosFx, 1.0f);
        if (_acertos++ == _resposta.Length - 1)
        {
            Debug.Log("Venceu");
            _venceu = true;
            _sucessoImage.SetActive(true);
            _pontuacaoFinalText.text = _pontos.ToString();
            StartCoroutine(WaitForFade());
            if (_pontos > 0 && _gm != null)
            {
                _gm.pontuacaoTotal += _pontos;
            }
        }
    }

    public void DecrementarAcerto()
    {
        _acertos--;
        _pontos -= _pontosPorAcerto;
        GameObject pontosFx = Instantiate(_menosPontosPrefab, _canvas.transform);
        pontosFx.GetComponent<Text>().text = "-" + _pontosPorAcerto + " pontos!";
        Destroy(pontosFx, 1.0f);
        _pontuacaoText.text = _pontos.ToString();
    }

    private IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(4.0f);
        _fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        if (_lm != null)
        {
            SceneManager.LoadScene(++LevelManager.sceneIndex);
        }
        else
        {
            SceneManager.LoadScene("SelecaoDeFases");
        }
    }
}
