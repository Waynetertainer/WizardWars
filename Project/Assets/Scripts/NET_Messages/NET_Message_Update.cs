using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NET_Messages
{
    [Serializable]
    public class NET_Message_Update : IMessage
    {
        //Values
        private byte[] InternalData;

        private DateTime TimeStamp = new DateTime();
        public DateTime timeStamp
        {
            get { return TimeStamp; }
        }

        //Constructor
        public NET_Message_Update(DateTime timeStamp)
        {
            TimeStamp = timeStamp;
        }

        //Interface functions
        public void SetData(object toSet)
        {//Serialize incoming object and save to [InternalData]
            byte[] data;
            BinaryFormatter binForm = new BinaryFormatter();
            using (MemoryStream memStream = new MemoryStream())
            {
                binForm.Serialize(memStream, toSet);
                data = memStream.ToArray();
            }
            InternalData = data;
        }
        public void SetData(byte[] toSet)
        {//Save incoming (serialized) data to [InternalData]
            InternalData = toSet;
        }
        public void GetData(ref byte[] getBytes, ref object getObject, bool serializable)
        {//If possible deserialize [InternalData] and return. / Also return [InternalData] as is.
            getBytes = InternalData;
            if (serializable == true)
            {
                try
                {
                    object obj;
                    BinaryFormatter binForm = new BinaryFormatter();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        memStream.Write(InternalData, 0, InternalData.Length);
                        memStream.Seek(0, SeekOrigin.Begin);
                        obj = (object)binForm.Deserialize(memStream);
                    }
                    getObject = obj;
                }
                catch
                {
                    getObject = null;
                }
            }
        }
    }
}
