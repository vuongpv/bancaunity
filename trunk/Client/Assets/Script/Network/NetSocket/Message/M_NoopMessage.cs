using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FHNetSocket
{
	/// <summary>
	/// Defined as No operation. Used for example to close a poll after the polling duration times out.
	/// </summary>
    public class M_NoopMessage : M_Message
    {
        public M_NoopMessage()
        {
            this.MessageType = SocketIOMessageTypes.Noop;
        }
        public static M_NoopMessage Deserialize(string rawMessage)
        {
			return new M_NoopMessage();
        }
    }
}
