#if true
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEditor;

public class RecriadorDeFase : MonoBehaviour
{
    [SerializeField]
    private Tilemap[] _TileMaps;
    public GameObject[] _prefabs, _arbusto, _arvore, _pedra;
    private GameObject canvas;
    GameObject GO;

    private void Awake()
    {
        _TileMaps = Recursos.GetTileMaps();
        _prefabs = Recursos.GetTilesArrays(Recursos.Objetos.PREFAB);
        _arbusto = Recursos.GetTilesArrays(Recursos.Objetos.ARBUSTO);
        _arvore = Recursos.GetTilesArrays(Recursos.Objetos.ARVORE);
        _pedra = Recursos.GetTilesArrays(Recursos.Objetos.PEDRA);
    }

    void Start()
    {
        canvas = GameObject.Find("Canvas");
#if UNITY_EDITOR
        TileObjectList tiles;
        try
        {
            tiles = JsonUtility.FromJson<TileObjectList>(GameObject.Find("ArmazenadorDeJson").GetComponent<JsonHolder>().conteudo);
        }
        catch (System.NullReferenceException)
        {
            tiles = CarregarDoJsonLocal();
            foreach (Tilemap t in _TileMaps)
            {
                t.gameObject.SetActive(false);
                t.gameObject.SetActive(true);
            }
        }

        foreach (var tile in tiles.TileList)
        {
            if (tile.TileMap < 0)
            {
                Debug.Log(tile);
                switch (tile.TileMap)
                {
                    case -1:
                        GO = GameObject.Find("Furbot");
                        //GO.transform.SetParent(GameObject.Find("Personagens").transform);
                        GO.transform.position = new Vector3(tile.X, tile.Y, -1);
                        GO.GetComponent<Furbot>().oldPosition = GO.transform.position;
                        break;
                    case -2:
                        GO = Instantiate(_prefabs[1], new Vector3(tile.X, tile.Y, -1), Quaternion.identity, canvas.transform.parent) as GameObject;
                        //GO.transform.SetParent(GameObject.Find("Personagens").transform);
                        GO.transform.localScale.Scale(new Vector3(2f, 2f, 1));
                        break;
                    case -3:
                        GO = Instantiate(_prefabs[2], new Vector3(tile.X, tile.Y, -1), Quaternion.identity, canvas.transform.parent) as GameObject;
                        GO.transform.localScale.Scale(new Vector3(2f, 2f, 1f));
                        GO.transform.SetParent(GameObject.Find("Coletaveis").transform);
                        break;
                    case -4:
                        GO = Instantiate(_prefabs[4], new Vector3(tile.X, tile.Y, -1), Quaternion.identity, canvas.transform.parent) as GameObject;
                        GO.transform.localScale.Scale(new Vector3(2f, 2f, 1f));
                        GO.transform.SetParent(GameObject.Find("Coletaveis").transform);
                        break;
                    case -5:
                        GO = Instantiate(_arvore[tile.Tile], new Vector3(tile.X, tile.Y, -1), Quaternion.identity, canvas.transform.parent) as GameObject;
                        GO.transform.localScale.Scale(new Vector3(2f, 2f, 1f));
                        GO.transform.SetParent(GameObject.Find("Cenario").transform);
                        break;
                    case -6:
                        GO = Instantiate(_arbusto[tile.Tile], new Vector3(tile.X, tile.Y, -1), Quaternion.identity, canvas.transform.parent) as GameObject;
                        GO.transform.localScale.Scale(new Vector3(2f, 2f, 1f));
                        GO.transform.SetParent(GameObject.Find("Cenario").transform);
                        break;
                    case -7:
                        GO = Instantiate(_pedra[tile.Tile], new Vector3(tile.X, tile.Y, -1), Quaternion.identity, canvas.transform.parent) as GameObject;
                        GO.transform.localScale.Scale(new Vector3(2f, 2f, 1f));
                        GO.transform.SetParent(GameObject.Find("Cenario").transform);
                        break;
                }

            }
            else
            {
                try
                {
                    _TileMaps[tile.TileMap].SetTile(new Vector3Int((int)tile.X, (int)tile.Y, 0), GetTileArray(tile.TileMap)[tile.Tile]);
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log("Index: " + tile.TileMap);
                    Debug.Log("Inner Exception: " + e.InnerException);
                    Debug.Log("Message: " + e.Message);
                    Debug.Log("Source: " + e.Source);
                    Debug.Log("StackTrace: " + e.StackTrace);
                    Debug.Log("Data: " + e.Data);
                }
            }
        }
#endif
        /*
        for (int i = 0; i < tiles.tileList.Count; i++)
        {
            if (tiles.tileList[i].TileMap.GetInstanceID().Equals(0))
            {
                GameObject.Find("Furbot").gameObject.transform.position = GameObject.Find("TilemapMoldura").GetComponent<Tilemap>().CellToWorld(tiles.tileList[0].Posicao);
            } else
            {
                tiles.tileList[i].TileMap.SetTile(tiles.tileList[i].Posicao, tiles.tileList[i].Tile);
            }
        }
        */
    }

    public Tile[] GetTileArray(int value)
    {
        var tile = gameObject.GetComponent<TileGenerator>();
        switch (value)
        {
            case 0: return tile._grama;
            case 1: return tile._areia;
            case 2: return tile._agua;
            case 3: return tile._caminhoTerra;
            case 4: return tile._caminhoAreia;
            case 5: return tile._estrada;
            default: throw new System.Exception();
        }
    }

    private GameObject GetCanvas()
    {
        return canvas;
    }

    public static TileObjectList CarregarDoJsonLocal()
    {
        TileObjectList tiles;
        //string path = EditorUtility.OpenFilePanel("Selecionar Fase", @"c:\MapasFurbot\", "json");
        string path = Directory.GetFiles(@"c:\MapasFurbot\")[Directory.GetFiles(@"c:\MapasFurbot\").Length -1];
        Debug.Log(path);
        if (path.Length != 0)
        {
            string jsonraw = File.ReadAllText(path);
            Debug.Log(jsonraw);
            tiles = JsonUtility.FromJson<TileObjectList>(jsonraw);

            return tiles;

        }
        return null;
    }


    [Serializable]
    public class TileObject
    {

        public float X;
        public float Y;
        public int TileMap;
        public int Tile;

        public override string ToString()
        {
            return X + ", " + Y + ", Tilemap: " + TileMap + ", Tile: " + Tile;
        }
    }

    [Serializable]
    public class TileObjectList
    {
        public List<TileObject> TileList;

        public override string ToString()
        {
            foreach (TileObject tile in TileList)
            {
                Debug.Log(tile.ToString());
            }
            return base.ToString();
        }
    }
}
#endif