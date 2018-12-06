/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ChainLightning : ScriptableObject, IUniqueSpell
{
    [SerializeField] private string _SpellName;
    [SerializeField] private int _Damage;
    [SerializeField] private int _Cost;
    [SerializeField] private int _Range;

    private List<int> mHitIds = new List<int>();

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
            mHitIds.Add(t.pCharacterId);
        }

        List<Tile> list = GridManager.pInstance.GetVisibleTiles(t, Range);
        for (int i = 0; i < list.Count; i++)
        {
            var tile = list[i];

            if (tile.pCharacterId != -1 && !mHitIds.Contains(tile.pCharacterId))
            {
                CastUnique(tile);
                return;
            }
        }

        mHitIds.Clear();
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
