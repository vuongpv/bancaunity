using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WebSocket4Net;
using FHNetSocket;

/// <summary>
/// Control client of socket.io for FishHurt
/// </summary>
public class FHSocketClient : FHSocketBase
{
		public bool Active;

		[HideInInspector]
		public FHRoomOnlinePlay
				fhRoomOnlinePlay = new FHRoomOnlinePlay ();

		List<RawMessageOnlinePlay> messageList = new List<RawMessageOnlinePlay> ();
		public event Action<int> checkedUserOnlineReady = null;
		public event Action<bool> checkedJoinRoomFailed = null;
		public event Action<string> logClientLogic = null;
		public event Action<string> getFinalResult = null;
		public event Action<bool> getReconnectResult = null;
		// Use this for initialization
		void Start ()
		{

		}
    

		// Update is called once per frame
		void Update ()
		{
				jobScheduler.Update ();
		}
		protected override bool OnDispatchMessage (object param)
		{
				try {
						lock (fhClientLock) {
								if (messageList.Count > 0) {
										List<RawMessageOnlinePlay> _list = new List<RawMessageOnlinePlay> ();
										for (int i = 0; i < messageList.Count; i++) {
												_list.Add (messageList [i]);
										}
										messageList.Clear ();
										for (int i = 0; i < _list.Count; i++) {
												DispathMessage (_list [i].msgName, _list [i].jsonData);
										}
										_list.Clear ();
								}

						}
				} catch (Exception ex) {
						Debug.LogError ("FH Error dispatch message" + ex.Message);
				}
				return true;
		}
		private void DispathMessage (string msgName, string jsonData)
		{
				//try
				//{
				int _eventNameID = int.Parse (msgName.Trim ());// for optimized just using message by number
				switch (_eventNameID) {
				case (int)FHMessageProperties.SubscribeResult:
						{
                        
								OnJoinRoomResult (jsonData);
						}
						break;
				case (int)FHMessageProperties.RoomReady:
						{
								OnRoomReady (jsonData);
						}
						break;
				case (int)FHMessageProperties.SycnReady:
						{
								OnSycnUserReady (jsonData);
						}
						break;
				case (int)FHMessageProperties.CaptureLogic:
						{
								OnLogClientLogicEvent (jsonData);
						}
						break;
				case (int)FHMessageProperties.FinalResult:
						{
								OnLogClientFinalResult (jsonData);
						}
						break;
				case (int)FHMessageProperties.ReConnect:
						{
								OnReconnectResult (jsonData);                    
						}
						break;

				default:
						{
								Debug.LogError ("FH FH FHSocketEvent_OnMessageEvent Server send undefine message:" + msgName + "_" + jsonData);
						}
						break;
				}
				//}
				//catch (Exception ex)
				//{
				//    Debug.LogError("FH FH FHSocketEvent_DispathMessage Exception:" + ex.Message + ":::" + jsonData);
				//}
		}
		protected override void OnMessageEvent (M_EventMessage msg)
		{
				//print("FHSocketClient:30, OnMessageEvent: " + msg.Encoded + msg.MessageType);
				if (msg.Json.name.Length < 1 || msg.Json.args == null || msg.Json.args.Length < 1) {
						Debug.LogError ("FH FH FHSocketEvent_OnMessageEvent: Message Error:" + msg.Encoded);
				}
				//Debug.Log("Add Message:" + msg.Json.name.Length);
				try {
						lock (fhClientLock) {
								messageList.Add (new RawMessageOnlinePlay (msg.Json.name, msg.Json.args [0].ToString ()));
						}

				} catch (Exception ex) {
						Debug.LogError ("FH FH FHSocketEvent_OnMessageEvent Exception:" + ex.Message);
				}

		}

    #region override game function
    
		/// <summary>
		/// Join to room
		/// </summary>
		public override void JoinRoom (SocketJoinRoomType roomtype, string userName)// data is a package message
		{
				Debug.LogError ("FH FH JoinRoom:" + roomtype);
				FHNetEvent_Subcribe joinGame = new FHNetEvent_Subcribe ((int)roomtype, userName);
				//FHNetEvent_ChangeGun n = new FHNetEvent_ChangeGun();
				//Debug.LogError(n.ToSimpleJson());


#if UNITY_IPHONE  || IOS
        Debug.LogError( joinGame.ToSimpleJson());
        this.Emit(joinGame.eventName.ToString(), joinGame.ToSimpleJson());
#else
				this.Emit (joinGame.eventName.ToString (), joinGame);
#endif
        
		}

		/// <summary>
		/// Join to room
		/// </summary>
		public void JoinPlayAuto (int roomtype, string roomName)// data is a package message
		{
				Debug.LogError ("FH FH JoinRoom Play Auto:" + roomtype);
				FHNetEvent_PlayAuto joinGameAI = new FHNetEvent_PlayAuto (roomtype, roomName);
#if UNITY_IPHONE  || IOS
        this.Emit(joinGameAI.eventName.ToString(), joinGameAI.ToSimpleJson());
#else
				this.Emit (joinGameAI.eventName.ToString (), joinGameAI);
#endif

		}
		/// <summary>
		/// Send To room I'm ready to play
		/// </summary>

		public void Send_OnlineReady (string SID)
		{

				FHNetEvent_SyscReady _event = new FHNetEvent_SyscReady ();
				_event.UID = SID;
        
#if UNITY_IPHONE  || IOS
        Emit(_event.eventName.ToString(), _event.ToSimpleJson());
#else
				Emit (_event.eventName.ToString (), _event);
#endif
		}
  
		/// <summary>
		/// Client custom logic event to server
		/// </summary>

		public void Send_ClientLogicEvent (string SID, string listEvent, int score)
		{
				//Debug.LogError("FH FH Send_ClientLogicEvent");
				FHNetEvent_ClientLogicEvent _event = new FHNetEvent_ClientLogicEvent ();
				_event.UID = SID;
				_event.listEvents = listEvent;
				_event.score = score;
        
#if UNITY_IPHONE  || IOS
        Emit(_event.eventName.ToString(), _event.ToSimpleJson());
#else
				Emit (_event.eventName.ToString (), _event);
#endif
		}

		/// <summary>
		/// Sycn final result
		/// </summary>

		public void Send_FinalResultEvent (string SID, int score)
		{
				//Debug.LogError("FH FH Send_ClientLogicEvent");
				FHNetEvent_FinalResult _event = new FHNetEvent_FinalResult ();
				_event.UID = SID;
				_event.score = score;
#if UNITY_IPHONE  || IOS
        Emit(_event.eventName.ToString(), _event.ToSimpleJson());
#else
				Emit (_event.eventName.ToString (), _event);
#endif
		}

		/// <summary>
		/// Client reconnect
		/// </summary>
		public void ReConnect (string _roomName)// data is a package message
		{
				Debug.LogWarning ("FH FH Reconnect:" + _roomName);
				FHNetEvent_ReConnect reJoinRoom = new FHNetEvent_ReConnect (_roomName);
        
#if UNITY_IPHONE  || IOS
        this.Emit(reJoinRoom.eventName.ToString(), reJoinRoom.ToSimpleJson());
#else
				this.Emit (reJoinRoom.eventName.ToString (), reJoinRoom);
#endif

		}
    #endregion


    #region function receive from server

		/// <summary>
		///  Result subscribe join from from server
		/// </summary>
		public void OnJoinRoomResult (string jsonObject)
		{
				try {
						Debug.LogWarning (jsonObject);
						FHNetEvent_SubcribeResult evt = null;
#if UNITY_IPHONE  || IOS
            evt = new FHNetEvent_SubcribeResult();
            if (!evt.Deserialize(SimpleJSON.JSONNode.Parse(jsonObject)))
            {
                Debug.LogError("OnJoinRoomResult Deserialize failed");
                return;
            }
            Debug.LogError(evt.ToSimpleJson());
#else
						evt = FHUtils.ToObject<FHNetEvent_SubcribeResult> (jsonObject);
#endif



						if (evt.result == false) {
								if (checkedJoinRoomFailed != null) {
										checkedJoinRoomFailed (true);
								}
						} else {
								isJoinRoom = true;
								roomType = evt.roomType;
								roomName = evt.roomName;
						} 
						Debug.LogError ("FH FH OnJoinRoomResult OKIE");
				} catch (Exception ex) {
						Debug.LogError ("FH FH FHSocketEvent_OnJoinRoomResult Exception:" + ex.Message);
				}
		}
		/// <summary>
		/// Room is ready to access game
		/// </summary>
		public void OnRoomReady (string jsonObject)
		{
				try {
						Debug.LogError (jsonObject);
						FHNetEvent_RoomInfoReady evt = null;
#if UNITY_IPHONE  || IOS
            evt = new FHNetEvent_RoomInfoReady();
            if (!evt.Deserialize(SimpleJSON.JSONNode.Parse(jsonObject)))
            {
                Debug.LogError("OnRoomReady Deserialize failed");
                return;
            }
#else
            
						evt = FHUtils.ToObject<FHNetEvent_RoomInfoReady> (jsonObject);
#endif
						isJoinRoom = true;
						fhRoomOnlinePlay.listPlayer.Clear ();
						bool result = fhRoomOnlinePlay.Init (evt.RoomName, evt.roomType, evt.routeID, evt.timePlay, evt.timeUpdate, evt.kindPlay, evt.player_Names, evt.player_SIDs, evt.player_locations, evt.taxPercent, evt.price);
						if (!result) {
								FHNetworkManager.Client ().SetNetworkException ();
								Debug.LogError ("FH FH SSSSSSSSSSS" + evt.player_Names);
						}
						SceneManager.instance.LoadSceneWithLoading (FHScenes.Online);
						Debug.LogError ("FH FH OnRoomReady OKIE");
				} catch (Exception ex) {
						Debug.LogError ("FH FH FHSocketEvent_OnRoomReady Exception:" + ex.Message);
				}
		}
		/// <summary>
		///One of user say he is ready now
		/// </summary>
		public void OnSycnUserReady (string jsonObject)
		{
				try {
						FHNetEvent_SyscReady evt = null;
#if UNITY_IPHONE  || IOS
            evt = new FHNetEvent_SyscReady();
            if (!evt.Deserialize(SimpleJSON.JSONNode.Parse(jsonObject)))
            {
                Debug.LogError("OnSycnUserReady Deserialize failed");
                return;
            }
#else
            
						evt = FHUtils.ToObject<FHNetEvent_SyscReady> (jsonObject);
#endif
						FHNetworkManager.TimeBeginSync = evt.timeServer;
						if (checkedUserOnlineReady != null) {
								checkedUserOnlineReady (evt.CountReady);
						}
				} catch (Exception ex) {
						Debug.LogError ("FH FH FHSocketEvent_OnSycnUserReady Exception:" + ex.Message);
				}
		}

		/// <summary>
		///Receive capture logic event of one user
		/// </summary>
		public void OnLogClientLogicEvent (string jsonObject)
		{
				//Debug.LogError("FH FH OnLogClientLogicEvent");
				if (logClientLogic != null) {
						logClientLogic (jsonObject);
				}
		}

		public void OnLogClientFinalResult (string jsonObject)
		{
				if (getFinalResult != null) {
						getFinalResult (jsonObject);
				}
		}
		public void OnReconnectResult (string jsonObject)
		{
				Debug.LogError ("OnReconnectResult:" + jsonObject);
				if (getReconnectResult != null) {
						getReconnectResult (true);
				}
		}
    #endregion


    #region Testing
		class testss : par
		{
				public int aaa = 123;
				public float bbb = 1.25f;
				public string ccc = "thuantq01";
				public bool ddd = true;
		}
		class par
		{
				public float fff = 3456.2f;
				public string uuu = "111223-gss";
		}
    #endregion
}
