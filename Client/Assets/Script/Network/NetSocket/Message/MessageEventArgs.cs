using System;
using System.Collections.Generic;
using System.Text;

namespace FHNetSocket
{
	public class MessageEventArgs : EventArgs
	{
        public M_Message Message { get; private set; }

        public MessageEventArgs(M_Message msg)
			: base()
		{
			this.Message = msg;
		}
	}
}
