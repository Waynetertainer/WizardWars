//Class to strore / read game data (independent of data type)
//Class to serialize / deserialize internal dictionary

//Update V2 (dynamic dictionary key name length added)
//Final V1 (testing was successful) (17.01.2014)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NET_Multiplayer
{
    public class NET_DataPack
    {
        private Dictionary<string, object> gameData = new Dictionary<string, object>();

        //---------------------------------------------------------------------------------------------//
        //Empty Constructor:
        //---------------------------------------------------------------------------------------------//
        public NET_DataPack()
        {
        }
        //---------------------------------------------------------------------------------------------//
        //Serialized Constructor:
        //---------------------------------------------------------------------------------------------//
        public NET_DataPack(byte[] serialized)
        {
            DeSerializeData(serialized);
        }


        //---------------------------------------------------------------------------------------------//
        //Public functions:
        //---------------------------------------------------------------------------------------------//

        //Function to set or update value within dictionary
        public void SetData(string dataName, object data)
        {
            if (dataName != "" && dataName != null)
            {
                if (gameData.ContainsKey(dataName) == true)
                {
                    gameData[dataName] = data;
                }
                else
                {
                    gameData.Add(dataName, data);
                }
            }
            else
            {
                Console.WriteLine("ERROR: Key name = null");
            }
        }

        //Function to get and return a value within dictionary
        public object GetData(string dataName)
        {
            if (gameData.ContainsKey(dataName) == true)
            {
                return gameData[dataName];
            }
            else
            {
                return null;
            }
        }

        //Function to clear dictionary
        public void ClearData()
        {
            gameData.Clear();
        }

        //Function to serialize data within dictionary
        public byte[] SerializeData()
        {
            byte[] pack = new byte[0];

            BinaryFormatter binForm = new BinaryFormatter();
            foreach (KeyValuePair<string, object> NET_GDG in gameData)
            {
                // Byte data structure: (k = Key)
                // [0 0 0 0 (int32)| 1 -  x | 0 0 0 0 (int32)| 1 -  x]
                // [k.Name.lenght  | k.Name | k.Data.lenght  | k.Data]

                //Serialize dictionary entry data
                byte[] keyData;
                using (MemoryStream ms = new MemoryStream())
                {
                    binForm.Serialize(ms, NET_GDG.Value);
                    keyData = ms.ToArray();
                }
                byte[] keyName = Encoding.UTF8.GetBytes(NET_GDG.Key);
                byte[] keyNameLength = BitConverter.GetBytes((int)keyName.Length);
                byte[] keyDataLength = BitConverter.GetBytes((int)keyData.Length);

                //Combine to final structure
                byte[] keyDataPair = new byte[keyName.Length + 4 + keyData.Length + 4];
                keyNameLength.CopyTo(keyDataPair, 0);
                keyName.CopyTo(keyDataPair, 4);
                keyDataLength.CopyTo(keyDataPair, keyName.Length + 4);
                keyData.CopyTo(keyDataPair, keyName.Length + 8);

                //Copy into final array
                byte[] buffer = pack;
                pack = new byte[buffer.Length + keyDataPair.Length];
                buffer.CopyTo(pack, 0);
                keyDataPair.CopyTo(pack, buffer.Length);
            }
            if (pack.Length != 0)
            {
                //Return serialized data if successful
                return pack;
            }
            else
            {
                //Return null if serialization failed
                return null;
            }
        }

        //Function to deserialize data to dictionary
        public void DeSerializeData(byte[] serialized)
        {
            BinaryFormatter binForm = new BinaryFormatter();
            byte[] pack = serialized;
            while (pack.Length > 0)
            {
                //Get key name and its length
                int keyNameLength = BitConverter.ToInt32(pack, 0);
                string keyName = Encoding.UTF8.GetString(pack, 4, keyNameLength);

                //Get key data and its length
                int keyDataLength = BitConverter.ToInt32(pack, keyNameLength + 4);
                byte[] keyDataSer = new byte[keyDataLength];
                Array.Copy(pack, keyNameLength + 8 ,keyDataSer, 0, keyDataLength);

                //Deserialize key data
                object keyData;
                using (MemoryStream memStream = new MemoryStream())
                {
                    memStream.Write(keyDataSer, 0, keyDataSer.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    keyData = (object)binForm.Deserialize(memStream);
                }

                //Add deserialiazed data to dictionary
                if (gameData.ContainsKey(keyName) == true)
                {
                    gameData[keyName] = keyData;
                }
                else
                {
                    gameData.Add(keyName, keyData);
                }

                //Cut off deserialized key value pair
                byte[] rest = new byte[pack.Length - keyNameLength - keyDataLength - 8];
                Array.Copy(pack, (keyNameLength + keyDataLength + 8), rest, 0, (pack.Length - keyNameLength - keyDataLength - 8));
                pack = rest;
            }
        }
    }
}
