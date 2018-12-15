using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
    ePatrouille,
    eFight,
    eHunt
};

public class AIevaluator : MonoBehaviour
{
    public AIState pAIState = AIState.ePatrouille;
    public List<Tile> pPatrouillePoints = new List<Tile>();
    public Character pActiveTarget;

    private Character mCharacter;
    private int mCoverRoundCount = 0;
    private int mPatWaypointID = 0;
    private List<Tile> pSteps = new List<Tile>();

    void Start()
    {
        mCharacter = GetComponent<Character>();//connect to parent player
    }


    private void EvaluateAI()
    {
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
        while (mCharacter.pApCurrent == 0) // do stuff until AP are spend
        {

            switch (pAIState)
            {
                case AIState.ePatrouille: // totally broken!
                    if (pSteps.Count == 0) // if no path exists get a new one
                    {
                        ++mPatWaypointID;
                        
                        pSteps = GridManager.pInstance.GetPathTo(mCharacter.pTile, pPatrouillePoints[mPatWaypointID]);
                    }
                    
                    while (mCharacter.pApCurrent > 0 && mPatWaypointID < pSteps.Count)
                    {
                        findTarget();
                        mCharacter.Move(pSteps[mPatWaypointID]);
                        ++mPatWaypointID;
                    }


                    break;
                case AIState.eFight:
                    // find better cover
                    // if ap-cost to next cover < remaining AP
                    // move to new cover
                    if (mCharacter.pApCurrent > mCharacter._Cost)
                    {
                        //fire signature spell against active target
                    }
                    if (pActiveTarget.pHpCurrent <= 0) // Ziel erlegt
                    {
                        findTarget();
                        break;
                    }
                    while (mCharacter.pApCurrent > 0)
                    {
                        // shoot normal spell at target
                        if (pActiveTarget.pHpCurrent <= 0)
                        {
                            findTarget();
                            break;
                        }
                    }

                    break;
                case AIState.eHunt:
                    //find route to last known position of active target
                    //do
                    findTarget();
                    // select closest enemy as active target
                    // switch state to eFight
                    //early brake
                    //move to next tile in route
                    //while list of steps to pat-point != empty

                    break;
                default:
                    Debug.LogError("AI in unknown state");
                    break;
            }
        }
        // return if char has still points to use
        #endregion
    }

    private void findTarget()
    {

        if (true) // if new target is available
        {
            //pActive target = newly found target
            pAIState = AIState.eFight;
        }
        else
        {
            pAIState = AIState.ePatrouille;
        }
    }
}