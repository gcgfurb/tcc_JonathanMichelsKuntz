#if true
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

#pragma warning disable 0649
public class Gerador : MonoBehaviour
{
    public Tilemap[] TileMaps;
    public TMP_Dropdown _Dropdown, _caminho, _objeto;
    public GameObject[] _prefabs, _arbusto, _arvore, _pedra;


    private Vector3Int previous;
    private Vector3 previousObject;
    private bool furbotNoMapa;
    private TileGenerator _Tile;
    private Vector3 currentObject;
    private Vector3Int currentCell;
    [SerializeField]
    private UnityEngine.UI.Button _btSalvar, _btConfirmar, _btNegar;
    private bool taNaArea;
    [SerializeField]
    private GameObject _painel;

    private void Awake()
    {
        TileMaps = Recursos.GetTileMaps();
        _prefabs = Recursos.GetTilesArrays(Recursos.Objetos.PREFAB);
        _arbusto = Recursos.GetTilesArrays(Recursos.Objetos.ARBUSTO);
        _arvore = Recursos.GetTilesArrays(Recursos.Objetos.ARVORE);
        _pedra = Recursos.GetTilesArrays(Recursos.Objetos.PEDRA);
    }

    void Start()
    {
        _Tile = gameObject.GetComponent<TileGenerator>();
        _btSalvar.onClick.AddListener(delegate { Salvar(); });
    }


    // do late so that the player has a chance to move in update if necessary
    private void LateUpdate()
    {
        _caminho.gameObject.SetActive(_Dropdown.value == 3);
        _objeto.gameObject.SetActive(_Dropdown.value == 6);



        try
        {
            if (_Dropdown.value != 6)
            {

                if (Input.GetMouseButton(0))
                {
                    currentCell = TileMaps[_Dropdown.value + GetTileValue()].LocalToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    currentCell.x += 1;
                    //Debug.Log(TileMaps[_Dropdown.value + GetTileValue()]);
                    if (currentCell != previous && ((currentCell.x >= -8 && currentCell.x <= 4) && (currentCell.y >= -3 && currentCell.y <= 4)))
                    {
                        // Debug.Log(TileMaps[_Dropdown.value].name + ", " + currentCell);
                        for (int i = 0; i < TileMaps.Length; i++)
                        {
                            TileMaps[i].SetTile(currentCell, null);
                        }
                        TileMaps[_Dropdown.value + GetTileValue()].SetTile(currentCell, GetTile(_Dropdown.value));
                        /*
                        if(_Dropdown.value == 2)
                        {
                            EhUmLago();
                        }*/
                        _Tile.UpdateTile();

                        previous = currentCell;

                    }
                }

            }
            else
            {
                if (taNaArea)
                {
                    SpawnarObjetos();
                    taNaArea = false;
                }
            }
        }

        catch (System.IndexOutOfRangeException i)
        {

        }

    }

    private void OnMouseDown()
    {
        taNaArea = true;
    }

    private void SpawnarObjetos()
    {
        currentObject = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(currentObject);
        currentObject.z = 91F;
        //Debug.Log(currentObject);

        GameObject GO;

        switch (_objeto.value)
        {
            case 0:
                GameObject furbotAntigo;

                if (OFurbotTaAi(out furbotAntigo))
                {
                    Destroy(furbotAntigo);
                }
                GO = Instantiate(_prefabs[0], currentObject, Quaternion.identity);
                GO.transform.position = ConverterPosMundoParaPosCell(currentObject);
                GO.transform.parent = GetCanvas().transform.parent;
                GO.transform.localScale.Scale(new Vector3(2f, 2f, 0f));
                break;
            case 1:
                GO = Instantiate(_prefabs[1], currentObject, Quaternion.identity);
                GO.transform.position = ConverterPosMundoParaPosCell(currentObject) + new Vector3(0, -0.4f, 0);
                GO.transform.parent = GetCanvas().transform.parent;
                GO.transform.localScale.Scale(new Vector3(2f, 2f, 0f));
                break;
            case 2:
                GO = Instantiate(_prefabs[2], currentObject, Quaternion.identity);
                GO.transform.position = ConverterPosMundoParaPosCell(currentObject) + new Vector3(0, -0.4f, 0);
                GO.transform.parent = GetCanvas().transform.parent;
                GO.transform.localScale.Scale(new Vector3(2f, 2f, 0f));
                break;
            case 3:
                GO = Instantiate(_prefabs[3], currentObject, Quaternion.identity);
                GO.transform.position = ConverterPosMundoParaPosCell(currentObject) + new Vector3(0, -0.4f, 0);
                GO.transform.parent = GetCanvas().transform.parent;
                GO.transform.localScale.Scale(new Vector3(2f, 2f, 0f));
                break;
            case 4:
                GO = Instantiate(_prefabs[4], currentObject, Quaternion.identity);
                GO.transform.position = ConverterPosMundoParaPosCell(currentObject) + new Vector3(0, -0.4f, 0);
                GO.transform.parent = GetCanvas().transform.parent;
                GO.transform.localScale.Scale(new Vector3(2f, 2f, 0f));
                break;
            case 5:
                GO = Instantiate(_arvore[UnityEngine.Random.Range(0, _arvore.Length)], currentObject, Quaternion.identity);
                GO.transform.position = ConverterPosMundoParaPosCell(currentObject) + new Vector3(0.2f, 0.2f, 0);
                GO.transform.parent = GetCanvas().transform.parent;
                GO.transform.localScale.Scale(new Vector3(2f, 2f, 0f));
                break;
            case 6:
                GO = Instantiate(_arbusto[UnityEngine.Random.Range(0, _arbusto.Length)], currentObject, Quaternion.identity);
                GO.transform.position = ConverterPosMundoParaPosCell(currentObject) + new Vector3(0, -0.4f, 0);
                GO.transform.parent = GetCanvas().transform.parent;
                GO.transform.localScale.Scale(new Vector3(2f, 2f, 0f));
                break;
            case 7:
                GO = Instantiate(_pedra[UnityEngine.Random.Range(0, _pedra.Length)], currentObject, Quaternion.identity);
                GO.transform.position = ConverterPosMundoParaPosCell(currentObject) + new Vector3(0, -0.4f, 0);
                GO.transform.parent = GetCanvas().transform.parent;
                GO.transform.localScale.Scale(new Vector3(2f, 2f, 0f));
                break;
            default: Debug.Log("Deu ruim"); break;
        }
    }

    public Vector3 ConverterPosMundoParaPosCell(Vector3 posMundo)
    {
        Vector3Int v = TileMaps[0].WorldToCell(posMundo);
        Vector3 posCell = TileMaps[0].GetCellCenterWorld(v);
        return posCell + new Vector3(0, 0.4f, 0f);
    }

    private GameObject GetCanvas()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.activeInHierarchy && go.tag.Equals("Canvas"))
            {
                return go;
            }
        }
        throw new System.Exception("Canvas não encontrado");
    }

    private bool OFurbotTaAi(out GameObject furbot)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.activeInHierarchy && go.tag.Equals("Player"))
            {
                furbot = go;
                return true;
            }
        }
        furbot = null;
        return false;
    }

    public Tile GetTile(int value)
    {
        switch (value)
        {
            case 0: return _Tile.GetGrama();
            case 1: return _Tile.GetAreia();
            case 2: return _Tile.GetAgua(currentCell);
            case 3: return _Tile.GetCaminho(currentCell);
            case 4: return _Tile.GetEstrada(currentCell);
            case 5: DeleteTile(currentCell); return null;
            default: throw new System.Exception("Ainda não implementado");
        }
    }

    public void DeleteTile(Vector3Int position)
    {
        foreach (Tilemap Tile in TileMaps)
        {
            Tile.SetTile(position, null);
        }
    }

    public int GetTileValue()
    {
        if (_Dropdown.value == 4)
        {

            return 1;
        }

        if (_caminho.IsActive())
        {
            return _caminho.value;
        }
        else return 0;
    }

    private void Salvar()
    {
        TileObjectList tiles = new TileObjectList();
        Vector3Int posicao;

        for (int y = -3; y <= 4; y++)
        {
            for (int x = -8; x <= 4; x++)
            {
                posicao = new Vector3Int(x, y, 0);

                for (int i = 0; i < TileMaps.Length; i++)
                {

                    if (TileMaps[i].HasTile(posicao))
                    {
                        //Debug.Log(TileMaps[i].GetTile(posicao));
                        TileObject tile = new TileObject();
                        tile.X = posicao.x;
                        tile.Y = posicao.y;
                        tile.TileMap = i;
                        tile.Tile = GetTileID(GetTileArrayID(i), TileMaps[i].GetTile(posicao));
                        tiles.TileList.Add(tile);
                    }
                }
            }
        }
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.activeInHierarchy)
            {
                TileObject tile = new TileObject();

                switch (go.tag)
                {
                    case "Player":
                        tile.X = go.transform.position.x;
                        tile.Y = go.transform.position.y;
                        tile.TileMap = -1; //os objetos terão valores negativos enquanto as tiles serão positivas
                        tile.Tile = 0;
                        tiles.TileList.Add(tile);
                        break;
                    case "Buggien":
                        tile.X = go.transform.position.x;
                        tile.Y = go.transform.position.y;
                        tile.TileMap = -2;
                        tile.Tile = 0;
                        tiles.TileList.Add(tile);
                        break;
                    case "Vida":
                        tile.X = go.transform.position.x;
                        tile.Y = go.transform.position.y;
                        tile.TileMap = -3;
                        tile.Tile = 0;
                        tiles.TileList.Add(tile);
                        break;
                    case "Tesouro":
                        tile.X = go.transform.position.x;
                        tile.Y = go.transform.position.y;
                        tile.TileMap = -4;
                        tile.Tile = 0;
                        tiles.TileList.Add(tile);
                        break;
                    case "Cenario":
                        if (go.name.ToLower().Contains(("arvore")))
                        {
                            tile.X = go.transform.position.x;
                            tile.Y = go.transform.position.y;
                            tile.TileMap = -5;
                            tile.Tile = GetArrayValue(_arvore, go);
                            tiles.TileList.Add(tile);
                        }
                        else if (go.name.ToLower().Contains(("arbusto")))
                        {
                            tile.X = go.transform.position.x;
                            tile.Y = go.transform.position.y;
                            tile.TileMap = -6;
                            tile.Tile = GetArrayValue(_arbusto, go);
                            tiles.TileList.Add(tile);
                        }
                        else if (go.name.ToLower().Contains(("pedra")))
                        {
                            tile.X = go.transform.position.x;
                            tile.Y = go.transform.position.y;
                            tile.TileMap = -7;
                            tile.Tile = GetArrayValue(_pedra, go);
                            tiles.TileList.Add(tile);
                        }

                        break;
                }
            }
        }


#if true
        string SAVE_FOLDER = @"c:\MapasFurbot\"  /*= ""*/;

        if (!Directory.Exists(SAVE_FOLDER))
        {
            DirectoryInfo DIRECTORY = Directory.CreateDirectory(SAVE_FOLDER);
        }
        string SAVE_PATH = SAVE_FOLDER + System.DateTime.Now.ToString("ddMMyHHmmss") + " - " + (Directory.GetFiles(SAVE_FOLDER).Length + 1) + ".json";

        string json = JsonUtility.ToJson(tiles);

        Debug.Log(json);

        using (StreamWriter sw = File.CreateText(SAVE_PATH))
        {
            sw.WriteLine(json);
            sw.Flush();
            sw.Close();

        }

        _painel.SetActive(true);

        _btConfirmar.onClick.AddListener(delegate
        {
            _painel.SetActive(false);
            GameObject jsonObj = GameObject.Find("ArmazenadorDeJson");
            jsonObj.GetComponent<JsonHolder>().conteudo = json;
            DontDestroyOnLoad(jsonObj);
            SceneManager.LoadScene("FaseGerada");
        });
        _btNegar.onClick.AddListener(delegate { _painel.SetActive(false); });
    }
#endif
    public TileBase[] GetTileArrayID(int value)
    {

        switch (value)
        {
            case 0: return _Tile._grama;
            case 1: return _Tile._areia;
            case 2: return _Tile._agua;
            case 3: return _Tile._caminhoTerra;
            case 4: return _Tile._caminhoAreia;
            case 5: return _Tile._estrada;
            default: throw new Exception();
        }
    }

    public int GetTileID(TileBase[] tileArray, TileBase tile)
    {
        for (int i = 0; i < tileArray.Length; i++)
        {
            if (tileArray[i].Equals(tile))
            {
                return i;
            }
        }
        throw new Exception();
    }
    public int GetArrayValue(GameObject[] array, GameObject go)
    {

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].gameObject.name.Replace("(Clone)", "").Equals(go.name.Replace("(Clone)", "")))
            {
                return i;
            }
        }
        if (true)
            throw new System.Exception();
    }

    public void EhUmLago()
    {
        Vector3Int posicaoOriginal = new Vector3Int();
        for (int y = -3; y <= 4; y++)
        {
            for (int x = -8; x <= 4; x++)
            {
                posicaoOriginal = new Vector3Int(x, y, 0);

                if (TileMaps[2].HasTile(posicaoOriginal))
                {
                    break;
                }
            }

        }

        //TileMaps[2].FloodFill(posicaoOriginal, _Tile._agua[0]);

        /*
         if(IsLakeRound(posicaoOriginal, posicaoOriginal, Direcao.AQUI))
         {
             Debug.Log("É um Lago");
         } else
         {
             Debug.Log("Não é um Lago");
         }
         */
    }

    public bool IsLakeRound(Vector3Int Origem, Vector3Int posicao, Direcao dir)
    {
        Vector3Int novaPosicao;
        if (TileMaps[2].HasTile(new Vector3Int(posicao.x, posicao.y - 1, 0)) && dir != Direcao.ACIMA)
        {
            novaPosicao = new Vector3Int(posicao.x, posicao.y - 1, 0);
            if (novaPosicao.Equals(Origem))
            {
                return true;
            }
            else return IsLakeRound(Origem, new Vector3Int(posicao.x, posicao.y - 1, 0), Direcao.ABAIXO);
        }
        else if (TileMaps[2].HasTile(new Vector3Int(posicao.x + 1, posicao.y, 0)) && dir != Direcao.ESQUERDA)
        {
            novaPosicao = new Vector3Int(posicao.x + 1, posicao.y, 0);
            if (novaPosicao.Equals(Origem))
            {
                return true;
            }
            else return IsLakeRound(Origem, new Vector3Int(posicao.x + 1, posicao.y, 0), Direcao.DIREITA);
        }
        else if (TileMaps[2].HasTile(new Vector3Int(posicao.x, posicao.y + 1, 0)) && dir != Direcao.ABAIXO)
        {
            novaPosicao = new Vector3Int(posicao.x, posicao.y + 1, 0);
            if (novaPosicao.Equals(Origem))
            {
                return true;
            }
            else return IsLakeRound(Origem, new Vector3Int(posicao.x, posicao.y + 1, 0), Direcao.ACIMA);
        }
        else if (TileMaps[2].HasTile(new Vector3Int(posicao.x - 1, posicao.y, 0)) && dir != Direcao.DIREITA)
        {
            novaPosicao = new Vector3Int(posicao.x - 1, posicao.y, 0);
            if (novaPosicao.Equals(Origem))
            {
                return true;
            }
            else return IsLakeRound(Origem, new Vector3Int(posicao.x - 1, posicao.y, 0), Direcao.ESQUERDA);
        }
        else return false;
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
        public List<TileObject> TileList = new List<TileObject>();

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