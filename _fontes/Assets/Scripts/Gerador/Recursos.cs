using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Recursos : MonoBehaviour
{

    private static Tilemap[] TileMaps;
    private static GameObject[] _prefabs, _arbusto, _arvore, _pedra;

    public static Tilemap[] GetTileMaps()
    {
        TileMaps = new Tilemap[6];
        TileMaps[0] = GameObject.Find("TilemapGrama").GetComponent<Tilemap>();
        TileMaps[1] = GameObject.Find("TilemapAreia").GetComponent<Tilemap>();
        TileMaps[2] = GameObject.Find("TilemapAgua").GetComponent<Tilemap>();
        TileMaps[3] = GameObject.Find("TilemapCaminhoTerra").GetComponent<Tilemap>();
        TileMaps[4] = GameObject.Find("TilemapCaminhoAreia").GetComponent<Tilemap>();
        TileMaps[5] = GameObject.Find("TilemapCaminhoEstrada").GetComponent<Tilemap>();
        return TileMaps;
    }

    public static GameObject[] GetTilesArrays(Objetos objetos)
    {
        switch (objetos)
        {
            case Objetos.ARBUSTO:
                _arbusto = new GameObject[7];
                _arbusto[0] = Resources.Load("Prefabs/Cenario/Arbusto01") as GameObject;
                _arbusto[1] = Resources.Load("Prefabs/Cenario/Arbusto02") as GameObject;
                _arbusto[2] = Resources.Load("Prefabs/Cenario/Arbusto03") as GameObject;
                _arbusto[3] = Resources.Load("Prefabs/Cenario/Arbusto04") as GameObject;
                _arbusto[4] = Resources.Load("Prefabs/Cenario/Arbusto05") as GameObject;
                _arbusto[5] = Resources.Load("Prefabs/Cenario/Arbusto06") as GameObject;
                _arbusto[6] = Resources.Load("Prefabs/Cenario/Arbusto07") as GameObject;
                return _arbusto;
            case Objetos.ARVORE:
                _arvore = new GameObject[3];
                _arvore[0] = Resources.Load("Prefabs/Cenario/Arvore02") as GameObject;
                _arvore[1] = Resources.Load("Prefabs/Cenario/Arvore03") as GameObject;
                _arvore[2] = Resources.Load("Prefabs/Cenario/Arvore04") as GameObject;
                return _arvore;
            case Objetos.PEDRA:
                _pedra = new GameObject[4];
                _pedra[0] = Resources.Load("Prefabs/Cenario/Pedra01") as GameObject;
                _pedra[1] = Resources.Load("Prefabs/Cenario/Pedra02") as GameObject;
                _pedra[2] = Resources.Load("Prefabs/Cenario/Pedra03") as GameObject;
                _pedra[3] = Resources.Load("Prefabs/Cenario/Pedra04") as GameObject;
                return _pedra;
            case Objetos.PREFAB:
                _prefabs = new GameObject[5];
                _prefabs[0] = Resources.Load("Prefabs/Personagens/Furbot") as GameObject;
                _prefabs[1] = Resources.Load("Prefabs/Personagens/Buggien") as GameObject;
                _prefabs[2] = Resources.Load("Prefabs/Coletaveis/ColetavelVida") as GameObject;
                _prefabs[3] = Resources.Load("Prefabs/Coletaveis/ColetavelEnergia") as GameObject;
                _prefabs[4] = Resources.Load("Prefabs/Coletaveis/Tesouro") as GameObject;
                return _prefabs;
            default: throw new System.Exception();
        }
    }




    public enum Objetos
    {
        PREFAB, ARBUSTO, PEDRA, ARVORE
    }
}
