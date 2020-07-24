using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizController : MonoBehaviour
{
    private static string[] perguntas/* =
    {
        "Qual parte da planta recebe os nutrientes do solo?",
        "Qual o nome do maior aquífero do Brasil?",
        "Qual o nome que se dá à uma porção de terra cercada de água por todos os lados?",
        "Qual a estação do ano que antecede a primavera?",
        "À qual classe de animais o ser humano pertence?",
        "Em qual região do país se encontra o estado da Bahia?",
        "Comprei dez balas e comi quatro, quantas balas restam?",
        "Qual a capital do Brasil?",
        "Como se escreve o número oito em algarismos romanos?",
        "Em que direção o sol nasce todos os dias?",
        "Em que direção o sol se põe todos os dias?",
        "Qual é o maior país do mundo?",
        "Quantos séculos há em um milênio?",
        "Qual é o maior continente do mundo?",
        "Qual é o maior animal do mundo?",
        "Qual é a estrela que possui luz própria?"
    }*/;

    private static string[] respostas/* =
    {
    "Raíz",
    "Guarani",
    "Ilha",
    "Inverno",
    "Mamíferos",
    "Nordeste",
    "Seis",
    "Brasília",
    "VIII",
    "Leste",
    "Oeste",
    "Rússia",
    "Dez",
    "Ásia",
    "Baleia",
    "Sol"
    }*/;

    private static List<int> perguntasJaFeitas = new List<int>();

    public static string GerarPergunta(out string resposta)
    {

        if (LevelManager.faseAtual <= 6)
        {
            perguntas = new string[]{
                    "Qual é o país que possui a maior parte da Amazônia?",
                    "Qual é o maior rio da Amazônia?",
                    "Qual é a cidade mais populosa na região Amazônica?",
                    "Qual é o maior estado da amazônia brasileira?",
                    "A Floresta Amazônica está presente em quantos estados brasileiros?",
                    "A Floresta Amazônica está presente em quantos países?"
                };
            respostas = new string[]{
                    "Brasil",
                    "Amazonas",
                    "Manaus",
                    "Amazonas",
                    "Nove",
                    "Nove"
                };
        }
        else if (LevelManager.faseAtual < 11)
        {

            perguntas = new string[]{
                    "O Egito é banhado por qual mar?",
                    "Qual é o nome do maior rio do Egito?",
                    "Qual é a capital do Egito?",
                    "Como eram chamados os \"Reis\" do Egito Antigo?",
                    "Como é o nome da planta que os egipcios utilizavam para fazer papel?",
                    "Como são chamadas as estruturas triangulares usadas como túmulo de grandes faraós?"
                };
            respostas = new string[]{
                    "Mediterrâneo",
                    "Nilo",
                    "Cairo",
                    "Faraós",
                    "Papiro",
                    "Pirâmides"
                };
        }

        int tentativas = 0;
        while (true)
        {
            int sorteio = (int)Random.Range(0, perguntas.Length);
            if (!perguntasJaFeitas.Contains(sorteio))
            {
                resposta = "";
                resposta = respostas[sorteio];
                if (Application.isMobilePlatform)
                {
                    if (resposta.Length <= 6)
                    {
                        perguntasJaFeitas.Add(sorteio);
                        return perguntas[sorteio];
                    }
                }
                else
                {
                    perguntasJaFeitas.Add(sorteio);
                    return perguntas[sorteio];
                }
            }
            if (++tentativas == 25)
            {
                resposta = respostas[0];
                return perguntas[0];
            }
        }
    }

    public static void ReiniciarPerguntasSorteadas()
    {
        perguntasJaFeitas.Clear();
    }

    public static void AdicionarPergunta(string pergunta, string resposta)
    {
        if (pergunta != "" && resposta != "")
        {
            string[] novoPerguntas = new string[perguntas.Length + 1];
            perguntas.CopyTo(novoPerguntas, 0);
            novoPerguntas[novoPerguntas.Length - 1] = pergunta;
            perguntas = novoPerguntas;

            string[] novoRespostas = new string[respostas.Length + 1];
            respostas.CopyTo(novoRespostas, 0);
            novoRespostas[novoRespostas.Length - 1] = resposta;
            respostas = novoRespostas;
            Debug.Log("Pergunta adicionada.");
        }
        else
        {
            Debug.Log("Campo de pergunta ou resposta inválido.");
        }
    }
}
