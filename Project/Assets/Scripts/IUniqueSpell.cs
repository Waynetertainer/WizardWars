/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using UnityEngine;

public interface IUniqueSpell
{
    string SpellName { get; }
    int Damage { get; }
    int Cost { get; }
    int Range { get; }

    Character CurrentCharacter { get; }
    GameObject VFXPrefab { get; }
    GameObject VFXSpawner { get; }


    void CastUnique(Tile t);
    void ShowUniquePreview(Tile t);
    void HideUniquePreview(Tile t);
}
