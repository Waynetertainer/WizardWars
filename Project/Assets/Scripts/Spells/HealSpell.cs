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
        Debug.Log("Healed");
    }
}
