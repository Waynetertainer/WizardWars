using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using NET_System;

namespace NET_Multiplayer
{
    public class NET_TcpConnection
    {
        private Queue<NET_Messages.IMessage> tcpIN_MessageQueue = new Queue<NET_Messages.IMessage>();
        private Queue<NET_Messages.IMessage> tcpOUT_MessageQueue = new Queue<NET_Messages.IMessage>();
        private Queue<NET_Messages.IMessage> tcpSEN_MessageQueue = new Queue<NET_Messages.IMessage>();

        private int sendRetryCount = 0;
        private TCP_Connection_State state = TCP_Connection_State.TCPC_Disconnected;
        private Thread thread;
        private TcpClient client;
        private NetworkStream stream;

        //---------------------------------------------------------------------------------------------//
        //Constructor:
        //---------------------------------------------------------------------------------------------//
        public NET_TcpConnection(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
            client.ReceiveTimeout = 100;
            client.SendTimeout = 100;
            client.NoDelay = true;
        }

        //---------------------------------------------------------------------------------------------//
        //Private functions:
        //---------------------------------------------------------------------------------------------//
        private void TCP_ConnectionThread()
        {
            NET_Debug.InjectMessage("[NET_TCP_CN]", "[OK] Thread started");
            //While connection established
            while (state == TCP_Connection_State.TCPC_Connected)
            {
                TCP_Receive();
                TCP_Sending();
            }
            client.Client.Disconnect(true);
            stream.Close();
            stream.Dispose();
            client.Close();
            NET_Debug.InjectMessage("[NET_TCP_CN]", "[OK] Thread ended");
            this.state = TCP_Connection_State.TCPC_DeleteMe;
        }
        private void TCP_Receive()
        {
            if (stream.DataAvailable == true)
            {
                try
                {
                    byte[] messageData = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //Read byte size of message
                        byte[] header = new byte[8];
                        byte[] message = new byte[1];

                        this.stream.Read(header, 0, 8);
                        if (header[0] == 65 && header[1] == 81 && header[2] == 69 && header[3] == 88)
                        {
                            int totalSize = BitConverter.ToInt32(header, 4);
                            int size = totalSize;

                            //Read main message
                            while (size > 0)
                            {
                                byte[] buffer = new byte[size];
                                int received = this.stream.Read(buffer, 0, size);
                                ms.Write(buffer, 0, received);
                                size -= received;
                            }
                            messageData = ms.ToArray();
                        }
                        else
                        {
                            NET_Debug.InjectMessage("[NET_TCP_CN]", "[ERROR] Wrong header");
                        }
                    }
                    //Deserialize message
                    if (messageData != null)
                    {
                        NET_Messages.IMessage deserialized = TCP_Deserialize(messageData);
                        if (deserialized != null)
                        {
                            lock (tcpIN_MessageQueue)
                            {
                                tcpIN_MessageQueue.Enqueue(deserialized);
                            }
                        }
                    }
                }
                catch
                {
                    NET_Debug.InjectMessage("[NET_TCP_CN]", "[ERROR] Receiving");
                    //Nothing to do here...
                }
            }
            Thread.Sleep(1);
        }
        private void TCP_Sending()
        {
            NET_Messages.IMessage message;

            if (tcpOUT_MessageQueue.Count > 0)
            {
                //Copy out queue to sen queue
                lock (tcpOUT_MessageQueue)
                {
                    tcpSEN_MessageQueue = new Queue<NET_Messages.IMessage>(tcpOUT_MessageQueue);
                    tcpOUT_MessageQueue.Clear();
                }

                lock (tcpSEN_MessageQueue)
                {
                    while (tcpSEN_MessageQueue.Count > 0)
                    {
                        message = tcpSEN_MessageQueue.Dequeue();
                        byte[] messageData = TCP_Serialize(message);

                        if (messageData != null)
                        {
                            sendRetryCount = 0;
                            bool retryResult = false;

                            //Sending first try...
                            retryResult = TCP_Send(messageData);

                            //Sending failed --> Retry!
                            while (retryResult == false && sendRetryCount < 5)
                            {
                                retryResult = TCP_Send(messageData);
                                sendRetryCount++;
                                NET_Debug.InjectMessage("[NET_TCP_CN]", "[ERROR] Senging: Retrying " + sendRetryCount);
                                Thread.Sleep(1);
                            }
                            if (sendRetryCount == 5)
                            {
                                //This is bad! The message was not sendable
                                //--> Target disconnected?
                                NET_Debug.InjectMessage("[NET_TCP_CN]", "[FATAL ERROR] Senging");
                            }
                        }
                        else
                        {
                            //This is really bad! The message was not serializable...
                        }
                        Thread.Sleep(1);
                    }
                }
            }
        }
        private bool TCP_Send(byte[] messageData)
        {
            try
            {
                byte[] header = new byte[8];
                header[0] = 65;//A
                header[1] = 81;//Q
                header[2] = 69;//E
                header[3] = 88;//X
                byte[] messageLength = BitConverter.GetBytes((int)messageData.Length);
                Array.Copy(messageLength, 0, header, 4, 4);

                stream.Write(header, 0, 8);
                stream.Write(messageData, 0, messageData.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private NET_Messages.IMessage TCP_Deserialize(byte[] messageData)
        {
            try
            {
                BinaryFormatter binForm = new BinaryFormatter();
                using (MemoryStream memStream = new MemoryStream())
                {
                    memStream.Write(messageData, 0, messageData.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    object obj = (object)binForm.Deserialize(memStream);
                    return obj as NET_Messages.IMessage;
                }
            }
            catch
            {
                NET_Debug.InjectMessage("[NET_TCP_CN]", "[FATAL ERROR] Deserializing failed");
                return null;
            }
        }
        private byte[] TCP_Serialize(NET_Messages.IMessage message)
        {
            try
            {
                BinaryFormatter binForm = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    binForm.Serialize(ms, message);
                    return ms.ToArray();
                }
            }
            catch
            {
                NET_Debug.InjectMessage("[NET_TCP_CN]", "[FATAL ERROR] Serializing failed");
                return null;
            }
        }

        //---------------------------------------------------------------------------------------------//
        //Public functions:
        //---------------------------------------------------------------------------------------------//
        public void Connected()
        {
            NET_Debug.InjectMessage("[NET_TCP_CN]", "[OK] Connected");
            state = TCP_Connection_State.TCPC_Connected;
            this.thread = new Thread(TCP_ConnectionThread);
            this.thread.Name = "[NET_TCP] connection thread";
            this.thread.Start();
        }
        public void Disconnect(int playerID)
        {
            if (state == TCP_Connection_State.TCPC_Connected)
            {
                NET_Debug.InjectMessage("[NET_TCP_CN]", "[OK] Disconnected");
                NET_Messages.NET_Message_Disconnect disconnectMessage = new NET_Messages.NET_Message_Disconnect(DateTime.Now);
                NET_DataPack pack = new NET_DataPack();
                pack.SetData("PlayerID", playerID);
                disconnectMessage.SetData(pack.SerializeData());
                lock (tcpOUT_MessageQueue)
                {
                    tcpOUT_MessageQueue.Enqueue(disconnectMessage);
                }
                TCP_Sending();
                this.state = TCP_Connection_State.TCPC_Disconnected;
            }
        }
        public void Disconnected()
        {
            if (state == TCP_Connection_State.TCPC_Connected)
            {
                this.state = TCP_Connection_State.TCPC_Disconnected;
            }
        }
        public void TCP_EnqueueMessage(NET_Messages.IMessage message)
        {
            lock (tcpOUT_MessageQueue)
            {
                tcpOUT_MessageQueue.Enqueue(message);
            }
        }
        public NET_Messages.IMessage[] TCP_DequeueMessages()
        {
            lock (tcpIN_MessageQueue)
            {
                NET_Messages.IMessage[] messages = tcpIN_MessageQueue.ToArray();
                tcpIN_MessageQueue.Clear();
                return messages;
            }
        }
        public string GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TCPC_Status: " + state.ToString());
            sb.AppendLine("TCPC_QIN: " + tcpIN_MessageQueue.Count.ToString());
            sb.AppendLine("TCPC_QOUT: " + tcpOUT_MessageQueue.Count.ToString());
            return sb.ToString();
        }
        public string GetState()
        {
            return this.state.ToString();
        }

        public enum TCP_Connection_State
        {
            TCPC_Connected,
            TCPC_Disconnected,
            TCPC_DeleteMe
        }
    }
}
