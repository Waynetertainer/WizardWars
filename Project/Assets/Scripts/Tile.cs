using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class Tile : MonoBehaviour
{
    public Vector2Int pPosition;
    public int pCharacterId = -1;
    public eBlockType pBlockType;
    public eVisibility eVisibility;
    public Color Color;

    protected bool mMouseOver;
    private LineRenderer mLine;

    private void Start()
    {
        mLine = FindObjectOfType<LineRenderer>();
        transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "X " + pPosition.x;
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = "Y " + pPosition.y;
    }

    public void ResetReachable()
    {
        for (int i = 0; i < 6; i++)
        {
            transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ResetVisibility()
    {
        foreach (var trans in gameObject.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = 9;
        }
        //if (pOccupant != null)
        //{
        //    pOccupant.GetComponent<MeshRenderer>().enabled = false;
        //}
    }

    public void IsVisible()
    {
        foreach (var trans in gameObject.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = 0;
        }

        if (pCharacterId != -1)
        {
            EntityManager.pInstance.GetCharacterForId(pCharacterId).GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void IsReachable(Character character)
    {
        for (int j = 0; j < 6; j++)
        {
            Tile tempTile = GridManager.pInstance.GetNeighbour(this, j);
            if (tempTile == null || !character.pReachableTiles.Contains(tempTile))
            {
                transform.GetChild(0).GetChild(j).gameObject.SetActive(true);
            }
        }
    }

    public static int Distance(Tile a, Tile b)
    {
        int dx = Mathf.Abs(a.pPosition.x - b.pPosition.x);
        int dy = Mathf.Abs(a.pPosition.y - b.pPosition.y);
        return dy + Mathf.Max(0, (dx - dy) / 2);
    }
    public List<Tile> GetNeighbours()
    {
        List<Tile> neighbours = new List<Tile>();
        for (int j = 0; j < 6; j++)
        {
            Tile tempTile = GridManager.pInstance.GetNeighbour(this, j);
            if (tempTile != null)
            {
                neighbours.Add(tempTile);
            }
        }
        return neighbours;
    }

    protected virtual void OnMouseEnter()
    {
        switch (GameManager.pInstance.pGameState)
        {
            case eGameState.Move:
                mMouseOver = true;
                GridManager.pInstance.DrawPath(GridManager.pInstance.GetPathTo(GameManager.pInstance.pActiveCharacter.pTile, this));
                break;
            case eGameState.FireSkill:
                Color = GetComponent<Renderer>().material.color;
                GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            case eGameState.FireUnique:
                Color = GetComponent<Renderer>().material.color;
                GameManager.pInstance.pActiveCharacter.ShowUniquePreview(this);
                break;
        }
    }

    protected virtual void OnMouseExit()
    {
        switch (GameManager.pInstance.pGameState)
        {
            case eGameState.Move:
                mLine.gameObject.SetActive(false);
                mMouseOver = false;
                for (int i = 0; i < GridManager.pInstance.pPathPainter.transform.childCount; i++)
                {
                    GridManager.pInstance.pPathPainter.transform.GetChild(i).gameObject.SetActive(false);
                }
                break;
            case eGameState.FireSkill:
                GetComponent<Renderer>().material.SetColor("_Color", Color);
                break;
            case eGameState.FireUnique:
                GameManager.pInstance.pActiveCharacter.HideUniquePreview(this);
                break;
        }

    }

    private void Update()
    {
        if (mMouseOver)
        {
            if (Input.GetMouseButtonDown(1))
            {

            }
        }
    }
}

[Serializable]
public struct TileStruct
{
    public Vector2Int pPosition;
    public eBlockType pBlockType;
    public eVisibility pVisibilty;
}