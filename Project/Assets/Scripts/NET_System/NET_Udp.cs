//Class to encapsulate UDP protocol related functions
//Class handles broadcasting / receiving of messages automatically
//Class is compatible to NET_Message interface
//Class uses one thread, whitch is controlled internally
//Class is handling deserialization of incoming messages

//Update V2 (Change to dynamic event based system --> needs testing) (15.01.2014)
//Update V3 (<-- revert, event system uselessly complicated --> locking global queues) (19.01.2014)
//Update V4 (changed queues to ConcurrentQueue --> No locking needed? = faster?)
//Update V5 (<-- revert, queues instad of concurrent queues (Unity only supports .Net profile 2.0)

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NET_System;

namespace NET_Multiplayer
{
    public class NET_Udp
    {
        private Queue<NET_Messages.IMessage> udpIN_MessageQueue = new Queue<NET_Messages.IMessage>();
        private Queue<NET_Messages.IMessage> udpOUT_MessageQueue = new Queue<NET_Messages.IMessage>();

        private UDP_Status status = UDP_Status.UDP_Waiting;
        private bool isHost = false;
        private UdpClient udpClient = new UdpClient();
        private IPEndPoint endPoint;
        private int udpPort = 11000;
        private Thread udpMainThread;

        //---------------------------------------------------------------------------------------------//
        //Constructor:
        //---------------------------------------------------------------------------------------------//
        public NET_Udp(bool isHost)
        {
            this.isHost = isHost;
            if (isHost == false)
            {
                endPoint = new IPEndPoint(IPAddress.Any, udpPort);
                udpClient.EnableBroadcast = true;
                udpClient.Client.Bind(endPoint);
            }
            else
            {
                endPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), udpPort);
                udpClient.EnableBroadcast = true;
            }
            udpMainThread = new Thread(UDP_MainThread);
            udpMainThread.Name = "[NET_UDP] system thread";
            udpMainThread.Start();
        }

        //---------------------------------------------------------------------------------------------//
        //Private functions:
        //---------------------------------------------------------------------------------------------//
        private void UDP_Broadcast(byte[] message)
        {
            //Header constructor [0 0 0 0 | 65 81 69 88 | lenght]
            //                   [check 0 | check AQEX  | lenght]
            byte[] header = new byte[12];
            header[0] = 0;
            header[1] = 0;
            header[2] = 0;
            header[3] = 0;
            header[4] = 65;//A
            header[5] = 81;//Q
            header[6] = 69;//E
            header[7] = 88;//X
            BitConverter.GetBytes((int)message.Length).CopyTo(header, 8);//Length of main message

            //Send header
            udpClient.Send(header, header.Length, endPoint);

            //Send message
            udpClient.Send(message, message.Length, endPoint);
        }
        private void UDP_Listen()
        {
            if (udpClient.Available >= 12)
            {
                byte[] buffer = udpClient.Receive(ref endPoint);
                byte[] header = new byte[12];
                byte[] message = new byte[1];//Placeholder

                Array.Copy(buffer, 0, header, 0, 12);//Copy bytes from buffer[] to header[]

                //Check if message is valid
                if (header[0] == 0 && header[1] == 0 && header[2] == 0 && header[3] == 0)
                {
                    if (header[4] == 65 && header[5] == 81 && header[6] == 69 && header[7] == 88)
                    {
                        int mainMessageLength = BitConverter.ToInt32(header, 8);
                        message = new byte[mainMessageLength];
                        int rest = mainMessageLength;
                        if (buffer.Length - 12 > 0)
                        {
                            //Looks like this never happens. Seems like (receive) handles amount of received bytes automatically.
                            //Copy rest of message bytes from buffer[] to message[]
                            if (buffer.Length - 12 <= rest)
                            {
                                Array.Copy(buffer, 0, message, 12, buffer.Length - 12);
                                rest -= buffer.Length - 12;
                            }
                            else
                            {
                                Array.Copy(buffer, 0, message, 12, rest);
                                rest = 0;
                            }
                        }
                        while (rest > 0)
                        {//Receive rest of message data
                            if (udpClient.Available > 0)
                            {
                                buffer = null;
                                buffer = udpClient.Receive(ref endPoint);
                                if (buffer.Length <= rest)
                                {
                                    Array.Copy(buffer, 0, message, mainMessageLength - rest, buffer.Length);
                                    rest -= buffer.Length;
                                }
                                else
                                {
                                    //Looks like this never happens. (See above)
                                    Array.Copy(buffer, 0, message, mainMessageLength - rest, rest);
                                    rest = 0;
                                }
                            }
                        }
                        //Deserialize binary data
                        NET_Messages.IMessage deserialized = UDP_Deserialize(message);

                        //Enqueue message to global queue
                        if (deserialized != null)
                        {
                            lock (udpIN_MessageQueue)
                            {
                                udpIN_MessageQueue.Enqueue(deserialized);    
                            }
                        }         
                    }
                }
            }
        }
        private void UDP_MainThread()
        {
            while (status != UDP_Status.UDP_Shutdown)
            {
                while (status == UDP_Status.UDP_Running)
                {
                    //Host is only broadcasting
                    if (isHost == true)
                    {
                        if (udpOUT_MessageQueue.Count > 0)
                        {
                            lock (udpOUT_MessageQueue)
                            {
                                while (udpOUT_MessageQueue.Count > 0)
                                {
                                    NET_Messages.IMessage message = udpOUT_MessageQueue.Dequeue();
                                    UDP_Broadcast(UDP_Serialize(message));
                                }
                            }
                        }
                    }
                    //Client is only listening
                    else
                    {
                        UDP_Listen();
                    }
                    Thread.Sleep(60);
                }
                Thread.Sleep(250);
            }
            udpClient.Client.Close();
            udpClient.Close();
            //Game is closing! --> Exit thread
        }
        private NET_Messages.IMessage UDP_Deserialize(byte[] messageData)
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
                return null;
            }
        }
        private byte[] UDP_Serialize(NET_Messages.IMessage message)
        {
            
            BinaryFormatter binForm = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                binForm.Serialize(ms, message);
                return ms.ToArray();
            }
        }

        //---------------------------------------------------------------------------------------------//
        //Public functions:
        //---------------------------------------------------------------------------------------------//
        public void UDP_SetPort(int port)
        {
            this.udpPort = port;
        }
        public string UDP_GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UDP_Status: " + status.ToString());
            sb.AppendLine("UDP_QIN: " + udpIN_MessageQueue.Count.ToString());
            sb.AppendLine("UDP_QOUT: " + udpOUT_MessageQueue.Count.ToString());
            return sb.ToString();
        }
        public void UDP_EnqueueMessage(NET_Messages.IMessage message)
        {
            lock (udpOUT_MessageQueue)
            {
                udpOUT_MessageQueue.Enqueue(message);
            }
        }
        public NET_Messages.IMessage[] UDP_DequeueMessages()
        {
            lock (udpIN_MessageQueue)
            {
                NET_Messages.IMessage[] messages = udpIN_MessageQueue.ToArray();
                udpIN_MessageQueue.Clear();
                return messages;
            }
        }
        public void UDP_Stop()
        {
            if (this.status == UDP_Status.UDP_Running)
            {
                this.status = UDP_Status.UDP_Waiting;
            }
        }
        public void UDP_Start()
        {
            if (this.status == UDP_Status.UDP_Waiting)
            {
                this.status = UDP_Status.UDP_Running;
            }
        }
        public void UDP_Shutdown()
        {
            if (this.status == UDP_Status.UDP_Waiting)
            {
                this.status = UDP_Status.UDP_Shutdown;
            }
        }

        //UDP status enum
        public enum UDP_Status
        {
            UDP_Waiting,
            UDP_Running,
            UDP_Shutdown
        }
    }
}
