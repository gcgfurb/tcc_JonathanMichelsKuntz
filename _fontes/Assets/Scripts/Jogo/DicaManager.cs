using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DicaManager : MonoBehaviour
{

    public Text textoDica;

    private void OnEnable()
    {
        switch (SceneManager.GetActiveScene().name)
        {

            case "Fase 1":
                textoDica.text = "Vá até a meleca. É um sinal de que os buggiens passaram por aqui!";
                break;
            case "Fase 6.2":
                textoDica.text = "Colete o item do faraó.";
                break;
            case "Fase 7.2":
                textoDica.text = "Colete os itens do faraó.";
                break;
            case "Fase 8.2":
                textoDica.text = "Colete o papiro.";
                break;
            case "Fase 9":
                textoDica.text = "Puxe as alavancas na ordem descrita no papiro, localizado no inventário. Use o comando <i>'puxarAlavanca();'</i> em frente " +
                    "a uma alavanca.";
                break;
            case "Fase 9.2":
                textoDica.text = "Colete o papiro.";
                break;
            case "Fase 10":
                textoDica.text = "Ande com o Furbot sobre as figuras no chão da sala. A sequência correta deve estar escrita no papiro que coletamos.";
                break;
            case "Fase 11.2":
                textoDica.text = "Vá até o esquimo, ele parece precisar de ajuda.";
                break;
            case "Fase 12.2":
                textoDica.text = "Vá até o esquimo, ele parece precisar de ajuda.";
                break;
            case "Fase 13.2":
                textoDica.text = "Vá até o esquimo, ele parece precisar de ajuda.";
                break;
            case "Fase 14.2":
                textoDica.text = "Vá até o esquimo, ele parece precisar de ajuda.";
                break;
            case "Fase 15.2":
                textoDica.text = "Vá até o esquimo, ele parece precisar de ajuda.";
                break;
            case "Fase 16.2":
                textoDica.text = "Colete a válvula. Ela é necessária para acabar com a alimentação de meleca na nave.";
                break;
            case "Fase 16.3":
                textoDica.text = "Vá até a base da válvula para a alimentação de meleca na nave ser bloqueada.";
                break;
            case "Fase 17.2":
                textoDica.text = "Colete a válvula. Ela é necessária para acabar com a alimentação de meleca na nave.";
                break;
            case "Fase 17.3":
                textoDica.text = "Vá até a base da válvula para a alimentação de meleca na nave ser bloqueada. Para passar pelo buggiens você "+
                    "precisa empurrá-los. Para empurrá-los ande com o Furbot para cima do buggien na direção que você deseja que ele seja empurrado.";
                break;
            case "Fase 18":
                textoDica.text = "Selecione os buggiens e programe-os até os botões. Ao ativá-los ao mesmo tempo uma porta será liberada.";
                break;
            case "Fase 18.2":
                textoDica.text = "Selecione o buggien e programe-o até o botão. Colete a válvula. Ela é necessária para acabar com a alimentação "+
                    "de meleca na nave.";
                break;
            case "Fase 18.3":
                textoDica.text = "Programe o Furbot e o buggien até os botões de uma forma que seja possível liberar a passagem para o Furbot. "
                    + "Vá até a base da válvula para a alimentação de meleca na nave ser bloqueada.";
                break;
            case "Fase 19":
                textoDica.text = "Programe o Furbot e o buggien até os botões de uma forma que seja possível liberar a passagem para o Furbot.";
                break;
            case "Fase 19.2":
                textoDica.text = "Selecione o buggien e programe-o até o botão. É necessário que neste caso o buggien seja programado até o indicador "+
                    "de próxima fase, pois o buggien será útil.";
                break;
            case "Fase 19.3":
                textoDica.text = "Selecione o buggien e programe-o até o botão. É necessário que neste caso o buggien seja programado até o indicador " +
                    "de próxima fase, pois o buggien será útil.";
                break;
            case "Fase 20.2":
                textoDica.text = "Programe o Furbot até os botões para liberar formas de se movimentar nesta sala, para então coletar a chave.";
                break;
            case "Fase 20.3":
                textoDica.text = "Vá até o indicador da próxima fase, cuidado com as cascas de ovos.";
                break;
            case "Fase 20.4":
                textoDica.text = "Programe o Furbot até os botões para liberar formas de se movimentar nesta sala, para então entrar abrir a porta do." 
                    +"Imperador Buggien";
                break;
            default:
                textoDica.text = "Esta fase não possui dica. Continue seguindo as pistas deixadas pelos buggiens.";
                break;
        }
    }
}
