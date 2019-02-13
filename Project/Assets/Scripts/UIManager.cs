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

    //public Window pStatScreen;
    //public Window pSelectionScreen;
    //public Window pToolTipScreen;

    //public Toggle pMoveToggle;
    //public Toggle pFireToggle;


    //public Button pMoveButton;
    public Button pTurnButton;
    public GameObject pPauseMenu;

    [Header("Player 1 Settings")]
    public GameObject pStatPanelP1;
    public Text pStatTextP1;
    public Button pButtonCharacter1P1;
    public Button pButtonCharacter2P1;
    public Button pButtonCharacter3P1;

    public Button pButtonSkillP1;
    public Button pButtonUniqueAP1;
    public Button pButtonUniqueBP1;
    public Button pButtonUniqueCP1;

    public GameObject pEndScreenP1;

    [Header("Player 2 Settings")]
    public GameObject pStatPanelP2;
    public Text pStatTextP2;
    public Button pButtonCharacter1P2;
    public Button pButtonCharacter2P2;
    public Button pButtonCharacter3P2;

    public Button pButtonSkillP2;
    public Button pButtonUniqueAP2;
    public Button pButtonUniqueBP2;
    public Button pButtonUniqueCP2;

    public GameObject pEndScreenP2;

    public void Start()
    {
        //UpdatePlayerSelection();
    }

    public void CloseAllWindows()
    {
        //pStatScreen.Hide();
        //pSelectionScreen.Hide();

        pTurnButton.gameObject.SetActive(false);
        pStatPanelP1.SetActive(false);
        pButtonSkillP1.gameObject.SetActive(false);
        pButtonUniqueAP1.gameObject.SetActive(false);
        pButtonUniqueBP1.gameObject.SetActive(false);
        pButtonUniqueCP1.gameObject.SetActive(false);
        pEndScreenP1.SetActive(false);
        pStatPanelP2.SetActive(false);
        pButtonSkillP2.gameObject.SetActive(false);
        pButtonUniqueAP2.gameObject.SetActive(false);
        pButtonUniqueBP2.gameObject.SetActive(false);
        pButtonUniqueCP2.gameObject.SetActive(false);
        pEndScreenP2.SetActive(false);
    }

    /// <summary>
    /// Shows and refreshes the stat panel
    /// </summary>
    public void ShowStatScreen()
    {
        //pStatScreen.Show();
        Character character = GameManager.pInstance.pActiveCharacter;
        //StatScreen screen = (StatScreen)pStatScreen;

        switch (GameManager.pInstance.pCurrentFaction)
        {
            case eFactions.Player1:
                pStatPanelP1.SetActive(true);
                pStatTextP1.text = GameManager.pInstance.pActiveCharacter.pName + "\n" ;
                pStatTextP1.text += "Health: " + GameManager.pInstance.pActiveCharacter.pHpCurrent + "\n";
                pStatTextP1.text += "Actions: " + GameManager.pInstance.pActiveCharacter.pApCurrent / 10 + "\n" ;
                //TODO show Move and Vision range

                pButtonSkillP1.gameObject.SetActive(true);
                pButtonSkillP1.interactable = GameManager.pInstance.pActiveCharacter.pApCurrent > 10;

                pButtonUniqueAP1.gameObject.SetActive(GameManager.pInstance.pActiveCharacter.name == "Sorceress_01(Clone)"); //some damage spell
                pButtonUniqueBP1.gameObject.SetActive(GameManager.pInstance.pActiveCharacter.name == "Sorceress_02(Clone)"); //chain lightning
                pButtonUniqueCP1.gameObject.SetActive(GameManager.pInstance.pActiveCharacter.name == "Sorceress_03(Clone)"); //heal

                pButtonUniqueAP1.interactable = GameManager.pInstance.pActiveCharacter.pApCurrent > 10; //some damage spell
                pButtonUniqueBP1.interactable = GameManager.pInstance.pActiveCharacter.pApCurrent > 10; //chain lightning
                pButtonUniqueCP1.interactable = GameManager.pInstance.pActiveCharacter.pApCurrent > 10; //heal

                pTurnButton.gameObject.SetActive(true);

                break;
            case eFactions.Player2:
                pStatPanelP2.SetActive(true);
                pStatTextP2.text = GameManager.pInstance.pActiveCharacter.pName + "\n";
                pStatTextP2.text += "Health: " + GameManager.pInstance.pActiveCharacter.pHpCurrent + "\n";
                pStatTextP2.text += "Actions: " + GameManager.pInstance.pActiveCharacter.pApCurrent / 10 + "\n";

                pButtonSkillP2.gameObject.SetActive(true);
                pButtonSkillP2.interactable = GameManager.pInstance.pActiveCharacter.pApCurrent > 10;

                pButtonUniqueAP2.gameObject.SetActive(GameManager.pInstance.pActiveCharacter.name == "Enemy_01(Clone)"); //some damage spell
                pButtonUniqueBP2.gameObject.SetActive(GameManager.pInstance.pActiveCharacter.name == "Enemy_02(Clone)"); //chain lightning
                pButtonUniqueCP2.gameObject.SetActive(GameManager.pInstance.pActiveCharacter.name == "Enemy_03(Clone)"); //heal

                pButtonUniqueAP2.interactable = GameManager.pInstance.pActiveCharacter.pApCurrent > 10; //some damage spell
                pButtonUniqueBP2.interactable = GameManager.pInstance.pActiveCharacter.pApCurrent > 10; //chain lightning
                pButtonUniqueCP2.interactable = GameManager.pInstance.pActiveCharacter.pApCurrent > 10; //heal

                pTurnButton.gameObject.SetActive(true);

                break;
        }



    }
    
    public void UpdatePlayerSelection()
    {
        switch (GameManager.pInstance.pCurrentFaction)
        {
            case eFactions.Spawner:
                pButtonCharacter1P1.gameObject.SetActive(false);
                pButtonCharacter2P1.gameObject.SetActive(false);
                pButtonCharacter3P1.gameObject.SetActive(false);
                pButtonCharacter1P2.gameObject.SetActive(false);
                pButtonCharacter2P2.gameObject.SetActive(false);
                pButtonCharacter3P2.gameObject.SetActive(false);
                break;
            case eFactions.Player1:
                pButtonCharacter1P1.gameObject.SetActive(EntityManager.pInstance.pGetFactionEntities(eFactions.Player1)[0].pHpCurrent > 0);
                pButtonCharacter2P1.gameObject.SetActive(EntityManager.pInstance.pGetFactionEntities(eFactions.Player1)[1].pHpCurrent > 0);
                pButtonCharacter3P1.gameObject.SetActive(EntityManager.pInstance.pGetFactionEntities(eFactions.Player1)[2].pHpCurrent > 0);
                pButtonCharacter1P2.gameObject.SetActive(false);
                pButtonCharacter2P2.gameObject.SetActive(false);
                pButtonCharacter3P2.gameObject.SetActive(false);
                //TODO might break after first character kill
                pButtonCharacter1P1.interactable = EntityManager.pInstance.pGetFactionEntities(eFactions.Player1)[0].pApCurrent > 0;
                pButtonCharacter2P1.interactable = EntityManager.pInstance.pGetFactionEntities(eFactions.Player1)[1].pApCurrent > 0;
                pButtonCharacter3P1.interactable = EntityManager.pInstance.pGetFactionEntities(eFactions.Player1)[2].pApCurrent > 0;

                break;
            case eFactions.AI1:
                pButtonCharacter1P1.gameObject.SetActive(false);
                pButtonCharacter2P1.gameObject.SetActive(false);
                pButtonCharacter3P1.gameObject.SetActive(false);
                pButtonCharacter1P2.gameObject.SetActive(false);
                pButtonCharacter2P2.gameObject.SetActive(false);
                pButtonCharacter3P2.gameObject.SetActive(false);
                break;
            case eFactions.Player2:
                pButtonCharacter1P1.gameObject.SetActive(false);
                pButtonCharacter2P1.gameObject.SetActive(false);
                pButtonCharacter3P1.gameObject.SetActive(false);
                pButtonCharacter1P2.gameObject.SetActive(EntityManager.pInstance.pGetFactionEntities(eFactions.Player2)[0].pHpCurrent > 0);
                pButtonCharacter2P2.gameObject.SetActive(EntityManager.pInstance.pGetFactionEntities(eFactions.Player2)[1].pHpCurrent > 0);
                pButtonCharacter3P2.gameObject.SetActive(EntityManager.pInstance.pGetFactionEntities(eFactions.Player2)[2].pHpCurrent > 0);
                //TODO might break after first character kill
                pButtonCharacter1P2.interactable = EntityManager.pInstance.pGetFactionEntities(eFactions.Player2)[0].pApCurrent > 0;
                pButtonCharacter2P2.interactable = EntityManager.pInstance.pGetFactionEntities(eFactions.Player2)[1].pApCurrent > 0;
                pButtonCharacter3P2.interactable = EntityManager.pInstance.pGetFactionEntities(eFactions.Player2)[2].pApCurrent > 0;

                break;
            case eFactions.AI2:
                pButtonCharacter1P1.gameObject.SetActive(false);
                pButtonCharacter2P1.gameObject.SetActive(false);
                pButtonCharacter3P1.gameObject.SetActive(false);
                pButtonCharacter1P2.gameObject.SetActive(false);
                pButtonCharacter2P2.gameObject.SetActive(false);
                pButtonCharacter3P2.gameObject.SetActive(false);

                break;
            default:
                break;
        }


  
    }

    public void SetState(string state)
    {
        eGameState s = (eGameState)Enum.Parse(typeof(eGameState), state);
        GameManager.pInstance.ChangeState(s);
    }


    /// <summary>
    /// translates Strings from Buttonevents to enum and sets Gamemanager accodingly
    /// </summary>
    /// <param name="state"></param>
    public void SetState(eGameState state)
    {
        if (GameManager.pInstance.pGameState == eGameState.Firing || GameManager.pInstance.pGameState == eGameState.Moving)
            return;

        GameManager.pInstance.ChangeState(state);
    }

    /*
    public void Refresh()
    {
        switch (GameManager.pInstance.pGameState)
        {
            case eGameState.Select:
                CloseAllWindows();
                break;
            case eGameState.Selected:
                CloseAllWindows();
                ShowStatScreen();
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
            case eGameState.EndTurn:
                CloseAllWindows();
                break;
            case eGameState.EndOfMatch:
                CloseAllWindows();
                // TODO add win screen
                if (EntityManager.pInstance.pGetAliveFactionEntities(eFactions.Player1).Count == 0)
                {
                    pEndScreenP2.SetActive(true);
                }
                else
                {
                    pEndScreenP1.SetActive(true);
                }

                
                //pEndScreen.Show(EntityManager.pInstance.pPCPlayers.Count == 0);
                break;
        }
    }

    */
    public void ShowTooltip(string action)
    {
        //pToolTipScreen.Show(action);
    }

    public void HideToolTip()
    {
        //pToolTipScreen.Hide();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResumeGame()
    {
        pPauseMenu.SetActive(false);
        GameManager.pInstance.ChangeState(eGameState.Select);
    }

    public void ShowPauseMenu()
    {
        pPauseMenu.SetActive(true);
        GameManager.pInstance.ChangeState(eGameState.InMenu);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");

    }

    /// <summary>
    /// Gets the list of all player characters of the current faction and selects the one with the given number. Not PlayerID!
    /// </summary>
    /// <param name="charNumber"></param>
    public void SelectCharacter(int charNumber)
    {
        GameManager.pInstance.pActiveCharacter?.Deselect();
        GameManager.pInstance.pActiveCharacter = EntityManager.pInstance.pGetFactionEntities(GameManager.pInstance.pCurrentFaction)[charNumber];
        CameraMovement.SetTarget(GameManager.pInstance.pActiveCharacter.transform);

        GameManager.pInstance.ChangeState(eGameState.Move);
    }
}