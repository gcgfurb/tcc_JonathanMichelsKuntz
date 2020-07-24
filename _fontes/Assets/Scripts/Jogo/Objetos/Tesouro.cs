using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tesouro : MonoBehaviour
{
    /// <summary>
    /// Este método destrói o objeto tesouro.
    /// </summary>
    public void Destruir()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals(this.tag))
        {
            this.Destruir();
        }
    }
}
