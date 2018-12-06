/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using UnityEngine;

public interface IUniqueSpell
{
    string SpellName { get; }
    void CastUnique(Tile t);
    void ShowUniquePreview(Tile t);
    void HideUniquePreview(Tile t);
}
