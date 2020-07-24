using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compilador : MonoBehaviour
{
    //Componentes
    private UIManager _uiManager;
    private Furbot _furbot;
    private Analisador _analisador;
    private GameManager _gameManager;
    private BuggienAndroid _buggien;

    //Strings
    private string _textoLido;
    private string _textoGabarito;
    private string _textoAntigo;
    private bool _condicaoRetorno;
    private bool isFurbot;

    //Pilhas
    private Stack _pontoDeRetorno = new Stack();

    //Integers
    private int _ultimoPontoDeRetorno;
    private int index;


    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("CanvasInterface").GetComponent<UIManager>();
        isFurbot = gameObject.name.Equals("Furbot");
        if (isFurbot)
        {
            _furbot = GetComponent<Furbot>();
        }
        else
        {
            _buggien = GetComponent<BuggienAndroid>();
        }
        _analisador = GetComponent<Analisador>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    /// <summary>
    /// Remove os comandos já digitados do codigo atual
    /// </summary>
    /// <param name="textoAtual">Código atual</param>
    /// <returns>Código sem os comandos já usados</returns>
    private string[] LimparTexto(string[] textoAtual)
    {
        string[] textoAntigo;
        if (_textoLido.Length > 0)
        {
            textoAntigo = _textoLido.Split('\n');
        }
        else
        {
            textoAntigo = _textoAntigo.Split('\n');
        }
        string textoLimpo = "";

        if (textoAntigo.Length > textoAtual.Length)
        {
            throw new Exception();
        }
        else
        {
            for (int i = 0; i < textoAntigo.Length; i++)
            {
                if (textoAntigo[i].Equals(textoAtual[i].Replace("\r", "")))
                {
                    textoAtual[i] = "";
                }
            }
            for (int i = 0; i < textoAtual.Length; i++)
            {
                if (!textoAtual[i].Equals(""))
                {
                    textoLimpo += textoAtual[i] + "\n";
                }
            }
            return textoLimpo.Split('\n');
        }

    }

    /// <summary>
    /// Lê casa linha digitada no console e executa a devida ação.
    /// </summary>
    public IEnumerator LerComandos(string codigo)
    {
        bool PodeLer = true;
        string[] textoDividido;
        bool ChaveDoSe = false;
        int linhaInicio = 0;


        if (_furbot.IsPrimeiroPasso())
        {
            _furbot.SetPosicaoInicial(transform.position);
            _furbot.SetPrimeiroPasso(false);
        }


        _analisador.SetTexto(_uiManager.GetTexto());
        if (_uiManager.Recomecar())
        {
            textoDividido = codigo.Split('\n');
            _textoAntigo = codigo.TrimEnd();

            if (!LevelManager.isInterfaceTangivel)
            {
                linhaInicio = 0;
                _textoLido = "";
                _textoGabarito = "";

                _furbot.ReiniciarValores();
            }
        }
        else
        {
            if (!_textoAntigo.Equals(""))
            {
                try
                {
                    textoDividido = LimparTexto(codigo.Split('\n'));
                    if (textoDividido[0].Equals(""))
                    {
                        _uiManager.RecuperarTexto();
                        _uiManager.debug.SetActive(false);
                        _uiManager.btnExecutar.GetComponentInChildren<Text>().text = "EXECUTAR";
                        _uiManager.btnExecutar.image.sprite = _uiManager.btnExecutarPlay;
                        _uiManager.btnRecomecar.interactable = true;
                        _uiManager.ifCodigo.interactable = true;
                        yield break;
                    }
                    linhaInicio = _textoLido.Split('\n').Length - 1;
                    _textoAntigo = codigo.TrimEnd();
                }
                catch (Exception e)
                {
                    textoDividido = codigo.Split('\n');
                    linhaInicio = 0;
                    _textoAntigo = codigo.TrimEnd();
                }
            }
            else
            {
                textoDividido = codigo.Split('\n');
                linhaInicio = 0;
                _textoAntigo = codigo.TrimEnd();
            }
        }

        try
        {
            _uiManager.SalvarTexto();
            CompilarCodigo(codigo.ToCharArray());
            _uiManager.RecuperarTexto();
        }
        catch (System.ArgumentException e)
        {
            _analisador.IncrementarErrosComp();
            _uiManager.AddLog("Erro de compilação - " + e.Message);

            _furbot.AbrirCaixaDeErro(e.Message, out PodeLer);
        }
        catch (Exception ex)
        {
            _analisador.IncrementarErrosComp();
            _uiManager.AddLog("Erro de compilação - " + ex.Message);

            _furbot.AbrirCaixaDeErro("Erro de compilação", out PodeLer);
        }
        if (PodeLer)
        {
            _furbot.emExecucao = true;
            _uiManager.AddLog("Compilado com sucesso!");

            // _uiManager.debug.SetActive(true);
            int ponteiroLinha = 0;
            string linha;
            _uiManager.SalvarTexto();
            while (ponteiroLinha < textoDividido.Length && !_furbot.bateu)  // é aqui que ele começa a ler mesmo
            {
                index = ponteiroLinha + linhaInicio;
                if (!_uiManager.emDialogo)
                    _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(index);

                linha = textoDividido[ponteiroLinha].ToLower().Trim();
                _textoLido += linha + "\n";
                if (linha.Contains("puxaralavanca();"))
                {
                    if (_furbot.usarAlavanca == true)
                    {
                        _furbot.puzzleManager.AtivarAlavanca(_furbot.alavancaAtual.GetComponent<Alavanca>().id);
                    }
                    else
                    {
                        _uiManager.Diga("Não há nenhuma alavanca aqui");
                    }
                    yield return new WaitForSeconds(1.0f);
                }
                if (linha.Contains("andar(")) // tratamento do comando andar
                {
                    if (linha.Contains("direita"))
                    {

                        _textoGabarito += "andar(direita);";
                        _furbot.Andar(Direcao.DIREITA);
                        yield return new WaitForSeconds(1.0f);

                    }
                    else if (linha.Contains("esquerda"))
                    {
                        _textoGabarito += "andar(esquerda);";
                        _furbot.Andar(Direcao.ESQUERDA);
                        yield return new WaitForSeconds(1.0f);

                    }
                    else if (linha.Contains("acima"))
                    {
                        _textoGabarito += "andar(acima);";

                        _furbot.Andar(Direcao.ACIMA);
                        yield return new WaitForSeconds(1.0f);

                    }
                    else if (linha.Contains("abaixo"))
                    {
                        _textoGabarito += "andar(abaixo);";

                        _furbot.Andar(Direcao.ABAIXO);
                        yield return new WaitForSeconds(1.0f);
                    }
                }

                if (linha.Contains("enquanto(")) // tratamento do comando enquanto
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(ponteiroLinha);

                    yield return new WaitForSeconds(0.3f);

                    Condicional(linha, out _condicaoRetorno);
                    if (_condicaoRetorno)
                    {
                        _pontoDeRetorno.Push(ponteiroLinha);
                    }
                    else
                    {
                        while (!textoDividido[ponteiroLinha].Trim().ToLower().Contains("}"))
                        {
                            ponteiroLinha++;
                        }
                    }
                }

                if (linha.Contains("se")) // tratamento do comando se
                {
                    if (linha.Contains("senao"))
                    {
                        _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(ponteiroLinha);

                        yield return new WaitForSeconds(0.3f);

                        if (_condicaoRetorno)
                        {
                            while (!textoDividido[ponteiroLinha].Trim().ToLower().Equals("}"))
                            {
                                if (ponteiroLinha + 1 <= textoDividido.Length)
                                {
                                    ponteiroLinha++;
                                }
                            }
                        }
                    }
                    _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(ponteiroLinha);

                    yield return new WaitForSeconds(0.3f);

                    Condicional(linha, out _condicaoRetorno);
                    if (!_condicaoRetorno)
                    {
                        while (!textoDividido[ponteiroLinha].Trim().ToLower().Equals("}"))
                        {
                            if (ponteiroLinha + 1 <= textoDividido.Length)
                            {
                                ponteiroLinha++;
                            }
                        }
                    }
                    else
                    {
                        ChaveDoSe = true;
                    }
                }



                if (linha.Contains("diga("))
                {
                    string texto = linha.Replace("diga(", "").Replace(");", "");
                    StartCoroutine(_uiManager.Diga(texto));
                }

                if (linha.Trim().Equals("}"))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(ponteiroLinha);

                    yield return new WaitForSeconds(0.1f);
                    if (ChaveDoSe)
                    {
                        ChaveDoSe = false;
                        continue;
                    }
                    Condicional(textoDividido[GetPontoDeRetorno()], out _condicaoRetorno);
                    if (_condicaoRetorno)
                    {
                        if (_pontoDeRetorno.Count != 0)
                        {
                            ponteiroLinha = (int)_pontoDeRetorno.Peek();
                            _ultimoPontoDeRetorno = (int)_pontoDeRetorno.Peek();
                        }
                    }
                    else
                    {
                        if (_pontoDeRetorno.Count != 0)
                        {
                            _ultimoPontoDeRetorno = (int)_pontoDeRetorno.Pop();
                        }
                    }
                }
                if (ponteiroLinha < textoDividido.Length - 1)
                {
                    ponteiroLinha++;
                }
                else break;
            }
            _furbot.bateu = false;
        }
        _furbot.direcaoAtual = "Parado";
        _uiManager.debug.SetActive(false);
        _uiManager.btnExecutar.GetComponentInChildren<Text>().text = "EXECUTAR";
        _uiManager.btnExecutar.image.sprite = _uiManager.btnExecutarPlay;
        _uiManager.btnRecomecar.interactable = true && !_uiManager.emDialogo;
        _uiManager.ifCodigo.interactable = true && !_uiManager.emDialogo;
        _furbot.emExecucao = false;
        _uiManager.AtualizarNumeros();
        index = 0;
        _uiManager.permitidoDigitarComandos = true && !_uiManager.emDialogo;
        if (!_uiManager.emDialogo)
        {
            _uiManager.RecuperarTexto();
        }
        //Debug.Log(_textoLido);
    }

    /// <summary>
    /// Quando um laço de repetição é concluído, este método é chamado para refazer a iteração.
    /// </summary>
    /// <returns>A linha que é pra fazer a nova iteração.</returns>
    private int GetPontoDeRetorno()
    {
        if (_pontoDeRetorno.Count != 0)
        {
            return (int)_pontoDeRetorno.Peek();
        }
        return _ultimoPontoDeRetorno;
    }

    /// <summary>
    /// Se o "not" for verdadeiro, este método inverte o valor da "condicao".
    /// </summary>
    /// <param name="not">Equivale ao operador "NOT", se for true, ele inverte o valor da "condicao",
    /// senão, ele mantém o valor atual de "condicao".</param>
    /// <param name="condicao">Variável a ser invertida (ou não).</param>
    /// <returns>O valor de "condicao" invertido, ou não.</returns>
    private bool VerificaNaoString(bool not, bool condicao)
    {
        if (not)
        {
            return !condicao;
        }
        else return condicao;
    }
    /// <summary>
    /// Lê a linha e atribui o valor dela a um bool.
    /// </summary>
    /// <param name="linha">Linha a ser lida.</param>
    /// <param name="condicao">Valor da linha lida.</param>
    /// <returns>O valor booleano convertido de uma string na linguagem do Furbot</returns>
    public bool Condicional(string linha, out bool condicao)
    {
        Direcao DIR = Direcao.AQUI;
        if (linha.ToLower().Contains("direita"))
        {
            DIR = Direcao.DIREITA;
        }
        else if (linha.ToLower().Contains("esquerda"))
        {
            DIR = Direcao.ESQUERDA;
        }
        else if (linha.ToLower().Contains("acima"))
        {
            DIR = Direcao.ACIMA;
        }
        else if (linha.ToLower().Contains("abaixo"))
        {
            DIR = Direcao.ABAIXO;
        }
        else if (linha.ToLower().Contains("aqui"))
        {
            DIR = Direcao.AQUI;
        }

        bool not = linha.ToLower().Contains("!");

        if (linha.ToLower().Contains("ehvazio("))
        {
            return condicao = VerificaNaoString(not, EhVazio(DIR));
        }

        if (linha.ToLower().Contains("ehfim("))
        {
            return condicao = VerificaNaoString(not, EhFim(DIR));
        }

        if (linha.ToLower().Contains("eh("))
        {
            if (linha.ToLower().Contains("areia"))
            {
                return condicao = VerificaNaoString(not, Eh("areia", DIR));
            }
            else if (linha.ToLower().Contains("grama"))
            {
                return condicao = VerificaNaoString(not, Eh("grama", DIR));
            }
            else if (linha.ToLower().Contains("caminho"))
            {
                return condicao = VerificaNaoString(not, Eh("caminho", DIR));
            }
            else if (linha.ToLower().Contains("agua"))
            {
                return condicao = VerificaNaoString(not, Eh("agua", DIR));
            }
        }

        return condicao = false;
    }
    /// <summary>
    /// Verifica se o terreno na direção informada é igual ao terreno informado.
    /// </summary>
    /// <param name="terreno">Terreno a ser comparado.</param>
    /// <param name="DIR">Lado pra do Furbot para verificar o terreno.</param>
    /// <returns>Retorna "true" se os terrenos forem iguais.</returns>
    public bool Eh(string terreno, Direcao DIR)
    {
        string TerrenoDoSensor = "";

        switch (DIR)
        {
            case Direcao.DIREITA:
                TerrenoDoSensor = _furbot.GetSensor(Direcao.DIREITA).ColetadoDoTrigger;
                break;

            case Direcao.ESQUERDA:
                TerrenoDoSensor = _furbot.GetSensor(Direcao.ESQUERDA).ColetadoDoTrigger;
                break;

            case Direcao.ACIMA:
                TerrenoDoSensor = _furbot.GetSensor(Direcao.ACIMA).ColetadoDoTrigger;
                break;

            case Direcao.ABAIXO:
                TerrenoDoSensor = _furbot.GetSensor(Direcao.ABAIXO).ColetadoDoTrigger;
                break;
        }
        return TerrenoDoSensor.Equals(terreno);
    }

    /// <summary>
    /// Verifica se na direção informada não possui nenhum objeto.
    /// </summary>
    /// <param name="direcao">Direção a ser verificada.</param>
    /// <returns>Retorna "true" se houver algum desses objetos.</returns>
    public bool EhVazio(Direcao direcao)
    {
        string Objeto = "";

        switch (direcao)
        {
            case Direcao.DIREITA:
                Objeto = _furbot.GetSensor(Direcao.DIREITA).ColetadoDoTrigger;
                break;
            case Direcao.ESQUERDA:
                Objeto = _furbot.GetSensor(Direcao.ESQUERDA).ColetadoDoTrigger;
                break;
            case Direcao.ACIMA:
                Objeto = _furbot.GetSensor(Direcao.ACIMA).ColetadoDoTrigger;
                break;
            case Direcao.ABAIXO:
                Objeto = _furbot.GetSensor(Direcao.ABAIXO).ColetadoDoTrigger;
                break;
        }
        return !(Objeto.Equals("buggien") || Objeto.Equals("tesouro") || Objeto.Equals("Coletavel") || Objeto.Equals("pedra") || Objeto.Equals("arbusto") || Objeto.Equals("arvore") || Objeto.Equals("cenario") || Objeto.Equals("paredepiramide") || Objeto.Equals("laboratorio"));
    }
    public bool EhFim(Direcao direcao)
    {
        string Objeto = "";

        switch (direcao)
        {
            case Direcao.DIREITA:
                Objeto = _furbot.GetSensor(Direcao.DIREITA).ColetadoDoTrigger;
                break;
            case Direcao.ESQUERDA:
                Objeto = _furbot.GetSensor(Direcao.ESQUERDA).ColetadoDoTrigger;
                break;
            case Direcao.ACIMA:
                Objeto = _furbot.GetSensor(Direcao.ACIMA).ColetadoDoTrigger;
                break;
            case Direcao.ABAIXO:
                Objeto = _furbot.GetSensor(Direcao.ABAIXO).ColetadoDoTrigger;
                break;
        }
        return Objeto.Equals("parede");
    }

    /*
    /// <summary>
    /// Verifica se o código digitado possui algum erro.
    /// </summary>
    /// <param name="codigo">Código digitado no Console.</param>
    public void CompilarCodigo(string[] codigo)
    {
        Stack PilhaDeChaves = new Stack();
        bool se = false;
        string linha;
        string substring;
        for (int i = 0; i < codigo.Length; i++)
        {
            linha = codigo[i].ToLower().Replace(" ", "").Trim(); // ignorei os espaços e deixei tudo minúsculo

            if (linha.Equals("")) //coloque todas as suas exceções aqui
            {
                continue;
            }

            if (linha.Equals("}")) //tratamento
            {
                try
                {
                    if (PilhaDeChaves.Peek().Equals('{')) //eu utilizo uma pilha pra ver se a última chave era aberta ou fechada,
                    {                                     // pra ver se são proporcionais
                        PilhaDeChaves.Pop();
                        continue;
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                    throw new System.ArgumentException("Necessário abrir chaves (\"{\")");
                }
            }

            /*
             * Estou usando essa variável substring para cortar as linhas e verificar os caracteres
             * vou usar o um exemplo e em cada substring eu mostro o que deveria fazer
             * Esse é o exemplo:
             * andar(direita);
             *//*

            if (linha.Contains("("))
            {
                substring = linha.Substring(0, linha.IndexOf("(")); // andar
            }
            else
            {
                _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                throw new System.ArgumentException("Linha " + (i + 1) + ": \"(\" não encontrado");
            }



            if (substring.Equals("andar"))
            {
                substring = linha.Substring(linha.IndexOf("(")); // (direita);
                if (!substring.StartsWith("("))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);

                    throw new System.ArgumentException("Linha " + (i + 1) + ": \"(\" não encontrado");
                }
                substring = substring.Substring(1); // direita);

                if (substring.Contains(")"))
                {

                    substring = substring.Substring(0, substring.IndexOf(")")); // direita
                }
                else
                {
                    throw new System.ArgumentException("Linha " + (i + 1) + ": \")\" não encontrado");
                }

                if (!(substring.Equals("direita") ||
                      substring.Equals("esquerda") ||
                      substring.Equals("acima") ||
                      substring.Equals("abaixo")))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);

                    throw new System.ArgumentException("Linha " + (i + 1) + ": É necessário informar uma direção válida");
                }
                substring = linha.Substring(linha.IndexOf(")")); // );

                if (!substring.StartsWith(")"))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);

                    throw new System.ArgumentException("Linha " + (i + 1) + ": \")\" não encontrado");
                }
                substring = substring.Substring(1); // ;
                if (!substring.Equals(";"))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);

                    throw new System.ArgumentException("Linha " + (i + 1) + ": É necessário por \";\" no final do comando");
                }
            }
            else if (substring.Equals("enquanto") || substring.Equals("se")) // tanto o "enquanto" quanto o "se" utilizam as mesmas condicionais
            {
                if (substring.Equals("se"))
                {
                    _analisador.IncrementarSe();
                    se = true; //uso para verificar se há um senao sem um se
                }
                else if (substring.Equals("enquanto"))
                {
                    if (_gameManager.enquanto == false && !LevelManager.devMode)
                    {

                        throw new System.ArgumentException("Enquanto ainda não está liberado");
                    }
                    else
                    {
                        _analisador.IncrementarEnquanto();
                    }
                }

                /*
                 * No caso do "eh", utilizarei o exemplo do "(!eh(caminho,direita)){"
                 *//*

                substring = linha.Substring(linha.IndexOf("(")); // (!eh(caminho,direita)){

                if (!substring.StartsWith("("))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                    throw new System.ArgumentException("Linha " + (i + 1) + ": \"(\" não encontrado");
                }

                substring = substring.Substring(1); // eh(!caminho,direita)){
                if (substring.Contains("("))
                {
                    substring = substring.Substring(0, substring.IndexOf("(")); // !eh
                }
                else
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                    throw new System.ArgumentException("Linha " + (i + 1) + ": O segundo \"(\" não foi encontrado");
                }

                if (substring.StartsWith("!"))
                {
                    substring = substring.Substring(1); // eh
                }

                if (!(substring.Equals("eh") || substring.Equals("ehvazio") || substring.Equals("ehfim")))
                {

                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                    throw new System.ArgumentException("Linha " + (i + 1) + ": informe uma expressão válida");

                }
                else
                {
                    if (substring.Equals("eh"))
                    {
                        substring = linha.Substring(linha.IndexOf("(")); //(!eh(caminho,direita)){
                        substring = substring.Substring(1); //!eh(caminho,direita)){
                        substring = substring.Substring(substring.IndexOf("(")); // (caminho,direita)){

                        if (!substring.Contains(","))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                            throw new System.ArgumentException("Linha " + (i + 1) + ": é necessário separar os parâmetros por uma vírgula (\",\")");
                        }

                        substring = substring.Substring(1, substring.IndexOf(",") - 1); // caminho

                        if (!substring.Equals("caminho")) // por enquanto só utilizamos um tipo de terreno, mas se preciso, coloque por meio de um ||
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                            throw new System.ArgumentException("Linha " + (i + 1) + ": informe um parâmetro válido");
                        }

                        substring = linha.Substring(linha.IndexOf(",")); //,direita)){
                        if (substring.Contains(")"))
                        {
                            substring = substring.Substring(1, substring.IndexOf(")") - 1); //direita
                        }
                        else
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                            throw new System.ArgumentException("Linha " + (i + 1) + ": \")\" não encontrado");
                        }

                        if (!(substring.Equals("direita") ||
                              substring.Equals("esquerda") ||
                              substring.Equals("acima") ||
                              substring.Equals("abaixo")))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);

                            throw new System.ArgumentException("Linha " + (i + 1) + ": É necessário informar uma direção válida");
                        }

                        substring = linha.Substring(linha.IndexOf(")") + 1); // ){

                        if (!substring.StartsWith(")"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                            throw new System.ArgumentException("Linha " + (i + 1) + ": \")\" não encontrado");
                        }

                        substring = substring.Substring(1); // {

                        if (!substring.StartsWith("{"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                            throw new System.ArgumentException("Linha " + (i + 1) + ": \"{\" não encontrado");
                        }
                        else
                        {
                            PilhaDeChaves.Push('{');
                        }

                    }

                    //O ehFim e ehVazio também possuem o mesmo tratamento, para exemplo utilizarei o "(ehfim(direita)){"

                    if (substring.Equals("ehfim") || substring.Equals("ehvazio"))
                    {
                        substring = linha.Substring(linha.IndexOf("(")); // (ehfim(direita)){
                        substring = substring.Substring(1); // ehfim(direita)){
                        substring = substring.Substring(substring.IndexOf("(")); // (direita)){
                        substring = substring.Substring(1, substring.IndexOf(")") - 1); // direita

                        if (!(substring.Equals("direita") ||
                              substring.Equals("esquerda") ||
                              substring.Equals("acima") ||
                              substring.Equals("abaixo")))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);

                            throw new System.ArgumentException("Linha " + (i + 1) + ": É necessário informar uma direção válida");
                        }

                        substring = linha.Substring(linha.IndexOf(")") + 1); // ){

                        if (!substring.StartsWith(")"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                            throw new System.ArgumentException("Linha " + (i + 1) + ": \")\" não encontrado");
                        }

                        substring = substring.Substring(1); // {

                        if (!substring.StartsWith("{"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                            throw new System.ArgumentException("Linha " + (i + 1) + ": \"{\" não encontrado");
                        }
                        else
                        {
                            PilhaDeChaves.Push('{');
                        }

                    }
                }
            }/*
            else if (substring.Equals("senao"))
            {
                if (!se)
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                    throw new System.ArgumentException("Linha " + (i + 1) + ": \"senao\" sem um \"se\"");
                }
                else
                {
                    if (_gameManager.senao)
                    {
                        //_analisador.Incrementarsenao();

                    }
                    else
                    {
                        _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                        throw new System.ArgumentException("Linha " + (i + 1) + ": este comando ainda não está disponível");
                    }

                    se = false; // para poder utilizar em um outro "se"

                    if (!substring.EndsWith("{"))
                    {
                        _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                        throw new System.ArgumentException("Linha " + (i + 1) + ": \"{\" não encontrado");
                    }
                    else
                    {
                        PilhaDeChaves.Push("{");
                    }
                }
            }*//*
            else if (substring.Equals("puxaralavanca"))
            {
                substring = linha.Substring(linha.IndexOf("("));

                if (!substring.StartsWith("("))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                    throw new System.ArgumentException("Linha " + (i + 1) + ": \"(\" não encontrado");
                }

                substring = substring.Substring(1);

                if (!substring.StartsWith(")"))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                    throw new System.ArgumentException("Linha " + (i + 1) + ": \")\" não encontrado");
                }

                substring = substring.Substring(1);

                if (!substring.Equals(";"))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                    throw new System.ArgumentException("Linha " + (i + 1) + ": É necessário por \";\" no final do comando");
                }

            }
            else
            {
                _uiManager.ifCodigo.text = _uiManager.SetTextoErro(i);
                throw new System.ArgumentException("Linha " + (i + 1) + ": comando inválido");
            }

        }
        if (PilhaDeChaves.Count != 0)
        {
            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(-1);
            Debug.Log("{}: " + PilhaDeChaves.Count);
            throw new System.ArgumentException("Necessário fechar chaves (\"}\")");
        }

    }
    */

    public void CompilarCodigo(char[] codigo)
    {
        List<Token> tokens = new List<Token>();
        string palavra = "";
        int linha = 1;
        for (int letra = 0; letra < codigo.Length; letra++) // analisador léxico
        {
            if (codigo[letra] != '\r' || codigo[letra] != ' ')
            {
                palavra += codigo[letra];
                switch (palavra.ToLower())
                {
                    case "andar":
                        tokens.Add(Token.ANDAR);
                        palavra = "";
                        break;
                    case "enquanto":
                        if (!_gameManager.enquanto)
                        {
                            throw new ArgumentException("O 'enquanto' ainda não pode ser usado, colete todas as sílabas para usar");
                        }
                        tokens.Add(Token.ENQUANTO);
                        palavra = "";
                        break;
                    case "ehvazio":
                        tokens.Add(Token.EHVAZIO);
                        palavra = "";
                        break;
                    case "ehfim":
                        tokens.Add(Token.EHFIM);
                        palavra = "";
                        break;
                    case "eh":
                        if (codigo[letra + 1] != '(')
                        {
                            break;
                        }
                        tokens.Add(Token.EH);
                        palavra = "";
                        break;
                    case "se":
                        if (!_gameManager.se)
                        {
                            throw new ArgumentException("O 'se' ainda não pode ser usado, colete a sílaba para usar");
                        }
                        tokens.Add(Token.SE);
                        palavra = "";
                        break;
                    case "puxaralavanca":
                        if (LevelManager.faseAtual != 9)
                        {
                            throw new ArgumentException("Não tem nenhuma alavanca para você puxar por aqui");
                        }
                        tokens.Add(Token.PUXAR_ALAVANCA);
                        palavra = "";
                        break;
                    case "direita":
                    case "esquerda":
                    case "acima":
                    case "abaixo":
                        tokens.Add(Token.DIRECAO);
                        palavra = "";
                        break;
                    case "caminho":
                        tokens.Add(Token.TERRENO);
                        palavra = "";
                        break;
                }
                switch (codigo[letra].ToString())
                {

                    case "(":
                        if (!palavra.Equals("("))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(linha - 1);
                            throw new ArgumentException(string.Format("Você escreveu \"{0}\" na linha {1} eu não entendi o que significa isso", palavra, linha));
                        }
                        tokens.Add(Token.ABRE_PARENTESES);
                        palavra = "";
                        break;
                    case ")":
                        if (!palavra.Equals(")"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(linha - 1);
                            throw new ArgumentException(string.Format("Você escreveu \"{0}\" na linha {1} eu não entendi o que significa isso", palavra, linha));
                        }
                        tokens.Add(Token.FECHA_PARENTESES);
                        palavra = "";
                        break;
                    case ";":
                        if (!palavra.Equals(";"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(linha - 1);
                            throw new ArgumentException(string.Format("Você escreveu \"{0}\" na linha {1} eu não entendi o que significa isso", palavra, linha));
                        }
                        tokens.Add(Token.PONTO_VIRGULA);
                        palavra = "";
                        break;
                    case "{":
                        if (!palavra.Equals("{"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(linha - 1);
                            throw new ArgumentException(string.Format("Você escreveu \"{0}\" na linha {1} eu não entendi o que significa isso", palavra, linha));
                        }
                        tokens.Add(Token.ABRE_CHAVES);
                        palavra = "";
                        break;
                    case "}":
                        if (!palavra.Equals("}"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(linha - 1);
                            throw new ArgumentException(string.Format("Você escreveu \"{0}\" na linha {1} eu não entendi o que significa isso", palavra, linha));
                        }
                        tokens.Add(Token.FECHA_CHAVES);
                        palavra = "";
                        break;
                    case "!":
                        if (!palavra.Equals("!"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(linha - 1);
                            throw new ArgumentException(string.Format("Você escreveu \"{0}\" na linha {1} eu não entendi o que significa isso", palavra, linha));
                        }
                        tokens.Add(Token.NOT);
                        palavra = "";
                        break;
                    case ",":
                        if (!palavra.Equals(","))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(linha - 1);
                            throw new ArgumentException(string.Format("Você escreveu \"{0}\" na linha {1} eu não entendi o que significa isso", palavra, linha));
                        }
                        tokens.Add(Token.VIRGULA);
                        palavra = "";
                        break;
                    case "\n":
                        if (!palavra.Equals("\n"))
                        {
                            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(linha - 1);
                            throw new ArgumentException(string.Format("Você escreveu \"{0}\" na linha {1} eu não entendi o que significa isso", palavra, linha).Replace("\n", ""));
                        }
                        tokens.Add(Token.BARRA_N);
                        linha++;
                        palavra = "";
                        break;
                }
            }
        }
        if (palavra.Length > 0)
        {
            throw new ArgumentException(string.Format("Você escreveu \"{0}\" eu não entendi o que significa isso", palavra));
        }
        Stack PilhaDeChaves = new Stack();
        linha = 1;
        Token tokenEsperado = Token.ANDAR;
        try
        {
            for (int token = 0; token < tokens.Count; token++) // analisador sintático
            {
                switch (tokens[token])
                {
                    case Token.ANDAR:
                        tokenEsperado = Token.ABRE_PARENTESES;

                        if (tokens.Count > token + 1 && tokens[++token] == Token.ABRE_PARENTESES)
                        {
                            tokenEsperado = Token.DIRECAO;
                            if (tokens.Count > token + 1 && tokens[++token] == Token.DIRECAO)
                            {
                                tokenEsperado = Token.FECHA_PARENTESES;
                                if (tokens.Count > token + 1 && tokens[++token] == Token.FECHA_PARENTESES)
                                {
                                    tokenEsperado = Token.PONTO_VIRGULA;
                                    if (tokens.Count > token + 1 && tokens[++token] == Token.PONTO_VIRGULA)
                                    {
                                        if (token < tokens.Count - 1)
                                        {

                                            if (tokens[token + 1] != Token.BARRA_N && tokens[token + 1] != Token.FECHA_PARENTESES)
                                            {
                                                throw new ArgumentException(string.Format("Ei! Lá na linha {0} você esqueceu de quebrar a linha", linha));
                                            }

                                        }
                                        break;
                                    }
                                    LancarErro(linha);
                                    break;

                                }

                            }
                        }
                        LancarErro(tokens[token], tokenEsperado, linha);
                        break;

                    case Token.SE:
                    case Token.ENQUANTO:
                        tokenEsperado = Token.ABRE_PARENTESES;
                        if (tokens.Count > token + 1 && tokens[++token] == Token.ABRE_PARENTESES)
                        {
                            if (tokens.Count > token + 1)
                                token++;

                            if (tokens[token] == Token.NOT)
                            {
                                token++;
                            }
                            tokenEsperado = Token.EH;
                            if (tokens[token] == Token.EH)
                            {
                                tokenEsperado = Token.ABRE_PARENTESES;
                                if (tokens.Count > token + 1 && tokens[++token] == Token.ABRE_PARENTESES)
                                {
                                    tokenEsperado = Token.TERRENO;
                                    if (tokens.Count > token + 1 && tokens[++token] == Token.TERRENO)
                                    {
                                        tokenEsperado = Token.VIRGULA;
                                        if (tokens.Count > token + 1 && tokens[++token] == Token.VIRGULA)
                                        {
                                            tokenEsperado = Token.DIRECAO;
                                            if (tokens.Count > token + 1 && tokens[++token] == Token.DIRECAO)
                                            {
                                                tokenEsperado = Token.FECHA_PARENTESES;
                                                if (tokens.Count > token + 1 && tokens[++token] == Token.FECHA_PARENTESES)
                                                {
                                                    if (tokens.Count > token + 1 && tokens[++token] == Token.FECHA_PARENTESES)
                                                    {
                                                        tokenEsperado = Token.ABRE_CHAVES;
                                                        if (tokens.Count > token + 1 && tokens[++token] == Token.ABRE_CHAVES)
                                                        {
                                                            PilhaDeChaves.Push(Token.ABRE_CHAVES);
                                                            if (tokens.Count > token + 1)
                                                            {
                                                                if (token < tokens.Count - 1)
                                                                {

                                                                    if (tokens[token + 1] != Token.BARRA_N)
                                                                    {
                                                                        throw new ArgumentException(string.Format("Ei! Lá na linha {0} você esqueceu de quebrar a linha", linha));
                                                                    }

                                                                }
                                                                break;
                                                            }

                                                        }

                                                    }

                                                }

                                            }

                                        }

                                    }

                                }
                            }
                            else if (tokens[token] == Token.EHFIM || tokens[token] == Token.EHVAZIO)
                            {
                                tokenEsperado = Token.ABRE_PARENTESES;
                                if (tokens.Count > token + 1 && tokens[++token] == Token.ABRE_PARENTESES)
                                {
                                    tokenEsperado = Token.DIRECAO;
                                    if (tokens.Count > token + 1 && tokens[++token] == Token.DIRECAO)
                                    {
                                        tokenEsperado = Token.FECHA_PARENTESES;
                                        if (tokens.Count > token + 1 && tokens[++token] == Token.FECHA_PARENTESES)
                                        {
                                            if (tokens.Count > token + 1 && tokens[++token] == Token.FECHA_PARENTESES)
                                            {
                                                tokenEsperado = Token.ABRE_CHAVES;
                                                if (tokens.Count > token + 1 && tokens[++token] == Token.ABRE_CHAVES)
                                                {
                                                    PilhaDeChaves.Push(Token.ABRE_CHAVES);
                                                    if (token < tokens.Count - 1)
                                                    {

                                                        if (tokens[token + 1] != Token.BARRA_N)
                                                        {
                                                            throw new ArgumentException(string.Format("Ei! Lá na linha {0} você esqueceu de quebrar a linha", linha));
                                                        }

                                                    }
                                                    break;
                                                }
                                                LancarErro(linha);
                                                break;
                                            }

                                        }

                                    }

                                }

                            }
                        }

                        LancarErro(tokens[token], tokenEsperado, linha);

                        break;


                    case Token.PUXAR_ALAVANCA:
                        tokenEsperado = Token.ABRE_PARENTESES;
                        if (tokens.Count > token + 1 && tokens[++token] == Token.ABRE_PARENTESES)
                        {
                            tokenEsperado = Token.FECHA_PARENTESES;
                            if (tokens.Count > token + 1 && tokens[++token] == Token.FECHA_PARENTESES)
                            {
                                tokenEsperado = Token.PONTO_VIRGULA;
                                if (tokens.Count > token + 1 && tokens[++token] == Token.PONTO_VIRGULA)
                                {
                                    if (token < tokens.Count - 1)
                                    {

                                        if (tokens[token + 1] != Token.BARRA_N)
                                        {
                                            throw new ArgumentException(string.Format("Ei! Lá na linha {0} você esqueceu de quebrar a linha", linha));
                                        }

                                    }
                                    break;
                                }
                            }
                        }

                        LancarErro(tokens[token], tokenEsperado, linha);

                        break;

                    case Token.FECHA_CHAVES:
                        if (PilhaDeChaves.Count == 0)
                        {
                            throw new ArgumentException("Ei! Você esqueceu de abrir chaves '{'.");
                        }
                        if (PilhaDeChaves.Peek().Equals(Token.ABRE_CHAVES))
                        {
                            PilhaDeChaves.Pop();
                        }
                        else
                        {
                            throw new ArgumentException("Ei! Você esqueceu de abrir chaves '{'.");
                        }
                        break;
                    case Token.BARRA_N:
                        linha++;
                        break;
                }
            }
        }
        catch (IndexOutOfRangeException i)
        {
            LancarErro(linha);
        }

        if (PilhaDeChaves.Count > 0)
        {
            throw new ArgumentException("Você esqueceu de fechar as chaves '}'.");
        }
    }
    public void LancarErro(int linha)
    {
        throw new ArgumentException(string.Format("Ei! na linha {0} você esqueceu de escrever algo!", linha));
    }

    public void LancarErro(Token tokenAtual, Token tokenPrevisto, int linha)
    {
        string frase;

        if (linha != -1)
        {
            frase = string.Format("Eita! Na linha {0} era pra ter {1}, mas eu encontrei {2}", linha, TokenParaString(tokenPrevisto), TokenParaString(tokenAtual));
        }
        else
        {
            frase = string.Format("Nossa! Era pra ser {0}, mas eu encontrei {1}", TokenParaString(tokenPrevisto), TokenParaString(tokenAtual));
        }
        if (linha > 0)
        {
            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(linha - 1);
        }
        else
        {
            _uiManager.ifCodigo.text = _uiManager.SetTextoErro(0);
        }
        throw new ArgumentException(frase);
    }

    public string TokenParaString(Token token)
    {
        switch (token)
        {
            case Token.ABRE_CHAVES:
                return "o abre chaves '{'";
            case Token.ABRE_PARENTESES:
                return "o abre parênteses '('";
            case Token.BARRA_N:
                return "uma quebra de linha";
            case Token.DIRECAO:
                return "uma direção";
            case Token.EH:
            case Token.EHFIM:
            case Token.EHVAZIO:
                return "uma condição";
            case Token.FECHA_CHAVES:
                return "o fecha chaves '}'";
            case Token.FECHA_PARENTESES:
                return "o fecha parênteses ')'";
            case Token.NOT:
                return "o operador de negação '!'";
            case Token.PONTO_VIRGULA:
                return "o ponto e vírgula ';'";
            case Token.TERRENO:
                return "um terreno";
            case Token.VIRGULA:
                return "uma vírgula";
            case Token.ENQUANTO:
            case Token.PUXAR_ALAVANCA:
            case Token.SE:
            case Token.ANDAR:
                return "um método";
            default: return "erro";
        }
    }

    public int GetIndex()
    {
        return index;
    }


    /// <summary>
    /// Lê casa linha digitada no console e executa a devida ação.
    /// </summary>
    public IEnumerator LerComandosBuggien(string codigo)
    {
        bool PodeLer = true;
        string[] textoDividido;
        bool ChaveDoSe = false;
        int linhaInicio = 0;


        ////_analisador.SetTexto(_uiManager.GetTexto());
        if (_uiManager.Recomecar())
        {
            textoDividido = codigo.Split('\n');
            _textoAntigo = codigo.TrimEnd();
            linhaInicio = 0;
            _textoLido = "";
            _textoGabarito = "";
        }
        else
        {
            if (!_textoAntigo.Equals(""))
            {
                try
                {
                    textoDividido = LimparTexto(codigo.Split('\n'));
                    if (textoDividido[0].Equals(""))
                    {
                        _uiManager.RecuperarTexto();
                        _uiManager.debug.SetActive(false);
                        _uiManager.btnExecutar.GetComponentInChildren<Text>().text = "EXECUTAR";
                        _uiManager.btnExecutar.image.sprite = _uiManager.btnExecutarPlay;
                        _uiManager.btnRecomecar.interactable = true;
                        _uiManager.ifCodigo.interactable = true;
                        yield break;
                    }
                    linhaInicio = _textoLido.Split('\n').Length - 1;
                    _textoAntigo = codigo.TrimEnd();
                }
                catch (Exception e)
                {
                    textoDividido = codigo.Split('\n');
                    linhaInicio = 0;
                    _textoAntigo = codigo.TrimEnd();
                }
            }
            else
            {
                textoDividido = codigo.Split('\n');
                linhaInicio = 0;
                _textoAntigo = codigo.TrimEnd();
            }
        }

        try
        {
            _uiManager.SalvarTexto();
            CompilarCodigo(codigo.ToCharArray());
            _uiManager.RecuperarTexto();
            // _buggien.VoltarProInicio();

        }
        catch (System.ArgumentException e)
        {
            ////_analisador.IncrementarErrosComp();
            _uiManager.AddLog("Erro de compilação - " + e.Message);

            //_buggien.AbrirCaixaDeErro(e.Message, out PodeLer);
        }
        if (PodeLer)
        {
            _buggien.emExecucao = true;

            _uiManager.AddLog("Compilado com sucesso! (buggien)");

            //_uiManager.debug.SetActive(true);
            int ponteiroLinha = 0;
            string linha;
            _uiManager.SalvarTexto();
            while (ponteiroLinha < textoDividido.Length && !_buggien.bateu)  // é aqui que ele começa a ler mesmo
            {
                index = ponteiroLinha + linhaInicio;
                if (!_uiManager.emDialogo)
                    _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(index);

                linha = textoDividido[ponteiroLinha].ToLower().Trim();
                _textoLido += linha + "\n";
                /* por enquanto buggien não puxa alavanca
                if (linha.Contains("puxaralavanca();"))
                {
                    if (_buggien.usarAlavanca == true)
                    {
                        _buggien.puzzleManager.AtivarAlavanca(_buggien.alavancaAtual.GetComponent<Alavanca>().id);
                    }
                    else
                    {
                        _uiManager.Diga("Não há nenhuma alavanca aqui");
                    }
                    yield return new WaitForSeconds(1.0f);
                }*/
                if (linha.Contains("andar(")) // tratamento do comando andar
                {
                    if (linha.Contains("direita"))
                    {

                        _textoGabarito += "andar(direita);";
                        _buggien.Andar(Direcao.DIREITA);
                        yield return new WaitForSeconds(1.0f);

                    }
                    else if (linha.Contains("esquerda"))
                    {
                        _textoGabarito += "andar(esquerda);";
                        _buggien.Andar(Direcao.ESQUERDA);
                        yield return new WaitForSeconds(1.0f);

                    }
                    else if (linha.Contains("acima"))
                    {
                        _textoGabarito += "andar(acima);";

                        _buggien.Andar(Direcao.ACIMA);
                        yield return new WaitForSeconds(1.0f);

                    }
                    else if (linha.Contains("abaixo"))
                    {
                        _textoGabarito += "andar(abaixo);";

                        _buggien.Andar(Direcao.ABAIXO);
                        yield return new WaitForSeconds(1.0f);
                    }
                }

                if (linha.Contains("enquanto(")) // tratamento do comando enquanto
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(ponteiroLinha);

                    yield return new WaitForSeconds(0.3f);

                    CondicionalBuggien(linha, out _condicaoRetorno);
                    if (_condicaoRetorno)
                    {
                        _pontoDeRetorno.Push(ponteiroLinha);
                    }
                    else
                    {
                        while (!textoDividido[ponteiroLinha].Trim().ToLower().Equals("}"))
                        {
                            ponteiroLinha++;
                        }
                    }
                }

                if (linha.Contains("se")) // tratamento do comando se
                {
                    if (linha.Contains("senao"))
                    {
                        _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(ponteiroLinha);

                        yield return new WaitForSeconds(0.3f);

                        if (_condicaoRetorno)
                        {
                            while (!textoDividido[ponteiroLinha].Trim().ToLower().Equals("}"))
                            {
                                if (ponteiroLinha + 1 <= textoDividido.Length)
                                {
                                    ponteiroLinha++;
                                }
                            }
                        }
                    }
                    _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(ponteiroLinha);

                    yield return new WaitForSeconds(0.3f);

                    CondicionalBuggien(linha, out _condicaoRetorno);
                    if (!_condicaoRetorno)
                    {
                        while (!textoDividido[ponteiroLinha].Trim().ToLower().Equals("}"))
                        {
                            if (ponteiroLinha + 1 <= textoDividido.Length)
                            {
                                ponteiroLinha++;
                            }
                        }
                    }
                    else
                    {
                        ChaveDoSe = true;
                    }
                }



                if (linha.Contains("diga("))
                {
                    string texto = linha.Replace("diga(", "").Replace(");", "");
                    StartCoroutine(_uiManager.Diga(texto));
                }

                if (linha.Trim().Equals("}"))
                {
                    _uiManager.ifCodigo.text = _uiManager.SetTextoDebug(ponteiroLinha);

                    yield return new WaitForSeconds(0.1f);
                    if (ChaveDoSe)
                    {
                        ChaveDoSe = false;
                        continue;
                    }
                    CondicionalBuggien(textoDividido[GetPontoDeRetorno()], out _condicaoRetorno);
                    if (_condicaoRetorno)
                    {
                        if (_pontoDeRetorno.Count != 0)
                        {
                            ponteiroLinha = (int)_pontoDeRetorno.Peek();
                            _ultimoPontoDeRetorno = (int)_pontoDeRetorno.Peek();
                        }
                    }
                    else
                    {
                        if (_pontoDeRetorno.Count != 0)
                        {
                            _ultimoPontoDeRetorno = (int)_pontoDeRetorno.Pop();
                        }
                    }
                }
                if (ponteiroLinha < textoDividido.Length - 1)
                {
                    ponteiroLinha++;
                }
                else break;
            }
            _buggien.bateu = false;
        }
        _buggien.direcaoAtual = "Parado";
        _uiManager.debug.SetActive(false);
        _uiManager.btnExecutar.GetComponentInChildren<Text>().text = "EXECUTAR";
        _uiManager.btnExecutar.image.sprite = _uiManager.btnExecutarPlay;
        _uiManager.btnRecomecar.interactable = true && !_uiManager.emDialogo;
        _uiManager.ifCodigo.interactable = true && !_uiManager.emDialogo;
        _uiManager.AtualizarNumeros();
        index = 0;
        _uiManager.permitidoDigitarComandos = true && !_uiManager.emDialogo;
        if (!_uiManager.emDialogo)
        {
            _uiManager.RecuperarTexto();
        }
        //Debug.Log(_textoLido);
    }

    /// <summary>
    /// Lê a linha e atribui o valor dela a um bool.
    /// </summary>
    /// <param name="linha">Linha a ser lida.</param>
    /// <param name="condicao">Valor da linha lida.</param>
    /// <returns>O valor booleano convertido de uma string na linguagem do Furbot</returns>
    public bool CondicionalBuggien(string linha, out bool condicao)
    {
        Direcao DIR = Direcao.AQUI;
        if (linha.ToLower().Contains("direita"))
        {
            DIR = Direcao.DIREITA;
        }
        else if (linha.ToLower().Contains("esquerda"))
        {
            DIR = Direcao.ESQUERDA;
        }
        else if (linha.ToLower().Contains("acima"))
        {
            DIR = Direcao.ACIMA;
        }
        else if (linha.ToLower().Contains("abaixo"))
        {
            DIR = Direcao.ABAIXO;
        }
        else if (linha.ToLower().Contains("aqui"))
        {
            DIR = Direcao.AQUI;
        }

        bool not = linha.ToLower().Contains("!");

        if (linha.ToLower().Contains("ehvazio("))
        {
            return condicao = VerificaNaoString(not, BuggienEhVazio(DIR));
        }

        if (linha.ToLower().Contains("ehfim("))
        {
            return condicao = VerificaNaoString(not, BuggienEhFim(DIR));
        }

        if (linha.ToLower().Contains("eh("))
        {
            if (linha.ToLower().Contains("areia"))
            {
                return condicao = VerificaNaoString(not, BuggienEh("areia", DIR));
            }
            else if (linha.ToLower().Contains("grama"))
            {
                return condicao = VerificaNaoString(not, BuggienEh("grama", DIR));
            }
            else if (linha.ToLower().Contains("caminho"))
            {
                return condicao = VerificaNaoString(not, BuggienEh("caminho", DIR));
            }
            else if (linha.ToLower().Contains("agua"))
            {
                return condicao = VerificaNaoString(not, BuggienEh("agua", DIR));
            }
        }

        return condicao = false;
    }
    /// <summary>
    /// Verifica se o terreno na direção informada é igual ao terreno informado.
    /// </summary>
    /// <param name="terreno">Terreno a ser comparado.</param>
    /// <param name="DIR">Lado pra do Furbot para verificar o terreno.</param>
    /// <returns>Retorna "true" se os terrenos forem iguais.</returns>
    public bool BuggienEh(string terreno, Direcao DIR)
    {
        string TerrenoDoSensor = "";

        switch (DIR)
        {
            case Direcao.DIREITA:
                TerrenoDoSensor = _buggien.GetSensor(Direcao.DIREITA).ColetadoDoTrigger;
                break;

            case Direcao.ESQUERDA:
                TerrenoDoSensor = _buggien.GetSensor(Direcao.ESQUERDA).ColetadoDoTrigger;
                break;

            case Direcao.ACIMA:
                TerrenoDoSensor = _buggien.GetSensor(Direcao.ACIMA).ColetadoDoTrigger;
                break;

            case Direcao.ABAIXO:
                TerrenoDoSensor = _buggien.GetSensor(Direcao.ABAIXO).ColetadoDoTrigger;
                break;
        }
        return TerrenoDoSensor.Equals(terreno);
    }

    /// <summary>
    /// Verifica se na direção informada não possui nenhum objeto.
    /// </summary>
    /// <param name="direcao">Direção a ser verificada.</param>
    /// <returns>Retorna "true" se houver algum desses objetos.</returns>
    public bool BuggienEhVazio(Direcao direcao)
    {
        string Objeto = "";

        switch (direcao)
        {
            case Direcao.DIREITA:
                Objeto = _buggien.GetSensor(Direcao.DIREITA).ColetadoDoTrigger;
                break;
            case Direcao.ESQUERDA:
                Objeto = _buggien.GetSensor(Direcao.ESQUERDA).ColetadoDoTrigger;
                break;
            case Direcao.ACIMA:
                Objeto = _buggien.GetSensor(Direcao.ACIMA).ColetadoDoTrigger;
                break;
            case Direcao.ABAIXO:
                Objeto = _buggien.GetSensor(Direcao.ABAIXO).ColetadoDoTrigger;
                break;
        }
        return !(Objeto.Equals("buggien") || Objeto.Equals("tesouro") || Objeto.Equals("Coletavel") || Objeto.Equals("pedra") || Objeto.Equals("arbusto") || Objeto.Equals("arvore") || Objeto.Equals("cenario") || Objeto.Equals("paredepiramide") || Objeto.Equals("laboratorio"));
    }
    public bool BuggienEhFim(Direcao direcao)
    {
        string Objeto = "";

        switch (direcao)
        {
            case Direcao.DIREITA:
                Objeto = _buggien.GetSensor(Direcao.DIREITA).ColetadoDoTrigger;
                break;
            case Direcao.ESQUERDA:
                Objeto = _buggien.GetSensor(Direcao.ESQUERDA).ColetadoDoTrigger;
                break;
            case Direcao.ACIMA:
                Objeto = _buggien.GetSensor(Direcao.ACIMA).ColetadoDoTrigger;
                break;
            case Direcao.ABAIXO:
                Objeto = _buggien.GetSensor(Direcao.ABAIXO).ColetadoDoTrigger;
                break;
        }
        return Objeto.Equals("parede");
    }



    public enum Token
    {
        ANDAR, ABRE_PARENTESES, FECHA_PARENTESES, DIRECAO, PONTO_VIRGULA, ENQUANTO, EH, EHVAZIO,
        EHFIM, NOT, TERRENO, ABRE_CHAVES, FECHA_CHAVES, SE, PUXAR_ALAVANCA, VIRGULA, BARRA_N
    }
}