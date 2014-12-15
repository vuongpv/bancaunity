using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;

namespace SocketIOClient.Messages
{

    public class JsonEncodedEventMessage 
    {
         public string name { get; set; }

         public object[] args { get; set; }

        public JsonEncodedEventMessage()
        {
        }
        
		public JsonEncodedEventMessage(string name, object payload) : this(name, new[]{payload})
        {

        }
        
		public JsonEncodedEventMessage(string name, object[] payloads)
        {
            this.name = name;
            this.args = payloads;
        }

		public object GetFirstArgAs(Type type)
		{
			try
			{
				var firstArg = this.args.FirstOrDefault();
				if (firstArg != null)
				{
					string data = JsonWriter.Serialize(firstArg);
					if(string.IsNullOrEmpty(data))
					{	
						return Activator.CreateInstance(type);
					}
					return JsonReader.Deserialize(data, type);
				}
			}
			catch (Exception ex)
			{
				// add error logging here
               // UnityEngine.Debug.LogError(this.args[0].ToString());
				UnityEngine.Debug.LogError("GetFirstArgAs:" + ex.Message);
			}
			return null;
		}

//        public T GetFirstArgAs<T>()
//		{
//            try
//            {
//                var firstArg = this.args.FirstOrDefault();
//                if (firstArg != null)
//					return JsonFx.Json.JsonReader.Deserialize<T>(firstArg.ToString());
//            }
//            catch (Exception ex)
//            {
//                // add error logging here
//				UnityEngine.Debug.LogError("GetFirstArgAs:" + ex.Message);
//                throw;
//            }
//            return default(T);
//        }
//        public IEnumerable<T> GetArgsAs<T>()
//        {
//            List<T> items = new List<T>();
//            foreach (var i in this.args)
//            {
//				items.Add( JsonFx.Json.JsonReader.Deserialize<T>(i.ToString()) );
//            }
//            return items.AsEnumerable();
//        }

		//ko the serialize array of object
        //public string ToJsonString()
        //{
			//return JsonWriter.Serialize(this);
			//return MiniJSON.Json.Serialize(this);//bi loi tren ios

        //}

	
        public static JsonEncodedEventMessage Deserialize(string jsonString)
        {
			//UnityEngine.Debug.Log("Deserialize " + jsonString);
			JsonEncodedEventMessage msg = null;
			try 
			{

				msg = JsonReader.Deserialize<JsonEncodedEventMessage>(jsonString);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("Deserialize " + ex.Message);
			}
            return msg;
        }


	
    }

	public class JsonSendingEncodedEventMessage
	{
		public string name { get; set; }
		
		public object args { get; set; }

		
		public JsonSendingEncodedEventMessage(string name, object payload) 
		{
			this.name = name;
			this.args = payload;
		
		}

		public string ToJsonString()
		{

			//UnityEngine.Debug.Log("typeof " + this.args.GetType().ToString());
			string obj = JsonWriter.Serialize(args);

			return string.Format("{{ \"name\":\"{0}\", \"args\": {1} }}", name, obj);
			
		}
	}
}
