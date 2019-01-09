/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Window pStatScreen;
    public Window pSelectionScreen;
    public Window pEndScreen;

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

    public void CloseAllWindows()
    {
        pStatScreen.Hide();
        pSelectionScreen.Hide();
        pEndScreen.Hide();
    }

    public void ShowStatScreen()
    {
        pStatScreen.Show();
        Character character = GameManager.pInstance.pActiveCharacter;
        StatScreen screen = (StatScreen)pStatScreen;
        screen.pNameText.text = character.pName;
        screen.pHpText.text = "Health: " + character.pHpCurrent;
        screen.pApText.text = "Action Points: " + character.pApCurrent + "/" + character.pAp;
        screen.pMoveRangeText.text = "Move Range: " + character.pWalkRange;
        screen.pVisionRangeText.text = "Vision Range: " + character.pVisionRange;

        pMoveButton.interactable = character.pApCurrent >= character.Cost;
        pSkillButton.interactable = character.pApCurrent >= character.Cost;
        pUniqueButton.GetComponentInChildren<Text>().text = character.pUniqueSpell.SpellName;
        pUniqueButton.interactable = !character.pFired && character.pApCurrent >= character.pUniqueSpell.Cost;
    }

    public void ShowSelectionScreen()
    {
        Character character = GameManager.pInstance.pActiveCharacter;
        pSelectionScreen.Show(character);
        StatScreen screen = (StatScreen)pSelectionScreen;
        screen.pNameText.text = character.pName;
        screen.pHpText.text = "Health: " + character.pHpCurrent;
        screen.pApText.text = "Action Points: " + character.pApCurrent;
        screen.pMoveRangeText.text = "Move Range: " + character.pWalkRange;
        screen.pVisionRangeText.text = "Vision Range: " + character.pVisionRange;
    }

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
            case eGameState.EndOfMatch:
                CloseAllWindows();
                pEndScreen.Show(EntityManager.pInstance.pPCPlayers.Count == 0);
                break;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");

    }
}