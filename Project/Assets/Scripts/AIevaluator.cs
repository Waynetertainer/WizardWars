using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIevaluator
{

    public static IEnumerator EvaluateAI(Character mCharacter)
    {
        Debug.Log("Starte AI für " + mCharacter.pName + " Faction:" + mCharacter.pFaction + " Pat:" + mCharacter.pPatrouilleSelection);
        //mCharacter.pApCurrent = mCharacter.pAp;
        eAIState pAIState = eAIState.Patrouille;
        Character pActiveTarget = null;

        List<Tile> pSteps = new List<Tile>();

        #region AI v2
        while (mCharacter.pApCurrent > 9)
        {
            Debug.Log("AI current AP " + mCharacter.pApCurrent);
            yield return new WaitForSeconds(0.5f); // blocks debug stepping. Remove if nessesary
            switch (pAIState)
            {
                case eAIState.Patrouille:

                    #region Patrouille
                    if (mCharacter.pPatrouilleSelection == ePatrouilleSelection.Static) // wenn statischer gegner ohne wegpunkte, early exit
                    {
                        pActiveTarget = AIfindTarget(mCharacter);
                        if (pActiveTarget != null)
                        {
                            pAIState = eAIState.Fight;
                            Debug.Log("AI switching from patrol to fight mode");
                            break;
                        }
                        else
                        {
                            mCharacter.pApCurrent = 0;
                            Debug.Log("AI Nothing to do, skipping turn");
                            break;
                        }
                    }
                    

                    Tile patrolTile;
                    if (mCharacter.pFaction == eFactions.AI1)
                    {
                        if (mCharacter.pPatrouilleSelection == ePatrouilleSelection.A)
                            patrolTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1PatrouilleA[mCharacter.mPatWaypointID]);
                        else
                            patrolTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1PatrouilleB[mCharacter.mPatWaypointID]);
                    }
                    else
                    {
                        if (mCharacter.pPatrouilleSelection == ePatrouilleSelection.A)
                            patrolTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2PatrouilleA[mCharacter.mPatWaypointID]);
                        else
                            patrolTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2PatrouilleB[mCharacter.mPatWaypointID]);
                    }


                    pSteps = GridManager.pInstance.GetPathTo(mCharacter.pTile, patrolTile);
                    if (pSteps == null) break;
                    int currentStepsLeft = mCharacter.pWalkRange;

                    if (pSteps.Count < 3) //close enough to pat-point?
                    {
                        Debug.Log("AI patrol to next waypoint");
                        mCharacter.mPatWaypointID = (mCharacter.mPatWaypointID + 1) % (mCharacter.pPatrouilleSelection == ePatrouilleSelection.A ? GridManager.pInstance.pCurrentLevel.pAI1PatrouilleA.Length : GridManager.pInstance.pCurrentLevel.pAI1PatrouilleB.Length);
                        if (mCharacter.pFaction == eFactions.AI1)
                        {
                            if (mCharacter.pPatrouilleSelection == ePatrouilleSelection.A)
                                patrolTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1PatrouilleA[mCharacter.mPatWaypointID]);
                            else
                                patrolTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI1PatrouilleB[mCharacter.mPatWaypointID]);
                        }
                        else
                        {
                            if (mCharacter.pPatrouilleSelection == ePatrouilleSelection.A)
                                patrolTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2PatrouilleA[mCharacter.mPatWaypointID]);
                            else
                                patrolTile = GridManager.pInstance.GetTileAt(GridManager.pInstance.pCurrentLevel.pAI2PatrouilleB[mCharacter.mPatWaypointID]);
                        }

                        pSteps = GridManager.pInstance.GetPathTo(mCharacter.pTile, patrolTile);
                    }

                    if (pSteps.Count > mCharacter.pWalkRange)
                    {
                        yield return mCharacter.MoveEnumerator(pSteps[pSteps.Count - mCharacter.pWalkRange]);
                    }
                    else
                    {
                        yield return mCharacter.MoveEnumerator(pSteps[0]);
                    }


                    pActiveTarget = AIfindTarget(mCharacter);
                    if (pActiveTarget != null)
                        pAIState = eAIState.Fight;
                    #endregion
                    break;
                case eAIState.Fight:
                    #region Fight
                    Debug.Log(mCharacter.pName + " is searching for cover against " + pActiveTarget.pName);

                    List<Tile> walkableTiles = GridManager.pInstance.GetReachableTiles(mCharacter.pTile, mCharacter.pWalkRange);
                    //remove tiles with players on it
                    for (int i = walkableTiles.Count - 1; i > 0; --i)
                    {
                        if (walkableTiles[i].pCharacterId != -1)
                            walkableTiles.Remove(walkableTiles[i]);
                    }
                    eBlockType currentCoverTarget = eBlockType.Empty;

                    //Creating lists with all options to choose from

                    List<Tile> coverPositionsFull = new List<Tile>();
                    List<Tile> coverPositionsHalf = new List<Tile>();
                    foreach (Tile walkableTile in walkableTiles)
                    {
                        if (GridManager.pInstance.GetVisibilityToTarget(mCharacter.pTile, pActiveTarget.pTile, mCharacter.pVisionRange) == eVisibility.Seethrough
                            && walkableTile.pCharacterId == -1) // now with occupied tiles
                        {
                            switch (GridManager.pInstance.GetCoverFromTarget(mCharacter.pTile, pActiveTarget.pTile))
                            {
                                case eBlockType.Blocked:
                                    if (currentCoverTarget != eBlockType.HalfBlocked)
                                        currentCoverTarget = eBlockType.Blocked;
                                    coverPositionsFull.Add(walkableTile);
                                    break;
                                case eBlockType.HalfBlocked:
                                    currentCoverTarget = eBlockType.HalfBlocked;
                                    coverPositionsHalf.Add(walkableTile);
                                    break;
                            }
                        }
                    }

                    // sort the choosen options by distance. Switch just for avoiding unnessesary sorting of tiles that are not suitable

                    List<Tile> wayToTarget = new List<Tile>();
                    switch (currentCoverTarget)
                    {
                        case eBlockType.Empty:
                            wayToTarget = GridManager.pInstance.GetPathTo(pActiveTarget.pTile, mCharacter.pTile);
                            break;
                        case eBlockType.Blocked:
                            coverPositionsFull.Sort((a, b) => Tile.Distance(a, pActiveTarget.pTile).CompareTo(Tile.Distance(b, pActiveTarget.pTile)));
                            wayToTarget = GridManager.pInstance.GetPathTo(mCharacter.pTile, coverPositionsFull[0]);
                            break;
                        case eBlockType.HalfBlocked:
                            coverPositionsHalf.Sort((a, b) => Tile.Distance(a, pActiveTarget.pTile).CompareTo(Tile.Distance(b, pActiveTarget.pTile)));
                            wayToTarget = GridManager.pInstance.GetPathTo(mCharacter.pTile, coverPositionsHalf[0]);
                            break;
                        default:
                            break;
                    }

                    /*
                    // if we are going to get cover
                    if (currentCoverTarget != eBlockType.Empty && wayToTarget.Count <= mCharacter.pApCurrent / mCharacter.pWalkCost && wayToTarget.Count > 1)
                    {
                        Debug.Log("Move to cover because it seems reachable");
                        for (int i = 0; i < (wayToTarget.Count > mCharacter.pWalkRange ? mCharacter.pWalkRange : wayToTarget.Count); ++i) // walk until range is spent or target reached
                        {
                            yield return AIevaluator.AImove(mCharacter, pSteps[1]);
                        }
                        mCharacter.pApCurrent -= 10;
                        break;
                    }
                    */
                    // if we need to get in range and no cover is present
                    if (currentCoverTarget == eBlockType.Empty && wayToTarget.Count > mCharacter.Range)
                    {
                        // weglist durchgehen und einen punkt wählen der dicht genug für schuss ist und nicht blockiert ist
                        int possibleFinalWaypoint = 0;
                        for (int wayPointCounter = wayToTarget.Count - 1; wayPointCounter > 0; --wayPointCounter)
                        {
                            if (wayToTarget[wayPointCounter].pCharacterId == -1 && wayToTarget.Count - wayPointCounter - 1 <= mCharacter.Range)
                            {
                                possibleFinalWaypoint = wayPointCounter;
                            }
                            else if (wayToTarget[wayPointCounter].pCharacterId == -1 && wayToTarget.Count - wayPointCounter - 1 > mCharacter.Range)
                            {
                                possibleFinalWaypoint = wayPointCounter;
                                break;
                            }
                        }

                        Debug.Log("Move to Enemy because out of range, possible " + wayToTarget.Count.ToString() + " steps");
                        /*
                        for (int stepCount = 1; stepCount <= possibleFinalWaypoint; ++stepCount)
                        {
                            yield return AIevaluator.AImove(mCharacter, wayToTarget[stepCount]);
                        }
                        */

                        //informing Grid about changes
                        mCharacter.pTile.pCharacterId = -1;
                        wayToTarget[possibleFinalWaypoint].pCharacterId = EntityManager.pInstance.GetIdForCharacter(mCharacter);
                        mCharacter.pTile = wayToTarget[possibleFinalWaypoint];

                        // substract cost for step
                        mCharacter.pApCurrent -= 10;

                        /*
                        if (wayToTarget[1].pCharacterId != -1 && wayToTarget.Count >1) //targettile is blocked by another player.
                        {
                            yield return AIevaluator.AImove(mCharacter, wayToTarget[1]);
                            yield return AIevaluator.AImove(mCharacter, wayToTarget[2]); //HACK: step over target. Needs loop
                        }
                        else
                        {
                            yield return AIevaluator.AImove(mCharacter, wayToTarget[1]);
                        }
                        */
                        break;
                    }

                    if (mCharacter.pApCurrent < 15) // not enough AP to shoot
                    {
                        Debug.Log("No shots left, skipping turn");
                        mCharacter.pApCurrent = 0;
                        break;
                    }
                    Debug.Log("AI shooting Normal spell at " + pActiveTarget.pName);
                    mCharacter.StandardAttack(pActiveTarget.pTile);// shoot normal spell at target

                    pActiveTarget = AIfindTarget(mCharacter); //check for survivors
                    if (pActiveTarget == null)
                        pAIState = eAIState.Patrouille;
                    #endregion
                    break;
                default:
                    Debug.LogError("AI in unknown state");
                    break;
            }
        }// loop if char has still points to use

        #endregion

        // end AI turn
        Debug.Log("AI spent all AP, end turn");
        yield return new WaitForSeconds(1);
        mCharacter.pAura.SetActive(false);
        EntityManager.pInstance.EndRound(mCharacter);

        //ending AI turn and switch to next player
        GameManager.pInstance.pCurrentFaction = GameManager.pInstance.pCurrentFaction == eFactions.AI1 ? eFactions.Player1 : eFactions.Player2;
        //EntityManager.pInstance.pGetFactionEntities(pCurrentFraction)[0].Select(); // move camera to an faction entity
        GameManager.pInstance.ChangeState(eGameState.Select);

    }



    /// <summary>
    /// Searches for new targets for the AI and returns a visible Target or null.
    /// </summary>
    /// <param name="mCharacter">AI-Character to process</param>
    /// <returns>Returns the closest visible player character or null.</returns>
    public static Character AIfindTarget(Character mCharacter)
    {
        Debug.Log(mCharacter.pName + " is searching for targets ");
        List<Character> visibleCharaters = new List<Character>();

        //find visible and revealed players
        if (mCharacter.pFaction == eFactions.AI1)
        {
            foreach (var playerChar in EntityManager.pInstance.pGetAliveFactionEntities(eFactions.Player2))
            {
                if (GridManager.pInstance.GetVisibilityToTarget(mCharacter.pTile, playerChar.pTile, mCharacter.pVisionRange) == eVisibility.Seethrough) // character visible
                {
                    visibleCharaters.Add(playerChar);
                }
            }
            foreach (var playerChar in EntityManager.pInstance.pGetFactionEntities(eFactions.AI2))
            {
                if (GridManager.pInstance.GetVisibilityToTarget(mCharacter.pTile, playerChar.pTile, mCharacter.pVisionRange) == eVisibility.Seethrough) // character visible
                {
                    visibleCharaters.Add(playerChar);
                }
            }
        }
        else // faction AI2
        {
            foreach (var playerChar in EntityManager.pInstance.pGetAliveFactionEntities(eFactions.Player1))
            {
                if (GridManager.pInstance.GetVisibilityToTarget(mCharacter.pTile, playerChar.pTile, mCharacter.pVisionRange) == eVisibility.Seethrough) // character visible
                {
                    visibleCharaters.Add(playerChar);
                }
            }
            foreach (var playerChar in EntityManager.pInstance.pGetFactionEntities(eFactions.AI1))
            {
                if (GridManager.pInstance.GetVisibilityToTarget(mCharacter.pTile, playerChar.pTile, mCharacter.pVisionRange) == eVisibility.Seethrough) // character visible
                {
                    visibleCharaters.Add(playerChar);
                }
            }

        }


        // find closest enemy

        int targetDistance = int.MaxValue;
        Character returnCharacter = null;
        foreach (Character playerChar in visibleCharaters)
        {
            int currentDistance = Tile.Distance(mCharacter.pTile, playerChar.pTile);
            if (currentDistance < targetDistance)
            {
                targetDistance = currentDistance;
                returnCharacter = playerChar;
            }
        }

        if (returnCharacter != null)
        {

            Debug.Log("Found Enemy " + returnCharacter.pName + " at distance " + targetDistance);
        }
        else
        {
            Debug.Log("Found no Enemy");
        }
        return returnCharacter;
    }

    /// <summary>
    /// Moves the Player to the target tile. Use for one single Step only. No pathfinding!
    /// !Does not Set Tile occupation Automatically!
    /// </summary>
    /// <param name="mCharacter">AI Character to move</param>
    /// <param name="targetTile">Adjenct Tile to place the character on</param>
    /// <returns></returns>
    /*
    public static IEnumerator AImove(Character mCharacter, Tile targetTile)
    {

        //moving the object
        mCharacter.transform.LookAt(targetTile.transform.position);
        mCharacter.transform.localEulerAngles = new Vector3(-90, mCharacter.transform.localEulerAngles.y, 0);
        while (Vector3.Distance(mCharacter.transform.position, targetTile.transform.position) >= 0.1f)
        {
            mCharacter.transform.position = Vector3.Lerp(mCharacter.transform.position, targetTile.transform.position, Time.deltaTime * 5);
            yield return new WaitForEndOfFrame(); ;
        }

        yield return null;
    }*/
}