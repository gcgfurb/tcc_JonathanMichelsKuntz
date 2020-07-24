using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using OpenCVForUnity;
using OpenCVForUnity.MlModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ObjdetectModule;

using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnity.UtilsModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnityExample;
using UnityEngine.EventSystems;
using System;
using Assets.Scripts.Jogo;
using Assets.Scripts;

public class InterfaceTangivel : MonoBehaviour
{
    private int IndexUltPecaExecutada;
    private GameObject PacoteInterfaceTangivel;
    private Texture2D TexturaImagem;
    private Detector DetectorPecaModelos;
    private Mat MatrizImagem;
    private Peca PecaPlay;
    private float TempEspera, TempAtual;
    private bool isCalibrando;
    private GameObject PecaAdd;
    private WebCamControle WebCamConfiguracao;

    public Renderer FundoCalibrar;
    public RawImage FundoWebCam;
    public Button BtnVisualizar, BtnTrocaDeCam, BtnConfigurar;
    public UIManager ControleJogo;
    public GameObject RolagemPecaModelos, PecaModelo;
    public ScrollRect RolagemVisualPecaModelo;

    void Start()
    {
        Utils.setDebugMode(LevelManager.devMode);
        this.PecaPlay = new Peca(TipoPeca.PLAY,0);
        this.WebCamConfiguracao = new WebCamControle();

        if (this.BtnVisualizar != null)
            this.BtnVisualizar.onClick.AddListener(delegate { VisualizaWebCam(); });

        if (this.BtnTrocaDeCam != null)
            this.BtnTrocaDeCam.onClick.AddListener(delegate { this.WebCamConfiguracao.ProximaWebCam(); });

        if (this.BtnConfigurar != null)
            this.BtnConfigurar.onClick.AddListener(delegate { VerificaTipoInteracao(); });

        if (this.FundoCalibrar != null || this.FundoWebCam != null)
        {
            this.isCalibrando = this.FundoCalibrar != null;
            this.DetectorPecaModelos = new Detector();
            this.VerificaTipoInteracao();
        }
    }

    private void ConfiguraTexturaImagem(Texture2D TexturaImagem)
    {
        if (this.FundoCalibrar != null)
        {
            this.FundoCalibrar.material.mainTexture = TexturaImagem;
        }
        else
        {
            if (this.FundoWebCam != null)
                this.FundoWebCam.texture = TexturaImagem;
        }
    }

    private void VerificaTipoInteracao()
    {
        if (this.PacoteInterfaceTangivel == null)
        {
            this.PacoteInterfaceTangivel = this.gameObject;
            this.PacoteInterfaceTangivel.SetActive(LevelManager.isInterfaceTangivel);
        }

        if (LevelManager.isInterfaceTangivel)
        {
            this.WebCamConfiguracao.Play();
        }
        else
        {
            this.WebCamConfiguracao.Stop();
        }
    }

    private void VisualizaWebCam()
    {
        if (this.FundoCalibrar != null)
        {
            this.FundoCalibrar.gameObject.SetActive(!this.FundoCalibrar.gameObject.activeSelf);
        }
        else
        {
            if (this.FundoWebCam != null)
                this.FundoWebCam.gameObject.SetActive(!this.FundoWebCam.gameObject.activeSelf);
        }
    }

    private Mat RetornaMatriz()
    {
        if (this.WebCamConfiguracao.IsFuncionando())
        {
            if (this.MatrizImagem == null ||
                this.MatrizImagem.rows() != this.WebCamConfiguracao.getWebCamImagem().height ||
                this.WebCamConfiguracao.getWebCamImagem().width != this.MatrizImagem.cols())
            {
                this.MatrizImagem = new Mat(this.WebCamConfiguracao.getWebCamImagem().height, this.WebCamConfiguracao.getWebCamImagem().width, CvType.CV_8UC3);
            }

            Utils.webCamTextureToMat(this.WebCamConfiguracao.getWebCamImagem(), this.MatrizImagem);

            return this.MatrizImagem;
        }
        else
        {
            return null;
        }
    }

    private Texture2D RetornaTexturaImagem(Mat img)
    {
        if (img == null)
            img = this.RetornaMatriz();

        if (img != null && img.cols() > 0 && img.rows() > 0)
        {
            if (this.TexturaImagem == null || this.TexturaImagem.width != img.cols() || this.TexturaImagem.height != img.rows())
                this.TexturaImagem = new Texture2D(img.cols(), img.rows(), TextureFormat.RGBA32, false);
            
            return this.TexturaImagem;
        }
        else
        {
            return null;
        }
    }

    void OnDestroy()
    {
        this.WebCamConfiguracao.Stop();

        Texture2D.Destroy(this.TexturaImagem);

        this.TexturaImagem = null;

        if (this.FundoCalibrar != null)
        {
            this.FundoCalibrar.material.mainTexture = null;
        }
        else
        {
            if (this.FundoWebCam != null)
                this.FundoWebCam.material.mainTexture = null;
        }
    }

    void Update()
    {
        if (this.WebCamConfiguracao.IsFuncionando())
        {
            this.MatrizImagem = this.RetornaMatriz();
            this.ConfiguraTexturaImagem(this.RetornaTexturaImagem(this.MatrizImagem));

            if(this.TempEspera > 0)
                this.TempAtual += Time.deltaTime;

            if (this.TempAtual >= this.TempEspera && 
                this.MatrizImagem != null && 
                this.DetectorPecaModelos != null && 
                ((this.ControleJogo != null && 
                !this.ControleJogo.emDialogo && 
                !this.ControleJogo.furbot.emExecucao) || 
                this.ControleJogo == null))
            {
                if (this.isCalibrando)
                    this.DetectorPecaModelos.CalibrarPecas(this.MatrizImagem, this.FundoCalibrar.gameObject);

                this.MatrizImagem = this.DetectorPecaModelos.VeriicaPecas(this.MatrizImagem, this.isCalibrando);

                if (this.MatrizImagem != null)
                {
                    this.ConfiguraTexturaImagem(this.RetornaTexturaImagem(MatrizImagem));

                    if (this.ControleJogo != null && 
                        this.DetectorPecaModelos.GetListaPecas() != null && 
                        this.DetectorPecaModelos.GetListaPecas().Count != this.IndexUltPecaExecutada)
                    {
                        if (this.PecaAdd != null && this.IndexUltPecaExecutada > 0)
                            Destroy(this.PecaAdd);

                        if (!this.isCalibrando) {
                            this.TempAtual = 0;
                            this.TempEspera = 3;
                        }

                        this.ControleJogo.ExecutarCodigo();
                        
                        this.MostrarPecaModeloUI(this.PecaPlay);

                        this.RolagemVisualPecaModelo.verticalScrollbar.value = 0f;
                        Canvas.ForceUpdateCanvases();
                    }
                }
            }

            if (this.TempAtual > this.TempEspera)
            {
                this.TempEspera = 0;
            }

            Utils.matToTexture2D(this.MatrizImagem, this.TexturaImagem);
        }
    }

    public string CodigoPecasLista()
    {
        string codigoPecaModelos = "";

        if (this.DetectorPecaModelos.GetListaPecas() != null && this.IndexUltPecaExecutada < this.DetectorPecaModelos.GetListaPecas().Count)
        {
            for (int i = this.DetectorPecaModelos.GetListaPecas().Count-1; i > this.IndexUltPecaExecutada -1; i--)
            {
                this.MostrarPecaModeloUI(this.DetectorPecaModelos.GetListaPecas()[i]);
            }

            for (int i = this.IndexUltPecaExecutada - (this.IndexUltPecaExecutada > 0 ? 1 : 0); i < this.DetectorPecaModelos.GetListaPecas().Count; i++)
            {
                codigoPecaModelos += this.DetectorPecaModelos.GetListaPecas()[i].GetComando(codigoPecaModelos) + "\n";
            }

            this.IndexUltPecaExecutada = this.DetectorPecaModelos.GetListaPecas().Count;
        }

        return codigoPecaModelos;
    }

    private void MostrarPecaModeloUI(Peca PecaModelo)
    {
        this.PecaAdd = Instantiate(this.PecaModelo);

        this.PecaAdd.GetComponent<SpriteRenderer>().sprite = PecaModelo.GetTipoImagem();
        this.PecaAdd.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = PecaModelo.GetDirecaoImagem();
        this.PecaAdd.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = PecaModelo.GetQuantidadeImagem();

        this.PecaAdd.transform.SetParent(RolagemPecaModelos.transform, false);
    }
}