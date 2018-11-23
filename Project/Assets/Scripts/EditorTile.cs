/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEngine;

public class EditorTile : Tile
{
    protected override void OnMouseEnter()
    {
        mColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    protected override void OnMouseExit()
    {
        GetComponent<Renderer>().material.SetColor("_Color", mColor);
    }
}
