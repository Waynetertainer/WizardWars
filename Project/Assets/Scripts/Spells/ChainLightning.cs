/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour, IUniqueSpell
{

    [SerializeField] private string _SpellName;
    [SerializeField] private int _Damage;
    [SerializeField] private int _Cost;
    [SerializeField] private int _Range;
    [SerializeField] private GameObject _VFXPrefab;
    [SerializeField] private GameObject _VFXPrefab2;
    [SerializeField] private GameObject _VFXSpawner;

    private List<int> mHitIds = new List<int>();
    private Vector3 mNextSpawn;

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

    public Character CurrentCharacter { get { return GetComponentInParent<Character>(); } }

    public GameObject VFXPrefab
    {
        get { return _VFXPrefab; }
    }

    public GameObject VFXSpawner
    {
        get { return _VFXSpawner; }
    }

    public void CastUnique(Tile t)
    {
        mNextSpawn = _VFXSpawner.transform.position;
        StartCoroutine(CastUniqueCoroutine(t, Damage));
    }

    //public void CastUnique(Tile t, int damage)
    //{
    //    EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(damage);
    //    mHitIds.Add(t.pCharacterId);

    //    List<Tile> list = GridManager.pInstance.GetVisibleTiles(t, Range);
    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        var tile = list[i];

    //        if (tile.pCharacterId != -1 && !mHitIds.Contains(tile.pCharacterId))
    //        {
    //            CastUnique(tile, damage - 1);
    //            return;
    //        }
    //    }

    //    mHitIds.Clear();
    //}

    private IEnumerator CastUniqueCoroutine(Tile t, int damage)
    {
        if (damage == 1)
            yield break;

        if (t.pCharacterId != -1 && t.pCharacterId != EntityManager.pInstance.GetIdForCharacter(CurrentCharacter))
        {
            var inst = Instantiate(damage == Damage ? _VFXPrefab : _VFXPrefab2, mNextSpawn, Quaternion.identity);
            inst.transform.LookAt(EntityManager.pInstance.GetCharacterForId(t.pCharacterId).transform.position
                                  + new Vector3(0, CurrentCharacter.pFraction == eFraction.PC ? 1 : 0.5f, 0));
            mHitIds.Add(t.pCharacterId);

            yield return new WaitForSeconds(damage == Damage ? 3 : 0.5f);
            EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(Damage);
            mNextSpawn = EntityManager.pInstance.GetCharacterForId(t.pCharacterId).transform.position;
            mNextSpawn = new Vector3(mNextSpawn.x, mNextSpawn.y + (EntityManager.pInstance.GetCharacterForId(t.pCharacterId).pFraction == eFraction.Player ? 1 : 0.5f), mNextSpawn.z);
        }

        List<Tile> list = GridManager.pInstance.GetVisibleTiles(t, Range);
        for (int i = 0; i < list.Count; i++)
        {
            var tile = list[i];

            if (tile.pCharacterId != -1 && !mHitIds.Contains(tile.pCharacterId) && tile.pCharacterId != EntityManager.pInstance.GetIdForCharacter(CurrentCharacter))
            {
                StartCoroutine(CastUniqueCoroutine(tile, damage - 1));
                yield break;
            }
        }
        mHitIds.Clear();

        if (CurrentCharacter.pApCurrent > 0)
        {
            GameManager.pInstance.ChangeState(eGameState.Move);
        }
        else
        {
            yield return new WaitForSeconds(damage == Damage ? 3 : 1);
            GameManager.pInstance.ChangeState(eGameState.End);
        }
    }

    public void ShowUniquePreview(Tile tile)
    {
        tile.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
    }

    public void HideUniquePreview(Tile tile)
    {
        tile.GetComponent<Renderer>().material.SetColor("_Color", tile.Color);
    }
}
