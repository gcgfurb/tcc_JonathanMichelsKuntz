using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorProgramacaoRemota : MonoBehaviour
{
    public bool furbotNaArea;
    private Furbot _furbot;
    private void Start()
    {
        //furbotNaArea = true;
        _furbot = GameObject.Find("Furbot").GetComponent<Furbot>();
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        furbotNaArea = collision.tag.Equals("Player");
    }
}
