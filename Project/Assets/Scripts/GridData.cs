using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridData
{
    public Tile[][] pGrid;
    //public Vector2Int[][] pSpawnPoints;
    public int[][][] spawns;

    public string startpositions;
}
