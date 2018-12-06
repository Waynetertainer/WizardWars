/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class HealSpell : IUniqueSpell
{
    public string SpellName { get; private set; }

    public HealSpell()
    {
        SpellName = "Heal";
    }

    public void CastUnique(Tile t)
    {
        if (t.pCharacterId != -1)
        {
            EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(-2);
        }
    }

    public void ShowUniquePreview(Tile tile)
    {
        tile.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        ;
    }

    public void HideUniquePreview(Tile tile)
    {
        tile.GetComponent<Renderer>().material.SetColor("_Color", tile.Color);
        ;
    }
}