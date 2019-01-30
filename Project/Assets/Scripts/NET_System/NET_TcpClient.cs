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
    public class NET_TcpClient
    {
        private int TCPPort = 11001;
        private NET_TcpConnection connectionToServer;
        private TCP_Status status = TCP_Status.TCP_Disconnected;

        //---------------------------------------------------------------------------------------------//
        //Public functions:
        //---------------------------------------------------------------------------------------------//
        public void TCP_SetPort(int port)
        {
            this.TCPPort = port;
        }
        public string TCP_GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TCP_Status: " + status.ToString());
            if (connectionToServer != null)
            {
                sb.AppendLine(connectionToServer.GetStatus());
            }
            else
            {
                sb.AppendLine("TCPC_Status: null");
            }
            return sb.ToString();
        }
        public bool TCP_ConnectToServer(IPAddress serverIP)
        {
            if (status == TCP_Status.TCP_Disconnected)
            {
                TcpClient client = new TcpClient(AddressFamily.InterNetwork);
                //Try to connect to server
                var result = client.BeginConnect(serverIP, TCPPort, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));

                //Connection failed
                if (!success)
                {
                    return false;
                }

                //Connection build
                client.EndConnect(result);
                NET_TcpConnection connection = new NET_TcpConnection(client);
                connection.Connected();
                connectionToServer = connection;
                status = TCP_Status.TCP_Connected;
                return true;
            }
            else return false;
        }
        public bool TCP_DisconnectFromServer(int playerID)
        {
            if (status == TCP_Status.TCP_Connected)
            {
                connectionToServer.Disconnect(playerID);
                status = TCP_Status.TCP_Disconnected;
                return true;
            }
            else return false;
        }
        public bool TCP_Disconnected()
        {
            if (status == TCP_Status.TCP_Connected)
            {
                connectionToServer.Disconnected();
                status = TCP_Status.TCP_Disconnected;
                return true;
            }
            else return false;
        }
        public void TCP_EnqueueMessage(NET_Messages.IMessage message)
        {
            connectionToServer.TCP_EnqueueMessage(message);
        }
        public NET_Messages.IMessage[] TCP_DequeueMessages()
        {
            if (status == TCP_Status.TCP_Connected)
            {
                return this.connectionToServer.TCP_DequeueMessages();
            }
            else return new NET_Messages.IMessage[0];
        }

        //TCP status enum
        public enum TCP_Status
        {
            TCP_Connected,
            TCP_Disconnected
        }
    }
}
