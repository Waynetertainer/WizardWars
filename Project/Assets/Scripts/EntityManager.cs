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
        return mAllEntities.Values.ToList().FindAll(T => T.pFaction == faction && T.pHpCurrent > 0 && T.pApCurrent > 9);
    }


    /*
    [HideInInspector]
    public List<Tile> pPointsOfInterest = new List<Tile>(); //List of Tiles witch might trigger AIHunt, if in range
    */

    [SerializeField]
    private List<Character> mPlayer1Prefabs = new List<Character>();
    [SerializeField]
    private List<Character> mPlayer2Prefabs = new List<Character>();
    [SerializeField] private Character mAI1Prefab;
    [SerializeField] private Character mAI2Prefab;

    private Dictionary<int, Character> mAllEntities = new Dictionary<int, Character>();
    [HideInInspector] public int mAI1EntityPointer = 0;
    [HideInInspector] public int mAI2EntityPointer = 0;
    private int spawnCounter = 0; // unique counter for entities
    private ePatrouilleSelection mLastAISpawnPatSelection = ePatrouilleSelection.A;
    private int spawnCooldown = 2;
    private int spawnCooldownCurrent = 2;

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


        //player 1 characters
        for (int i = 0; i < mPlayer1Prefabs.Count; i++)
        {
            Tile tile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pPlayer1Spawns[i]);
            Character e = Instantiate(mPlayer1Prefabs[i], tile.transform.position, mPlayer1Prefabs[i].transform.rotation);
            mAllEntities.Add(spawnCounter, e);
            e.pTile = tile;
            e.pTile.pCharacterId = GetIdForCharacter(e);
            e.pApCurrent = e.pAp;
            e.pHpCurrent = e.pHp;
            e.pFaction = eFactions.Player1;
            ++spawnCounter;
        }

        //player 2 characters
        for (int i = 0; i < mPlayer2Prefabs.Count; i++)
        {
            Character e = Instantiate(mPlayer2Prefabs[i], GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pPlayer2Spawns[i]).transform.position, mPlayer2Prefabs[i].transform.rotation);
            mAllEntities.Add(spawnCounter, e);
            e.pTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pPlayer2Spawns[i]);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            e.pApCurrent = e.pAp;
            e.pHpCurrent = e.pHp;
            e.pFaction = eFactions.Player2;
            ++spawnCounter;
        }

        //AI1 characters already on field

        for (int i = 0; i < GridManager.pInstance.pCurrentLevel.pAI1Spawns.Length; ++i)
        {
            Character e = Instantiate(mAI1Prefab, GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1Spawns[i]).transform.position, mAI1Prefab.transform.rotation);
            mAllEntities.Add(spawnCounter, e);
            e.pApCurrent = e.pAp;
            e.pHpCurrent = e.pHp;
            e.pFaction = eFactions.AI1;
            e.pPatrouilleSelection = (i % 2 == 0 ? ePatrouilleSelection.A : ePatrouilleSelection.B);
            e.pTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1Spawns[i]);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            ++spawnCounter;
        }

        //AI2 characters already on field
        for (int i = 0; i < GridManager.pInstance.pCurrentLevel.pAI2Spawns.Length; ++i)
        {
            Character e = Instantiate(mAI2Prefab, GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2Spawns[i]).transform.position, mAI2Prefab.transform.rotation);
            mAllEntities.Add(spawnCounter, e);
            e.pApCurrent = e.pAp;
            e.pHpCurrent = e.pHp;
            e.pFaction = eFactions.AI2;
            e.pPatrouilleSelection = (i % 2 == 0 ? ePatrouilleSelection.A : ePatrouilleSelection.B);
            e.pTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2Spawns[i]);
            e.pTile.pCharacterId = GetIdForCharacter(e);
            ++spawnCounter;
        }
    }

    public void SpawnAI()
    {
        
        if (--spawnCooldownCurrent > 0)
            return;

        // AI1
        Character e;
         e = Instantiate(mAI1Prefab, GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1Spawner[spawnCounter % 2]).transform.position, mAI1Prefab.transform.rotation);
        mAllEntities.Add(spawnCounter, e);
        e.pApCurrent = e.pAp;
        e.pHpCurrent = e.pHp;
        e.pFaction = eFactions.AI1;
        e.pPatrouilleSelection = mLastAISpawnPatSelection;
        e.pTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1Spawner[spawnCounter % 2]);
        e.pTile.pCharacterId = GetIdForCharacter(e);
        ++spawnCounter;

        // AI2
        e = Instantiate(mAI2Prefab, GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2Spawner[(spawnCounter - 1) % 2]).transform.position, mAI2Prefab.transform.rotation);
        mAllEntities.Add(spawnCounter, e);
        e.pApCurrent = e.pAp;
        e.pHpCurrent = e.pHp;
        e.pFaction = eFactions.AI2;
        e.pPatrouilleSelection = mLastAISpawnPatSelection;
        e.pTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2Spawner[(spawnCounter - 1) % 2]);
        e.pTile.pCharacterId = GetIdForCharacter(e);
        ++spawnCounter;

        mLastAISpawnPatSelection = (mLastAISpawnPatSelection == ePatrouilleSelection.A ? ePatrouilleSelection.B : ePatrouilleSelection.A);
        spawnCooldownCurrent = ++spawnCooldown;

    }

    public void KillCharacter(Character c)
    {
        //c.pTile.pBlockType = eBlockType.Empty;
        Debug.Log(c.pName + " has been killed");

        if (c.pFaction == eFactions.AI1 || c.pFaction == eFactions.AI2)
        {
            mAllEntities.Remove(GetIdForCharacter(c));
            Destroy(c.gameObject);
        }
        else
        {
            c.pHpCurrent = 0;
            c.pApCurrent = 0;
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
        character.pApCurrent = 0;

        if (pGetCurrentFactionEntities(character.pFaction).Count == 0)
        {
            foreach (Character chara in pGetAliveFactionEntities(character.pFaction))
            {
                chara.pApCurrent = chara.pAp;
                chara.pMoved = false;
                chara.pFired = false;
                chara.pAura.SetActive(true);
            }
        }
    }
}