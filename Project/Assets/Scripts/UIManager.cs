﻿/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Window pStatScreen;
    public Window pSelectionScreen;

    public Toggle pMoveToggle;
    public Toggle pFireToggle;


    public Button pMoveButton;
    public Button pEndButton;
    public Button pSkillButton;
    public Button pUniqueButton;

    private void OnEnable()
    {
        pMoveButton.onClick.AddListener(delegate { SetState(eGameState.Move); });
        pEndButton.onClick.AddListener(delegate { SetState(eGameState.End); });
        pSkillButton.onClick.AddListener(delegate { SetState(eGameState.FireSkill); });
        pUniqueButton.onClick.AddListener(delegate { SetState(eGameState.FireUnique); });
    }

    private void OnDisable()
    {
        pMoveToggle.onValueChanged.RemoveAllListeners();
        pFireToggle.onValueChanged.RemoveAllListeners();
    }

    public void CloseAllWindows()
    {
        pStatScreen.Hide();
        pSelectionScreen.Hide();
    }

    public void ShowStatScreen()
    {
        pStatScreen.Show();
        Character character = GameManager.pInstance.pActiveCharacter;
        StatScreen screen = (StatScreen)pStatScreen;
        screen.pHpText.text = "Health: " + character.pHp;
        screen.pApText.text = "Action Points: " + character.pCurrentAp;
        screen.pMoveRangeText.text = "Move Range: " + character.pWalkRange;
        screen.pVisionRangeText.text = "Vision Range: " + character.pVisionRange;

        pMoveButton.interactable = !character.pMoved || (character.pMoved && !character.pFired);
        pSkillButton.interactable = !character.pFired;
        pUniqueButton.GetComponentInChildren<Text>().text = character.pUniqueSpell.SpellName;
        pUniqueButton.interactable = !character.pFired;
    }

    public void ShowSelectionScreen()
    {
        Character character = GameManager.pInstance.pActiveCharacter;
        pSelectionScreen.Show(character);
        StatScreen screen = (StatScreen)pSelectionScreen;
        screen.pHpText.text = "Health: " + character.pHp;
        screen.pApText.text = "Action Points: " + character.pCurrentAp;
        screen.pMoveRangeText.text = "Move Range: " + character.pWalkRange;
        screen.pVisionRangeText.text = "Vision Range: " + character.pVisionRange;
    }

    //public void SwitchToMove()
    //{
    //    pFireToggle.onValueChanged.RemoveAllListeners();
    //    pMoveToggle.isOn = true;
    //    pFireToggle.isOn = !pMoveToggle.isOn;
    //    GameManager.pInstance.pGameState = eGameState.Move;
    //    pFireToggle.onValueChanged.AddListener(delegate { SwitchToFire(); });
    //}

    //public void SwitchToFireball()
    //{
    //    pMoveToggle.onValueChanged.RemoveAllListeners();
    //    pFireToggle.isOn = true;
    //    pMoveToggle.isOn = !pFireToggle.isOn;
    //    GameManager.pInstance.pGameState = eGameState.Fire;
    //    pMoveToggle.onValueChanged.AddListener(delegate { SwitchToMove(); });
    //}

    public void SetState(string state)
    {
        eGameState s = (eGameState)Enum.Parse(typeof(eGameState), state);
        GameManager.pInstance.ChangeState(s);
    }
    public void SetState(eGameState state)
    {
        GameManager.pInstance.ChangeState(state);
    }

    public void Refresh()
    {
        switch (GameManager.pInstance.pGameState)
        {
            case eGameState.Select:
                CloseAllWindows();
                break;
            case eGameState.Selected:
                CloseAllWindows();
                ShowSelectionScreen();
                break;
            case eGameState.Move:
                CloseAllWindows();
                ShowStatScreen();
                break;
            case eGameState.Moving:
                break;
            case eGameState.WaitForInput:
                CloseAllWindows();
                ShowStatScreen();
                break;
            case eGameState.FireSkill:
                CloseAllWindows();
                ShowStatScreen();
                break;
            case eGameState.FireUnique:
                CloseAllWindows();
                ShowStatScreen();
                break;
            case eGameState.End:
                CloseAllWindows();
                break;
        }
    }
}
