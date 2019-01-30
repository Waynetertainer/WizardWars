//One package to rule them all...

using System;
using System.Collections.Generic;
using System.Text;
using NET_System;
using NET_Multiplayer;

namespace NET_System
{
    public class NET_UberPack
    {
        private NET_DataPack packP1 = null;
        private NET_DataPack packP2 = null;
        private NET_DataPack packP3 = null;
        private NET_DataPack packP4 = null;

        private byte[] combinedPacks;

        //---------------------------------------------------------------------------------------------//
        //Constructor:
        //---------------------------------------------------------------------------------------------//
        public NET_UberPack(byte[] serialized)
        {
            this.combinedPacks = serialized;
            int readPos = 0;

            int pack1Length = BitConverter.ToInt32(combinedPacks, readPos);
            readPos += 4;
            byte[] packP1b = new byte[pack1Length];
            Array.Copy(combinedPacks, readPos, packP1b, 0, pack1Length);
            readPos += pack1Length;
            if (packP1b.Length > 0)
            {
                packP1 = new NET_DataPack(packP1b);
            }

            int pack2Length = BitConverter.ToInt32(combinedPacks, readPos);
            readPos += 4;
            byte[] packP2b = new byte[pack2Length];
            Array.Copy(combinedPacks, readPos, packP2b, 0, pack2Length);
            readPos += pack2Length;
            if (packP2b.Length > 0)
            {
                packP2 = new NET_DataPack(packP2b);
            }

            int pack3Length = BitConverter.ToInt32(combinedPacks, readPos);
            readPos += 4;
            byte[] packP3b = new byte[pack3Length];
            Array.Copy(combinedPacks, readPos, packP3b, 0, pack3Length);
            readPos += pack3Length;
            if (packP3b.Length > 0)
            {
                packP3 = new NET_DataPack(packP3b);
            }

            int pack4Length = BitConverter.ToInt32(combinedPacks, readPos);
            readPos += 4;
            byte[] packP4b = new byte[pack4Length];
            Array.Copy(combinedPacks, readPos, packP4b, 0, pack4Length);
            if (packP4b.Length > 0)
            {
                packP4 = new NET_DataPack(packP4b);
            }
        }
        public NET_UberPack(NET_DataPack packP1, NET_DataPack packP2, NET_DataPack packP3, NET_DataPack packP4)
        {
            // Byte data structure:
            // [0 0 0 0 (int32)| 0 -  x | 0 0 0 0 (int32)| 0 -  x |... ]
            // [pack1Length    | packP1 | pack2Length    | packP2 |... ]

            this.packP1 = packP1;
            this.packP2 = packP2;
            this.packP3 = packP3;
            this.packP4 = packP4;

            byte[] serP1Length = new byte[4];
            byte[] serP1 = new byte[0];
            byte[] serP2Length = new byte[4];
            byte[] serP2 = new byte[0];
            byte[] serP3Length = new byte[4];
            byte[] serP3 = new byte[0];
            byte[] serP4Length = new byte[4];
            byte[] serP4 = new byte[0];

            if (this.packP1 != null)
            {
                serP1 = this.packP1.SerializeData();
            }
            if (this.packP2 != null)
            {
                serP2 = this.packP2.SerializeData();
            }
            if (this.packP3 != null)
            {
                serP3 = this.packP3.SerializeData();
            }
            if (this.packP4 != null)
            {
                serP4 = this.packP4.SerializeData();
            }

            serP1Length = BitConverter.GetBytes((int)serP1.Length);
            serP2Length = BitConverter.GetBytes((int)serP2.Length);
            serP3Length = BitConverter.GetBytes((int)serP3.Length);
            serP4Length = BitConverter.GetBytes((int)serP4.Length);

            this.combinedPacks = new byte[4 + serP1.Length + 4 + serP2.Length + 4 + serP3.Length + 4 + serP4.Length];
            int copyPos = 0;

            Array.Copy(serP1Length, 0, combinedPacks, copyPos, 4);
            copyPos += 4;
            Array.Copy(serP1, 0, combinedPacks, copyPos, serP1.Length);
            copyPos += serP1.Length;

            Array.Copy(serP2Length, 0, combinedPacks, copyPos, 4);
            copyPos += 4;
            Array.Copy(serP2, 0, combinedPacks, copyPos, serP2.Length);
            copyPos += serP2.Length;

            Array.Copy(serP3Length, 0, combinedPacks, copyPos, 4);
            copyPos += 4;
            Array.Copy(serP3, 0, combinedPacks, copyPos, serP3.Length);
            copyPos += serP3.Length;

            Array.Copy(serP4Length, 0, combinedPacks, copyPos, 4);
            copyPos += 4;
            Array.Copy(serP4, 0, combinedPacks, copyPos, serP4.Length);
        }

        //---------------------------------------------------------------------------------------------//
        //Public functions:
        //---------------------------------------------------------------------------------------------//
        public NET_DataPack GetPack(int playerID)
        {
            if (playerID == 1)
            {
                return packP1;
            }
            else if (playerID == 2)
            {
                return packP2;
            }
            else if (playerID == 3)
            {
                return packP3;
            }
            else
            {
                return packP4;
            }
        }
        public byte[] SerializePacks()
        {
            return combinedPacks;
        }
    }
}
