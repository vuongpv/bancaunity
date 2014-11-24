using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FHNetSocket;

public enum FHMessageProperties
{
		None=0,
		// function for register socket.io
		Subscribe=1,// dang ky mot slot(dung cho socket.io)
		SubscribeResult=2,// result subscribe from server socket.io
		UnSubscribe=3,// huy dang ky (dung cho socket.io)
		RoomReady=4,// Room have already to play, broadcast information room to all room's clients
		SycnReady=5,// sync ready (dung cho socket.io)
		CaptureLogic=6,// log event client to broadcast to room
		FinalResult =7,// sync final result
		Benckmark=8,// benck mark server
		PlayAuto=9,// play with AI
		ReConnect=10,// reconnect
		// logicGame
		FigureDown=101,
		FigureUp = 102,
		FigureMove = 103,
		Shot = 104,
		HitFish=105,
		UpgradeGun = 106,
		ChangeMoney = 107,
		ChangeLevel = 108,
		StopShot = 109,
}
public class FHNetEvent_Base
{
		public int eventName = (int)FHMessageProperties.None;

		public string ToSimpleJson ()
		{
				return "{" + GetStringElement () + "}";
		}
		public virtual string GetStringElement ()
		{
				string str = "";
				str += "\"eventName\":" + eventName;
				return str;
		}
		public virtual bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						eventName = int.Parse ((string)json ["eventName"]);
						return true;
				} catch (Exception ex) {
						Debug.LogError (" FHNetEvent_Base failed ex:" + ex.Message);
						return false;
				}
		}
}

#region  Message Event Client Logic
public class FHNetEvent_Logic : FHNetEvent_Base
{
		public float eventTime = 0;
		public string ToSimpleJson ()
		{
				return "{" + GetStringElement () + "}";
		}
		public virtual string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"eventTime\":" + eventTime;
				return str;
		}
		public virtual bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						eventTime = float.Parse ((string)json ["eventTime"]);
						return _resBase;
				} catch (Exception ex) {
						Debug.LogError ("FHNetEvent_Logic failed ex:" + ex.Message);
						return false;
				}
		}
}
public class FHNetEvent_ChangeGun : FHNetEvent_Logic
{
		public int gID = 0;
		public FHNetEvent_ChangeGun ()
		{
				eventName = (int)FHMessageProperties.UpgradeGun;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"gID\":" + gID;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						gID = int.Parse ((string)json ["gID"]);
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}

}
public class FHNetEvent_Figure : FHNetEvent_Logic
{
		public float rA = 0;
    
		public FHNetEvent_Figure ()
		{
				eventName = (int)FHMessageProperties.FigureMove;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"rA\":" + rA;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						rA = float.Parse ((string)json ["rA"]);
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}
public class FHNetEvent_Shot : FHNetEvent_Logic
{
		public float rA = 0;// rotate Angle
		public int gID = 0;
		//public long guidGun=0;
		public FHNetEvent_Shot ()
		{
				eventName = (int)FHMessageProperties.Shot;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"rA\":" + rA + ",";
				str += "\"gID\":" + gID;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						gID = int.Parse ((string)json ["gID"]);
						rA = float.Parse ((string)json ["rA"]);
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}

public class FHNetEvent_StopShot : FHNetEvent_Logic
{
		public FHNetEvent_StopShot ()
		{
				eventName = (int)FHMessageProperties.StopShot;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement ();
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				return base.Deserialize (json);
		}

}    
public class FHNetEvent_HitFish : FHNetEvent_Logic
{
		public float rA = 0;
		public int gID = 0;
		public int[] fDieID;
		public float lX;
		public float lY;
		public float lZ;
    
		public FHNetEvent_HitFish ()
		{
				eventName = (int)FHMessageProperties.HitFish;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"rA\":" + rA + ",";
				str += "\"gID\":" + gID + ",";
				str += "\"lX\":" + lX + ",";
				str += "\"lY\":" + lY + ",";
				str += "\"lZ\":" + lZ;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						rA = float.Parse ((string)json ["rA"]);
						gID = int.Parse ((string)json ["gID"]);
						lX = float.Parse ((string)json ["lX"]);
						lY = float.Parse ((string)json ["lY"]);
						lZ = float.Parse ((string)json ["lZ"]);
						SimpleJSON.JSONArray arr = (SimpleJSON.JSONArray)SimpleJSON.JSONArray.Parse (json ["fDieID"].ToString ());
						if (arr != null) {
								fDieID = new int[arr.Count];
								for (int i = 0; i < arr.Count; i++) {
										fDieID [i] = int.Parse (arr [i]);
								}
						}
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}
public class FHNetEvent_ChangeMoney : FHNetEvent_Logic
{
		public int cMoney;
		public FHNetEvent_ChangeMoney ()
		{
				eventName = (int)FHMessageProperties.ChangeMoney;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"cMoney\":" + cMoney;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						cMoney = int.Parse ((string)json ["cMoney"]);
						return _resBase;
				} catch (Exception ex) {
						Debug.LogError ("FHNetEvent_ChangeMoney failed");
						return false;
				}
		}
}
public class FHNetEvent_Level : FHNetEvent_Logic
{
		public int level;
		public FHNetEvent_Level ()
		{
				eventName = (int)FHMessageProperties.ChangeLevel;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"level\":" + level;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						level = int.Parse ((string)json ["level"]);
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}
#endregion 


#region Message Event Client_server 
// function control for sockerIO
public class FHNetEvent_Subcribe : FHNetEvent_Base
{
		public string userName;
		public int roomType;
		public FHNetEvent_Subcribe ()
		{
				eventName = (int)FHMessageProperties.Subscribe;
		}
		public FHNetEvent_Subcribe (int _roomType, string _userName)
		{
				eventName = (int)FHMessageProperties.Subscribe;
				roomType = _roomType;
				userName = _userName;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"userName\":" + "\"" + userName + "\"" + ",";
				str += "\"roomType\":" + roomType;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				Debug.LogError (json);
				try {
						bool _resBase = base.Deserialize (json);
						userName = (string)json ["userName"];
						roomType = int.Parse ((string)json ["roomType"]);
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}

public class FHNetEvent_PlayAuto : FHNetEvent_Base
{
		public string userName;
		public int roomType;
		public FHNetEvent_PlayAuto (int _roomType, string _userName)
		{
				eventName = (int)FHMessageProperties.PlayAuto;
				roomType = _roomType;
				userName = _userName;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"userName\":" + "\"" + userName + "\"" + ",";
				str += "\"roomType\":" + roomType;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						userName = (string)json ["userName"];
						roomType = int.Parse ((string)json ["roomType"]);
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}



public class FHNetEvent_SubcribeResult : FHNetEvent_Base
{
		public string roomName;
		public int roomType;
		public string deviceID;
		public bool result = false;
		public FHNetEvent_SubcribeResult ()
		{
				eventName = (int)FHMessageProperties.SubscribeResult;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"roomName\":" + "\"" + roomName + "\"" + ",";
				str += "\"roomType\":" + roomType + ",";
				str += "\"deviceID\":" + "\"" + deviceID + "\"" + ",";
				str += "\"result\":" + result.ToString ();
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						roomName = (string)json ["roomName"];
						roomType = int.Parse ((string)json ["roomType"]);
						deviceID = (string)json ["deviceID"];
						result = bool.Parse ((string)json ["result"]);
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}

// function control for sockerIO
public class FHNetEvent_UnSubscribe : FHNetEvent_Base
{
		public bool result;
		public string roomID;
		public FHNetEvent_UnSubscribe ()
		{
				eventName = (int)FHMessageProperties.UnSubscribe;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"roomID\":" + "\"" + roomID + "\"" + ",";
				str += "\"result\":" + result.ToString ();
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						roomID = (string)json ["roomID"];
						result = bool.Parse ((string)json ["result"]);
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}

// function control for sockerIO
public class FHNetEvent_RoomInfoReady : FHNetEvent_Base
{
		public int roomType;
		public int routeID;
		public string RoomName;
		public string player_Names;
		public string player_SIDs;
		public string player_locations;// vi tri tuong ung 1-4
		public int timePlay = 100;//default
		public float timeUpdate = 0.1f;
		public int kindPlay = 0;
		public float taxPercent = 1.0f;
		public FHNetEvent_RoomInfoReady ()
		{
				eventName = (int)FHMessageProperties.RoomReady;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"roomType\":" + roomType + ",";
				str += "\"routeID\":" + routeID + ",";
				str += "\"RoomName\":" + "\"" + RoomName + "\"" + ",";
				str += "\"player_Names\":" + "\"" + player_Names + "\"" + ",";
				str += "\"player_SIDs\":" + "\"" + player_SIDs + "\"" + ",";
				str += "\"player_locations\":" + "\"" + player_locations + "\"" + ",";
				str += "\"timePlay\":" + timePlay + ",";
				str += "\"timeUpdate\":" + timeUpdate + ",";
				str += "\"kindPlay\":" + kindPlay + ",";
				str += "\"taxPercent\":" + taxPercent;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				//Debug.LogError(json);
				try {
						bool _resBase = base.Deserialize (json);
						roomType = int.Parse ((string)json ["roomType"]);
						routeID = int.Parse ((string)json ["routeID"]);

						RoomName = (string)json ["RoomName"];
						player_Names = (string)json ["player_Names"];
						player_SIDs = (string)json ["player_SIDs"];
						player_locations = (string)json ["player_locations"];

						timePlay = int.Parse ((string)json ["timePlay"]);
						timeUpdate = float.Parse ((string)json ["timeUpdate"]);
						kindPlay = int.Parse ((string)json ["kindPlay"]);
						if (json ["taxPercent"] != null) {
								taxPercent = float.Parse ((string)json ["taxPercent"]);
						}
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}

public class FHNetEvent_SyscReady : FHNetEvent_Base
{
		public string SID;
		public long timeServer;
		public int CountReady;
		public FHNetEvent_SyscReady ()
		{
				eventName = (int)FHMessageProperties.SycnReady;
		}

		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"SID\":" + "\"" + SID + "\"" + ",";
				str += "\"timeServer\":" + timeServer + ",";
				str += "\" CountReady\":" + CountReady;
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						SID = (string)json ["SID"];
						timeServer = long.Parse ((string)json ["timeServer"]);
						if (json ["CountReady"] != null) {
								CountReady = int.Parse ((string)json ["CountReady"]);
						}
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}

public class FHNetEvent_ClientLogicEvent: FHNetEvent_SyscReady
{
		public int score;
		public string listEvents;
		public FHNetEvent_ClientLogicEvent ()
		{
				eventName = (int)FHMessageProperties.CaptureLogic;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"score\":" + score + ",";
				str += "\"listEvents\":" + "\"" + listEvents + "\"";
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				//Debug.LogError(json);
				try {
						bool _resBase = base.Deserialize (json);
						if (json ["score"] != null) {
								Debug.LogError ("iN HERE");
								score = int.Parse ((string)json ["score"]);
						}
						listEvents = (string)json ["listEvents"]; 
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}

public class FHNetEvent_FinalResult : FHNetEvent_Base
{
		public string SID;
		public int score;//gold
		public FHNetEvent_FinalResult ()
		{
				eventName = (int)FHMessageProperties.FinalResult;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"SID\":" + "\"" + SID + "\"" + ",";
				str += "\"score\":" + score + ",";
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						SID = (string)json ["SID"];
						score = int.Parse ((string)json ["score"]);   
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}
public class FHNetEvent_ReConnect : FHNetEvent_Base
{
		public string roomName;
		public FHNetEvent_ReConnect (string _roomName)
		{
				eventName = (int)FHMessageProperties.ReConnect;
				roomName = _roomName;
		}
		public override string GetStringElement ()
		{
				string str = base.GetStringElement () + ",";
				str += "\"roomName\":" + "\"" + roomName + "\"";
				return str;
		}
		public override bool Deserialize (SimpleJSON.JSONNode json)
		{
				try {
						bool _resBase = base.Deserialize (json);
						roomName = (string)json ["roomName"];
						return _resBase;
				} catch (Exception ex) {
						return false;
				}
		}
}

#endregion



public class FHNetEvent_Capture
{

		public Dictionary<int,FHNetEvent_Logic> listEvent;
		public void Reset ()
		{
				listEvent.Clear ();
		}
		public FHNetEvent_Capture ()
		{
				listEvent = new Dictionary<int, FHNetEvent_Logic> ();
		}
		public void AddEvent (FHNetEvent_Logic _evt)
		{
				listEvent [_evt.eventName] = _evt;
		}
		public void AddEventFigure (float rotateAngle)
		{
				FHNetEvent_Figure _evt = new FHNetEvent_Figure ();
				_evt.eventTime = FHLanNetwork.GetNetworkTime ();
				_evt.rA = rotateAngle;
				AddEvent (_evt);
		}
		public void AddEventChangeGun (int gunID)
		{
				FHNetEvent_ChangeGun _evt = new FHNetEvent_ChangeGun ();
				_evt.eventTime = FHLanNetwork.GetNetworkTime ();
				_evt.gID = gunID;
				AddEvent (_evt);
		}
		public void AddEventShot (int gunID, long guidGun, float rotateAngle)
		{
				FHNetEvent_Shot _evt = new FHNetEvent_Shot ();
				_evt.eventTime = FHLanNetwork.GetNetworkTime ();
				_evt.gID = gunID;
				_evt.rA = rotateAngle;
				//_evt.guidGun = guidGun;
				AddEvent (_evt);
		}
		public void AddEventHitFish (int gunID, long guidGun, Vector3 location, int[] fishDie)
		{
        
				FHNetEvent_HitFish _evt = new FHNetEvent_HitFish ();
        
				_evt.eventTime = FHLanNetwork.GetNetworkTime ();
				_evt.gID = gunID;
				//_evt.location = location;
				//_evt.guidGun = guidGun;
				_evt.fDieID = fishDie;

				FHNetEvent_HitFish oldEvt = null;
				FHNetEvent_Logic dataUnsend = null;
				if (listEvent.TryGetValue (_evt.eventName, out dataUnsend)) {
						oldEvt = (FHNetEvent_HitFish)dataUnsend;
						if (oldEvt.fDieID.Length > 0) {
								int[] _lstFist = new int[oldEvt.fDieID.Length + _evt.fDieID.Length];
								for (int i=0; i<oldEvt.fDieID.Length; i++) {
										_lstFist [i] = oldEvt.fDieID [i];
								}
								for (int i = 0; i < _evt.fDieID.Length; i++) {
										_lstFist [i + oldEvt.fDieID.Length] = _evt.fDieID [i];
								}
								_evt.fDieID = _lstFist;
						}
				}
				AddEvent (_evt);
		}
		public void AddEventChangeScore (int money)
		{
				FHNetEvent_ChangeMoney _evt = new FHNetEvent_ChangeMoney ();
				_evt.eventTime = FHLanNetwork.GetNetworkTime ();
				_evt.cMoney = money;
				AddEvent (_evt);
		}
		public void AddEventStopShot ()
		{
				FHNetEvent_StopShot _evt = new FHNetEvent_StopShot ();
				_evt.eventTime = FHLanNetwork.GetNetworkTime ();
				AddEvent (_evt);
		}
}



public class FHLanCapturePackage
{
		public NetworkPlayer player;
		public byte[] data;
    
		public FHLanCapturePackage (NetworkPlayer info, byte[] str)
		{
				player = info;
				data = str;
		}
		public FHLanCapturePackage (NetworkPlayer info, FHNetEvent_Logic obj)
		{
				player = info;
				data = FHLanCapturePackage.Serialize (obj);
		}

		public static byte[] Serialize (FHNetEvent_Logic obj)
		{
				//var stream = new MemoryStream();
				//if(obj.eventName==FHMessageProperties.HitFish)
				//    ProtoBuf.Serializer.NonGeneric.Serialize(stream, (FHNetEvent_HitFish)obj);
				//else if (obj.eventName == FHMessageProperties.Shot)
				//    ProtoBuf.Serializer.NonGeneric.Serialize(stream, (FHNetEvent_Shot)obj);
				//else
				//    ProtoBuf.Serializer.NonGeneric.Serialize(stream, obj);
				////test
				//// byte[] _data = stream.ToArray();
				////FHNetEvent_Logic test = FHLanCapturePackage.Deserialize(_data);
				////Debug.LogError(obj.eventName+","+test.eventName+"..."+obj.eventTime+","+test.eventTime);
				//return stream.ToArray();
				return null;
		}
		public static FHNetEvent_Logic Deserialize (byte[] data)
		{
				return null;
				//return ProtoBuf.Serializer.Deserialize<FHNetEvent_Logic>(new MemoryStream(data));
		}
}

public class FHOnlineCapturePackage
{
		public string SID;
		public string jsonMsgs;
		private static string splitString = "$";
		public FHOnlineCapturePackage ()
		{

		}
		public FHOnlineCapturePackage (string _SID, Dictionary<int, FHNetEvent_Logic> listEvent)
		{
				SID = _SID;
				jsonMsgs = "";
				foreach (KeyValuePair<int,FHNetEvent_Logic> pair in listEvent) {
						FHNetEvent_Logic _event = pair.Value;
#if UNITY_IPHONE  || IOS
            string json = _event.ToSimpleJson();
            json=json.Replace("\"", "\\\"");
            //Debug.LogError("DDDD:"+json);
#else
						string json = SimpleJson.SimpleJson.SerializeObject (_event);
						//Debug.LogError("DDDD:"+json);
#endif
						jsonMsgs += json + splitString;

				}
				//Debug.LogError(jsonMsgs);
		}
		public static Dictionary<int, FHNetEvent_Logic> Deserialize (string msgData)
		{
				Dictionary<int, FHNetEvent_Logic> evts = new Dictionary<int, FHNetEvent_Logic> ();
       
				string[] split = new string[] { splitString };
				string[] sub = msgData.Split (split, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < sub.Length; i++) {
						try {
								//sub[i] = sub[i].Replace("\\\"", "\"");
								if (sub [i].Length > 0) {
										FHNetEvent_Logic eventLogic = new FHNetEvent_Logic ();
										eventLogic.Deserialize (SimpleJSON.JSONNode.Parse (sub [i]));
										evts [eventLogic.eventName] = eventLogic;
										bool _res = false;
										switch (eventLogic.eventName) {
#if UNITY_IPHONE  || IOS
                        case (int)FHMessageProperties.FigureMove:
                            eventLogic = new FHNetEvent_Figure();
                            _res = eventLogic.Deserialize(SimpleJSON.JSONNode.Parse(sub[i]));
                            break;
                        case (int)FHMessageProperties.Shot:
                            eventLogic = new FHNetEvent_Shot();
                            _res = eventLogic.Deserialize(SimpleJSON.JSONNode.Parse(sub[i]));
                            break;
                        case (int)FHMessageProperties.HitFish:
                            eventLogic = new FHNetEvent_HitFish();
                            _res = eventLogic.Deserialize(SimpleJSON.JSONNode.Parse(sub[i]));
                            break;
                        case (int)FHMessageProperties.UpgradeGun:
                            eventLogic = new FHNetEvent_ChangeGun();
                            _res = eventLogic.Deserialize(SimpleJSON.JSONNode.Parse(sub[i]));
                            break;
                        case (int)FHMessageProperties.ChangeMoney:
                            eventLogic = new FHNetEvent_ChangeMoney();
                            _res = eventLogic.Deserialize(SimpleJSON.JSONNode.Parse(sub[i]));
                            break;
                        case (int)FHMessageProperties.ChangeLevel:
                            eventLogic = new FHNetEvent_Level();
                            _res = eventLogic.Deserialize(SimpleJSON.JSONNode.Parse(sub[i]));
                            break;
                        case (int)FHMessageProperties.StopShot:
                            eventLogic = new FHNetEvent_StopShot();
                            _res = eventLogic.Deserialize(sub[i]);
                            break;
                        case (int)FHMessageProperties.FigureDown:
                            eventLogic = new FHNetEvent_Figure();
                            _res = eventLogic.Deserialize(SimpleJSON.JSONNode.Parse(sub[i]));
                            break;
                       
#else
										case (int)FHMessageProperties.FigureMove:
//                            eventLogic = FHUtils.ToObject<FHNetEvent_Figure>(sub[i]);
												Debug.LogWarning ("=================FigureMove");
												break;
										case (int)FHMessageProperties.Shot:
//                            eventLogic = FHUtils.ToObject<FHNetEvent_Shot>(sub[i]);
												break;
										case (int)FHMessageProperties.HitFish:
												Debug.LogWarning ("=================HitFish");
//                            eventLogic = FHUtils.ToObject<FHNetEvent_HitFish>(sub[i]);
												break;
										case (int)FHMessageProperties.UpgradeGun:
												Debug.LogWarning ("=================UpgradeGun");
//                            eventLogic = FHUtils.ToObject<FHNetEvent_ChangeGun>(sub[i]);
												break;
										case (int)FHMessageProperties.ChangeMoney:
												Debug.LogWarning ("=================ChangeMoney");
//                            eventLogic = FHUtils.ToObject<FHNetEvent_ChangeMoney>(sub[i]);
												break;
										case (int)FHMessageProperties.ChangeLevel:
												Debug.LogWarning ("=================ChangeLevel");
//                            eventLogic = FHUtils.ToObject<FHNetEvent_Level>(sub[i]);
												break;
										case (int)FHMessageProperties.StopShot:
												Debug.LogWarning ("=================StopShot");
//                            eventLogic = FHUtils.ToObject<FHNetEvent_StopShot>(sub[i]);
												break;
										case (int)FHMessageProperties.FigureDown:
												Debug.LogWarning ("=================FigureDown");
//                            eventLogic = FHUtils.ToObject<FHNetEvent_Figure>(sub[i]);
												break;
#endif
										}
										// if (_res == false && eventLogic!=null)
										// Debug.LogError(_res + "," + eventLogic.eventName + ", " + sub[i]);
										evts [eventLogic.eventName] = eventLogic;
								}
						} catch (Exception ex) {
								UnityEngine.Debug.LogError ("Exception:" + ex.Message + "," + sub [i]);
								continue;
						}
				}
				return evts;
		}
}