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

    //[HideInInspector]
    //public List<Character> pCurrentAI1Players
    //{
    //    get { return mCurrentEntities.FindAll(T => T.pFaction == eFactions.AI1); }
    //}
    //[HideInInspector]
    //public List<Character> pCurrentAI2Players
    //{
    //    get { return mCurrentEntities.FindAll(T => T.pFaction == eFactions.AI2); }
    //}
    //[HideInInspector]
    //public List<Character> pAI1Players
    //{
    //    get { return mAllEntities.Values.ToList().FindAll(T => T.pFaction == eFactions.AI1); }
    //}
    //[HideInInspector]
    //public List<Character> pAI2Players
    //{
    //    get { return mAllEntities.Values.ToList().FindAll(T => T.pFaction == eFactions.AI2); }
    //}
    public List<Character> pGetFactionEntities(eFactions faction)
    {
        return mAllEntities.Values.ToList().FindAll(T => T.pFaction == faction);
    }
    public List<Character> pGetAliveFactionEntities(eFactions faction)
    {
        return mAllEntities.Values.ToList().FindAll(T => T.pFaction == faction && T.pHpCurrent > 0);
    }
    public List<Character> pGetCurrentFactionEntities(eFactions faction)
    {
        return mCurrentEntities.FindAll(T => T.pFaction == faction);
    }
    [HideInInspector]
    public List<Character> pCurrentPlayer1Players
    {
        get { return mCurrentEntities.FindAll(T => T.pFaction == eFactions.Player1); }
    }
    [HideInInspector]
    public List<Character> pCurrentPlayer2Players
    {
        get { return mCurrentEntities.FindAll(T => T.pFaction == eFactions.Player2); }
    }
    public List<Character> pPlayer1Players
    {
        get { return mAllEntities.Values.ToList().FindAll(T => T.pFaction == eFactions.Player1); }
    }
    public List<Character> pPlayer2Players
    {
        get { return mAllEntities.Values.ToList().FindAll(T => T.pFaction == eFactions.Player2); }
    }

    /*
    [HideInInspector]
    public List<Tile> pPointsOfInterest = new List<Tile>(); //List of Tiles witch might trigger AIHunt, if in range
    */

    [SerializeField]
    private List<Character> mPlayer1Prefabs = new List<Character>();
    [SerializeField]
    private List<Character> mPlayer2Prefabs = new List<Character>();
    [SerializeField] private Character mAIPrefab;

    private Dictionary<int, Character> mAllEntities = new Dictionary<int, Character>();
    private List<Character> mCurrentEntities = new List<Character>();
    [HideInInspector] public int mAI1EntityPointer = 0;
    [HideInInspector] public int mAI2EntityPointer = 0;

    public List<ScriptableObject> DBG_Spells = new List<ScriptableObject>();

    private void Awake()
    {
        if (pInstance == null)

            pInstance = this;

        else if (pInstance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void SpawnCharacters()
    {
        int spawnCounter = 0;

        //player 1 characters
        for (int i = 0; i < mPlayer1Prefabs.Count; i++)
        {
            Tile tile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pPlayer1Spawns[i]);
            Character e = Instantiate(mPlayer1Prefabs[i], tile.transform.position, mPlayer1Prefabs[i].transform.rotation);
            mAllEntities.Add(spawnCounter, e);
            e.pTile = tile;
            e.pTile.pCharacterId = GetIdForCharacter(e);
            e.pFaction = eFactions.Player1;
            //e.pTile.pBlockType = eBlockType.Blocked;
            mCurrentEntities.Add(e);
            ++spawnCounter;
        }

        //player 2 characters
        for (int i = 0; i < mPlayer2Prefabs.Count; i++)
        {
            Character e = Instantiate(mPlayer2Prefabs[i], GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pPlayer2Spawns[i]).transform.position, mPlayer2Prefabs[i].transform.rotation);
            mAllEntities.Add(spawnCounter, e);
            e.pTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pPlayer2Spawns[i]);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            e.pFaction = eFactions.Player2;
            mCurrentEntities.Add(e);
            ++spawnCounter;
        }

        //AI1 characters already on field

        for (int i = 0; i < GridManager.pInstance.pCurrentLevel.pAI1Spawns.Length; ++i)
        {
            Character e = Instantiate(mAIPrefab, GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1Spawns[i]).transform.position, mAIPrefab.transform.rotation);
            mAllEntities.Add(spawnCounter, e);
            e.pFaction = eFactions.AI1;
            e.pPatrouilleSelection = (i % 2 == 0 ? ePatrouilleSelection.A : ePatrouilleSelection.B);
            e.pTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1Spawns[i]);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            mCurrentEntities.Add(e);
            ++spawnCounter;
        }

        //AI2 characters already on field
        for (int i = 0; i < GridManager.pInstance.pCurrentLevel.pAI2Spawns.Length; ++i)
        {
            Character e = Instantiate(mAIPrefab, GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2Spawns[i]).transform.position,
                mAIPrefab.transform.rotation);
            mAllEntities.Add(spawnCounter, e);
            e.pFaction = eFactions.AI2;
            e.pPatrouilleSelection = (i % 2 == 0 ? ePatrouilleSelection.A : ePatrouilleSelection.B);
            e.pTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2Spawns[i]);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            mCurrentEntities.Add(e);
            ++spawnCounter;
        }
    }

    public void KillCharacter(Character c)
    {
        //c.pTile.pBlockType = eBlockType.Empty;
        Debug.Log(c.pName + " has been killed");

        if (c.pFaction == eFactions.AI1 || c.pFaction == eFactions.AI2)
        {
            mAllEntities.Remove(GetIdForCharacter(c));
            mCurrentEntities.Remove(c);
            Destroy(c.gameObject);
        }
        else
        {
            mCurrentEntities.Remove(c);
            c.gameObject.SetActive(false);
            if (pGetAliveFactionEntities(c.pFaction).Count == 0) //killed playerfaction
            {
                GameManager.pInstance.ChangeState(eGameState.EndOfMatch);
            }
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

        if (mCurrentEntities.Find(T => T.pFaction == character.pFaction) == null)
            ResetCharacters(character.pFaction);
    }

    private void ResetCharacters(eFactions fraction)
    {
        foreach (var e in mAllEntities.Values.ToList().FindAll(T => T.pFaction == fraction))
        {
            e.pMoved = false;
            e.pFired = false;
            e.pAura.SetActive(true);
            mCurrentEntities.Add(e);
        }
    }
}