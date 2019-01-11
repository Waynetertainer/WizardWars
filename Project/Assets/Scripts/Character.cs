﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Occupant, IUniqueSpell
{
    public string pName;
    public int pHp = 10;
    //[HideInInspector] 
    public int pHpCurrent;
    public int pAp = 10;
    //[HideInInspector] 
    public int pApCurrent;
    public int pVisionRange = 10;
    public int pWalkRange = 10;
    public bool pHasBeenRevealed = false;


    public bool pEffectHit;

    [Range(1, 20)] public int pWalkCost;

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

    public Character CurrentCharacter
    {
        get { return this; }
    }

    public GameObject VFXPrefab
    {
        get { return _VFXPrefab; }
    }
    public GameObject VFXSpawner
    {
        get { return _VFXSpawner; }
    }

    [HideInInspector] public List<Tile> pReachableTiles;
    [HideInInspector] public List<Tile> pVisibleTiles;
    [HideInInspector] public bool pMoved;
    [HideInInspector] public bool pFired;
    public List<Tile> pAIPatrouillePoints = new List<Tile>(); // used for AI
    [HideInInspector] public int mPatWaypointID = 0; // used for AI
    public CharacterHealthBar pHealthBarScript;

    [SerializeField] public string _SpellName = "Fireball";
    [SerializeField] public int _Damage = 2;
    [SerializeField] public int _Cost = 2;
    [SerializeField] public int _Range = 4;

    //public ScriptableObject pUniqueSpellScriptable;

    public IUniqueSpell pUniqueSpell;

    [Header("VFX")]
    public GameObject pAura;
    [SerializeField] private GameObject _VFXPrefab;
    [SerializeField] private GameObject _VFXSpawner;

    private bool mIsActiveCharacter;

    private void Start()
    {
        //pUniqueSpell = pUniqueSpellScriptable as IUniqueSpell;
        pUniqueSpell = GetComponents<IUniqueSpell>()[1];
        pHpCurrent = pHp;
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

        //pTile.pBlockType = eBlockType.Empty;
        List<Tile> path = GridManager.pInstance.GetPathTo(pTile, targetTile);

        for (int i = path.Count - 1; i >= 0; i--)
        {
            var tile = path[i];

            transform.LookAt(tile.transform.position);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

            while (Vector3.Distance(transform.position, tile.transform.position) >= 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, tile.transform.position, Time.deltaTime * 5);
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
        }

        pTile.pCharacterId = -1;
        pApCurrent -= Tile.Distance(pTile, targetTile) * pWalkCost;
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
        //pTile.pBlockType = eBlockType.Blocked;
        if (pFraction == eFraction.PC)
            yield break;

        if (pApCurrent > 0)
        {
            GameManager.pInstance.ChangeState(eGameState.Move);
        }
        else
        {
            GameManager.pInstance.ChangeState(eGameState.End);
        }
    }

    public void StandardAttack(Tile t)
    {
        if (t.pCharacterId == -1)
            return;

        StartCoroutine(StandardAttackCoroutine(t));

    }

    private IEnumerator StandardAttackCoroutine(Tile t)
    {
        transform.LookAt(t.transform.position);
        transform.localEulerAngles = new Vector3(pFraction == eFraction.Player ? 0 : -90, transform.localEulerAngles.y, 0);

        var inst = Instantiate(_VFXPrefab, _VFXSpawner.transform);
        inst.transform.LookAt(EntityManager.pInstance.GetCharacterForId(t.pCharacterId).transform.position
                                    + new Vector3(0, pFraction == eFraction.PC ? 1 : 0.5f, 0));

        yield return new WaitUntil(() => pEffectHit);

        pEffectHit = false;
        pApCurrent -= Cost;
        //TODO: Check for Cover between tiles to reduce damage
        RaycastHit[] mHits = Physics.SphereCastAll(pTile.transform.position, 0.1f, t.transform.position - pTile.transform.position, Vector3.Distance(t.transform.position, pTile.transform.position));
        eBlockType maxCover = eBlockType.Empty;
        /*
        foreach (RaycastHit hit in mHits)
        {
            if (hit.transform.gameObject.GetComponent<Character>().pTile.pBlockType == eBlockType.Blocked)
            {
                Debug.Log("Full cover so no damage!");
                return;
            }
            else if (hit.transform.gameObject.GetComponent<Character>().pTile.pBlockType == eBlockType.HalfBlocked)
            {
                Debug.Log("Half cover, half damage");
            }
        }
        */



        Debug.Log("Damage for " + EntityManager.pInstance.GetCharacterForId(t.pCharacterId).pName + " Amount: " + Damage.ToString() + " HPCurrent: " + EntityManager.pInstance.GetCharacterForId(t.pCharacterId).pHpCurrent.ToString());
        EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(Damage);

        if (this.pFraction == eFraction.Player)
        {
            if (pApCurrent > 0)
            {
                GameManager.pInstance.ChangeState(eGameState.FireSkill);
            }
            else
            {
                yield return new WaitForSeconds(1);
                GameManager.pInstance.ChangeState(eGameState.End);
            }
        }


    }

    public void CastUnique(Tile t)
    {
        if (pUniqueSpell != null)
        {
            pUniqueSpell.HideUniquePreview(t);
            pApCurrent -= pUniqueSpell.Cost;
            pFired = true;
            pUniqueSpell.CastUnique(t);
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
        pHpCurrent -= damage;

        if (pHpCurrent <= 0)
            EntityManager.pInstance.KillCharacter(this);
    }

    private void Update()
    {
        //GetComponent<Renderer>().material.SetColor("_Color", mIsActiveCharacter ? Color.white :
        //                                                        pFraction == eFraction.PC ? Color.blue : Color.red);
    }

    public void Select()
    {
        CameraMovement.SetTarget(transform);
        GameManager.pInstance.pActiveCharacter = this;
        mIsActiveCharacter = true;
        pApCurrent = pAp;
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
        pReachableTiles = GridManager.pInstance.GetReachableTiles(pTile, pApCurrent / pWalkCost);
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