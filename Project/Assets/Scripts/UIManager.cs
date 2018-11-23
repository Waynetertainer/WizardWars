/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject pSelectionScreen;

    public Toggle pMoveToggle;
    public Toggle pFireToggle;

    private void OnEnable()
    {
        pMoveToggle.onValueChanged.AddListener(delegate { SwitchToMove(); });
        pMoveToggle.isOn = true;
        pFireToggle.onValueChanged.AddListener(delegate { SwitchToFire(); });
        pFireToggle.isOn = false;
    }

    private void OnDisable()
    {
        pMoveToggle.onValueChanged.RemoveAllListeners();
        pFireToggle.onValueChanged.RemoveAllListeners();
    }

    public void CloseAllWindos()
    {
        pSelectionScreen.SetActive(false);
    }

    public void ShowSelectionScreen()
    {
        CloseAllWindos();
        pSelectionScreen.SetActive(true);
    }

    public void SwitchToMove()
    {
        pFireToggle.onValueChanged.RemoveAllListeners();
        pMoveToggle.isOn = true;
        pFireToggle.isOn = !pMoveToggle.isOn;
        GameManager.pInstance.pGameState = eGameState.Move;
        pFireToggle.onValueChanged.AddListener(delegate { SwitchToFire(); });
    }

    public void SwitchToFire()
    {
        pMoveToggle.onValueChanged.RemoveAllListeners();
        pFireToggle.isOn = true;
        pMoveToggle.isOn = !pFireToggle.isOn;
        GameManager.pInstance.pGameState = eGameState.Fire;
        pMoveToggle.onValueChanged.AddListener(delegate { SwitchToMove(); });
    }
}
