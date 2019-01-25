/*
* Copyright (c) Jannik Lietz
* www.janniklietz.wordpress.com
*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager pInstance = null;

    [HideInInspector] public List<Character> pCurrentAI1Players
    {
        get { return mCurrentEntities.FindAll(T => T.pFraction == eFraction.AI1); }
    }
    [HideInInspector] public List<Character> pCurrentAI2Players
    {
        get { return mCurrentEntities.FindAll(T => T.pFraction == eFraction.AI2); }
    }
    [HideInInspector] public List<Character> pAI1Players
    {
        get { return mAllEntities.Values.ToList().FindAll(T => T.pFraction == eFraction.AI1); }
    }
    [HideInInspector] public List<Character> pAI2Players
    {
        get { return mAllEntities.Values.ToList().FindAll(T => T.pFraction == eFraction.AI2); }
    }

    [HideInInspector] public List<Character> pCurrentPlayer1Players
    {
        get { return mCurrentEntities.FindAll(T => T.pFraction == eFraction.Player1); }
    }
    [HideInInspector]public List<Character> pCurrentPlayer2Players
    {
        get { return mCurrentEntities.FindAll(T => T.pFraction == eFraction.Player2); }
    }
    public List<Character> pPlayer1Players
    {
        get { return mAllEntities.Values.ToList().FindAll(T => T.pFraction == eFraction.Player1); }
    }
    public List<Character> pPlayer2Players
    {
        get { return mAllEntities.Values.ToList().FindAll(T => T.pFraction == eFraction.Player2); }
    }

    /*
    [HideInInspector]
    public List<Tile> pPointsOfInterest = new List<Tile>(); //List of Tiles witch might trigger AIHunt, if in range
    */

    [SerializeField]
    private List<Character> mPlayerPrefabs = new List<Character>();
    [SerializeField]
    private List<Character> mAIPrefabs = new List<Character>();

    private Dictionary<int, Character> mAllEntities = new Dictionary<int, Character>();
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
            mAllEntities.Add(i, e);
            e.pTile = GridManager.pInstance.GetTileAt(playerSpawns[i]);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            //e.pTile.pBlockType = eBlockType.Blocked;
            mCurrentEntities.Add(e);
        }

        for (int i = 0; i < mAIPrefabs.Count; i++)
        {
            Character e = Instantiate(mAIPrefabs[i], GridManager.pInstance.GetTileAt(pcSpawns[i]).transform.position,
                mAIPrefabs[i].transform.rotation);
            mAllEntities.Add(i + mPlayerPrefabs.Count, e);
            e.pFraction = eFraction.AI1; //TODO auch für gegner AI spawnen!
            e.pTile = GridManager.pInstance.GetTileAt(pcSpawns[i]);
            e.pTile.pCharacterId = i + mPlayerPrefabs.Count;
            //e.pTile.pBlockType = eBlockType.Blocked;
            mCurrentEntities.Add(e);
        }
    }

    public void KillCharacter(Character c)
    {
        //c.pTile.pBlockType = eBlockType.Empty;
        Debug.Log(c.pName + " has been killed");
        mAllEntities.Remove(GetIdForCharacter(c));
        mCurrentEntities.Remove(c);
        Destroy(c.gameObject);

        if (pCurrentPlayer1Players.Count == 0 || pCurrentPlayer2Players.Count == 0)
        {
            GameManager.pInstance.ChangeState(eGameState.EndOfMatch);
        }
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
        int index = mAllEntities.Values.ToList().IndexOf(character);
        return mAllEntities.ElementAt(index).Key;
    }

    public void EndRound(Character character)
    {
        mCurrentEntities.Remove(character);

        if (mCurrentEntities.Find(T => T.pFraction == character.pFraction) == null)
            ResetCharacters(character.pFraction);
    }

    private void ResetCharacters(eFraction fraction)
    {
        foreach (var e in mAllEntities.Values.ToList().FindAll(T => T.pFraction == fraction))
        {
            e.pMoved = false;
            e.pFired = false;
            e.pAura.SetActive(true);
            mCurrentEntities.Add(e);
        }
    }
}