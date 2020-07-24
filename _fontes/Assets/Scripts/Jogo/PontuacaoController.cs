using System;
using System.Collections.Generic;
using UnityEngine;
public class PontuacaoController : MonoBehaviour
{
    //Integers (ESTATICOS)
    //Recompensa por coletaveis
    public static int pontosPorTesouro = 500;
    public static int pontosPorVida = 1000;
    public static int pontosPorPista = 250;

    public static int pontuacaoUltimaFase;

    private static List<int> qtdTesourosFase = new List<int>();
    private static List<int> qtdPontosFase = new List<int>();
    private static List<int> totalTesourosFase = new List<int>();
    public static int pontosFase;

    public static void ChecaPontuacaoFase(int tesouros, out int diferencaTesouros, out int pontuacaoSobresalente, out bool jaJogou)
    {
        int pontuacao, quantidadeAnterior;
        quantidadeAnterior = 0;
        jaJogou = false;
        if (tesouros > 0)
        {
            if (qtdTesourosFase.Count == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    qtdTesourosFase.Add(0);
                }
            }
            quantidadeAnterior = qtdTesourosFase[LevelManager.faseAtual - 1];
            if (quantidadeAnterior < tesouros)
            {
                qtdTesourosFase[LevelManager.faseAtual - 1] = tesouros;
            }
        }
        diferencaTesouros = tesouros - quantidadeAnterior;
        pontuacaoSobresalente = 0;
        pontuacao = (pontosPorTesouro * tesouros);
        if (diferencaTesouros > 0)
        {
            pontuacaoSobresalente = diferencaTesouros * pontosPorTesouro;
        }
        if (quantidadeAnterior > 0)
        {
            jaJogou = true;
        }
        AtribuirNoVetor(qtdPontosFase, pontosFase);
    }

    public static void SetTotalTesouros(int quant)
    {
        AtribuirNoVetor(totalTesourosFase, quant);
    }

    private static void AtribuirNoVetor(List<int> vetor, int quant)
    {
        try
        {
            vetor[LevelManager.faseAtual - 1] = quant;
        }
        catch (ArgumentOutOfRangeException)
        {
            for (int i = 0; i < 20; i++)
            {
                vetor.Add(0);
            }
            vetor[LevelManager.faseAtual - 1] = quant;
        }
    }

    public static int GetTotalTesouros()
    {
        return totalTesourosFase[LevelManager.faseAtual - 1];
    }

    public static List<int> GetQtdTesourosFase()
    {
        return qtdTesourosFase;
        /*
        List<int> qtdTesouros = new List<int>();
        for (int i = 0; i < qtdTesourosFase.Count; i++)
        {
            qtdTesouros.Add(qtdTesourosFase[i]);
        }
        return qtdTesouros;
        */
    }

    public static List<int> GetQtdPontosFase()
    {
        return qtdPontosFase;
    }

    public static void SetDadosServidor(List<int> qtdPontosFase, List<int> qtdTesourosFase)
    {
        PontuacaoController.qtdPontosFase = qtdPontosFase;
        PontuacaoController.qtdTesourosFase = qtdTesourosFase;
    }
}
