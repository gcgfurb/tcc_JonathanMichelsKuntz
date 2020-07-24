using OpenCVForUnityExample;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuConfiguracao : MonoBehaviour
{
    //Objetos
    public Button btnInterfaceTangivel, btnNormal, btnSalvar, btnCancelar;
    public Transform ConteinerIntefaceTangivel, ConteinerSelecaoInterface;

    void Start()
    {
        btnInterfaceTangivel.onClick.AddListener(delegate { AlterarTipoInterface(true); });
        btnNormal.onClick.AddListener(delegate { AlterarTipoInterface(false); });
        btnSalvar.onClick.AddListener(delegate { Salvar(); });
        btnCancelar.onClick.AddListener(delegate { AlterarTipoInterface(false); });
    }

    private void AlterarTipoInterface(bool isInterface)
    {
        LevelManager.isInterfaceTangivel = isInterface;
        ConteinerIntefaceTangivel.gameObject.SetActive(isInterface);
        ConteinerSelecaoInterface.gameObject.SetActive(!isInterface);

        PlayerPrefs.SetInt("isInterfaceTangivel", Convert.ToInt32(isInterface));

        if (!isInterface)
            this.VoltarScene();
    }

    private void VoltarScene()
    {
        var scene = PlayerPrefs.GetString("sceneAnteriorConfiguracao");

        //WebCam.Stop();

        if (String.IsNullOrEmpty(scene)) 
        { 
            SceneManager.LoadScene("MenuPrincipal");
        }
        else
        {
            SceneManager.LoadScene(scene);
        }

    }

    private void Salvar()
    {
        if (ColorBlobDetector.GetColorPoint() != null)
        {
            PlayerPrefs.SetFloat("ScalarPeca1", Convert.ToSingle(ColorBlobDetector.GetColorPoint().val[0]));
            PlayerPrefs.SetFloat("ScalarPeca2", Convert.ToSingle(ColorBlobDetector.GetColorPoint().val[1]));
            PlayerPrefs.SetFloat("ScalarPeca3", Convert.ToSingle(ColorBlobDetector.GetColorPoint().val[2]));
            PlayerPrefs.SetFloat("ScalarPeca4", Convert.ToSingle(ColorBlobDetector.GetColorPoint().val[3]));
        }
        else
        {
            PlayerPrefs.SetFloat("ScalarPeca1", 0);
            PlayerPrefs.SetFloat("ScalarPeca2", 0);
            PlayerPrefs.SetFloat("ScalarPeca3", 0);
            PlayerPrefs.SetFloat("ScalarPeca4", 0);
        }

        PlayerPrefs.SetInt("isInterfaceTangivel", Convert.ToInt32(ColorBlobDetector.GetColorPoint() != null));
        this.VoltarScene();
    }
}