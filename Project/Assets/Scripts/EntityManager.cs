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
    
    [HideInInspector]
    public List<Tile> pPointsOfInterest = new List<Tile>(); //List of Tiles witch might trigger AIHunt, if in range

    [SerializeField]
    private List<Character> mPlayerPrefabs = new List<Character>();
    [SerializeField]
    private List<Character> mAIPrefabs = new List<Character>();

    private List<Character> mAllEntities = new List<Character>();
    private List<Character> mCurrentEntities = new List<Character>();

    public List<ScriptableObject> DBG_Spells = new List<ScriptableObject>();

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
        for (int i = 0; i < mPlayerPrefabs.Count; i++)
        {
            Character e = Instantiate(mPlayerPrefabs[i], GridManager.pInstance.GetTileAt(playerSpawns[i]).transform.position,
                mPlayerPrefabs[i].transform.rotation);
            mAllEntities.Add(e);
            e.pTile = GridManager.pInstance.GetTileAt(playerSpawns[i]);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            mCurrentEntities.Add(e);
        }

        for (int i = 0; i < mAIPrefabs.Count; i++)
        {
            Character e = Instantiate(mAIPrefabs[i], GridManager.pInstance.GetTileAt(pcSpawns[i]).transform.position,
                mAIPrefabs[i].transform.rotation);
            mAllEntities.Add(e);
            e.pFraction = eFraction.PC;
            e.pTile = GridManager.pInstance.GetTileAt(pcSpawns[i]);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            mCurrentEntities.Add(e);
        }
    }

    public void KillCharacter(Character c)
    {
        Debug.Log(c.pName + " has been killed");
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