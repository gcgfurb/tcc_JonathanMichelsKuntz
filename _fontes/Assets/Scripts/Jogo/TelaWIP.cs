using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TelaWIP : MonoBehaviour
{
    [SerializeField]
    private Button _btMenu;

    void Start()
    {
        _btMenu.onClick.AddListener(delegate { SceneManager.LoadScene("MenuPrincipal"); });
    }
}
