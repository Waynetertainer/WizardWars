/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using UnityEngine;

[CreateAssetMenu]
public class DamageSpell : ScriptableObject, IUniqueSpell
{
    [SerializeField]private string _SpellName;
    [SerializeField]private int _Damage;
    [SerializeField]private int _Cost;
    [SerializeField]private int _Range;

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

    public void CastUnique(Tile t)
    {
        if (t.pCharacterId != -1)
        {
            EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(Damage);
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