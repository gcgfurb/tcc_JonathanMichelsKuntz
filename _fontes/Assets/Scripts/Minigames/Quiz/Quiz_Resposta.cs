using UnityEngine;

public class Quiz_Resposta : MonoBehaviour
{
    public string respostaCorreta;

    public bool ocupado;

    public GameObject atual;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Quiz_letra letra = collision.GetComponent<Quiz_letra>();
        if (!letra.movendoDeVolta && atual == null)
        {
            letra.atribuicao = this.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Quiz_letra letra = collision.GetComponent<Quiz_letra>();
        if (!letra.movendoDeVolta && atual == null)
        {
            letra.atribuicao = this.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == atual)
        {
            other.GetComponent<Quiz_letra>().atribuicao = null;
            atual = null;
            ocupado = false;
        }
    }
}
