using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatilho : MonoBehaviour
{
    [SerializeField] private Sprite gatilhoUp;
    [SerializeField] private Sprite gatilhoDown;

    public int id;
    public bool pressionado;

    private PortasManager _portasManager;

    // Start is called before the first frame update
    void Start()
    {
        _portasManager = GameObject.Find("PortasManager").GetComponent<PortasManager>();
        pressionado = false;
    }


    public void Pressionar(bool condicao)
    {
        if (condicao)
        {
            pressionado = true;
            GetComponent<SpriteRenderer>().sprite = gatilhoDown;
        }
        else
        {
            pressionado = false;
            GetComponent<SpriteRenderer>().sprite = gatilhoUp;
        }
        _portasManager.ChecarPorta(id);
    }

    public void OnMouseEnter()
    {
        foreach (GameObject porta in _portasManager.portas)
        {
            if(porta.GetComponent<Porta>().id == this.id)
            {
                porta.GetComponent<Porta>().DestacarPorta(true);
            }
        }
    }
    public void OnMouseExit()
    {
        foreach (GameObject porta in _portasManager.portas)
        {
            if (porta.GetComponent<Porta>().id == this.id)
            {
                porta.GetComponent<Porta>().DestacarPorta(false);
            }
        }
    }
    
}
