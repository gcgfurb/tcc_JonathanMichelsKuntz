using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScriptEx : MonoBehaviour
{
    //Objetos (SERIALIZED)
    [SerializeField]
    private GameObject[] pontosDeFoco;

    //Booleans
    private bool _moveu;
    public bool pularAutomaticamente;

    //Integers
    private int _index;

    //Floats
    private float _moveCameraTimer, _cameraMoveSpeed;

    // Use this for initialization
    void Start()
    {
        _cameraMoveSpeed = 0.025f;
        _moveCameraTimer = 3.0f;
        _moveu = false;
        _index = 0;
        StartCoroutine(EsperarFade(true));
    }

    // Update is called once per frame
    void Update()
    {
        if (_index < pontosDeFoco.Length)
        {
            if (_moveu)
            {
                if (!pularAutomaticamente)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                    {
                        StartCoroutine(MoverCamera(pontosDeFoco[_index++]));
                    }
                }
                else
                {
                    StartCoroutine(MoverCamera(pontosDeFoco[_index++]));
                }
            }
        }
    }

    /*
     * Move a câmera em direção à cada ponto de foco na cena.
     * os pontos sao definidos no vetor pontosDeFoco no proprio editor unity.
     */
    private IEnumerator MoverCamera(GameObject foco)
    {
        Debug.Log("Iniciou movimento de camera.");
        _moveu = false;
        float endTime = Time.time + _moveCameraTimer;
        while (Time.time < endTime)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, foco.transform.position, _cameraMoveSpeed);
            yield return null;
        }
        Debug.Log("Terminou movimento da camera.");
        if (pularAutomaticamente)
        {
            StartCoroutine(TempoParaLer(foco.GetComponent<PontoDeFoco>().GetTempoDeLeitura()));
        }
        else
        {
            _moveu = true;
        }
    }

    /*
     * Co-rotina para aguardar um tempo (_tempoLeitura) em cada ponto de foco.
     */
    private IEnumerator TempoParaLer(float _tempoLeitura)
    {
        Debug.Log("Iniciou tempo para leitura (" + _tempoLeitura + " segundos)");
        float endTime = Time.time + _tempoLeitura;
        while (Time.time < endTime)
        {
            yield return null;
        }
        Debug.Log("Terminou tempo de leitura.");
        _moveu = true;
        if (_index == pontosDeFoco.Length)
        {
            StartCoroutine(EsperarFade(false));
        }
    }

    /*
     * Co-rotina para aguardar o término de cada fade (in ou out)
     * no caso do fade in, esperar que o efeito termine antes de iniciar o movimento da camera.
     * no caso do fade out, esperar que o efeito termine para carregar a próxima cena.
     */
    private IEnumerator EsperarFade(bool fadeIn)
    {
        float endTime = Time.time + 1.5f;
        while (Time.time < endTime)
        {
            yield return null;
        }
        if (fadeIn)
        {
            StartCoroutine(MoverCamera(pontosDeFoco[_index++]));
        }
    }
}
