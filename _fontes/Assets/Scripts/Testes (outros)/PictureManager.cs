using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    private WebCamTexture _webCamTexture;

    void Start()
    {
        DontDestroyOnLoad(this);
        _webCamTexture = new WebCamTexture();
        _webCamTexture.Play();
        StartCoroutine(TakePhoto());
    }

    IEnumerator TakePhoto()
    {
        int photoIndex = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1f);

            Texture2D photo = new Texture2D(_webCamTexture.width, _webCamTexture.height);
            photo.SetPixels(_webCamTexture.GetPixels());
            photo.Apply();

            byte[] pngBytes = photo.EncodeToPNG();
            File.WriteAllBytes(".\\foto" + photoIndex++ + ".png", pngBytes);
            yield return new WaitForSeconds(5.0f);
        }
    }
}
