using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialProjecao : MonoBehaviour
{
    //Floats
    public float velocidadeTexto;

    private Animator _fade;

    //Images
    public Image interfaceExemplo;

    //Texts
    public Text dialogText;

    //Objetos
    public GameObject s223, dialogPanel, projecao, seta_programar, seta_vidas, seta_energia, seta_bau, seta_pontos, seta_inventario;
    public GameObject[] posicoes;

    //Animator
    public Animator seta_anim;

    //Strings
    public List<string[]> dialogo = new List<string[]>();
    public string[] falaAtual;

    //Integers
    public int indiceFala, indiceDialogo, indicePosicao, touchCount;

    //Booleans
    [SerializeField]
    private bool _pulavel;

    /* ---------------------------------------------------------------------------------------------------------------------------
     * DIALOGO DO TUTORIAL
     */
    public string[] falasIniciais = //Introducao do tutorial
    {
        "Olá!",
        "Então é você que irá nos ajudar?! Temos muito a fazer!",
        "Como eu sou uma das únicas que conhece o Furbot, vou te mostrar como ele funciona!"
    };

    public string[] falasParte1 = //Mostra a imagem da interface de exemplo
    {
        "Esta é a tela do Furbot!",
        "Aqui você pode dar comandos para ele, checar os contadores de energia, de tesouros e vidas."
    };

    public string[] falasParte2 = //Move a S223 para a direita, apontando para a janela de comandos.
    {
        "Nesta janela você pode escrever os comandos que quer que o Furbot execute!",
        "Basta escrever um ou mais comandos e depois clicar no botão Executar.",
        "Um comando de exemplo: andar(abaixo);",
        "Não esqueça do ponto e vírgula no final de cada comando!",
        "Caso você queira relembrar os comandos, clique no botão de ajuda no canto superior esquerdo!",
        /*"A caixinha de seleção Recomeçar faz com que os comandos sejam sempre executados do início...",
        "Assim o furbot sempre começara a executar seus comandos desde sua posição inicial da fase.",
        "Se você quiser continuar programando de onde o furbot parou, basta desmarcar a caixinha!"*/
    };

    public string[] falasParte3 = //Move a S223 para a barra inferior, no canto esquerdo, para apresentar as VIDAS.
    {
        "Nesta barra inferior, você pode conferir os contadores do Furbot!",
        "Este é o contador de vidas, não deixe que elas acabem!",
        "Caso você use todas as suas vidas, terá de recomeçar a região atual!",
    };

    public string[] falasParte4 = //Move a S223 um pouco mais para a direita, para apresentar a ENERGIA.
    {
        "Este é o contador de energia do Furbot!",
        "Cada linha de comando executada pelo Furbot gasta energia.",
        "Se você estiver andando em uma estrada ou caminho de terra, gastará menos energia!",
        "Entretanto, se andar fora do caminho, cada linha de comando gastara 5% de energia.",
        "Portanto, tenha cuidado ao explorar a fase! Sua energia pode acabar rapidamente.",
        "Caso sua energia chegue à zero, uma vida será perdida e a energia sera reabastecida."
    };

    public string[] falasParte5 = //Move a S223 um pouco mais para a direita, para apresentar os TESOUROS.
    {
        "Este é o contador de tesouros!",
        "Conforme você caminha pelas fases em busca da professora, você irá encontrar tesouros.",
        "Estes tesouros contém informações valiosas para o laboratório!",
        "Tente coletar o máximo que conseguir para ganhar mais pontos e recompensas!"
    };

    public string[] falasParte6 = //Move a S223 um pouco mais para a direita, para apresentar a PONTUACAO.
    {
        "Ao coletar tesouros, pistas ou concluir outros objetivos, você irá receber pontos!",
        "Os pontos acumulados na fase atual podem ser verificados aqui.",
        "Estes pontos representam o seu desempenho em cada fase!",
        "Eles também podem ser usados na loja para comprar melhorias para o Furbot!",
        "Por isso, tente coletar o máximo de tesouros e fazer todos os objetivos nas fases!!"
    };

    public string[] falasParte7 = //Move a S223 um pouco mais a direita, para apresentar o INVENTARIO.
    {
        "Esta é a mochila do Furbot!",
        "Ao andar pelo mundo, você precisará coletar alguns objetos.",
        "Os objetos que você coletou na fase irão aparecer aqui!"
    };

    public string[] falaFinal = //Fecha a interface e move a S223 para o centro da projecao, quando acabar troca de cena.
    {
        "Bem, acho que você já entendeu como funciona o Furbot!",
        "Vou ativa-lo para você praticar! Vejo você na área de testes!"
    };
    // ---------------------------------------------------------------------------------------------------------------------------

    private void Update()
    {
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                touchCount++;
                if (touchCount == 7)
                {
                    StartCoroutine(EsperarFade());
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F9)){
            StartCoroutine(EsperarFade());
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            if (touchCount++ == 7 && _pulavel)
            {
                StartCoroutine(EsperarFade());
            }
        }
#endif
    }

    void Start()
    {
        velocidadeTexto = 0.05f;
        indicePosicao = -1;
        dialogo.Add(falasIniciais);
        dialogo.Add(falasParte1);
        dialogo.Add(falasParte2);
        dialogo.Add(falasParte3);
        dialogo.Add(falasParte4);
        dialogo.Add(falasParte5);
        dialogo.Add(falasParte6);
        dialogo.Add(falasParte7);
        dialogo.Add(falaFinal);
        dialogText.text = "";
        StartCoroutine(EsperarProjecao());
        if (GameObject.Find("LevelManager") != null)
        {
            _pulavel = true;
        }
        _fade = GameObject.Find("Fade").GetComponent<Animator>();
    }

    void ComeçarDialogo()
    {
        indiceFala = 0;
        indiceDialogo = 0;
        falaAtual = dialogo[indiceDialogo];
        StartCoroutine(ReproduzirDialogo(falaAtual[indiceFala]));
    }

    void ProximaFrase()
    {
        dialogText.text = "";
        if (++indiceFala >= falaAtual.Length)
        {
            indiceDialogo++;
            if (indiceDialogo == 1)
            {
                interfaceExemplo.gameObject.SetActive(true);
            }
            if (indiceDialogo == 2)
            {
                seta_programar.SetActive(true);
            }
            if (indiceDialogo == 3)
            {
                seta_programar.SetActive(false);
                seta_vidas.SetActive(true);
            }
            if (indiceDialogo == 4)
            {
                seta_vidas.SetActive(false);
                seta_energia.SetActive(true);
            }
            if (indiceDialogo == 5)
            {
                seta_energia.SetActive(false);
                seta_bau.SetActive(true);
            }
            if (indiceDialogo == 6)
            {
                seta_bau.SetActive(false);
                seta_pontos.SetActive(true);
            }
            if (indiceDialogo == 7)
            {
                seta_pontos.SetActive(false);
                seta_inventario.SetActive(true);
            }
            if (indiceDialogo == dialogo.Count)
            {
                StartCoroutine(EsperarFade());
            }
            if (indiceDialogo == dialogo.Count - 1)
            {
                seta_inventario.SetActive(false);
                interfaceExemplo.gameObject.SetActive(false);
            }
            dialogPanel.SetActive(false);
            if (indiceDialogo < dialogo.Count)
                falaAtual = dialogo[indiceDialogo];
            indiceFala = 0;
            if (indiceDialogo > 1)
            {
                StartCoroutine(MoverS223_IniciarDialogo());
            }
            else
            {
                StartCoroutine(ReproduzirDialogo(falaAtual[indiceFala]));
            }
        }
        else
        {
            StartCoroutine(ReproduzirDialogo(falaAtual[indiceFala]));
        }
    }

    private IEnumerator EsperarFade()
    {
        _fade.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("SelecaoDeFases");
    }

    private IEnumerator EsperarProjecao()
    {
        yield return new WaitForSeconds(3.0f);
        s223.SetActive(true);
        StartCoroutine(EsperarS223_IniciarDialogo());
    }

    private IEnumerator EsperarS223_IniciarDialogo()
    {
        yield return new WaitForSeconds(1.0f);
        ComeçarDialogo();
    }

    private IEnumerator ReproduzirDialogo(string falaAtual)
    {
        dialogPanel.SetActive(true);
        int contadorDeChar = 0;
        while (contadorDeChar < falaAtual.Length)
        {
            dialogText.text += falaAtual[contadorDeChar++];
            yield return new WaitForSeconds(velocidadeTexto);
        }
        yield return new WaitForSeconds(3.5f);
        ProximaFrase();
    }

    private IEnumerator MoverS223_IniciarDialogo()
    {
        float endTime = Time.time + 2.0f;
        if (++indicePosicao == 6)
        {
            indicePosicao = 0;
        }
        while (Time.time < endTime)
        {
            s223.transform.position = Vector2.MoveTowards(s223.transform.position, posicoes[indicePosicao].transform.position, 0.05f);
            yield return null;
        }
        if (indiceDialogo < dialogo.Count)
            StartCoroutine(ReproduzirDialogo(falaAtual[indiceFala]));
    }
}
