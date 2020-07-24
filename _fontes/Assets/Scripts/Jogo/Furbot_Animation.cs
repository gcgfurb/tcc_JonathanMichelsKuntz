using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furbot_Animation : MonoBehaviour
{
    //Objetos
    private Animator _anim;
    private Furbot _furbot;

    //Strings
    [SerializeField]
    private string _animacaoAtual;

    // Use this for initialization
    void Start()
    {
        _anim = GetComponent<Animator>();
        try
        {
            _furbot = GameObject.Find("Furbot").GetComponent<Furbot>();
        }catch(System.NullReferenceException n)
        {
            _furbot = GameObject.Find("FurbotNaoCanonico").GetComponent<Furbot>();
        }

    }

    public void MoverDireita()
    {
        _anim.speed = 1;
        _anim.SetBool("Andar_Direita", true);
        _anim.SetBool("Andar_Acima", false);
        _anim.SetBool("Andar_Abaixo", false);
        _anim.SetBool("Andar_Esquerda", false);
        _animacaoAtual = _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    public void MoverEsquerda()
    {
        _anim.speed = 1;
        _anim.SetBool("Andar_Esquerda", true);
        _anim.SetBool("Andar_Acima", false);
        _anim.SetBool("Andar_Abaixo", false);
        _anim.SetBool("Andar_Direita", false);
        _animacaoAtual = _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    public void MoverAcima()
    {
        _anim.speed = 1;
        _anim.SetBool("Andar_Acima", true);
        _anim.SetBool("Andar_Abaixo", false);
        _anim.SetBool("Andar_Direita", false);
        _anim.SetBool("Andar_Esquerda", false);
        _animacaoAtual = _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    public void MoverAbaixo()
    {
        _anim.speed = 1;
        _anim.SetBool("Andar_Abaixo", true);
        _anim.SetBool("Andar_Acima", false);
        _anim.SetBool("Andar_Direita", false);
        _anim.SetBool("Andar_Esquerda", false);
        _animacaoAtual = _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    public void Parar()
    {
        _anim.speed = 0;
        //_anim.SetBool("Parado", true);
        _anim.SetBool("Andar_Abaixo", false);
        _anim.SetBool("Andar_Acima", false);
        _anim.SetBool("Andar_Direita", false);
        _anim.SetBool("Andar_Esquerda", false);
    }
}