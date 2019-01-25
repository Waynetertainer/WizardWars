/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject
{
    public Vector2Int pSize;
    public List<TileStruct> pListGrid = new List<TileStruct>();
    public Vector2Int[] pPlayer1Spawns = new Vector2Int[3];
    public Vector2Int[] pAI1Spawns = new Vector2Int[3];
    public Vector2Int[] pPlayer2Spawns = new Vector2Int[3];
    public Vector2Int[] pAI2Spawns = new Vector2Int[3];
    public Vector2Int[] pAI1Spawner = new Vector2Int[2];
    public Vector2Int[] pAI2Spawner = new Vector2Int[2];


}
