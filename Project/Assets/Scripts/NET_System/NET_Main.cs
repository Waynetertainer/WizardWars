//Class to encapsulate all {NET} related subsystems
//Class provides public functions as interface to main game
//Class handles all {NET} related functions

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NET_System;

namespace NET_Multiplayer
{
    public class NET_Main
    {
        //Timestepping
        private float timeStep = 100.0f;
        private DateTime lastTime = DateTime.Now;
        private DateTime thisTime = DateTime.Now;

        //Datastructures
        private Dictionary<string, NET_ServerInfo> serverInfos = new Dictionary<string, NET_ServerInfo>();
        private List<NET_EventCall> eventCalls = new List<NET_EventCall>();

        //Subsystems
        private NET_Udp udpSystem;
        private NET_TcpServer tcpServer;
        private NET_TcpClient tcpClient;

        //Main threading
        private Thread mainThread;
        private NET_State state = NET_State.NET_Waiting;
   
        //GameData objects for player 1 - 4
        private int playerCount = 1;
        private int playerID = 0;
        private GAME_State gameState = GAME_State.WaitingForPlayers;
        private NET_DataPack[] playersGameData = new NET_DataPack[4];

        //Variables
        private string name;
        private bool isHost = false;
        private IPAddress localIP;
        private IPAddress serverIP = IPAddress.Any;

/// <summary>
/// Central network control.
/// </summary>
/// <param name="isHost">Sets the NET_Main to server or client mode</param>
/// <param name="timeStep">Heartbeat of the server</param>
/// <param name="name">Servername</param>
/// <param name="log"></param>
        public NET_Main(bool isHost, float timeStep, string name, bool log)
        {
            this.name = name;
            this.timeStep = timeStep;
            this.isHost = isHost;
            if (isHost == true)
            {
                this.playerID = 1;
            }
            if (log == false)
            {
                NET_Debug.DeactivateLogging();
            }

            playersGameData[0] = new NET_DataPack();
            playersGameData[1] = new NET_DataPack();
            playersGameData[2] = new NET_DataPack();
            playersGameData[3] = new NET_DataPack();
        }

        //---------------------------------------------------------------------------------------------//
        //Private functions:
        //---------------------------------------------------------------------------------------------//
        private void NET_MainThread()
        {
            if (isHost == true)
            {
                while (state != NET_State.NET_Shutdown)
                {
                    //Broadcast server infos
                    NET_Messages.NET_Message_ServerBroadcast message = new NET_Messages.NET_Message_ServerBroadcast(DateTime.Now);
                    NET_DataPack pack = new NET_DataPack();
                    pack.SetData("PlayerSlotsUsed", playerCount);
                    pack.SetData("ServerIPAddress", localIP.ToString());
                    pack.SetData("GameState", gameState);
                    pack.SetData("ServerName", name);
                    message.SetData(pack.SerializeData());
                    udpSystem.UDP_EnqueueMessage(message);

                    Thread.Sleep(200);
                }
            }
        }
        private void NET_ProcessMessage(NET_Messages.IMessage message)
        {
            //---------------------------------------------------------------------------------------------//
            //Message handling:
            //---------------------------------------------------------------------------------------------//
            if (message != null)
            {
                //Host message handling
                if (isHost == true)
                {
                    //Update message
                    if (message as NET_Messages.NET_Message_Update != null)
                    {
                        object objectBuffer = 0;
                        byte[] byteBuffer = new byte[1];
                        message.GetData(ref byteBuffer, ref objectBuffer, false);
                        NET_DataPack pack = new NET_DataPack(byteBuffer);

                        //Set update data to internal slots
                        int messagePID = (int)pack.GetData("PlayerID");
                        playersGameData[messagePID - 1] = pack;
                    }
                    //Event call message
                    else if (message as NET_Messages.NET_Message_EventCall != null)
                    {
                        object objectBuffer = 0;
                        byte[] byteBuffer = new byte[1];
                        message.GetData(ref byteBuffer, ref objectBuffer, false);
                        NET_DataPack pack = new NET_DataPack(byteBuffer);

                        //Add event to list
                        NET_EventCall call = new NET_EventCall(pack);
                        eventCalls.Add(call);
                    }
                    //Disconnect message
                    else if (message as NET_Messages.NET_Message_Disconnect != null)
                    {
                        object objectBuffer = 0;
                        byte[] byteBuffer = new byte[1];
                        message.GetData(ref byteBuffer, ref objectBuffer, false);
                        NET_DataPack pack = new NET_DataPack(byteBuffer);

                        int messagePID = (int)pack.GetData("PlayerID");
                        tcpServer.TCP_Server_ClientDisconnected(messagePID);
                    }
                }
                //Client message handling
                else
                {
                    //Server broadcast
                    if (message as NET_Messages.NET_Message_ServerBroadcast != null)
                    {
                        object objectBuffer = 0;
                        byte[] byteBuffer = new byte[1];
                        message.GetData(ref byteBuffer, ref objectBuffer, false);
                        NET_DataPack pack = new NET_DataPack(byteBuffer);

                        string ipAddress = (string)pack.GetData("ServerIPAddress");
                        int playerC = (int)pack.GetData("PlayerSlotsUsed");
                        string serName = (string)pack.GetData("ServerName");
                        GAME_State state = (GAME_State)pack.GetData("GameState");
                        if (serverInfos.ContainsKey(ipAddress) == false)
                        {
                            NET_ServerInfo info = new NET_ServerInfo(ipAddress, playerC, gameState, serName);
                            serverInfos.Add(ipAddress, info);
                        }
                        else
                        {
                            NET_ServerInfo info = serverInfos[ipAddress];
                            info.INFO_SetPlayerCount(playerC);
                            info.INFO_SetState(gameState);
                            serverInfos[ipAddress] = info;
                        }
                        //If server broadcast is from connected server --> set player count
                        if (ipAddress == this.serverIP.ToString())
                        {
                            this.playerCount = playerC;
                        }
                    }
                    //Update message
                    else if (message as NET_Messages.NET_Message_Update != null)
                    {
                        object objectBuffer = 0;
                        byte[] byteBuffer = new byte[1];
                        message.GetData(ref byteBuffer, ref objectBuffer, false);
                        NET_UberPack uberPack = new NET_UberPack(byteBuffer);

                        //Unpack uber pack
                        NET_DataPack packP1 = uberPack.GetPack(1);
                        NET_DataPack packP2 = uberPack.GetPack(2);
                        NET_DataPack packP3 = uberPack.GetPack(3);
                        NET_DataPack packP4 = uberPack.GetPack(4);

                        //Set update data to internal slots
                        if (packP1 != null && playerID != 1)
                        {
                            playersGameData[0] = packP1;
                        }
                        if (packP2 != null && playerID != 2)
                        {
                            playersGameData[1] = packP2;
                        }
                        if (packP3 != null && playerID != 3)
                        {
                            playersGameData[2] = packP3;
                        }
                        if (packP4 != null && playerID != 4)
                        {
                            playersGameData[3] = packP4;
                        }
                    }
                    //Event call message
                    else if (message as NET_Messages.NET_Message_EventCall != null)
                    {
                        object objectBuffer = 0;
                        byte[] byteBuffer = new byte[1];
                        message.GetData(ref byteBuffer, ref objectBuffer, false);
                        NET_DataPack pack = new NET_DataPack(byteBuffer);

                        NET_EventCall call = new NET_EventCall(pack);
                        eventCalls.Add(call);
                    }
                    //Welcome message --> Set playerID for clients
                    else if (message as NET_Messages.NET_Message_Welcome != null)
                    {
                        object objectBuffer = 0;
                        byte[] byteBuffer = new byte[1];
                        message.GetData(ref byteBuffer, ref objectBuffer, false);
                        NET_DataPack pack = new NET_DataPack(byteBuffer);

                        this.playerID = (int)pack.GetData("YourPlayerID");
                    }
                    //Disconnect message
                    else if (message as NET_Messages.NET_Message_Disconnect != null)
                    {
                        object objectBuffer = 0;
                        byte[] byteBuffer = new byte[1];
                        message.GetData(ref byteBuffer, ref objectBuffer, false);
                        NET_DataPack pack = new NET_DataPack(byteBuffer);

                        int messagePID = (int)pack.GetData("PlayerID");
                        if (messagePID == playerID)
                        {
                            tcpClient.TCP_Disconnected();
                            playerID = 0;
                        }
                    }
                }
            }
        }
        private void NET_GetLocalIP()
        {
            StringBuilder sb = new StringBuilder();
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            try
            {
                foreach (NetworkInterface network in networkInterfaces)
                {
                    OperationalStatus test = network.OperationalStatus;
                    if (test == OperationalStatus.Up)
                    {
                        // Read the IP configuration for each network 
                        IPInterfaceProperties properties = network.GetIPProperties();
                        // Each network interface may have multiple IP addresses 
                        foreach (IPAddressInformation address in properties.UnicastAddresses)
                        {
                            // We're only interested in IPv4 addresses for now 
                            if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                                continue;

                            // Ignore loopback addresses (e.g., 127.0.0.1) 
                            if (IPAddress.IsLoopback(address.Address))
                                continue;

                            // Filter not connected or virtual networks
                            if (properties.GatewayAddresses.Count == 0)
                                continue;

                            sb.AppendLine(address.Address.ToString());
                        }
                    }
                }
                string allIPs = sb.ToString();
                if (allIPs == "")
                {
                    Console.WriteLine("No active network device found");
                }
                else
                {
                    string test = allIPs.Substring(0, allIPs.IndexOf(Environment.NewLine));
                    try
                    {
                        localIP = IPAddress.Parse(test);
                    }
                    catch
                    {
                        Console.WriteLine("No active network device found");
                    }
                }
            }
            catch
            {
                //Dirty unity hack... Seems to work only at the second run! No idea why.
                NET_GetLocalIP();
            }
        }

        //---------------------------------------------------------------------------------------------//
        //Public functions:
        //---------------------------------------------------------------------------------------------//
        /// <summary>
        /// Function starts all [NET] related threads and systems
        /// </summary>
        /// <returns>
        /// Returns (true) if all systems were started successfully / returns (false) if not
        /// </returns>
        public bool NET_Start()
        {//Start all NET systems
            NET_GetLocalIP();
            if (localIP != null && state == NET_State.NET_Waiting)
            {
                //Start tcp systems
                if (isHost == true)
                {
                    tcpServer = new NET_TcpServer();
                    tcpServer.TCP_Server_Start();
                }
                else
                {
                    tcpClient = new NET_TcpClient();
                }
                //Start udp system
                udpSystem = new NET_Udp(isHost);
                udpSystem.UDP_Start();

                //Start main system
                state = NET_State.NET_Running;
                mainThread = new Thread(NET_MainThread);
                mainThread.Name = "[NET_Main] system thread";
                mainThread.Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Functions suspend all [NET] related threads and systems 
        /// </summary>
        /// <returns>
        /// Returns (true) if all systems were suspended successfully / returns (false) if not
        /// </returns>
        public bool NET_Stop()
        {//Suspend all NET systems
            if (state == NET_State.NET_Running)
            {
                //Suspend main system
                state = NET_State.NET_Waiting;
                //Suspend udp system
                udpSystem.UDP_Stop();
                //Suspend tcp system
                if (isHost == true)
                {
                    tcpServer.TCP_Server_Stop();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Function stops all [NET] related threads and systems (First call [NET_Stop] then [NET_Shutdown] on game quit)
        /// </summary>
        /// <returns>
        /// Returns (true) if all systems were stopped successfully / returns (false) if not
        /// </returns>
        public bool NET_Shutdown()
        {
            if (state == NET_State.NET_Waiting)
            {
                //Stop main system
                state = NET_State.NET_Shutdown;
                //Stop udp system
                udpSystem.UDP_Shutdown();
                //Stop tcp system
                if (isHost == true)
                {
                    tcpServer.TCP_Server_Shutdown();
                }
                else
                {
                    tcpClient.TCP_DisconnectFromServer(playerID);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Function returns local players ID (1-4)
        /// </summary>
        /// <returns>
        /// Returns int (1-4) refering to local players ID
        /// </returns>
        public int NET_GetPlayerID()
        {
            return this.playerID;
        }
        /// <summary>
        /// Function sets internal player data pack with param playerData
        /// This pack will be sent on next [NET_Update] call
        /// </summary>
        /// <param name="playerData">
        /// Param (playerData) a [NET_DataPack] representing current players data
        /// </param>
        public void NET_SetData(NET_DataPack playerData)
        {
            if (this.playerID != 0)
            {
                playersGameData[playerID - 1] = playerData;
            }
        }
        /// <summary>
        /// Function to call on every update cycle
        /// </summary>
        public void NET_Update()
        {
            //Record current time
            thisTime = DateTime.Now;

            //Process all enqueued udp messages
            NET_Messages.IMessage[] udpMessages = udpSystem.UDP_DequeueMessages();
            if (udpMessages != null)
            {
                foreach (NET_Messages.IMessage MS in udpMessages)
                {
                    NET_ProcessMessage(MS);
                }
            }

            //Process all enqueued tcp messages
            if (isHost == true)
            {
                this.playerCount = tcpServer.TCP_Server_GetPlayerCount();

                NET_Messages.IMessage[] playerSlot2Messages = tcpServer.TCP_DequeueMessages(2);
                if (playerSlot2Messages != null)
                {
                    foreach (NET_Messages.IMessage MS in playerSlot2Messages)
                    {
                        NET_ProcessMessage(MS);
                    }
                }
                NET_Messages.IMessage[] playerSlot3Messages = tcpServer.TCP_DequeueMessages(3);
                if (playerSlot3Messages != null)
                {
                    foreach (NET_Messages.IMessage MS in playerSlot3Messages)
                    {
                        NET_ProcessMessage(MS);
                    }
                }
                NET_Messages.IMessage[] playerSlot4Messages = tcpServer.TCP_DequeueMessages(4);
                if (playerSlot4Messages != null)
                {
                    foreach (NET_Messages.IMessage MS in playerSlot4Messages)
                    {
                        NET_ProcessMessage(MS);
                    }
                }
            }
            else
            {
                NET_Messages.IMessage[] playersMessages = tcpClient.TCP_DequeueMessages();
                if (playersMessages != null)
                {
                    foreach (NET_Messages.IMessage MS in playersMessages)
                    {
                        NET_ProcessMessage(MS);
                    }
                }
            }

            //TimeStepping V1
            float delta = thisTime.Subtract(lastTime).Milliseconds;
            if ((float)Math.Abs(delta) > timeStep)
            {
                this.lastTime = DateTime.Now;
                //Create update message V2
                //Old system:
                //3 messages in / 12 messages out
                //New system:
                //3 messages in / 3 messages out
                if (playerID != 0 && playerCount > 1)
                {
                    NET_Messages.NET_Message_Update update = new NET_Messages.NET_Message_Update(DateTime.Now);

                    if (isHost == true)
                    {
                        NET_DataPack packP1 = null;
                        NET_DataPack packP2 = null;
                        NET_DataPack packP3 = null;
                        NET_DataPack packP4 = null;

                        if (playersGameData[0] != null)
                        {
                            packP1 = playersGameData[0];
                            packP1.SetData("PlayerID", 1);
                        }
                        if (playersGameData[1] != null)
                        {
                            packP2 = playersGameData[1];
                            packP2.SetData("PlayerID", 2);
                        }
                        if (playersGameData[2] != null)
                        {
                            packP3 = playersGameData[2];
                            packP3.SetData("PlayerID", 3);
                        }
                        if (playersGameData[3] != null)
                        {
                            packP4 = playersGameData[3];
                            packP4.SetData("PlayerID", 4);
                        }

                        //Create uber-pack containing packs from all players (System 1 to reduce message count by factor 3 --> but rise message size by 4)
                        NET_UberPack uberPack = new NET_UberPack(packP1, packP2, packP3, packP4);
                        update.SetData(uberPack.SerializePacks());

                        //Enqueue update message
                        tcpServer.TCP_EnqueueMessage(update, 2);
                        tcpServer.TCP_EnqueueMessage(update, 3);
                        tcpServer.TCP_EnqueueMessage(update, 4);
                    }
                    else
                    {
                        if (playersGameData[playerID - 1] != null)
                        {
                            NET_DataPack pack = playersGameData[playerID - 1];
                            pack.SetData("PlayerID", playerID);
                            update.SetData(pack.SerializeData());

                            tcpClient.TCP_EnqueueMessage(update);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Function returns internal player data pack represented by param playerID
        /// </summary>
        /// <param name="playerID">
        /// Param (playerID) to get player data pack from
        /// </param>
        /// <returns>
        /// Returns most current player data pack of playerID
        /// </returns>
        public NET_DataPack NET_GetData(int playerID)
        {
            return playersGameData[playerID - 1];
        }
        /// <summary>
        /// Function to get an array of [NET_ServerInfo] containing informartion about available servers
        /// </summary>
        /// <returns>
        /// Returns array of [NET_ServerInfo] containing informartion about available servers
        /// </returns>
        public NET_ServerInfo[] NET_GetServerInfo()
        {
            int x = 0;
            NET_ServerInfo[] servers = new NET_ServerInfo[serverInfos.Count];
            foreach (KeyValuePair<string, NET_ServerInfo> entry in serverInfos)
            {
                servers[x] = entry.Value;
                x++;
            }
            return servers;
        }
        /// <summary>
        /// Function to collect and return all relevant status messages
        /// </summary>
        /// <returns>
        /// Returns string containing all relevant status messages from [NET] systems
        /// </returns>
        public string NET_GetStates()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/--------------------------------------------------/");
            sb.AppendLine("/----NET System Status Mail:-----------------------/");
            sb.AppendLine("/--------------------------------------------------/");
            sb.AppendLine("");
            if (isHost == true)
            {
                sb.AppendLine("{SERVER INFOS}:");
                sb.AppendLine("[IP] : (" + this.localIP + ")");
                sb.AppendLine("[MAIN] thread status: (" + this.state.ToString() + ")");
                sb.AppendLine(udpSystem.UDP_GetStatus());
                sb.AppendLine(tcpServer.TCP_Server_GetStatus());
                sb.AppendLine("[GAME STATE] (" + this.gameState.ToString() + ")");
            }
            else
            {
                sb.AppendLine("{CLIENT INFOS}");
                sb.AppendLine("[IP] : (" + this.localIP + ")");
                sb.AppendLine(udpSystem.UDP_GetStatus());
                sb.AppendLine(tcpClient.TCP_GetStatus());
                sb.AppendLine("[PLAYERID]: (" + this.playerID + ")");
                sb.AppendLine("");
                sb.AppendLine("[TCP] servers found:");
                if (serverInfos != null)
                {
                    foreach (KeyValuePair<string, NET_ServerInfo> NSI in serverInfos)
                    {
                        sb.AppendLine("[SERVER]: IP: " + NSI.Value.INFO_GetAddress() + "/PC: " + NSI.Value.INFO_GetPlayerCount() + "/GS: " + NSI.Value.INFO_GetState() + "/SN: " + NSI.Value.INFO_GetServerName());
                    }
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Function to build a connection to a specified server
        /// </summary>
        /// <param name="serverInfo">
        /// Param (serverInfo) a [NET_ServerInfo] containing information about server to connect to
        /// </param>
        public bool NET_ConnectToServer(NET_ServerInfo serverInfo)
        {
            //Try to connect to server
            bool result = this.tcpClient.TCP_ConnectToServer(serverInfo.INFO_GetAddress());
            if (result == true)
            {
                this.serverIP = serverInfo.INFO_GetAddress();
            }
            return result;
        }
        /// <summary>
        /// Function to disconnect form current server
        /// </summary>
        public void NET_DisconnectFromServer()
        {
            //Inform host about your disconnect
            if (isHost == false && playerID != 0)
            {
                tcpClient.TCP_DisconnectFromServer(playerID);
                this.playerID = 0;
            }
        }
        /// <summary>
        /// Function to call an event on server/client
        /// If machine works as server:
        /// The event will be sent to all clients
        /// If machine works as client:
        /// The event will be sent to server only
        /// </summary>
        /// <param name="eventCall">
        /// Param (eventCall) the event to call
        /// </param>
        public void NET_CallEvent(NET_EventCall eventCall)
        {
            if (isHost == true)
            {
                NET_Messages.NET_Message_EventCall callMessage = new NET_Messages.NET_Message_EventCall(DateTime.Now);
                callMessage.SetData(eventCall.GetEventData().SerializeData());
                this.tcpServer.TCP_EnqueueMessage(callMessage, 2);
                this.tcpServer.TCP_EnqueueMessage(callMessage, 3);
                this.tcpServer.TCP_EnqueueMessage(callMessage, 4);
            }
            else
            {
                NET_Messages.NET_Message_EventCall callMessage = new NET_Messages.NET_Message_EventCall(DateTime.Now);
                callMessage.SetData(eventCall.GetEventData().SerializeData());
                this.tcpClient.TCP_EnqueueMessage(callMessage);
            }
        }
        /// <summary>
        /// Function to get all called events of this machine
        /// </summary>
        /// <returns>
        /// Returns all called events
        /// </returns>
        public List<NET_EventCall> NET_GetEventCalls()
        {
            List<NET_EventCall> temp = new List<NET_EventCall>(this.eventCalls);
            this.eventCalls.Clear();
            return temp;
        }
        /// <summary>
        /// Function to get the current client count (without host)
        /// </summary>
        /// <returns>
        /// Returns current client count
        /// </returns>
        public int NET_GetClientCount()
        {
            return this.tcpServer.TCP_Server_GetPlayerCount() - 1;
        }
        /// <summary>
        /// Function to transmit a command to NET system
        /// </summary>
        /// <param name="command">
        /// Param (command) the command to call
        /// </param>
        /// <returns>
        /// Returns a bool, if command was handles successfully or not
        /// </returns>
        public bool NET_CommandServer(NET_ServerCommand command)
        {
            if (command == NET_ServerCommand.COM_BlockNewClients)
            {
                this.tcpServer.TCP_Server_Command("BlockNewClients");
                return true;
            }
            else if (command == NET_ServerCommand.COM_AllowNewClients)
            {
                this.tcpServer.TCP_Server_Command("AllowNewClients");
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Function to set current gamestate, this gamestate will be broadcasted
        /// </summary>
        /// <param name="newState">
        /// Param (newState) the gamestate to set
        /// </param>
        public void NET_SetGameState(GAME_State newState)
        {
            this.gameState = newState;
        }

        private enum NET_State
        {
            NET_Waiting,
            NET_Running,
            NET_Shutdown
        }
    }
}
