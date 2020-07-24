using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer animacao1;
    public VideoPlayer animacao2;
    public VideoPlayer animacao3;
    public VideoPlayer animacao4;
    public VideoPlayer animacao5;
    public VideoPlayer animacao6;
    public VideoPlayer animacao7;
    public VideoPlayer animacao8;
    public VideoPlayer animacao9;
    public VideoPlayer videoAtual;
    public GameObject btnProximo;
    public int numClip;

    // Use this for initialization
    void Start()
    {
        numClip = 1;
        videoAtual = animacao1;
        animacao2.gameObject.SetActive(false);
        animacao3.gameObject.SetActive(false);
        animacao4.gameObject.SetActive(false);
        animacao5.gameObject.SetActive(false);
        animacao6.gameObject.SetActive(false);
        animacao7.gameObject.SetActive(false);
        animacao8.gameObject.SetActive(false);
        animacao9.gameObject.SetActive(false);

    }

    void Update()
    {
        if (!videoAtual.isPlaying)
        {
            if (!btnProximo.activeInHierarchy)
            {
                btnProximo.SetActive(true);
            }
        }

    }
    public void ProximoVideo()
    {
        numClip++;
        switch (numClip)
        {
            case 2:
                animacao1.gameObject.SetActive(false);
                animacao2.gameObject.SetActive(true);
                videoAtual = animacao2;
                animacao2.Play();
                break;
            case 3:
                animacao2.gameObject.SetActive(false);
                animacao3.gameObject.SetActive(true);
                videoAtual = animacao3;
                animacao3.Play();
                break;
            case 4:
                animacao3.gameObject.SetActive(false);
                animacao4.gameObject.SetActive(true);
                videoAtual = animacao4;
                animacao4.Play();
                break;
            case 5:
                animacao4.gameObject.SetActive(false);
                animacao5.gameObject.SetActive(true);
                videoAtual = animacao5;
                animacao5.Play();
                break;
            case 6:
                animacao5.gameObject.SetActive(false);
                animacao6.gameObject.SetActive(true);
                videoAtual = animacao6;
                animacao6.Play();
                break;
            case 7:
                animacao6.gameObject.SetActive(false);
                animacao7.gameObject.SetActive(true);
                videoAtual = animacao7;
                animacao7.Play();
                break;
            case 8:
                animacao7.gameObject.SetActive(false);
                animacao8.gameObject.SetActive(true);
                videoAtual = animacao8;
                animacao8.Play();
                break;
            case 9:
                animacao8.gameObject.SetActive(false);
                animacao9.gameObject.SetActive(true);
                videoAtual = animacao9;
                animacao9.Play();
                break;
            default:
                SceneManager.LoadScene("TutorialProjecao");
                break;
        }
        btnProximo.SetActive(false);
    }

    private IEnumerator EsperarFade()
    {
        //FadeOut
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("TutorialProjecao");
    }

}

