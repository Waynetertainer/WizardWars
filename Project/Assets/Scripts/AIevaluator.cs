using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIAgression
{
    eCautous,
    eNormal,
    eAgressive
};

public class AIevaluator : MonoBehaviour
{
    public AIAgression pAIAgression;

    private Character character;
    private int mCoverRoundCount = 0;

    void Start()
    {
        //connect to parent player
        character = GetComponent<Character>();
    }


    private void EvaluateAI()
    {
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

                }
               
            }
        }
        else // character hat wenig leben und ist nicht in deckung
        {
            if (true) //TODO: Deckung vor dem gegner vorhanden?
            {
                if (true) //TODO: Halbe deckung bevorzugen?!
                {
                    //ziele in reihenfolge angreifen: Support, Damage, Tank
                }
                else // wenn keine halbe, dann in volle deckung begeben
                {
                    //ziele in reihenfolge angreifen: Support, Damage, Tank
                }

            }
            else //TODO: wenn keine deckung vorhanden
            {
                //ziele in reihenfolge angreifen: Support, Damage, Tank
            }

        }

    }
}