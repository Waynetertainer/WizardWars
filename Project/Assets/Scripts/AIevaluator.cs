using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIevaluator
{

    public static IEnumerator EvaluateAI(Character mCharacter)
    {
        eAIState pAIState = eAIState.Patrouille;
        Character pActiveTarget = null;
        Tile pHuntingTarget = null;
        //int mCoverRoundCount = 0; //TODO: Implement Cover Count
        int mPatWaypointID = 0;
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
        while (mCharacter.pApCurrent > 0) // TODO: do stuff until AP are spend, possible infinite loop if char is in position but shot is too expensive
        {
            switch (pAIState)
            {
                case eAIState.Patrouille:
                    if (mCharacter.pAIPatrouillePoints.Count != 0) // wenn statischer gegner ohne wegpunkte
                    {
                        pActiveTarget = AIfindTarget(mCharacter);
                        if (pActiveTarget != null)
                        {
                            pAIState = eAIState.Fight;
                            break;
                        }
                        else
                        {
                            mCharacter.pApCurrent = 0;
                            break;
                        }
                    }

                    pSteps = GridManager.pInstance.GetPathTo(mCharacter.pTile, mCharacter.pAIPatrouillePoints[mPatWaypointID]);

                    if (pSteps.Count == 0) //wenn pat-zielpunkt erreicht zum nächsten wechseln
                    {
                        mPatWaypointID = (mPatWaypointID + 1) % mCharacter.pAIPatrouillePoints.Count;
                        pSteps = GridManager.pInstance.GetPathTo(mCharacter.pTile, mCharacter.pAIPatrouillePoints[mPatWaypointID]);
                    }

                    mCharacter.Move(pSteps[0]); // make one single step and check for targets
                    pActiveTarget = AIfindTarget(mCharacter);
                    if (pActiveTarget != null)
                        pAIState = eAIState.Fight;

                    break;
                case eAIState.Fight:
                    // find better cover
                    // if ap-cost to next cover < remaining AP
                    // move to new cover
                    // Visibility check
                    if (mCharacter.pApCurrent > mCharacter.pUniqueSpell.Cost) //TODO: Special cooldown? Sanity check?
                    {
                        mCharacter.CastUnique(pActiveTarget.pTile); //fire signature spell against active target

                        pActiveTarget = AIfindTarget(mCharacter); // check if target survived
                        if (pActiveTarget == null)
                            pAIState = eAIState.Patrouille;
                        break;
                    }

                    if (mCharacter.pApCurrent > mCharacter.Cost)
                    {
                        mCharacter.StandardAttack(pActiveTarget.pTile);// shoot normal spell at target
                        pActiveTarget= AIfindTarget(mCharacter); //check for survivors
                        if (pActiveTarget == null)
                            pAIState = eAIState.Patrouille;
                    }
                    
                    break;
                case eAIState.Hunt: //TODO: Hunting targets
                                    //find route to last known position of active target if it has not been killed and is out of sight
                                    //do
                    AIfindTarget(mCharacter);
                    // select closest enemy as active target
                    // switch state to eFight
                    //early brake,
                    //move to next tile in route
                    //while list of steps to pat-point != empty

                    break;
                default:
                    Debug.LogError("AI in unknown state");
                    break;
            }
        }// loop if char has still points to use

        #endregion

        GameManager.pInstance.ChangeState(eGameState.Selected); // end AI turn
        yield break;
    }

    /// <summary>
    /// Searches for new targets for the AI and returns a visible Target or null.
    /// </summary>
    /// <param name="mCharacter">AI-Character to process</param>
    /// <returns>Returns the closest visible player character or null.</returns>
    public static Character AIfindTarget(Character mCharacter)
    {
        List<Character> visibleCharaters = new List<Character>();

        foreach (var playerChar in EntityManager.pInstance.pCurrentPlayers) //find visible players
            if (GridManager.pInstance.GetVisibilityToTarget(mCharacter.pTile, playerChar.pTile, mCharacter.pVisionRange) == eVisibility.Seethrough)
                visibleCharaters.Add(playerChar);

        // find closest enemy

        float targetDistance = float.MaxValue;
        Character returnCharacter = null;
        foreach (Character playerChar in visibleCharaters)
        {
            float currentDistance = Vector3.Distance(mCharacter.transform.position, playerChar.transform.position); //TODO: Tile.Distance may be better here
            if (currentDistance < targetDistance)
            {
                targetDistance = currentDistance;
                returnCharacter = playerChar;
            }
        }

        return returnCharacter;
    }
}