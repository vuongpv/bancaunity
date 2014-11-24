using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FHNetSocket
{
    public class M_TextMessage : M_Message
    {
		private string eventName = "message";
		public override string Event
		{
			get	{ return eventName;	}
		}

        public M_TextMessage()
        {
            this.MessageType = SocketIOMessageTypes.Message;
        }
		public M_TextMessage(string M_TextMessage) : this()
		{
			this.MessageText = M_TextMessage;
		}

        public static M_TextMessage Deserialize(string rawMessage)
        {
			M_TextMessage msg = new M_TextMessage();
            //  '3:' [message id ('+')] ':' [message endpoint] ':' [data]
            //   3:1::blabla
			msg.RawMessage = rawMessage;

            string[] args = rawMessage.Split(SPLITCHARS, 4);
			if (args.Length == 4)
			{
				int id;
				if (int.TryParse(args[1], out id))
					msg.AckId = id;
				msg.Endpoint = args[2];
				msg.MessageText = args[3];
			}
			else
				msg.MessageText = rawMessage;
			
			return msg;
        }
    }
}
