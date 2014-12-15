using System;
using System.Collections.Generic;
using System.Text;


namespace FHNetSocket
{
    public class M_Heartbeat : M_Message
    {
        public static string _Heartbeat = "2::";

        public M_Heartbeat()
        {
            this.MessageType = SocketIOMessageTypes.M_Heartbeat;
        }

        public override string Encoded
        {
            get
            {
                return _Heartbeat;
            }
        }

    }
}
