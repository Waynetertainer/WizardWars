using UnityEngine;
using NET_System;

public class ServerSelectButtonScript : MonoBehaviour
{
    public NET_ServerInfo pSelectedServer;

    public void btnServerSelection()
    {
        NetworkManager.pInstance.pNetMain.NET_ConnectToServer(pSelectedServer);
        Camera.main.GetComponent<MainMenuHandler>().btnJoinServer();
    }
}
