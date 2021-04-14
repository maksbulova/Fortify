using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public static class TilemapsManager
{

    public static Tilemap TilemapStructure = GameObject.Find("Grid/structures").GetComponent<Tilemap>();
    public static Tilemap TilemapTerrain = GameObject.Find("Grid/terrain").GetComponent<Tilemap>();
    public static Tilemap TilemapEffects = GameObject.Find("Grid/effects").GetComponent<Tilemap>();

}
