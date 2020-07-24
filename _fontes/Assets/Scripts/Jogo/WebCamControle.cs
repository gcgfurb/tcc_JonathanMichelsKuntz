using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamControle : MonoBehaviour
{

    private WebCamTexture WebCamImg;
    private int IndexWebCam;

    void Start()
    {
        this.IndexWebCam = PlayerPrefs.GetInt("IndexWebCam");
    }

    void OnDestroy()
    {
        WebCamTexture.Destroy(this.WebCamImg);
    }

    private void ConfiguraWebCam()
    {
        WebCamDevice[] cameras = WebCamTexture.devices;

        if (cameras.Length > 0)
        {
            this.IndexWebCam = (this.IndexWebCam >= cameras.Length ? 0 : this.IndexWebCam);

            Debug.Log("Web Cam conectada : " + cameras[this.IndexWebCam].name);

            this.WebCamImg = new WebCamTexture(cameras[IndexWebCam].name);
            PlayerPrefs.SetInt("IndexWebCam", this.IndexWebCam);
        }
    }

    public void ProximaWebCam()
    {
        this.IndexWebCam++;
        this.Stop();
        this.Play();
    }

    public void Play()
    {
        this.ConfiguraWebCam();

        if (this.WebCamImg != null)
        {
                System.Threading.Thread.Sleep(500);
                this.WebCamImg.Play();
        }
    }

    public void Stop()
    {
        if (this.WebCamImg != null)
        {
            this.WebCamImg.Stop();
        }
    }

    public bool IsFuncionando()
    {
        return this.WebCamImg == null || this.WebCamImg != null && this.WebCamImg.isPlaying;
    }

    public WebCamTexture getWebCamImagem()
    {
        return this.WebCamImg;
    }
}
