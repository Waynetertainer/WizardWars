/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using UnityEngine;

[CreateAssetMenu]
public class DamageSpell : MonoBehaviour, IUniqueSpell
{
    [SerializeField] private string _SpellName;
    [SerializeField] private int _Damage;
    [SerializeField] private int _Cost;
    [SerializeField] private int _Range;
    [SerializeField] private int _Cooldown;
    [SerializeField] [Multiline] private string _Description;
    [SerializeField] private GameObject _VFXPrefab;
    [SerializeField] private GameObject _VFXSpawner;

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
        if (t.pCharacterId != -1)
        {
            EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(Damage);
        }
        if (CurrentCharacter.pApCurrent > 5)
        {
            GameManager.pInstance.ChangeState(eGameState.Move);
        }
        else
        {
            GameManager.pInstance.ChangeState(eGameState.EndTurn);
        }
    }

    public void ShowUniquePreview(Tile tile)
    {
        tile.GetComponent<Renderer>().material.SetColor("_Color", Color.magenta);
    }

    public void HideUniquePreview(Tile tile)
    {
        tile.GetComponent<Renderer>().material.SetColor("_Color", tile.Color);
    }
}