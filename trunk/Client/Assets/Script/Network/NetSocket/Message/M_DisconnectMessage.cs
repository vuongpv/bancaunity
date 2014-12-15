using System;
using System.Collections.Generic;
using System.Text;

namespace FHNetSocket
{
	/// <summary>
	/// Signals disconnection. If no endpoint is specified, disconnects the entire socket.
	/// </summary>
	/// <remarks>Disconnect a socket connected to the /test endpoint:
	///		0::/test
	/// </remarks>
    public class M_DisconnectMessage : M_Message
	{

		public override string Event
		{
			get { return "disconnect"; }
		}

		public M_DisconnectMessage() : base()
		{
			this.MessageType = SocketIOMessageTypes.Disconnect;
		}
		public M_DisconnectMessage(string endPoint)
			: this()
		{
			this.Endpoint = endPoint;
		}
        public static M_DisconnectMessage Deserialize(string rawMessage)
		{
            M_DisconnectMessage msg = new M_DisconnectMessage();
			//  0::
			//  0::/test
			msg.RawMessage = rawMessage;

			string[] args = rawMessage.Split(SPLITCHARS, 3);
			if (args.Length == 3)
			{
				if (!string.IsNullOrEmpty(args[2]))
					msg.Endpoint = args[2];
			}
			return msg;
		}
		public override string Encoded
		{
			get
			{
				return string.Format("0::{0}", this.Endpoint);
			}
		}
	}
}
