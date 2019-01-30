//This class encapsulates a NET_DataPack instance holding
//event call parameters to make NET_EventCalls compatible to
//NET_DataPack subsystem

using System;
using System.Collections.Generic;
using System.Text;
using NET_Multiplayer_V3;

namespace NET_System
{
    public class NET_EventCall
    {
        private NET_DataPack eventPack;

        //---------------------------------------------------------------------------------------------//
        //NEW Constructor:
        //---------------------------------------------------------------------------------------------//
        public NET_EventCall(string eventName)
        {
            this.eventPack = new NET_DataPack();
            this.eventPack.SetData("AQEX_EventCallName", eventName);
        }
        //---------------------------------------------------------------------------------------------//
        //IMPORT Constructor:
        //---------------------------------------------------------------------------------------------//
        public NET_EventCall(NET_DataPack import)
        {
            this.eventPack = import;
        }

        //---------------------------------------------------------------------------------------------//
        //Public functions:
        //---------------------------------------------------------------------------------------------//
        public void SetParam(string paramName, object paramValue)
        {
            this.eventPack.SetData(paramName, paramValue);
        }
        public object GetParam(string paramName)
        {
            return this.eventPack.GetData(paramName);
        }
        public string GetEventName()
        {
            return (string)this.eventPack.GetData("AQEX_EventCallName");
        }
        public NET_DataPack GetEventData()
        {
            return this.eventPack;
        }
    }
}
