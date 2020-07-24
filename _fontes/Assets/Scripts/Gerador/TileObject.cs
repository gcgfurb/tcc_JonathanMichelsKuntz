using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TileObject {


    
    public Vector3 Posicao { get; set; }
    //public Tilemap TileMap { get; set; }
    //public TileBase Tile { get; set; }

    public TileObject(Vector3 posicao/*, Tilemap tilemap, TileBase tile*/) {
        Posicao = posicao;
        //TileMap = tilemap;
        //Tile = tile;
    }

}
