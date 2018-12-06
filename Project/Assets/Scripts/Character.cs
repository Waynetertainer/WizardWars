using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Occupant, IUniqueSpell
{
    public int pHp = 10;
    public int pAp = 10;
    public int pCurrentAp;
    public int pVisionRange;
    public int pWalkRange;
    private bool mIsActiveCharacter;
    public List<Tile> pReachableTiles;
    public List<Tile> pVisibleTiles;
    public IUniqueSpell pUniqueSpell;
    public string SpellName { get; private set; }

    public bool pMoved;
    public bool pFired;

    public static Character CreateCharacter(eFraction fraction, Tile spawnTile, Character prefab, IUniqueSpell uniqueSpell)
    {
        Character e = Instantiate(prefab, new Vector3(
            spawnTile.transform.position.x,
            spawnTile.transform.position.y,
            spawnTile.transform.position.z)
            , prefab.transform.rotation);

        e.pFraction = fraction;
        e.pUniqueSpell = uniqueSpell;
        e.pTile = spawnTile;
        return e;
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
        pTile.pCharacterId = -1;
        pCurrentAp -= Tile.Distance(pTile, targetTile);
        pTile = targetTile;
        targetTile.pCharacterId = EntityManager.pInstance.GetIdForCharacter(this);
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

    public void StandardAttack(Tile t)
    {
        pFired = true;

        if (t.pCharacterId == -1)
            return;

        EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(1);
        t.GetComponent<Renderer>().material.SetColor("_Color", t.Color);
    }

    public void CastUnique(Tile t)
    {
        if (pUniqueSpell != null)
        {
            pUniqueSpell.HideUniquePreview(t);
            pUniqueSpell.CastUnique(t);
            pFired = true;
        }
    }

    public void ShowUniquePreview(Tile t)
    {
        if (pUniqueSpell != null)
        {
            pUniqueSpell.ShowUniquePreview(t);
        }
    }

    public void HideUniquePreview(Tile t)
    {
        if (pUniqueSpell != null)
        {
            pUniqueSpell.HideUniquePreview(t);
        }
    }

    public void DealDamage(int damage)
    {
        pHp -= damage;
        if (pHp <= 0)
            EntityManager.pInstance.KillCharacter(this);
    }

    private void Update()
    {
        GetComponent<Renderer>().material.SetColor("_Color", mIsActiveCharacter ? Color.white :
                                                                pFraction == eFraction.PC ? Color.blue : Color.red);
    }

    public void Select()
    {
        GameManager.pInstance.pActiveCharacter = this;
        mIsActiveCharacter = true;
        pCurrentAp = pAp;
    }
    public void Deselect()
    {
        HideRange();
        HideView();
        GameManager.pInstance.pActiveCharacter = null;
        GameManager.pInstance.pGridGameObject.SetActive(false);
        mIsActiveCharacter = false;
    }

    public void ShowRange()
    {
        pReachableTiles = GridManager.pInstance.GetReachableTiles(pTile, pCurrentAp < pWalkRange ? pCurrentAp : pWalkRange);
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