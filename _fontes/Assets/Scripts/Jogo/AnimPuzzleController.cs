using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPuzzleController : MonoBehaviour
{
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void Abrir()
    {
        _anim.SetBool("Abrir", true);
    }

    public void Fechar()
    {
        _anim.StopPlayback();
    }

    public void ReiniciarAnimacao()
    {
        _anim.SetBool("Abrir", false);
        _anim.Play("Fechado", -1, 0);
    }
}
