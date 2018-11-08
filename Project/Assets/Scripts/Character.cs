using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Character : Occupant
{
    public int pVisionRange;
    public int pWalkRange;
    private bool mIsActiveCharacter;
    public List<Tile> pReachableTiles;


    public void StandardAttack()
    {

    }

    public virtual void UniqueAttack()
    {

    }

    public void Move(Tile targetTile)
    {
        foreach (Tile tile in pReachableTiles)
        {
            tile.ResetTile();
        }
        transform.position = targetTile.transform.position;
        transform.position+= Vector3.up;
        pTile.pOccupant = null;
        pTile = targetTile;
        targetTile.pOccupant = this;
        pReachableTiles = GridManager.pInstance.GetReachableTiles(pTile, pWalkRange);
        foreach (Tile tile in pReachableTiles)
        {
            tile.IsReachable(this);
        }
    }

    public void Activation()
    {
        pReachableTiles = GridManager.pInstance.GetReachableTiles(pTile, pWalkRange);
        foreach (Tile tile in pReachableTiles)
        {
            tile.IsReachable(this);
        }
        GameManager.pInstance.pActiveCharacter = this;
        mIsActiveCharacter = true;
    }

    public void Deactivation()
    {
        foreach (Tile tile in pReachableTiles)
        {
            tile.ResetTile();
        }
        GameManager.pInstance.pActiveCharacter = null;
        mIsActiveCharacter = false;
    }
}
