using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Analisador : MonoBehaviour
{
    private int _coletaveis, _tesouros, _vidas, _energia, _batidas, _enquantos, _locaisIndevidos, _errosCompilacao, _linhas, _se, _senao;

    private float _tempo;

    private int _totalVidas, _totalEnergia, _totalColetaveis, _execucoesCodigo;

    public int totalTesouros;

    private string _codigo, _log, _codigoExtenso;

    public DateTime _horaInicioExercicio;

    public static Log ultimoLog;

    public void ExportarTXT()
    {
        string dir = @"c:\LogFurbot\";
        if (!Directory.Exists(dir))
        {
            DirectoryInfo di = Directory.CreateDirectory(dir);
        }
        string path = dir + (Directory.GetFiles(dir).Length + 1) + ".txt";
        using (StreamWriter sw = File.CreateText(path))
        {
            string[] texto = GerarTexto().Split('\n');
            for (int linha = 0; linha < texto.Length; linha++)
            {
                sw.WriteLine(texto[linha]);
            }
        }
    }

    public string GerarTexto()
    {
        if (_codigo != null)
        {
            string texto = "Código:\n" + _codigo +
                            "\n\nQuantidade de Coletáveis:" +
                            "\n\tTesouros: " + _tesouros + "/" + totalTesouros +
                            "\n\tEnergia: " + _energia + "/" + _totalEnergia +
                            "\n\tVidas: " + _vidas + "/" + _totalVidas +
                            "\n\tOutros: " + _coletaveis + "/" + _totalColetaveis +
                            "\nQuantidade de execucoes do código: " + _execucoesCodigo +
                            "\nQuantidade de Erros de Execução: " + (_batidas + _locaisIndevidos) + " (" + _batidas + " batidas e " + _locaisIndevidos + " passagens em locais indevidos)" +
                            "\nQuantidade de Erros de Compilação: " + _errosCompilacao +
                            "\nQuantidade de Linhas: " + _linhas +
                            "\nQuantidade de \"Enquanto\": " + _enquantos +
                            "\nQuantidade de \"Se\": " + _se +
                            "\nQuantidade de \"senao\": " + _senao +
                            "\nTempo: " + _tempo;
            return texto + _log;
        }
        else
        {
            string texto = "Código:\n\n" + "Nenhum código foi inserido" +
                            "\n\nQuantidade de Coletáveis:" +
                            "\n\tTesouros: " + _tesouros + "/" + totalTesouros +
                            "\n\tEnergia: " + _energia + "/" + _totalEnergia +
                            "\n\tVidas: " + _vidas + "/" + _totalVidas +
                            "\n\tOutros: " + _coletaveis + "/" + _totalColetaveis +
                            "\nQuantidade de execucoes do código: " + _execucoesCodigo +
                            "\nQuantidade de Erros de Execução: " + (_batidas + _locaisIndevidos) + " (" + _batidas + " batidas e " + _locaisIndevidos + " passagens em locais indevidos)" +
                            "\nTempo: " + _tempo;
            return texto + _log;
        }
    }

    public Log GerarLog()
    {
        ultimoLog = new Log();
        ultimoLog.codigo = _codigo;
        ultimoLog.log = _log;
        ultimoLog.linhas = _linhas;
        ultimoLog.locaisIndevidos = _locaisIndevidos;
        ultimoLog.errosCompilacao = _errosCompilacao;
        ultimoLog.batidas = _batidas;
        ultimoLog.se = _se;
        ultimoLog.senao = _senao;
        ultimoLog.tempo = GerarTempoResolucao();
        ultimoLog.tesouros = _tesouros;
        ultimoLog.vidas = _vidas;
        ultimoLog.execucoesCodigo = _execucoesCodigo;
        ultimoLog.coletaveis = _coletaveis;
        ultimoLog.totalColetaveis = _totalColetaveis;
        ultimoLog.totalEnergia = _totalEnergia;
        ultimoLog.totalVidas = _totalVidas;
        ultimoLog.totalTesouros = totalTesouros;
        ultimoLog.codigoExtenso = _codigoExtenso;

        return ultimoLog;
    }

    private float GerarTempoResolucao()
    {
        DateTime tempoFinal = System.DateTime.Now;
        return tempoFinal.Subtract(_horaInicioExercicio).Seconds;
    }

    public void IncrementarColetavel()
    {
        _coletaveis++;
    }
    public void IncrementarExecucoes()
    {
        _execucoesCodigo++;
    }
    public void IncrementarBatidas()
    {
        _batidas++;
    }
    public void IncrementarEnquanto()
    {
        _enquantos++;
    }
    public void IncrementarLocaisIndevidos()
    {
        _locaisIndevidos++;
    }
    public void IncrementarErrosComp()
    {
        _errosCompilacao++;
    }
    public void IncrementarSe()
    {
        _se++;
    }
    public void Incrementarsenao()
    {
        _senao++;
    }

    public void IncrementarVida()
    {
        _vidas++;
    }
    public void IncrementarEnergia()
    {
        _energia++;
    }
    public void IncrementarTesouro()
    {
        _tesouros++;
    }

    public void SetQtdLinhas(int qtdLinhas)
    {
        _linhas = qtdLinhas;
    }
    public void ResetarDados()
    {
        _coletaveis = 0;
        _batidas = 0;
        _enquantos = 0;
        _locaisIndevidos = 0;
        _errosCompilacao = 0;
        _execucoesCodigo = 0;
        _linhas = 0;
        _se = 0;
        _senao = 0;
        _codigo = "";
        _vidas = 0;
        _tesouros = 0;
        _energia = 0;
    }

    public void SetTexto(string texto)
    {
        _codigo = texto;
    }


    public void VarreduraDeMapa()
    {
        //zera todos os dados para recontagem
        _totalColetaveis = 0;
        _totalEnergia = 0;
        _totalVidas = 0;
        totalTesouros = 0;

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(); //instancia um array com todos os objetos da fase
        foreach (GameObject go in allObjects)
            if (go.activeInHierarchy) //ignora os objetos não ativos
            {
                switch (go.tag) //pega a tag do objeto para incrementar a sua quantidade
                {
                    case "Tesouro":
                        totalTesouros++;
                        break;
                    case "Vida":
                        _totalVidas++;
                        break;
                    case "Energia":
                        _totalEnergia++;
                        break;
                    case "Coletavel":
                        _totalColetaveis++;
                        break;
                }
            }
    }

    public void SetLog(string textLog)
    {
        this._log = textLog;
    }

    public void SetCodigoExtenso(string codigo)
    {
        _codigoExtenso += codigo;
    }

    public string GetCodigoExtenso()
    {
        return _codigoExtenso;
    }

    [Serializable]
    public class Log
    {
        public int coletaveis, tesouros, vidas, energia, batidas, enquantos, locaisIndevidos, errosCompilacao, linhas, se, senao, jogoId;

        public float tempo;

        public int totalVidas, totalEnergia, totalColetaveis;

        public int totalTesouros, fase, execucoesCodigo;

        public string codigo, log, codigoExtenso;
    }
}
