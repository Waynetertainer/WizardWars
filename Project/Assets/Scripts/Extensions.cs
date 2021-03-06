﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{

    public static Tile[][] ToJsonArray(this Tile[,] input)
    {
        Tile[][] output = new Tile[input.GetLength(0)][];
        for (int i = 0; i < input.GetLength(0); i++)
        {
            output[i] = new Tile[input.GetLength(1)];
            for (int j = 0; j < input.GetLength(1); j++)
            {
                if (input[i, j] != null)
                {
                    output[i][j] = new Tile();
                    output[i][j].pPosition = input[i, j].pPosition;
                    output[i][j].pCharacterId = input[i, j].pCharacterId;
                }
                else
                {
                    output[i][j] = null;
                }
            }
        }
        return output;
    }
    public static Vector2Int[][] ToJsonArray(this Vector2Int[,] input)
    {
        Vector2Int[][] output = new Vector2Int[input.GetLength(0)][];
        for (int i = 0; i < input.GetLength(0); i++)
        {
            output[i] = new Vector2Int[input.GetLength(1)];
            for (int j = 0; j < input.GetLength(1); j++)
            {
                output[i][j] = new Vector2Int();
                output[i][j].x = input[i, j].x;
                output[i][j].y = input[i, j].y;
            }
        }
        return output;
    }

    public static int[][][] ToJason3Array(this Vector2Int[,] input)
    {
        int[][][] output = new int[input.GetLength(0)][][];
        for (int i = 0; i < input.GetLength(0); i++)
        {
            output[i] = new int[input.GetLength(1)][];
            for (int j = 0; j < input.GetLength(1); j++)
            {
                output[i][j] = input[i, j].ToArray();
            }
        }

        return output;
    }

    public static int[] ToArray(this Vector2Int input)
    {
        return new int[2] { input.x, input.y };
    }

    public static bool isType<T>(this RaycastHit hit)
    {
        return hit.transform.GetComponent<T>() != null;
    }

    public static Vector2Int ToGridCoordinates(this Vector2Int coordinates)
    {
        return new Vector2Int(coordinates.x/2, coordinates.y);
    }
}
