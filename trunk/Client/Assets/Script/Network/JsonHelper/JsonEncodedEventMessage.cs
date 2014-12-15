using System;
using System.Collections.Generic;
using System.Linq;
//using System.Diagnostics;
using System.Text;
using SimpleJson.Reflection;

using UnityEngine;
namespace FHNetSocket
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

        public T GetFirstArgAs<T>()
        {
            try
            {
                var firstArg = this.args.FirstOrDefault();
                if (firstArg != null)
                    return SimpleJson.SimpleJson.DeserializeObject<T>(firstArg.ToString());
            }
            catch (Exception ex)
            {
                // add error logging here
                throw;
            }
            return default(T);
        }
        public IEnumerable<T> GetArgsAs<T>()
        {
            List<T> items = new List<T>();
            foreach (var i in this.args)
            {
                items.Add( SimpleJson.SimpleJson.DeserializeObject<T>(i.ToString()) );
            }
            return items.AsEnumerable();
        }

        public string ToJsonString()
        {
#if UNITY_IPHONE  || IOS
            return ToSimpleJSON();
#else
            return SimpleJson.SimpleJson.SerializeObject(this);
#endif
        }

        public static JsonEncodedEventMessage Deserialize(string jsonString)
        {
			JsonEncodedEventMessage msg = null;
#if UNITY_IPHONE  || IOS
            msg = DeserializeSimpleJSON(jsonString);
#else
            try { msg = SimpleJson.SimpleJson.DeserializeObject<JsonEncodedEventMessage>(jsonString); }
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
#endif
            
            return msg;
        }
        private string ToSimpleJSON()
        {
            return "{" + GetStringElement() + "}";
        }
        private string GetStringElement()
        {
            string str = "";
            str += "\"name\":" + "\"" +name + "\""+",";
            str += "\"args\":" + "[";
            for (int i = 0; i < args.Length; i++)
            {
                str += args[i].ToString();
            }
            str += "]";
            return str;
        }
        public static JsonEncodedEventMessage DeserializeSimpleJSON(string str_json)
        {
            SimpleJSON.JSONNode json = SimpleJSON.JSONNode.Parse(str_json);
            JsonEncodedEventMessage msg = new JsonEncodedEventMessage();
            try
            {
                msg.name = (string)json["name"];
                SimpleJSON.JSONArray arr = (SimpleJSON.JSONArray)SimpleJSON.JSONArray.Parse(json["args"].ToString());
                if (arr != null)
                {
                    msg.args = new string[arr.Count];
                    for (int i = 0; i < arr.Count; i++)
                    {
                        msg.args[i] = arr[i].ToString();
                    }
                }
                return msg;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
