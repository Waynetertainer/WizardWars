/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEngine;

public class AOE : IUniqueSpell
{
    public string SpellName { get; private set; }

    public AOE()
    {
        SpellName = "AOE";
    }
    public void CastUnique(Tile tile)
    {
        if (EntityManager.pInstance.GetCharacterForId(tile.pCharacterId) != null)
            EntityManager.pInstance.GetCharacterForId(tile.pCharacterId).DealDamage(5);

        for (int i = 0; i < 6; ++i)
        {
            Tile t = GridManager.pInstance.GetNeighbour(tile, i);
            if (t != null && EntityManager.pInstance.GetCharacterForId(t.pCharacterId) != null)
                EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(5);
        }
    }
}
