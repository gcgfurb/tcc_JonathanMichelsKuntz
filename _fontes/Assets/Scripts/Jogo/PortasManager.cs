using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortasManager : MonoBehaviour
{

    public GameObject[] portas;
    public GameObject[] gatilhos;

    // Start is called before the first frame update
    void Start()
    {
        portas = GameObject.FindGameObjectsWithTag("Porta");
        gatilhos = GameObject.FindGameObjectsWithTag("Gatilho");

        RelacionarPortas();
    }

    private void RelacionarPortas()
    {
        foreach(GameObject porta in portas)
        {
            Porta portaScript = porta.GetComponent<Porta>();
            foreach(GameObject gatilho in gatilhos)
            {
                Gatilho gatilhoScript = gatilho.GetComponent<Gatilho>();
                if(portaScript.id == gatilhoScript.id)
                {
                    portaScript.gatilhosRelacionados.Add(gatilho);
                }
            }
        }
    }

    public void ChecarPorta(int id)
    {
        foreach(GameObject porta in portas)
        {
            if(porta.GetComponent<Porta>().id == id)
            {
                if(porta.GetComponent<Porta>().ChecarGatilhos() == true)
                {
                    FecharAbrirPortas(id);
                }
            }
        }
    }

    public void FecharAbrirPortas(int id)
    {
        foreach(GameObject porta in portas)
        {
            if(id == porta.GetComponent<Porta>().id)
            {
                porta.GetComponent<Porta>().FecharPorta();
            }
            else
            {
                if (gatilhos.Length < 3)
                {
                    porta.GetComponent<Porta>().AbrirPorta();
                }
            }
        }
    }
}
