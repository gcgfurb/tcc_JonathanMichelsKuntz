using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Sensor : MonoBehaviour
{

    public string ColetadoDoTrigger;
    public bool obstaculo;
    public GameObject ObjColetado;
    public List<Collider2D> collidersColidindo;

    /// <summary>
    /// passa para uma variável o nome do objeto que o trigger coletou.
    /// </summary>
    /// <param name="collision">Objeto no qual o trigger colidiu</param>
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.tag == "Grama")
        {
            ColetadoDoTrigger = "grama";
        }
        else if (collision.tag == "Buggien")
        {
            ColetadoDoTrigger = "Buggien";
            obstaculo = true;
            ObjColetado = collision.gameObject;
        }
        else if (collision.tag == "Caminho")
        {
            ColetadoDoTrigger = "caminho";
        }
        else if (collision.tag == "Areia")
        {
            ColetadoDoTrigger = "areia";
        }
        else if (collision.tag == "Agua")
        {
            ColetadoDoTrigger = "agua";
            obstaculo = true;
        }
        else if (collision.tag == "ParedePiramide")
        {
            ColetadoDoTrigger = "paredepiramide";
            obstaculo = true;
        }
        else if (collision.tag == "Cenario")
        {
            ColetadoDoTrigger = "cenario";
            obstaculo = true;
        }
        else if (collision.tag == "Arvore")
        {
            ColetadoDoTrigger = "arvore";
            obstaculo = true;
        }
        else if (collision.tag == "Pedra")
        {
            ColetadoDoTrigger = "pedra";
            obstaculo = true;
        }
        else if (collision.tag == "Arbusto")
        {
            ColetadoDoTrigger = "arbusto";
            obstaculo = true;
        }
        else if (collision.tag == "Lixo")
        {
            ColetadoDoTrigger = "lixo";
            obstaculo = true;
        }
        else if (collision.tag == "Caixa")
        {
            ColetadoDoTrigger = "caixa";
            obstaculo = true;
        }
        else if (collision.tag == "Meleca")
        {
            ColetadoDoTrigger = "meleca";
            obstaculo = true;
        }

        else if (collision.tag == "Parede")
        {
            ColetadoDoTrigger = "parede";
            obstaculo = true;
        }
        else if(collision.tag == "Player")
        {
            ColetadoDoTrigger = "furbot";
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collidersColidindo.Contains(collision))
        {
            collidersColidindo.Add(collision);
        }
        if (collision.tag == "Grama")
        {   
            ColetadoDoTrigger = "grama";
        }
        else if (collision.tag == "Buggien")
        {
            ColetadoDoTrigger = "Buggien";
            obstaculo = true;
        }
        else if (collision.tag == "Caminho")
        {
            ColetadoDoTrigger = "caminho";
        }
        else if (collision.tag == "Areia")
        {
            ColetadoDoTrigger = "areia";
        }
        else if (collision.tag == "Agua")
        {
            ColetadoDoTrigger = "agua";
            obstaculo = true;
        }
        else if (collision.tag == "Cenario")
        {
            ColetadoDoTrigger = "cenario";
            obstaculo = true;
        }
        else if (collision.tag == "Arvore")
        {
            ColetadoDoTrigger = "arvore";
            obstaculo = true;
        }
        else if (collision.tag == "Pedra")
        {
            ColetadoDoTrigger = "pedra";
            obstaculo = true;
        }
        else if (collision.tag == "Arbusto")
        {
            ColetadoDoTrigger = "arbusto";
            obstaculo = true;
        }
        else if (collision.tag == "Parede")
        {
            ColetadoDoTrigger = "parede";
            obstaculo = true;
        }
        else if (collision.tag == "ParedePiramide")
        {
            ColetadoDoTrigger = "paredepiramide";
            obstaculo = true;
        }
        else if (collision.tag == "Lixo")
        {
            ColetadoDoTrigger = "lixo";
            obstaculo = true;
        }
        else if (collision.tag == "Caixa")
        {
            ColetadoDoTrigger = "caixa";
            obstaculo = true;
        }
        else if (collision.tag == "Meleca")
        {
            ColetadoDoTrigger = "meleca";
            obstaculo = true;
        }
        else if(collision.tag == "Alavanca")
        {
            GetComponentInParent<Furbot>().usarAlavanca = true;
            GetComponentInParent<Furbot>().alavancaAtual = collision.gameObject;
        }
    }

    /// <summary>
    /// Método para manipular a variável booleana após término de colisão.
    /// </summary>
    /// <param name="collision"> Componente do objeto colidido. </param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        collidersColidindo.Remove(collision);

        if (collision.tag == "Cenario")
        {
            obstaculo = false;
        }
        else if (collision.tag == "Arvore")
        {
            obstaculo = false;
        }
        else if (collision.tag == "Pedra")
        {
            obstaculo = false;
        }
        else if (collision.tag == "Arbusto")
        {
            obstaculo = false;
        }
        else if (collision.tag == "Agua")
        {
            obstaculo = false;
        }
        else if (collision.tag == "Buggien")
        {
            obstaculo = false;
        }
        else if (collision.tag == "ParedePiramide")
        {
            obstaculo = false;
        }
        else if (collision.tag == "Caminho")
        {
            ColetadoDoTrigger = "";
            obstaculo = false;
        }
        else if (collision.tag == "Parede")
        {
            ColetadoDoTrigger = "";
            obstaculo = false;
        }
        else if (collision.tag == "Lixo")
        {
            ColetadoDoTrigger = "";
            obstaculo = false;
        }
        else if (collision.tag == "Caixa")
        {
            ColetadoDoTrigger = "";
            obstaculo = false;
        }
        else if (collision.tag == "Meleca")
        {
            ColetadoDoTrigger = "";
            obstaculo = false;
        }
        else if (collision.tag == "Alavanca")
        {
            GetComponentInParent<Furbot>().usarAlavanca = false;
            GetComponentInParent<Furbot>().alavancaAtual = null;
        }
        else if(collision.tag == "Buggien")
        {
            ColetadoDoTrigger = "";
            obstaculo = false;
        }
    }

    public bool ContemCollider(string tagObjeto)
    {
        foreach(Collider2D collider in collidersColidindo)
        {
            if (collider.tag.Equals(tagObjeto))
            {
                return true;
            }
        }
        return false;
    }
}