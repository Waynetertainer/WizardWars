using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Character : Occupant
{
    public int pHp;
    public int pAp = 10;
    public int pVisionRange;
    public int pWalkRange;
    private bool mIsActiveCharacter;
    public List<Tile> pReachableTiles;
    public List<Tile> pVisibleTiles;

    public bool pMoved;
    public bool pFired;


    public void StandardAttack()
    {
        pFired = true;
    }

    public virtual void UniqueAttack()
    {
        pFired = true;
    }

    private void Update()
    {
        GetComponent<Renderer>().material.SetColor("_Color", mIsActiveCharacter ? Color.white : 
                                                                pFraction == eFraction.PC ? Color.blue : Color.red);
    }

    public void Move(Tile targetTile)
    {
        foreach (Tile tile in pReachableTiles)
        {
            tile.ResetReachable();
        }
        foreach (Tile tile in pVisibleTiles)
        {
            tile.ResetVisibility();
        }
        transform.position = targetTile.transform.position;
        transform.position += Vector3.up;
        pTile.pOccupant = null;
        pAp -= Tile.Distance(pTile, targetTile);
        pTile = targetTile;
        targetTile.pOccupant = this;
        pReachableTiles = GridManager.pInstance.GetReachableTiles(pTile, pWalkRange);
        foreach (Tile tile in pReachableTiles)
        {
            tile.IsReachable(this);
        }
        pVisibleTiles = GridManager.pInstance.GetVisibleTiles(pTile, pVisionRange);
        foreach (Tile tile in pVisibleTiles)
        {
            tile.IsVisible();
        }

        pMoved = true;
    }

    public void Select()
    {
        GameManager.pInstance.pActiveCharacter = this;
        mIsActiveCharacter = true;
    }
    public void Deselect()
    {
        HideRange();
        HideView();
        GameManager.pInstance.pActiveCharacter = null;
        mIsActiveCharacter = false;
    }

    public void ShowRange()
    {
        pReachableTiles = GridManager.pInstance.GetReachableTiles(pTile, pWalkRange);
        foreach (Tile tile in pReachableTiles)
        {
            tile.IsReachable(this);
        }
    }
    public void HideRange()
    {
        foreach (Tile tile in pReachableTiles)
        {
            tile.ResetReachable();
        }
    }

    public void ShowView()
    {
        pVisibleTiles = GridManager.pInstance.GetVisibleTiles(pTile, pVisionRange);
        foreach (Tile tile in pVisibleTiles)
        {
            tile.IsVisible();
        }
    }
    public void HideView()
    {
        foreach (Tile tile in pVisibleTiles)
        {
            tile.ResetVisibility();
        }
    }
}