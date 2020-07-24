using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjetosGeradorDeFase : MonoBehaviour
{

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1) && SceneManager.GetActiveScene().Equals("FaseGerada"))
        {
            Destroy(this.gameObject);
        }
    }
}
