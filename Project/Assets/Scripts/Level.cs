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
    public Vector2Int[] pPlayerSpawns = new Vector2Int[3];
    public Vector2Int[] pPCSpawns = new Vector2Int[3];

}
