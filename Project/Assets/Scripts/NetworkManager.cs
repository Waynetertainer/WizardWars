using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NET_Multiplayer;



public class NetworkManager : MonoBehaviour
{
    public static NetworkManager pInstance;
    public NET_Main pNetMain;
    private void Awake()
    {
        if (pInstance == null)
        {
            pInstance = this;
        }
        else if (pInstance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        pNetMain?.NET_Update();
    }

    private void OnApplicationQuit()
    {
        pNetMain?.NET_Stop();
        pNetMain?.NET_Shutdown();
    }
}
