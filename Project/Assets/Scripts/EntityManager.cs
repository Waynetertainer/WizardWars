/*
* Copyright (c) Jannik Lietz
* www.janniklietz.wordpress.com
*/

using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager pInstance = null;

    public List<Character> pCurrentPCPlayers
    {
        get { return mCurrentEntities.FindAll(T => T.pFraction == eFraction.PC); }
    }
    public List<Character> pPCPlayers
    {
        get { return mAllEntities.FindAll(T => T.pFraction == eFraction.PC); }
    }
    public List<Character> pCurrentPlayers
    {
        get { return mCurrentEntities.FindAll(T => T.pFraction == eFraction.Player); }
    }
    public List<Character> pPlayers
    {
        get { return mAllEntities.FindAll(T => T.pFraction == eFraction.Player); }
    }

    [SerializeField]
    private Character mPlayerPrefab;
    [SerializeField]
    private Character mAIPrefab;

    private List<Character> mAllEntities = new List<Character>();
    private List<Character> mCurrentEntities = new List<Character>();

    private void Awake()
    {
        if (pInstance == null)

            pInstance = this;

        else if (pInstance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void SpawnCharacters(Vector2Int[] playerSpawns, Vector2Int[] pcSpawns)
    {
        int x = 0;
        for (int i = 0; i < 2; i++)
        {
            Character e = Character.CreateCharacter(eFraction.Player, GridManager.pInstance.GetTileAt(playerSpawns[x]), mPlayerPrefab, new AOE());
            mAllEntities.Add(e);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            mCurrentEntities.Add(e);
            x++;
        }
        Character c = Character.CreateCharacter(eFraction.Player, GridManager.pInstance.GetTileAt(playerSpawns[x]), mPlayerPrefab, new HealSpell());
        mAllEntities.Add(c);
        c.pTile.pCharacterId = GetIdForCharacter(c);
        mCurrentEntities.Add(c);
        x++;

        x = 0;
        for (int i = 0; i < 3; i++)
        {
            Character e = Character.CreateCharacter(eFraction.PC, GridManager.pInstance.GetTileAt(pcSpawns[x]), mAIPrefab, new HealSpell());
            mAllEntities.Add(e);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            mCurrentEntities.Add(e);
            x++;
        }
    }

    public void KillCharacter(Character c)
    {
        mAllEntities.Remove(c);
        mCurrentEntities.Remove(c);
        Destroy(c.gameObject);
    }

    public Character GetCharacterForId(int id)
    {
        try
        {
            return mAllEntities[id];
        }
        catch
        {
            return null;
        }
    }

    public int GetIdForCharacter(Character character)
    {
        return mAllEntities.IndexOf(character);
    }

    public void EndRound(Character character)
    {
        mCurrentEntities.Remove(character);

        if (mCurrentEntities.Find(T => T.pFraction == character.pFraction) == null)
            ResetCharacters(character.pFraction);
    }

    private void ResetCharacters(eFraction fraction)
    {
        foreach (var e in mAllEntities.FindAll(T => T.pFraction == fraction))
        {
            e.pMoved = false;
            e.pFired = false;
            mCurrentEntities.Add(e);
        }
    }
}