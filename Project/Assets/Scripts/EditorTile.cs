/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using UnityEngine;

public class EditorTile : Tile
{
    public bool pIsSpawn;

    private void Start()
    {
        ChangeColor();
    }

    protected override void OnMouseEnter()
    {
        Color = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        mMouseOver = true;
    }

    private void Update()
    {
        if (!mMouseOver)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            eVisibility++;
            if ((int)eVisibility >= Enum.GetValues(typeof(eVisibility)).Length)
                eVisibility = 0;
            ChangeColor();
            GridManager.pInstance.SaveGrid();
        }
        if (Input.GetMouseButtonDown(0))
        {
            pBlockType++;
            if ((int)pBlockType >= Enum.GetValues(typeof(eBlockType)).Length)
                pBlockType = 0;
            ChangeColor();
            GridManager.pInstance.SaveGrid();
        }

        if (Input.GetMouseButtonDown(2))
        {
            ChangeColor();
            GridManager.pInstance.SaveGrid();
        }
    }

    protected override void OnMouseExit()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color);
        mMouseOver = false;
    }

    private void ChangeColor()
    {
        if (pIsSpawn)
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            Color = Color.green;
            return;
        }

        switch (pBlockType)
        {
            case eBlockType.Empty:
                switch (eVisibility)
                {
                    case eVisibility.Opaque:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
                        Color = Color.gray;
                        break;
                    case eVisibility.Seethrough:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                        Color = Color.white;
                        break;
                }
                break;
            case eBlockType.Blocked:
                switch (eVisibility)
                {
                    case eVisibility.Opaque:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                        Color = Color.black;
                        break;
                    case eVisibility.Seethrough:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                        Color = Color.blue;
                        break;
                }
                break;
            case eBlockType.HalfBlocked:
                switch (eVisibility)
                {
                    case eVisibility.Opaque:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                        Color = Color.yellow;
                        break;
                    case eVisibility.Seethrough:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
                        Color = Color.cyan;
                        break;
                }
                break;
        }
    }
}