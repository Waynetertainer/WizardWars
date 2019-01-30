using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace NET_System
{
    public class NET_ServerInfo
    {
        private string serverName;
        private DateTime lastIntel = DateTime.Now;
        private IPAddress serverAddress;
        private int playerCount;
        private GAME_State gameState;

        //---------------------------------------------------------------------------------------------//
        //Constructor:
        //---------------------------------------------------------------------------------------------//
        public NET_ServerInfo(string serverIPAddress, int playerCount, GAME_State gameState, string serverName)
        {
            this.serverName = serverName;
            this.serverAddress = IPAddress.Parse(serverIPAddress);
            this.playerCount = playerCount;
            this.gameState = gameState;
        }

        //---------------------------------------------------------------------------------------------//
        //Public functions:
        //---------------------------------------------------------------------------------------------//
        public IPAddress INFO_GetAddress()
        {
            return serverAddress;
        }
        public int INFO_GetPlayerCount()
        {
            return playerCount;
        }
        public GAME_State INFO_GetState()
        {
            return gameState;
        }
        public void INFO_SetPlayerCount(int playerCount)
        {
            this.playerCount = playerCount;
        }
        public void INFO_SetState(GAME_State gameState)
        {
            this.gameState = gameState;
        }
        public void INFO_SetIntel(DateTime newIntel)
        {
            this.lastIntel = newIntel;
        }
        public DateTime INFO_GetIntel()
        {
            return this.lastIntel;
        }
        public string INFO_GetServerName()
        {
            return this.serverName;
        }
    }
}
