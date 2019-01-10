/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEngine;

public class PlayerCollisionFlag : MonoBehaviour
{
    private Character mCharacter;
    private void OnEnable()
    {
        mCharacter = GetComponentInParent<Character>();
    }

    public void SetFlag()
    {
        mCharacter.pEffectHit = true;
    }
}
