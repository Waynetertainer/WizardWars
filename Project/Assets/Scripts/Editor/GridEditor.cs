/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridManager myScript = (GridManager)target;
        if (GUILayout.Button("Spawn Level"))
        {
            myScript.DestroyGrid();
            myScript.CreateGrid();
        }
        if (GUILayout.Button("Clear Level"))
        {
            Debug.Log("Attempting to destroy level");
            myScript.DestroyGrid();
        }
        if (GUILayout.Button("Create Navigation"))
        {
            Debug.Log("Guessing the Tile visibility and blocking status");
            myScript.CreateNavigation();
        }
    }
}
