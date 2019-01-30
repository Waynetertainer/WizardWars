//Interface for all NET_Messages V1

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace NET_Messages
{
    public interface IMessage
    {
        DateTime timeStamp { get; }
        void SetData(object setBytes);
        void SetData(byte[] setBytes);
        void GetData(ref byte[] getBytes, ref object getObject, bool serializable);
    }
}
