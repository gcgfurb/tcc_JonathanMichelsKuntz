#if true
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    public Tile[] _grama, _estrada, _areia, _caminhoTerra, _caminhoAreia, _agua;
    private Gerador gerador;
    bool[,,] matrizes;

    private void Awake()
    {
        CriarReferencias();
    }

    private void Start()
    {
        gerador = GetComponent<Gerador>();
        matrizes = new bool[,,] { { { false, true, false }, { true, true, true }, { false, true, false } },  //0
                                  { { false, false, false }, { false, true, false }, { false, false, false } },  //1
                                  { { false, true, false }, { false, true, true }, { false, true, false} },  //2
                                  { { false, false, false }, { false, true, true }, { false, false, false } },  //3
                                  { { false, true, false }, { true, true, true }, { false, false, false } },  //4
                                  { { false, true, false }, { false, true, false }, { false, false, false } },  //5
                                  { { false, true, false }, { true, true, false }, { false, true, false } },  //6
                                  { { false, false, false }, { true, true, false }, { false, false, false } },  //7
                                  { { false, false, false }, { true, true, true }, { false, true, false } },  //8
                                  { { false, false, false }, { false, true, false }, { false, true, false } },  //9
                                  { { false, true, false }, { true, true, true }, { false, true, false } },  //10
                                  { { false, true, false }, { true, true, true }, { false, true,false } },  //11
                                  { { false, true, false }, { true, true, true }, { false, true, false } },  //12
                                  { { false, true, false }, { true, true, true }, { false, true, false } },  //13
                                  { { false, false, false }, { true, true, true }, { false, true, false } },  //14
                                  { { false, false, false }, { false, true, true }, { false, true, false } },  //15
                                  { { false, true, false }, { true, true, false }, { false, false, false } },  //16
                                  { { false, true, false }, { false, true, true }, { false, false, false } },  //17
                                  { { false, false, false }, { true, true, false }, { false, true, false } },
                                  { { false, false, false }, { false, true, true }, { false, true, false } }
        };
    }

    private void CriarReferencias()
    {
        _grama = new Tile[3];
        _estrada = new Tile[15];
        _areia = new Tile[2];
        _caminhoTerra = new Tile[15];
        _caminhoAreia = new Tile[15];
        _agua = new Tile[15];

        _grama[0] = Resources.Load("Tiles/grama_1") as Tile;
        _grama[1] = Resources.Load("Tiles/grama_2") as Tile;
        _grama[2] = Resources.Load("Tiles/grama_3") as Tile;

        _estrada[0] = Resources.Load("Tiles/estrada_reta_2") as Tile;
        _estrada[1] = Resources.Load("Tiles/estrada_reta_1") as Tile;
        _estrada[2] = Resources.Load("Tiles/T_estrada_4") as Tile;
        _estrada[3] = Resources.Load("Tiles/T_estrada_2") as Tile;
        _estrada[4] = Resources.Load("Tiles/T_estrada_3") as Tile;
        _estrada[5] = Resources.Load("Tiles/T_estrada_1") as Tile;
        _estrada[6] = Resources.Load("Tiles/intersecao_estrada") as Tile;
        _estrada[7] = Resources.Load("Tiles/curva_estrada_3") as Tile;
        _estrada[8] = Resources.Load("Tiles/curva_estrada_4") as Tile;
        _estrada[9] = Resources.Load("Tiles/curva_estrada_2") as Tile;
        _estrada[10] = Resources.Load("Tiles/curva_estrada_1") as Tile;
        _estrada[11] = Resources.Load("Tiles/asfalto_fim_02") as Tile;
        _estrada[12] = Resources.Load("Tiles/asfalto_fim_04") as Tile;
        _estrada[13] = Resources.Load("Tiles/asfalto_fim_03") as Tile;
        _estrada[14] = Resources.Load("Tiles/asfalto_fim_01") as Tile;

        _areia[0] = Resources.Load("Tiles/areia-fundo-1") as Tile;
        _areia[1] = Resources.Load("Tiles/areia-fundo-2") as Tile;

        _caminhoTerra[0] = Resources.Load("Tiles/chao_2") as Tile;
        _caminhoTerra[1] = Resources.Load("Tiles/chao_1") as Tile;
        _caminhoTerra[2] = Resources.Load("Tiles/T_terra_4") as Tile;
        _caminhoTerra[3] = Resources.Load("Tiles/T_terra_2") as Tile;
        _caminhoTerra[4] = Resources.Load("Tiles/T_terra_3") as Tile;
        _caminhoTerra[5] = Resources.Load("Tiles/T_terra_1") as Tile;
        _caminhoTerra[6] = Resources.Load("Tiles/intersecao_terra") as Tile;
        _caminhoTerra[7] = Resources.Load("Tiles/curva_3") as Tile;
        _caminhoTerra[8] = Resources.Load("Tiles/curva_4") as Tile;
        _caminhoTerra[9] = Resources.Load("Tiles/curva_2") as Tile;
        _caminhoTerra[10] = Resources.Load("Tiles/curva_1") as Tile;
        _caminhoTerra[11] = Resources.Load("Tiles/terra_fim_02") as Tile;
        _caminhoTerra[12] = Resources.Load("Tiles/terra_fim_04") as Tile;
        _caminhoTerra[13] = Resources.Load("Tiles/terra_fim_03") as Tile;
        _caminhoTerra[14] = Resources.Load("Tiles/terra_fim_01") as Tile;

        _caminhoAreia[0] = Resources.Load("Tiles/areia_2") as Tile;
        _caminhoAreia[1] = Resources.Load("Tiles/areia_1") as Tile;
        _caminhoAreia[2] = Resources.Load("Tiles/T_areia_4") as Tile;
        _caminhoAreia[3] = Resources.Load("Tiles/T_areia_2") as Tile;
        _caminhoAreia[4] = Resources.Load("Tiles/T_areia_3") as Tile;
        _caminhoAreia[5] = Resources.Load("Tiles/T_areia_1") as Tile;
        _caminhoAreia[6] = Resources.Load("Tiles/intersecao_areia") as Tile;
        _caminhoAreia[7] = Resources.Load("Tiles/curva-areia-3") as Tile;
        _caminhoAreia[8] = Resources.Load("Tiles/curva-areia-4") as Tile;
        _caminhoAreia[9] = Resources.Load("Tiles/curva-areia-2") as Tile;
        _caminhoAreia[10] = Resources.Load("Tiles/curva-areia-1") as Tile;
        _caminhoAreia[11] = Resources.Load("Tiles/areia_fim_02") as Tile;
        _caminhoAreia[12] = Resources.Load("Tiles/areia_fim_04") as Tile;
        _caminhoAreia[13] = Resources.Load("Tiles/areia_fim_03") as Tile;
        _caminhoAreia[14] = Resources.Load("Tiles/areia_fim_01") as Tile;

        _agua[0] = Resources.Load("Tiles/fundo_agua_1") as Tile;
        _agua[1] = Resources.Load("Tiles/fundo_agua_2") as Tile;
        _agua[2] = Resources.Load("Tiles/fundo_agua_3") as Tile;
        _agua[3] = Resources.Load("Tiles/reta_agua_1") as Tile;
        _agua[4] = Resources.Load("Tiles/reta_agua_2") as Tile;
        _agua[5] = Resources.Load("Tiles/reta_agua_3") as Tile;
        _agua[6] = Resources.Load("Tiles/reta_agua_4") as Tile;
        _agua[7] = Resources.Load("Tiles/agua_curva_interior_01") as Tile;
        _agua[8] = Resources.Load("Tiles/agua_curva_interior_02") as Tile;
        _agua[9] = Resources.Load("Tiles/agua_curva_interior_03") as Tile;
        _agua[10] = Resources.Load("Tiles/agua_curva_interior_04") as Tile;
        _agua[11] = Resources.Load("Tiles/curva_agua_1") as Tile;
        _agua[12] = Resources.Load("Tiles/curva_agua_2") as Tile;
        _agua[13] = Resources.Load("Tiles/curva_agua_3") as Tile;
        _agua[14] = Resources.Load("Tiles/curva_agua_4") as Tile;

    }

    internal object GetTileEstrada()
    {
        throw new NotImplementedException();
    }

    public Tile GetGrama()
    {
        int sorteio = UnityEngine.Random.Range(0, 6);
        if (sorteio < 5)
        {
            return _grama[2];
        }
        else return _grama[UnityEngine.Random.Range(0, 2)];
    }

    public Tile GetAreia()
    {
        return _areia[UnityEngine.Random.Range(0, _grama.Length)];
    }

    public Tile GetCaminho(Vector3Int posicao)
    {
        bool vNorte, vSul, vLeste, vOeste, vCentro;
        vNorte = gerador.TileMaps[3].HasTile(new Vector3Int(posicao.x, posicao.y + 1, 0))
            || gerador.TileMaps[4].HasTile(new Vector3Int(posicao.x, posicao.y + 1, 0)) || gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x, posicao.y + 1, 0));
        vSul = gerador.TileMaps[3].HasTile(new Vector3Int(posicao.x, posicao.y - 1, 0))
            || gerador.TileMaps[4].HasTile(new Vector3Int(posicao.x, posicao.y - 1, 0)) || gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x, posicao.y - 1, 0));
        vLeste = gerador.TileMaps[3].HasTile(new Vector3Int(posicao.x + 1, posicao.y, 0))
            || gerador.TileMaps[4].HasTile(new Vector3Int(posicao.x + 1, posicao.y, 0)) || gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x + 1, posicao.y, 0));
        vOeste = gerador.TileMaps[3].HasTile(new Vector3Int(posicao.x - 1, posicao.y, 0))
            || gerador.TileMaps[4].HasTile(new Vector3Int(posicao.x - 1, posicao.y, 0)) || gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x - 1, posicao.y, 0));
        vCentro = gerador.TileMaps[3].HasTile(posicao)
            || gerador.TileMaps[4].HasTile(posicao) || gerador.TileMaps[5].HasTile(posicao);

        bool[,] matriz = new bool[3, 3] { { false, vNorte, false }, { vOeste, vCentro, vLeste }, { false, vSul, false } };

        return gerador._caminho.value == 0 ? GetTileCaminhoTerra(matriz) : GetTileCaminhoAreia(matriz);

    }

    public Tile GetEstrada(Vector3Int posicao)
    {
        /*
        bool vNorte, vSul, vLeste, vOeste, vCentro;
        vNorte = gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x, posicao.y + 1, 0));
        vSul = gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x, posicao.y - 1, 0));
        vLeste = gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x + 1, posicao.y, 0));
        vOeste = gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x - 1, posicao.y, 0));
        vCentro = gerador.TileMaps[5 ].HasTile(posicao) || gerador.TileMaps[4].HasTile(posicao);
        */

        bool vNorte, vSul, vLeste, vOeste, vCentro;
        vNorte = gerador.TileMaps[3].HasTile(new Vector3Int(posicao.x, posicao.y + 1, 0))
            || gerador.TileMaps[4].HasTile(new Vector3Int(posicao.x, posicao.y + 1, 0)) || gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x, posicao.y + 1, 0));
        vSul = gerador.TileMaps[3].HasTile(new Vector3Int(posicao.x, posicao.y - 1, 0))
            || gerador.TileMaps[4].HasTile(new Vector3Int(posicao.x, posicao.y - 1, 0)) || gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x, posicao.y - 1, 0));
        vLeste = gerador.TileMaps[3].HasTile(new Vector3Int(posicao.x + 1, posicao.y, 0))
            || gerador.TileMaps[4].HasTile(new Vector3Int(posicao.x + 1, posicao.y, 0)) || gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x + 1, posicao.y, 0));
        vOeste = gerador.TileMaps[3].HasTile(new Vector3Int(posicao.x - 1, posicao.y, 0))
            || gerador.TileMaps[4].HasTile(new Vector3Int(posicao.x - 1, posicao.y, 0)) || gerador.TileMaps[5].HasTile(new Vector3Int(posicao.x - 1, posicao.y, 0));
        vCentro = gerador.TileMaps[3].HasTile(posicao)
            || gerador.TileMaps[4].HasTile(posicao) || gerador.TileMaps[5].HasTile(posicao);
        bool[,] matriz = new bool[3, 3] { { false, vNorte, false }, { vOeste, vCentro, vLeste }, { false, vSul, false } };

        return GetTileEstrada(matriz);
    }

    public Tilemap GetTileMap(Vector3Int posicao)
    {
         short cont1 = 0, cont2 = 0;

         for (int i = posicao.y + 1; i < posicao.y - 2; i++)
         {
             for (int j = posicao.x - 1; j < posicao.x + 2; j++) //Popula uma matriz: 1 = Terra, 2 = Areia = 0 = nenhum dos dois.
             {
                 if (gerador.TileMaps[0].GetTile(new Vector3Int(i, j, 0)) != null || gerador.TileMaps[3].GetTile(new Vector3Int(i, j, 0)) != null)
                 {
                     cont1++;
                 }
                 else if (gerador.TileMaps[1].GetTile(new Vector3Int(i, j, 0)) != null || gerador.TileMaps[4].GetTile(new Vector3Int(i, j, 0)) != null)
                 {
                     cont2++;
                 }
             }
         }
         Debug.Log(cont1 + ", " + cont2);
         return cont2 > cont1 ? gerador.TileMaps[4] : gerador.TileMaps[3];
        //return gerador._caminho.value == 0 ? gerador.TileMaps[3] : gerador.TileMaps[4];
    }

    public Tile GetTileCaminhoTerra(bool[,] matriz)
    {
        if (!matriz[0, 1])
        {
            if (matriz[2, 1])
            {
                if (matriz[1, 0] && matriz[1, 2])
                {
                    return _caminhoTerra[5];
                }
                else if (matriz[1, 0])
                {
                    return _caminhoTerra[10];
                }
                else if (matriz[1, 2])
                {
                    return _caminhoTerra[9];
                }
                else
                {
                    return _caminhoTerra[13];
                }
            }
            else if (!matriz[1, 0] && matriz[1, 2])
            {
                return _caminhoTerra[11];
            }
            else if (matriz[1, 0] && !matriz[1, 2])
            {
                return _caminhoTerra[12];
            }
            else return _caminhoTerra[0];
        }
        else
        {
            if (matriz[2, 1])
            {
                if (matriz[1, 0] && matriz[1, 2])
                {
                    return _caminhoTerra[6];
                }
                else
                {
                    if (matriz[1, 0])
                    {
                        return _caminhoTerra[3];
                    }
                    else if (matriz[1, 2])
                    {
                        return _caminhoTerra[2];
                    }
                    else return _caminhoTerra[1];
                }
            }
            else
            {
                if (matriz[1, 0] && matriz[1, 2])
                {
                    return _caminhoTerra[4];
                }
                else
                {
                    if (matriz[1, 0])
                    {
                        return _caminhoTerra[8];
                    }
                    else if (matriz[1, 2])
                    {
                        return _caminhoTerra[7];
                    }
                    else return _caminhoTerra[14];
                }
            }
        }
    }

    public Tile GetTileCaminhoAreia(bool[,] matriz)
    {
        if (!matriz[0, 1])
        {
            if (matriz[2, 1])
            {
                if (matriz[1, 0] && matriz[1, 2])
                {
                    return _caminhoAreia[5];
                }
                else if (matriz[1, 0])
                {
                    return _caminhoAreia[10];
                }
                else
                {
                    return matriz[1, 2] ? _caminhoAreia[9] : _caminhoAreia[13];
                }
            }
            else if (!matriz[1, 0] && matriz[1, 2])
            {
                return _caminhoAreia[11];
            }
            else if (matriz[1, 0] && !matriz[1, 2])
            {
                return _caminhoAreia[12];
            }
            else return _caminhoAreia[0];
        }
        else
        {
            if (matriz[2, 1])
            {
                if (matriz[1, 0] && matriz[1, 2])
                {
                    return _caminhoAreia[6];
                }
                else
                {
                    if (matriz[1, 0])
                    {
                        return _caminhoAreia[3];
                    }
                    else if (matriz[1, 2])
                    {
                        return _caminhoAreia[2];
                    }
                    else return _caminhoAreia[1];
                }
            }
            else
            {
                if (matriz[1, 0] && matriz[1, 2])
                {
                    return _caminhoAreia[4];
                }
                else
                {
                    if (matriz[1, 0])
                    {
                        return _caminhoAreia[8];
                    }
                    else if (matriz[1, 2])
                    {
                        return _caminhoAreia[7];
                    }
                    else return matriz[2, 1] ? _caminhoAreia[1] : _caminhoAreia[14];
                }
            }
        }
    }

    public Tile GetTileEstrada(bool[,] matriz)
    {
        if (!matriz[0, 1])
        {
            if (matriz[2, 1])
            {
                if (matriz[1, 0] && matriz[1, 2])
                {
                    return _estrada[5];
                }
                else if (matriz[1, 0])
                {
                    return _estrada[10];
                }
                else if (matriz[1, 2])
                {
                    return _estrada[9];
                }
                else
                {
                    return _estrada[13];
                }
            }
            else if (!matriz[1, 0] && matriz[1, 2])
            {
                return _estrada[11];
            }
            else if (matriz[1, 0] && !matriz[1, 2])
            {
                return _estrada[12];
            }
            else return _estrada[0];
        }
        else
        {
            if (matriz[2, 1])
            {
                if (matriz[1, 0] && matriz[1, 2])
                {
                    return _estrada[6];
                }
                else
                {
                    if (matriz[1, 0])
                    {
                        return _estrada[3];
                    }
                    else if (matriz[1, 2])
                    {
                        return _estrada[2];
                    }
                    else return _estrada[1];
                }
            }
            else
            {
                if (matriz[1, 0] && matriz[1, 2])
                {
                    return _estrada[4];
                }
                else
                {
                    if (matriz[1, 0])
                    {
                        return _estrada[8];
                    }
                    else if (matriz[1, 2])
                    {
                        return _estrada[7];
                    }
                    else return _estrada[14];
                }
            }
        }
    }

    public void UpdateTile()
    {

        Vector3Int posicao;
        int valorOriginal = gerador._caminho.value;
        for (int y = -3; y <= 4; y++)
        {
            for (int x = -8; x <= 4; x++)
            {
                posicao = new Vector3Int(x, y, 0);

                if (gerador.TileMaps[3].HasTile(posicao))
                {
                    gerador._caminho.value = 0;
                    gerador.TileMaps[3].SetTile(posicao, GetCaminho(posicao));
                }

                if (gerador.TileMaps[4].HasTile(posicao))
                {
                    gerador._caminho.value = 1;
                    gerador.TileMaps[4].SetTile(posicao, GetCaminho(posicao));
                }

                if (gerador.TileMaps[5].HasTile(posicao))
                {
                    gerador.TileMaps[5].SetTile(posicao, GetEstrada(posicao));
                }
                if (gerador.TileMaps[2].HasTile(posicao))
                {
                    gerador.TileMaps[2].SetTile(posicao, GetAgua(posicao));
                }
            }
        }
        gerador._caminho.value = valorOriginal;
    }
    private int GetTileValue(TileBase tile, bool[] caminho)
    {
        for (int i = 0; i < caminho.Length; i++)
        {
            if (caminho[i].Equals(tile))
            {
                return i;
            }
        }
        return int.MaxValue;
    }

    public Tile GetAgua(Vector3Int posicao)
    {
        bool[,] m1 = new bool[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                m1[i, j] = gerador.TileMaps[2].HasTile(new Vector3Int(posicao.x - 1 + j, posicao.y - 1 + i, 0));
            }
        }
        m1[1, 1] = true;

        if (GetTileAgua(m1) == null)
        {
            m1[0, 0] = false;
            m1[0, 2] = false;
            m1[2, 0] = false;
            m1[2, 2] = false;

            if (GetTileAgua(m1) == null)
            {
                // printArray2d(m1);
                return _agua[0];
            }
            else return GetTileAgua(m1);

        }
        else return GetTileAgua(m1);

    }

    private void printArray2d(bool[,] matriz)
    {
        string aux = "";

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                aux += matriz[i, j] + " ";
            }
            aux += "\n";
        }
        Debug.Log(aux);
        aux = null;
    }

    private Tile GetTileAgua(bool[,] matriz)
    {
        for (int i = 0; i < 20; i++)
        {
            bool[,] m2 = new bool[3, 3];
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    m2[j, k] = matrizes[i, j, k];
                }
            }
            if (CompararMatriz(matriz, m2))
            {
                //Debug.Log(i + "," + GetNumeroArrayTile(i));
                return _agua[GetNumeroArrayTile(i)];
            }
        }
        return null;
    }

    public bool CompararMatriz(bool[,] m1, bool[,] m2)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (m1[i, j] != m2[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    private int GetNumeroArrayTile(int valor)
    {
        switch (valor)
        {
            case 0:
            case 1:
                return 0;
            case 2:
            case 3:
                return 3;
            case 4:
            case 5:
                return 4;
            case 6:
            case 7:
                return 5;
            case 8:
            case 9:
                return 6;
            case 10:
                return 10;
            case 11:
                return 8;
            case 12:
                return 9;
            case 13:
                return 10;
            case 14:
            case 18:
                return 14;
            case 15:
                return 13;
            case 16:
                return 11;
            case 17:
                return 12;
            default: return 0;
        }
    }
}
#endif