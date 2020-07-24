using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gabarito : MonoBehaviour
{


    public static string[]
    Fase1 = new string[] { "andar(ABAIXO);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);" },
    Fase2 = new string[] { "andar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ABAIXO);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(DIREITA);\nandar(ABAIXO);", "andar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(DIREITA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(DIREITA);\nandar(ABAIXO);", "andar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ABAIXO);\nandar(DIREITA);\nandar(DIREITA);\nandar(ABAIXO);\nandar(DIREITA);\nandar(ABAIXO);" },
    Fase3 = new string[] { "andar(ABAIXO);\nandar(ABAIXO);\nandar(DIREITA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ABAIXO);\nandar(ABAIXO);\nandar(DIREITA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ABAIXO);\nandar(ABAIXO);\nandar(DIREITA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(DIREITA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);" },
    Fase4 = new string[] { "andar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);" },
    Fase5 = new string[] { "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ACIMA);\nandar(DIREITA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ACIMA);\nandar(DIREITA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);", "andar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ABAIXO);\nandar(ACIMA);\nandar(DIREITA);\nandar(DIREITA);\nandar(DIREITA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);\nandar(ACIMA);\nandar(ACIMA);\nandar(ESQUERDA);\nandar(ESQUERDA);" },
    Fase6 = new string[] { "" },
    Fase6_2 = new string[] { "" },
    Fase7 = new string[] { "" },
    Fase7_2 = new string[] { "" },
    Fase8 = new string[] { "" },
    Fase8_2 = new string[] { "" },
    Fase9 = new string[] { "" },
    Fase9_2 = new string[] { "" },
    Fase10 = new string[] { "" },
    Fase10_2 = new string[] { "" },
    Fase11 = new string[] { "" },
    Fase12 = new string[] { "" },
    Fase13 = new string[] { "" },
    Fase14 = new string[] { "" },
    Fase15 = new string[] { "" },
    Fase16 = new string[] { "" },
    Fase17 = new string[] { "" },
    Fase18 = new string[] { "" },
    Fase19 = new string[] { "" },
    Fase20 = new string[] { "" };



    public enum Resultado
    {
        IGUAL, EQUIVALENTE, MENOR, MAIOR
    }

    public static Resultado CalcularGabarito(int fase, string codigo)
    {
        string[] digitado = LimparCodigo(codigo.ToLower().Replace(";", ";\n").Split('\n'));
        string[] gabarito = MelhorGabarito(codigo, fase).ToLower().Split('\n');

        if (digitado.Length == gabarito.Length)
        {
            for (int i = 0; i < digitado.Length; i++)
            {
                if (!digitado[i].Equals(gabarito[i]))
                {
                    return Resultado.EQUIVALENTE;
                }
            }
            return Resultado.IGUAL;
        }
        else
        {
            if (digitado.Length > gabarito.Length)
                return Resultado.MAIOR;
            else return Resultado.MENOR;

        }
    }
    public static string[] GetResultado(int fase)
    {
        switch (fase - 1)
        {
            case 0: return Fase1;
            case 1: return Fase2;
            case 2: return Fase3;
            case 3: return Fase4;
            case 4: return Fase5;
            case 5: return Fase6;
            case 6: return Fase7;
            case 7: return Fase8;
            case 8: return Fase9;
            case 9: return Fase10;
            case 10: return Fase11;
            case 11: return Fase12;
            case 12: return Fase13;
            case 13: return Fase14;
            case 14: return Fase15;
            case 15: return Fase16;
            case 16: return Fase17;
            case 17: return Fase18;
            case 18: return Fase19;
            case 19: return Fase20;
            default: throw new System.NotImplementedException();
        }
    }

    private static string[] LimparCodigo(string[] codigo)
    {
        try
        {
            string aux = "";
            foreach (string linha in codigo)
            {
                if (!linha.Equals(""))
                {
                    aux += linha + "¨";
                }
            }
            return aux.Replace("<b>", "").Replace("</b>", "").Remove(aux.Length - 1).Split('¨');
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.Log("ERRO RELACIONADO AO GABARITO.");
            return new string[1];
        }
    }

    public static int CompararTextos(string[] escrito, string[] gabarito)
    {
        try
        {
            escrito = LimparCodigo(escrito);
            gabarito = LimparCodigo(gabarito);
            double resultado = 0.0;
            if (escrito.Length == gabarito.Length)
            {
                for (int i = 0; i < gabarito.Length; i++)
                {
                    if (escrito[i].ToLower().Equals(gabarito[i].ToLower()))
                    {
                        resultado += 100.0 / gabarito.Length;
                    }
                }
                return (int)Math.Round(resultado);
            }
            else if (escrito.Length < gabarito.Length)
            {
                for (int i = 0; i < escrito.Length; i++)
                {
                    if (escrito[i].Equals(gabarito[i]))
                    {
                        resultado += 100 / gabarito.Length;
                    }
                }
                return (int)Math.Round(resultado - (gabarito.Length - escrito.Length));
            }
            else
            {
                for (int i = 0; i < gabarito.Length; i++)
                {
                    if (escrito[i].Equals(gabarito[i]))
                    {
                        resultado += 100 / gabarito.Length;
                    }
                }
                return (int)Math.Round(resultado - (escrito.Length - gabarito.Length));
            }
        }
        catch (NullReferenceException)
        {
            Debug.Log("Erro na correção do código fonte (gabarito)");
            return 0;
        }
    }

    public static int MelhorResultado(string escrito, string[] gabarito)
    {
        int resultado = 0;
        for (int i = 0; i < gabarito.Length; i++)
        {
            int comparado = CompararTextos(escrito.Split('\n'), gabarito[i].Split('\n'));
            if (comparado > resultado)
            {
                resultado = comparado;
            }
        }
        return resultado;
    }

    public static string MelhorGabarito(string escrito, int fase)
    {
        string[] gabarito = GetResultado(fase);
        int resultado = MelhorResultado(escrito, gabarito);

        for (int i = 0; i < gabarito.Length; i++)
        {
            if (CompararTextos(escrito.Split('\n'), gabarito[i].Split('\n')) == resultado)
            {
                Debug.Log(resultado + "%");
                if (GetResultado(fase)[i] != null)
                {
                    return GetResultado(fase)[i];
                }
                else
                {
                    Debug.Log("Gabarito não registrado");
                    return "";
                }
            }
        }
        return "";
    }
}
