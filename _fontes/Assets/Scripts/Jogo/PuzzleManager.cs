using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{

    [SerializeField] private Sprite _alavancaDesativada;
    [SerializeField] private Sprite _alavancaAtivada;

    [SerializeField] private GameObject[] _alavancas;
    [SerializeField] private GameObject[] _placasDePressao;

    private AnimPuzzleController _animPuzzleController;
    private int[] _respostaAlavanca = { 2, 5, 3, 4, 0, 1 };
    private int _indice;
    private Vector3 _escalaPressionado;
    private Vector3[] _posicoesIniciaisPlacas = new Vector3[4];

    // Puzzle 1 = alavanca
    // Puzzle 2 = placas de pressao
    private int numPuzzle;

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Fase 9")
        {
            numPuzzle = 1;
        }
        else if (SceneManager.GetActiveScene().name == "Fase 10")
        {
            numPuzzle = 2;
            _escalaPressionado = new Vector3(0.05f, 0.05f, 0);
            SalvarPosicoesIniciasPlacas();
        }

        _animPuzzleController = GameObject.Find("PortasPiramide").GetComponent<AnimPuzzleController>();
        _indice = 0;
    }

    private void SalvarPosicoesIniciasPlacas()
    {
        for(int i = 0; i < 4; i++)
        {
            _posicoesIniciaisPlacas[i] = _placasDePressao[i].transform.position;
        }
    }

    public void AtivarAlavanca(int id)
    {
        if(_indice <= 5)
        {
            _alavancas[id].GetComponent<SpriteRenderer>().sprite = _alavancaAtivada;
            if (id != _respostaAlavanca[_indice])
            {
                _indice = 0;
                DesativarAlavancas();
            }

            if (_indice == 5 && id == _respostaAlavanca[5])
            {
                AbrirPortas();
            }
            _indice++;
        }
        
    }

    public void DesativarAlavancas()
    {
        for(int i = 0; i < 6; i++)
        {
            _alavancas[i].GetComponent<SpriteRenderer>().sprite = _alavancaDesativada;
        }
    }

    public void AbrirPortas()
    {
        _animPuzzleController.Abrir();
    }

    public void PlacaPressionada(int id)
    {
        if (_indice < 4)
        {
            if(id == 3 && _indice == 3)
            {
                AbrirPortas();
            }
            if (id == _indice)
            {
                _indice++;
                _placasDePressao[id].transform.position -= _escalaPressionado;
                _placasDePressao[id].GetComponent<Simbolo>().ativada = true;
            }
            else
            {
                if(_placasDePressao[id].GetComponent<Simbolo>().ativada == false)
                {
                    StartCoroutine(DesapertarPlaca(id));
                }
            }
        }
    }

    public void DesativarPlacasDePressao()
    {
        for (int i = 0; i < 4; i++)
        {
            _placasDePressao[i].transform.position = _posicoesIniciaisPlacas[i];
            _placasDePressao[i].GetComponent<Simbolo>().ativada = false;
        }
    }

    public void ReiniciarPuzzle()
    {
        switch (numPuzzle)
        {
            case 1:
                DesativarAlavancas();
                break;
            case 2:
                DesativarPlacasDePressao();
                break;
        }
        _indice = 0;
        _animPuzzleController.ReiniciarAnimacao();
    }

    private IEnumerator DesapertarPlaca(int id)
    {
        _placasDePressao[id].transform.position -= _escalaPressionado;
        yield return new WaitForSeconds(0.6f);
        _placasDePressao[id].transform.position += _escalaPressionado;
    }
}
