using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quiz_letra : MonoBehaviour
{
    [SerializeField]
    private Canvas _myCanvas;
    [SerializeField]
    private QuizManager _qm;

    private string _valor;

    private bool _acertou;

    public bool movendoDeVolta;

    private Vector3 _screenPoint, _offset, _initialPosition;

    public GameObject atribuicao;

    // Use this for initialization
    void Start()
    {
        _initialPosition = this.transform.position;
        _valor = GetComponent<Text>().text;
        movendoDeVolta = false;
    }

    void OnMouseDrag()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_myCanvas.transform as RectTransform, Input.mousePosition, _myCanvas.worldCamera, out pos);
        transform.position = _myCanvas.transform.TransformPoint(pos);
        if (_acertou)
        {
            _qm.DecrementarAcerto();
            _acertou = false;
        }
    }

    private void OnMouseUp()
    {
        if (atribuicao == null || atribuicao.GetComponent<Quiz_Resposta>().ocupado)
        {
            StartCoroutine(MoverDeVolta());
        }
        else
        {
            this.transform.position = atribuicao.transform.position;
            atribuicao.GetComponent<Quiz_Resposta>().ocupado = true;
            atribuicao.GetComponent<Quiz_Resposta>().atual = this.gameObject;
            ChecarResposta();
        }
    }

    private void ChecarResposta()
    {
        if (_valor.ToLower().Equals(atribuicao.GetComponent<Quiz_Resposta>().respostaCorreta.ToLower()))
        {
            _acertou = true;
            _qm.IncrementarAcerto();
        }
    }

    public IEnumerator MoverDeVolta()
    {
        movendoDeVolta = true;
        _acertou = false;
        if (atribuicao != null)
        {
            atribuicao.GetComponent<Quiz_Resposta>().ocupado = false;
        }
        while (this.transform.position != _initialPosition)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, _initialPosition, 0.35f);
            yield return new WaitForSeconds(0.005f);
        }
        movendoDeVolta = false;
    }
}
