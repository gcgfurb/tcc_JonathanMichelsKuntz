using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S223 : MonoBehaviour
{
    //Objetos
    private SpriteRenderer _spriteRenderer;
    private UIManager _uiManager;
    [SerializeField]
    private Sprite[] _sprites;

    // Use this for initialization
    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _uiManager = GameObject.Find("CanvasInterface").GetComponent<UIManager>();
    }

    /// <summary>
    /// Inverte a imagem no eixo x.
    /// </summary>
    public void SetSprite(Direcao direcao)
    {
        switch (direcao)
        {
            case Direcao.DIREITA:
                _spriteRenderer.sprite = _sprites[0];
                break;
            case Direcao.ESQUERDA:
                _spriteRenderer.sprite = _sprites[1];
                break;
            case Direcao.ABAIXO:
                _spriteRenderer.sprite = _sprites[2];
                break;
            case Direcao.ACIMA:
                _spriteRenderer.sprite = _sprites[3];
                break;

        }
    }

    private void OnMouseDown()
    {
        if (!_uiManager.emDialogo)
            StartCoroutine(_uiManager.IniciarDialogo(0.2f));
    }
}
