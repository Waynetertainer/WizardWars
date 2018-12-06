/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEngine;

[CreateAssetMenu]
public class AOESpell : ScriptableObject, IUniqueSpell
{
    [SerializeField] private string _SpellName;
    [SerializeField] private int _Damage;
    [SerializeField] private int _Cost;
    [SerializeField] private int _Range;

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

    public void CastUnique(Tile tile)
    {
        if (EntityManager.pInstance.GetCharacterForId(tile.pCharacterId) != null)
            EntityManager.pInstance.GetCharacterForId(tile.pCharacterId).DealDamage(Damage);

        for (int i = 0; i < 6; ++i)
        {
            Tile t = GridManager.pInstance.GetNeighbour(tile, i);
            if (t != null && EntityManager.pInstance.GetCharacterForId(t.pCharacterId) != null)
                EntityManager.pInstance.GetCharacterForId(t.pCharacterId).DealDamage(Damage);
        }
    }

    public void ShowUniquePreview(Tile tile)
    {
        tile.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        for (int i = 0; i < 6; ++i)
        {
            Tile t = GridManager.pInstance.GetNeighbour(tile, i);
            if (t != null)
                t.GetComponent<Renderer>().material.SetColor("_Color", Color.red); ;
        }
    }

    public void HideUniquePreview(Tile tile)
    {
        tile.GetComponent<Renderer>().material.SetColor("_Color", tile.Color);
        for (int i = 0; i < 6; ++i)
        {
            Tile t = GridManager.pInstance.GetNeighbour(tile, i);
            if (t != null)
                t.GetComponent<Renderer>().material.SetColor("_Color", tile.Color); ;
        }
    }
}
