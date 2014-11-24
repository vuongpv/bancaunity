 using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace FHNetSocket
{
    public class M_EventMessage : M_Message
    {
        public object[] Args { get; set; }
		private static object ackLock = new object();
		private static int _akid = 0;
		private static int NextAckID
		{
			get
			{
				lock (ackLock)
				{
					_akid++;
					if (_akid < 0)
						_akid = 0;
					return _akid;
				}
			}
		}

		public Action<System.Object>  Callback;

        public M_EventMessage()
        {
            this.MessageType = SocketIOMessageTypes.Event;
        }

		public M_EventMessage(string eventName, object jsonObject, string endpoint  , Action<System.Object>  callBack  )
			: this()
        {
			this.Callback = callBack;
			this.Endpoint = endpoint;

			if (callBack != null)
				this.AckId = M_EventMessage.NextAckID;

			this.JsonEncodedMessage = new JsonEncodedEventMessage(eventName, jsonObject);
			this.MessageText = this.Json.ToJsonString();
        }

        public static M_EventMessage Deserialize(string rawMessage)
        {
			M_EventMessage evtMsg = new M_EventMessage();
            //  '5:' [message id ('+')] ':' [message endpoint] ':' [json encoded event]
            //   5:1::{"a":"b"}
			evtMsg.RawMessage = rawMessage;
			try
			{
				string[] args = rawMessage.Split(SPLITCHARS, 4); // limit the number of pieces
				if (args.Length == 4)
				{
					int id;
					if (int.TryParse(args[1], out id))
						evtMsg.AckId = id;
					evtMsg.Endpoint = args[2];
					evtMsg.MessageText = args[3];

					if (!string.IsNullOrEmpty(evtMsg.MessageText) &&
						evtMsg.MessageText.Contains("name\":") &&
						evtMsg.MessageText.Contains("args\":"))
					{
#if UNITY_IPHONE  || IOS
                        evtMsg.Json = JsonEncodedEventMessage.DeserializeSimpleJSON(evtMsg.MessageText);
#else
                        evtMsg.Json = SimpleJson.SimpleJson.DeserializeObject<JsonEncodedEventMessage>(evtMsg.MessageText);
#endif
                        evtMsg.Event = evtMsg.Json.name;
					}
					else
						evtMsg.Json = new JsonEncodedEventMessage();
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(ex);
			}
			return evtMsg;
        }

		public override string Encoded
		{
			get
			{
                int msgId = 4;// (int)this.MessageType;
				if (this.AckId.HasValue)
				{
					if (this.Callback == null)
						return string.Format("{0}:{1}:{2}:{3}", msgId, this.AckId ?? -1, this.Endpoint, this.MessageText);
					else
						return string.Format("{0}:{1}+:{2}:{3}", msgId, this.AckId ?? -1, this.Endpoint, this.MessageText);
				}
				else
					return string.Format("{0}::{1}:{2}", msgId, this.Endpoint, this.MessageText);
			}
		}
        //public virtual T ToObject<T>()
        //{
        //    try { return SimpleJson.SimpleJson.DeserializeObject<T>(this.Json); }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        
    }
}
