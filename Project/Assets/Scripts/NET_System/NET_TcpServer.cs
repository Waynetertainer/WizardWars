using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NET_System;

namespace NET_Multiplayer_V3
{
    public class NET_TcpServer
    {
        private bool blockNewClients = false;
        private int clientCount = 0;
        private TCP_Status serverStatus = TCP_Status.TCP_Shutdown;

        private TcpListener server;

        private int TCPPort = 11001;
        private Thread TCPMainThread;
        private NET_TcpConnection[] connections = new NET_TcpConnection[3];

        public NET_TcpServer()
        {
            this.server = new TcpListener(IPAddress.Any, TCPPort);
            this.server.Start();
        }

        //---------------------------------------------------------------------------------------------//
        //Private functions:
        //---------------------------------------------------------------------------------------------//
        private void TCP_MainThread()
        {
            while (serverStatus != TCP_Status.TCP_Shutdown)
            {
                while (serverStatus == TCP_Status.TCP_Running)
                {
                    //Connect to pending connections
                    if (clientCount < 3)
                    {
                        int emptySlot = -1;
                        if (connections[0] == null)
                        {
                            emptySlot = 0;
                        }
                        else if (connections[1] == null)
                        {
                            emptySlot = 1;
                        }
                        else if (connections[2] == null)
                        {
                            emptySlot = 2;
                        }
                        if (this.server.Pending() && blockNewClients == false)
                        {
                            //Build welcome message for new player
                            NET_Messages.NET_Message_Welcome welcomeMessage = new NET_Messages.NET_Message_Welcome(DateTime.Now);
                            NET_DataPack pack = new NET_DataPack();

                            pack.SetData("YourPlayerID", emptySlot + 2);
                            welcomeMessage.SetData(pack.SerializeData());

                            //Build new connection to player
                            TcpClient client = server.AcceptTcpClient();
                            connections[emptySlot] = new NET_TcpConnection(client);
                            connections[emptySlot].Connected();
                            //Enqueue welcome message
                            connections[emptySlot].TCP_EnqueueMessage(welcomeMessage);
                            clientCount++;
                        }
                    }
                    //Delete disconnected clients
                    for (int x = 0; x < 3; x++)
                    {
                        if (connections[x] != null)
                        {
                            if (connections[x].GetState() == "TCPC_DeleteMe")
                            {
                                connections[x] = null;
                                clientCount--;
                            }
                        }
                    }
                    Thread.Sleep(20);
                }
                //Delete disconnected clients
                for (int x = 0; x < 3; x++)
                {
                    if (connections[x] != null)
                    {
                        if (connections[x].GetState() == "TCPC_DeleteMe")
                        {
                            connections[x] = null;
                            clientCount--;
                        }
                    }
                }
                Thread.Sleep(10);
            }
            server.Stop();
            //Game is closing! --> Exit thread
        }

        //---------------------------------------------------------------------------------------------//
        //Public functions:
        //---------------------------------------------------------------------------------------------//
        public void TCP_Server_Start()
        {
            if (serverStatus == TCP_Status.TCP_Shutdown)
            {
                this.serverStatus = TCP_Status.TCP_Running;
                TCPMainThread = new Thread(TCP_MainThread);
                TCPMainThread.Name = "[NET_TCP] server thread";
                TCPMainThread.Start();
            }
        }
        public void TCP_Server_Stop()
        {
            if (serverStatus == TCP_Status.TCP_Running)
            {
                //Send disconnect to all clients
                for (int x = 0; x < 3; x++)
                {
                    if (connections[x] != null)
                    {
                        if (connections[x].GetState() == "TCPC_Connected")
                        {
                            connections[x].Disconnect(x + 2);
                        }
                    }
                }
                serverStatus = TCP_Status.TCP_Waiting;
            }
        }
        public void TCP_Server_Shutdown()
        {
            if (serverStatus == TCP_Status.TCP_Waiting)
            {
                serverStatus = TCP_Status.TCP_Shutdown;
            }
        }
        public void TCP_Server_ClientDisconnected(int playerID)
        {
            connections[playerID - 2].Disconnected();
        }
        public int TCP_Server_GetPlayerCount()
        {
            return this.clientCount +1;
        }
        public void TCP_EnqueueMessage(NET_Messages.IMessage message, int playerID)
        {
            if (playerID != 1 && message != null)
            {
                if (connections[playerID - 2] != null)
                {
                    this.connections[playerID - 2].TCP_EnqueueMessage(message);
                }
            }
        }
        public NET_Messages.IMessage[] TCP_DequeueMessages(int playerID)
        {
            if (playerID != 1)
            {
                if (connections[playerID - 2] != null)
                {
                    return this.connections[playerID - 2].TCP_DequeueMessages();
                }
                else return new NET_Messages.IMessage[0];
            }
            else return new NET_Messages.IMessage[0];
        }
        public string TCP_Server_GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TCPS_Status: " + this.serverStatus.ToString());
            sb.AppendLine("TCPS_Clients: " + this.clientCount.ToString());
            if (connections[0] != null)
            {
                sb.AppendLine("TCPS_C0: Connected");
                sb.AppendLine(connections[0].GetStatus());
            }
            else
            {
                sb.AppendLine("TCPS_C0: null");
            }
            if (connections[1] != null)
            {
                sb.AppendLine("TCPS_C1: Connected");
                sb.AppendLine(connections[1].GetStatus());
            }
            else
            {
                sb.AppendLine("TCPS_C1: null");
            }
            if (connections[2] != null)
            {
                sb.AppendLine("TCPS_C2: Connected");
                sb.AppendLine(connections[2].GetStatus());
            }
            else
            {
                sb.AppendLine("TCPS_C2: null");
            }
            
            return sb.ToString();
        }
        public void TCP_Server_Command(string command)
        {
            if (command == "BlockNewClients")
            {
                this.blockNewClients = true;
            }
            else if (command == "AllowNewClients")
            {
                this.blockNewClients = false;
            }
        }

        //TCP status enum
        public enum TCP_Status
        {
            TCP_Waiting,
            TCP_Running,
            TCP_Shutdown
        }
    }
}
