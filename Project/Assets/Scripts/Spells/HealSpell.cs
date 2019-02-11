/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using UnityEngine;

public class HealSpell : MonoBehaviour, IUniqueSpell
{
    [SerializeField] private string _SpellName = "";
    [SerializeField] private int _Damage = 0;
    [SerializeField] private int _Cost = 0;
    [SerializeField] private int _Range = 0;
    [SerializeField] private int _Cooldown;
    [SerializeField] [Multiline] private string _Description = "";
    [SerializeField] private GameObject _VFXPrefab = null;
    [SerializeField] private GameObject _VFXSpawner = null;

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
        var inst = Instantiate(_VFXPrefab, _VFXSpawner.transform);
        inst.transform.LookAt(t.transform.position
                              + new Vector3(0, 1, 0));

        //TODO Change with Grid line
        Vector2Int pos = CurrentCharacter.pTile.pPosition;
        for (int i = 0; i < 5; ++i)
        {
            Tile tile = GridManager.pInstance.GetTileAt(pos.x - i * 2, pos.y);
            if (tile.pCharacterId != -1 && EntityManager.pInstance.GetCharacterForId(tile.pCharacterId).pFaction == eFactions.Player1)
            {
                EntityManager.pInstance.GetCharacterForId(tile.pCharacterId).DealDamage(Damage);
            }
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
        tile.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
    }

    public void HideUniquePreview(Tile tile)
    {
        tile.GetComponent<Renderer>().material.SetColor("_Color", tile.Color);
    }
}