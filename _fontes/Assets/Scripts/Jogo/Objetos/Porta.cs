using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Porta : MonoBehaviour
{

    private Animator _anim;

    public int id;
    public string estado;
    public List<GameObject> gatilhosRelacionados = new List<GameObject>();
    public GameObject bolhaPrefab;
    public GameObject backgroundBolhas;

    void Start()
    {
        _anim = GetComponent<Animator>();
        estado = "aberta";
        StartCoroutine(SpawnBolha());
    }

    public void AbrirPorta()
    {
        estado = "aberta";
        _anim.SetBool("Destacado", false);
        _anim.SetBool("Abrir", true);
        _anim.SetBool("Fechar", false);
    }

    public void FecharPorta()
    {
        estado = "fechada";
        _anim.SetBool("Destacado", false);
        _anim.SetBool("Abrir", false);
        _anim.SetBool("Fechar", true);
    }

    public bool ChecarGatilhos()
    {
        foreach(GameObject gatilho in gatilhosRelacionados)
        {
            Gatilho gatilhoScript = gatilho.GetComponent<Gatilho>();
            if(gatilhoScript.pressionado == false)
            {
                return false;
            }
        }
        return true;
    }
    public void DestacarPorta(bool destacar)
    {
        if (destacar)
        {
            _anim.SetBool("Abrir", false);
            _anim.SetBool("Fechar", false);
            _anim.SetBool("Destacado", true);

        }
        else
        {
            if (estado.Equals("aberta"))
            {
                AbrirPorta();
            }
            else
            {
                FecharPorta();
            }
        }
    }

    public IEnumerator SpawnBolha()
    {
        while (true)
        {
            Vector3 posicaoRandom = new Vector3(Random.Range(0.45f, -0.45f), Random.Range(0.45f, -0.45f), 0);
            GameObject bolha = Instantiate(bolhaPrefab);
            bolha.transform.SetParent(backgroundBolhas.transform);
            bolha.transform.localPosition = posicaoRandom;
            StartCoroutine(BolhaExplodindo(bolha));
            yield return new WaitForSeconds(Random.Range(0.1f, 0.6f));
        }
    }

    public IEnumerator BolhaExplodindo(GameObject bolha)
    {
        float tamanhoMaxBolha = Random.Range(0.10f, 0.15f);
        while(bolha.transform.localScale.x < tamanhoMaxBolha)
        {
            bolha.transform.localScale += new Vector3(0.007f, 0.007f, 0.0f);
            yield return null;
        }
        Destroy(bolha.gameObject);
        yield return null;
    }
}
