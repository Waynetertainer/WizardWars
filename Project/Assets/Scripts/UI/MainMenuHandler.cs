using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NET_Multiplayer;
using NET_System;
using System.Collections.Generic;

public class MainMenuHandler : MonoBehaviour
{

    public Color BackgroundFlickerLight;
    public Color BackgroundFlickerDark;
    public Image BackgroundImage;
    public GameObject ServerButtonPrefab;
    public string LevelName;
    private List<GameObject> mServerButtonList = new List<GameObject>();

    [SerializeField] private GameObject mPanelMainMenu;
    [SerializeField] private GameObject mPanelLobby;
    [SerializeField] private GameObject mPanelServerSelection;
    [SerializeField] private Text mPanelNetworkDebug;
    private bool startLog = false;

    private void Start()
    {
        mPanelMainMenu.SetActive(true);
        mPanelLobby.SetActive(false);
        mPanelServerSelection.SetActive(false);
    }

    void FixedUpdate()
    {
        BackgroundImage.color = Color.Lerp(BackgroundFlickerLight, BackgroundFlickerDark, Random.value); // Spielerei :D
        if (startLog)
        {
            mPanelNetworkDebug.text = "Connected Clients" + NetworkManager.pInstance.pNetMain.NET_GetStates();
        }
    }

    public void btnStartSingleplayerGame()
    {
        SceneManager.LoadScene(LevelName);
    }

    public void btnLoadGame()
    {
        //TODO: Load a saved game
    }

    public void btnOptions()
    {
        //TODO: Show options menu
    }

    public void btnCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void btnExit()
    {
        Application.Quit();
    }

    public void btnHostGame()
    {
        NetworkManager.pInstance.pNetMain = new NET_Main(true, 20, "WizardWarsServer", false);
        mPanelMainMenu.SetActive(false);
        mPanelLobby.SetActive(true);
        mPanelServerSelection.SetActive(false);
        NetworkManager.pInstance.pNetMain.NET_SetGameState(GAME_State.WaitingForPlayers);
        NetworkManager.pInstance.pNetMain.NET_Start();
        startLog = true;
    }

    public void btnJoinGame()
    {
        NetworkManager.pInstance.pNetMain = new NET_Main(false, 20, "WizardWarsClient", false);
        mPanelMainMenu.SetActive(false);
        mPanelLobby.SetActive(false);
        mPanelServerSelection.SetActive(true);
        NetworkManager.pInstance.pNetMain.NET_Start();
    }

    public void btnMainMenu()
    {
        NetworkManager.pInstance.pNetMain?.NET_Stop();
        NetworkManager.pInstance.pNetMain?.NET_Shutdown();

        mPanelMainMenu.SetActive(true);
        mPanelLobby.SetActive(false);
        mPanelServerSelection.SetActive(false);
        startLog = false;
    }

    public void btnStartMultiplayerGame()
    {
        //TODO: send other players Level start
        SceneManager.LoadScene(LevelName);
    }

    public void btnPlayerReady()
    {
        //TODO: send other players ready status
    }

    public void btnJoinServer()
    {
        //TODO: connect to Server and show lobby
        mPanelMainMenu.SetActive(false);
        mPanelLobby.SetActive(true);
        mPanelServerSelection.SetActive(false);
    }

    public void btnRefreshServerList()
    {
        for (int i = mServerButtonList.Count-1; i >= 0; --i)
        {
            GameObject.Destroy(mServerButtonList[i]);
        }

        startLog = true;

        NET_ServerInfo[] mServers = NetworkManager.pInstance.pNetMain.NET_GetServerInfo();
        if (mServers.Length > 0)
        {
            foreach (NET_ServerInfo mServer in mServers)
            {
                GameObject btn = GameObject.Instantiate(ServerButtonPrefab);
                btn.GetComponentInChildren<Text>().text = mServer.INFO_GetServerName() + " " + mServer.INFO_GetAddress() + " PlayerCount: " + mServer.INFO_GetPlayerCount();
                btn.transform.parent = mPanelServerSelection.transform.GetChild(0);
                btn.GetComponent<ServerSelectButtonScript>().pSelectedServer = mServer;
                mServerButtonList.Add(btn);
            }
        }
        else
        {
            GameObject btn = GameObject.Instantiate(ServerButtonPrefab);
            btn.GetComponentInChildren<Text>().text = "No server found";
            btn.transform.parent = mPanelServerSelection.transform.GetChild(0);
            btn.GetComponent<ServerSelectButtonScript>().pSelectedServer = null;
            mServerButtonList.Add(btn);
            Debug.Log("No server found");
        }
    }
}
