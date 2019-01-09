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
    private Camera mCamera;

    private void Start()
    {
        mCharacter = GetComponentInParent<Character>();
        mHealthBar = GetComponentInChildren<Slider>();

        mHealthBar.maxValue = mCharacter.pHp;
        mHealthBar.value = mCharacter.pHp;

        mCamera = Camera.main;
    }

    private void Update()
    {
        mHealthBar.value = Mathf.Lerp(mHealthBar.value, mCharacter.pHpCurrent, Time.deltaTime);
        this.transform.LookAt(mCamera.transform);
    }
}
