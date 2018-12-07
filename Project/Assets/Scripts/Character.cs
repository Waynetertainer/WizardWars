using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Occupant, IUniqueSpell
{
    public string pName;
    public int pHp = 10;
    public int pAp = 10;
    public int pVisionRange = 10;
    public int pWalkRange = 10;
    public int pWalkCost = 1;

    public string SpellName
    {
        get { return _SpellName; }
    }
    public int Damage
    {
        get { return _Damage; }
    }
    public int Cost
    {
        get { return _Cost; }
    }
    public int Range
    {
        get { return _Range; }
    }

    [HideInInspector] public int pCurrentAp;
    [HideInInspector] public List<Tile> pReachableTiles;
    [HideInInspector] public List<Tile> pVisibleTiles;
    [HideInInspector] public bool pMoved;
    [HideInInspector] public bool pFired;

    [SerializeField] private string _SpellName = "Fireball";
    [SerializeField] private int _Damage = 2;
    [SerializeField] private int _Cost = 2;
    [SerializeField] private int _Range = 4;

    public ScriptableObject pUniqueSpellScriptable;

    public IUniqueSpell pUniqueSpell;

    private bool mIsActiveCharacter;

    private void Start()
    {
        pUniqueSpell = pUniqueSpellScriptable as IUniqueSpell;
    }

    public static Character CreateCharacter(eFraction fraction, Tile spawnTile, Character prefab, ScriptableObject uniqueSpell)
    {
        Character e = Instantiate(prefab, new Vector3(
            spawnTile.transform.position.x,
            spawnTile.transform.position.y,
            spawnTile.transform.position.z)
            , prefab.transform.rotation);

        e.pFraction = fraction;
        e.pUniqueSpell = uniqueSpell as IUniqueSpell;
        e.pTile = spawnTile;
        return e;
    }

    public void Move(Tile targetTile)
    {
        StartCoroutine(MoveEnumerator(targetTile));
    }

    public IEnumerator MoveEnumerator(Tile targetTile)
    {
        foreach (Tile tile in pReachableTiles)
        {
            tile.ResetReachable();
        }
        foreach (Tile tile in pVisibleTiles)
        {
            tile.ResetVisibility();
        }

        List<Tile> path = GridManager.pInstance.GetPathTo(pTile, targetTile);

        for (int i = path.Count - 1; i >= 0; i--)
        {
            var tile = path[i];

            while (Vector3.Distance(transform.position, tile.transform.position) >= 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, tile.transform.position, Time.deltaTime * 5);
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
        }

        pTile.pCharacterId = -1;
        pCurrentAp -= Tile.Distance(pTile, targetTile) * pWalkCost;
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
        GameManager.pInstance.ChangeState(eGameState.Move);
    }

    public void StandardAttack(Tile t)
    {
        if (t.pCharacterId == -1)
            return;

        pCurrentAp -= Cost;

        EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(1);
        t.GetComponent<Renderer>().material.SetColor("_Color", t.Color);
    }

    public void CastUnique(Tile t)
    {
        if (pUniqueSpell != null)
        {
            pUniqueSpell.HideUniquePreview(t);
            pCurrentAp -= pUniqueSpell.Cost;
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
        CameraMovement.SetTarget(transform);
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
        pReachableTiles = GridManager.pInstance.GetReachableTiles(pTile, pCurrentAp < pWalkRange / pWalkCost ? pCurrentAp : pWalkRange);
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
        if (GameManager.pInstance.pGameState == eGameState.FireSkill)
        {
            pVisibleTiles = GridManager.pInstance.GetVisibleTiles(pTile, pVisionRange > Range ? Range : pVisionRange);
        }
        else
        {
            pVisibleTiles = GridManager.pInstance.GetVisibleTiles(pTile, pVisionRange > pUniqueSpell.Range ? pUniqueSpell.Range : pVisionRange);
        }
        foreach (Tile tile in pVisibleTiles)
        {
            tile.IsVisible();
        }
    }
    public void HideView()
    {
        pVisibleTiles = GridManager.pInstance.GetVisibleTiles(pTile, pVisionRange);
        foreach (Tile tile in pVisibleTiles)
        {
            tile.ResetVisibility();
        }
    }

}