using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogo : MonoBehaviour
{

    //Objetos
    private Coroutine _rotinaTexto;
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


    //Floats
    private float _velocidadeTexto, _velocidadeTextoPadrao;

    //Strings
    public string[] _dialogoAtual;

    //Integers
    public int indiceDialogo;
    private int _contadorSkip;

    //Booleans
    public bool terminou;

    private void Awake()
    {
        _rotinaTexto = StartCoroutine(ReproduzirTexto(_dialogoAtual[indiceDialogo]));
    }


    // Update is called once per frame
    void Update()
    {
        if (indiceDialogo < _dialogoAtual.Length)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                PularFala();
            }
        }
    }
    public void PularFala()
    {
        if (++_contadorSkip == 1)
        {
            _velocidadeTexto = 0.005f;
        }
        else
        {
                if ((++indiceDialogo >= _dialogoAtual.Length) || _dialogoAtual == null)
                {
                    StopCoroutine(_rotinaTexto);
                    this.gameObject.SetActive(false);
                    indiceDialogo = 0;
                    _contadorSkip = 0;
                    _velocidadeTexto = _velocidadeTextoPadrao;
                    _dialogoAtual = new string[1];
                terminou = true;
                }
                else
                {
                    StopCoroutine(_rotinaTexto);
                    _contadorSkip = 0;
                    _velocidadeTexto = _velocidadeTextoPadrao;
                    _rotinaTexto = StartCoroutine(ReproduzirTexto(_dialogoAtual[indiceDialogo]));
                }
        }
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
    
   public int GetIndice()
    {
        return indiceDialogo;
    }
}


