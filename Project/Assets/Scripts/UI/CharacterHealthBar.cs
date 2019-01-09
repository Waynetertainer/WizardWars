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
    private GameObject mCamera;

    private void Start()
    {
        mCharacter = GetComponentInParent<Character>();
        mHealthBar = GetComponentInChildren<Slider>();
        //GetComponent<Canvas>().worldCamera = Camera.current;

        mHealthBar.maxValue = mCharacter.pHp;
        mHealthBar.value = mCharacter.pHp;

        mCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        this.transform.LookAt(mCamera.transform);
        mHealthBar.value = Mathf.Lerp(mHealthBar.value, mCharacter.pHpCurrent, Time.deltaTime);
    }
}
