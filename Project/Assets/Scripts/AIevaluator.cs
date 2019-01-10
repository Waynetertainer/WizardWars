﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIevaluator
{

    public static IEnumerator EvaluateAI(Character mCharacter)
    {
        Debug.Log("Starte AI für " + mCharacter.pName);
        mCharacter.pApCurrent = mCharacter.pAp;
        eAIState pAIState = eAIState.Patrouille;
        Character pActiveTarget = null;
        //int mCoverRoundCount = 0; //TODO: Implement Cover Count
        int shotsLeft = 2; // ai is only allowed to shoot twice

        List<Tile> pSteps = new List<Tile>();


        #region AI v1
        /*

        if (character.pCurrentAp > character.pHp * 0.25 || mCoverRoundCount > 0) //wenn in deckung war oder genug HP hat
        {
            if (true) // Ist ein Ziel sichtbar?
            {
                if (true) //Bin ichein Runenmagier und hab noch AP für ultimate übrig?
                {
                    //ziele in reihenfolge anvisieren: Support, Damage, Tank

                }

                while (character.pApCurrent >= character._Cost) // Solange AP vorhanden sind -> Ballern
                {
                    //ziele in reihenfolge anvisieren: Support, Damage, Tank
                }

            }
        }
        else // character hat wenig leben und ist nicht in deckung
        {
            if (true) //TODO: Gegner vorhanden und Deckung vor dem gegner vorhanden?
            {
                if (true) //TODO: Halbe deckung bevorzugen?!
                {
                    //zur halben deckung gehen
                }
                else // wenn keine halbe, dann in volle deckung begeben
                {
                    //zur vollen deckung gehen
                }

                if (true) //Bin ichein Runenmagier und hab noch AP für ultimate übrig?
                {
                    //ziele in reihenfolge anvisieren: Support, Damage, Tank

                }

                while (character.pApCurrent >= character._Cost) // Solange AP vorhanden sind -> Ballern
                {
                    //ziele in reihenfolge anvisieren: Support, Damage, Tank
                }


            }
            else //TODO: wenn keine deckung vorhanden und kein gegner
            {
                //richtung support laufen für heilung?
            }

        }*/
        #endregion

        #region AI v2
        while (mCharacter.pApCurrent > 0) //TODO: #2 do stuff until AP are spend, possible infinite loop if char is in position but shot is too expensive
        {
            Debug.Log("AI current AP " + mCharacter.pApCurrent);
            yield return new WaitForSeconds(0.5f); // blocks debug stepping. Remove in nessesary
            switch (pAIState)
            {
                case eAIState.Patrouille:
                    if (mCharacter.pAIPatrouillePoints.Count == 0) // wenn statischer gegner ohne wegpunkte
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
                    Debug.Log("AI patrol to next waypoint");
                    pSteps = GridManager.pInstance.GetPathTo(mCharacter.pTile, mCharacter.pAIPatrouillePoints[mCharacter.mPatWaypointID]);

                    if (pSteps.Count == 0) //wenn pat-zielpunkt erreicht zum nächsten wechseln
                    {
                        mCharacter.mPatWaypointID = (mCharacter.mPatWaypointID + 1) % mCharacter.pAIPatrouillePoints.Count;
                        pSteps = GridManager.pInstance.GetPathTo(mCharacter.pTile, mCharacter.pAIPatrouillePoints[mCharacter.mPatWaypointID]);
                    }

                    yield return AIevaluator.AImove(mCharacter, pSteps[1]); // make one single step and check for targets
                    pActiveTarget = AIfindTarget(mCharacter);
                    if (pActiveTarget != null)
                        pAIState = eAIState.Fight;

                    break;
                case eAIState.Fight:

                    Debug.Log(mCharacter.pName + " is searching for cover against " + pActiveTarget.pName);

                    List<Tile> walkableTiles = GridManager.pInstance.GetReachableTiles(mCharacter.pTile, mCharacter.pApCurrent / mCharacter.pWalkCost);
                    eBlockType currentCoverTarget = eBlockType.Empty;

                    //Creating lists with all options to choose from

                    List<Tile> coverPositionsFull = new List<Tile>();
                    List<Tile> coverPositionsHalf = new List<Tile>();
                    foreach (Tile walkableTile in walkableTiles)
                    {
                        if (GridManager.pInstance.GetVisibilityToTarget(mCharacter.pTile, pActiveTarget.pTile, mCharacter.pVisionRange) == eVisibility.Seethrough)
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

                    // if we are going to get cover
                    if (currentCoverTarget != eBlockType.Empty && wayToTarget.Count <= mCharacter.pApCurrent / mCharacter.pWalkCost && wayToTarget.Count > 1)
                    {
                        Debug.Log("Move to cover becaouse it seems reachable");
                        yield return AIevaluator.AImove(mCharacter, wayToTarget[1]);
                        break;
                    }

                    // if we need to get in range
                    if (currentCoverTarget == eBlockType.Empty && wayToTarget.Count > mCharacter.Range)
                    {
                        Debug.Log("Move to Enemy because out of range");
                        yield return AIevaluator.AImove(mCharacter, wayToTarget[1]);
                        break;
                    }



                    /*  //HACK: commented out because no special for AI is implemented
                    if (mCharacter.pApCurrent > mCharacter.pUniqueSpell.Cost) //TODO: #1 Special cooldown? Sanity check?
                    {
                        mCharacter.CastUnique(pActiveTarget.pTile); //fire signature spell against active target
                        Debug.Log("AI shooting Unique spell at " + pActiveTarget.pName + " for " + mCharacter.pUniqueSpell.Cost.ToString());

                        pActiveTarget = AIfindTarget(mCharacter); // check if target survived
                        if (pActiveTarget == null)
                            pAIState = eAIState.Patrouille;
                        break;
                    }
                    */
                    if (shotsLeft == 0)
                    {
                        Debug.Log("No shots left, skipping turn");
                        mCharacter.pApCurrent = 0;
                        break;
                    }
                    if (mCharacter.pApCurrent >= mCharacter.Cost)
                    {
                        Debug.Log("AI shooting Normal spell at " + pActiveTarget.pName + " for " + mCharacter.Cost.ToString());
                        mCharacter.StandardAttack(pActiveTarget.pTile);// shoot normal spell at target
                        --shotsLeft;

                        pActiveTarget = AIfindTarget(mCharacter); //check for survivors
                        if (pActiveTarget == null)
                            pAIState = eAIState.Patrouille;
                    }
                    else // needed to avoid inifite loop with one AP while engaged in fight with enemy
                    {
                        Debug.Log("Not enough AP to fight enemy, skipping");
                        mCharacter.pApCurrent = 0;
                    }

                    break;
                case eAIState.Hunt:

                    /*
                    pActiveTarget = AIfindTarget(mCharacter);
                    if (pActiveTarget != null) // did we find a target on the way to the point of interest?
                    {
                        pAIState = eAIState.Fight;
                        break;
                    }

                    if (EntityManager.pInstance.pPointsOfInterest.Count == 0) //early exit if nothing left to hunt
                    {
                        pAIState = eAIState.Patrouille;
                        break;
                    }

                    int tileDistance = int.MaxValue;
                    Tile moveTarget = null;
                    for (int poiCounter = EntityManager.pInstance.pPointsOfInterest.Count - 1; poiCounter > 0; --poiCounter)
                    {
                        int currentDistance = Tile.Distance(mCharacter.pTile, EntityManager.pInstance.pPointsOfInterest[poiCounter]);
                        if (currentDistance == 1) // if point of interest is resolved remove from list
                        {
                            EntityManager.pInstance.pPointsOfInterest.RemoveAt(poiCounter);
                        }
                        else
                        {
                            //choose closest one to move towards to
                            if (currentDistance < tileDistance)
                            {
                                moveTarget = EntityManager.pInstance.pPointsOfInterest[poiCounter];
                                tileDistance = currentDistance;
                            }
                        }
                    }

                    if (tileDistance > mCharacter.pVisionRange) // exit if closest POI is still out of percievable range
                    {
                        pAIState = eAIState.Patrouille;
                        break;
                    }


                    //move one tile to the point of interest
                    List<Tile> stepsToTarget = GridManager.pInstance.GetPathTo(mCharacter.pTile, moveTarget);
                    yield return AIevaluator.AImove(mCharacter, stepsToTarget[1]);
                    */
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
        GameManager.pInstance.ChangeState(eGameState.Select);
        EntityManager.pInstance.EndRound(mCharacter);

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
        foreach (var playerChar in EntityManager.pInstance.pPlayers)
            if (GridManager.pInstance.GetVisibilityToTarget(mCharacter.pTile, playerChar.pTile, mCharacter.pVisionRange) == eVisibility.Seethrough || playerChar.pHasBeenRevealed == true)
                visibleCharaters.Add(playerChar);

        // every visible character is revealed forever
        foreach (Character playerChar in visibleCharaters)
            playerChar.pHasBeenRevealed = true;

        
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
    /// Moves the Player to the target tile. Use for one single Step only. No pathfinding! Substracts movement cost of one Step from mCharacter.
    /// </summary>
    /// <param name="mCharacter">AI Character to move</param>
    /// <param name="targetTile">Adjenct Tile to place the character on</param>
    /// <returns></returns>
    public static IEnumerator AImove(Character mCharacter, Tile targetTile)
    {

        //moving the object

        while (Vector3.Distance(mCharacter.transform.position, targetTile.transform.position) >= 0.1f)
        {
            mCharacter.transform.position = Vector3.Lerp(mCharacter.transform.position, targetTile.transform.position, Time.deltaTime * 5);
            yield return new WaitForEndOfFrame(); ;
        }

        //HACK: jumping the object to target 
        //mCharacter.transform.position = targetTile.transform.position;

        // informing the grid about the changes
        mCharacter.pTile.pCharacterId = -1;
        targetTile.pCharacterId = EntityManager.pInstance.GetIdForCharacter(mCharacter);
        mCharacter.pTile = targetTile;

        // substract cost for step
        mCharacter.pApCurrent -= mCharacter.pWalkCost;

        yield return null;
    }
}