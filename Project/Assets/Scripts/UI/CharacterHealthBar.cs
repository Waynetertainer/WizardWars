/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
    private Character mCharacter;
    private Slider mHealthBar;

    private void Start()
    {
        mCharacter = GetComponentInParent<Character>();
        mHealthBar = GetComponentInChildren<Slider>();

        mHealthBar.maxValue = mCharacter.pHp;
        mHealthBar.value = mCharacter.pHp;

    }

    private void Update()
    {
        mHealthBar.value = Mathf.Lerp(mHealthBar.value, mCharacter.pHp, Time.deltaTime);
    }
}
