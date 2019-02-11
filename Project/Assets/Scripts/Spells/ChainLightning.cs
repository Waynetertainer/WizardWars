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

    [SerializeField] private string _SpellName = "";
    [SerializeField] private int _Damage = 0;
    [SerializeField] private int _Cost = 0;
    [SerializeField] private int _Range = 0;
    [SerializeField] private int _Cooldown = 0;
    [SerializeField] [Multiline] private string _Description = "";
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
    public int Cooldown
    {
        get { return _Cooldown; }
    }
    public string Description
    {
        get { return _Description; }
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
        if (damage != 1)
        {
            if (t.pCharacterId != -1 && t.pCharacterId != EntityManager.pInstance.GetIdForCharacter(CurrentCharacter))
            {
                try
                {
                    var inst = Instantiate(damage == Damage ? _VFXPrefab : _VFXPrefab2, mNextSpawn,
                        Quaternion.identity);
                    inst.transform.LookAt(EntityManager.pInstance.GetCharacterForId(t.pCharacterId).pHitTransform);
                    mHitIds.Add(t.pCharacterId);
                }
                catch
                {
                    Debug.Log("Catched Lightning first block");
                }
                yield return new WaitForSeconds(damage == Damage ? 3 : 0.5f);

                try
                {
                    EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(Damage);
                    mNextSpawn = EntityManager.pInstance.GetCharacterForId(t.pCharacterId).pHitTransform.position;

                    List<Tile> list = GridManager.pInstance.GetVisibleTiles(t, Range);
                    for (int i = 0; i < list.Count; i++)
                    {
                        var tile = list[i];

                        if (tile.pCharacterId != -1 && !mHitIds.Contains(tile.pCharacterId) && tile.pCharacterId !=
                            EntityManager.pInstance.GetIdForCharacter(CurrentCharacter))
                        {
                            StartCoroutine(CastUniqueCoroutine(tile, damage - 1));
                            yield break;
                        }
                    }

                    mHitIds.Clear();
                }
                catch
                {
                    Debug.Log("Oops something went wrong with the chain lightning so lets skip it");
                }
            }
        }
        if (CurrentCharacter.pApCurrent > 5)
        {
            GameManager.pInstance.ChangeState(eGameState.Move);
        }
        else
        {
            yield return new WaitForSeconds(damage == Damage ? 3 : 1);
            GameManager.pInstance.ChangeState(eGameState.EndTurn);
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
