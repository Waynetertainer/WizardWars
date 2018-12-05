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
        mColor = GetComponent<Renderer>().material.color;
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
        GetComponent<Renderer>().material.SetColor("_Color", mColor);
        mMouseOver = false;
    }

    private void ChangeColor()
    {
        if (pIsSpawn)
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            mColor = Color.green;
            return;
        }

        switch (pBlockType)
        {
            case eBlockType.Empty:
                switch (eVisibility)
                {
                    case eVisibility.Opaque:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
                        mColor = Color.gray;
                        break;
                    case eVisibility.Seethrough:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                        mColor = Color.white;
                        break;
                }
                break;
            case eBlockType.Blocked:
                switch (eVisibility)
                {
                    case eVisibility.Opaque:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                        mColor = Color.black;
                        break;
                    case eVisibility.Seethrough:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                        mColor = Color.blue;
                        break;
                }
                break;
            case eBlockType.HalfBlocked:
                switch (eVisibility)
                {
                    case eVisibility.Opaque:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                        mColor = Color.yellow;
                        break;
                    case eVisibility.Seethrough:
                        GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
                        mColor = Color.cyan;
                        break;
                }
                break;
        }
    }
}