using System;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Messages;
using System.Collections;


public class FHNetworkManager : MonoBehaviour
{
		public enum ServerType
		{
				LOCAL,
				HIEP,
				DIEU,
		}
	
		private static FHClient aClient;
	
		public string gameServerUrl = "127.0.0.1:3000";
	
		public static string GameServerUrl {
				get {
						return "http://" + instance.gameServerUrl;
				}
		}
	
		public string assetServerUrl = "127.0.0.1:8888";
	
		public static string AssetServerUrl {
				get {
						return "http://" + instance.assetServerUrl;
				}
		}
	
		public string webServerUrl = "127.0.0.1:82";
	
		public static string WebServerUrl {
				get {
						return "http://" + instance.webServerUrl;
				}
		}
	
		public ServerType serverType = ServerType.LOCAL;
	
	
		private static FHNetworkManager instance;
	
		public static FHNetworkManager Instance {
				get {
						return instance;
				}
		}
	
		void Start ()
		{
				Application.runInBackground = true;
				if (instance == null) {
						instance = this;
						FHClient c = GetComponent<FHClient> ();
						//			c.StartJobs();
			
						FHNetworkManager.Init (c);
				}
		}
	
		public static FHClient Client ()
		{
				return aClient;
		}
		public static bool IsConnected {
				get {
						return (aClient != null && aClient.Client.IsConnected);
				}
		}
	
		//	public static RoomType curRoomType = RoomType.None;
		public static string curRoomId = "";
		public static string curRoomName = "";
		public static string userName = "";//device id + username
	
		//	public static string GetRoomServerName()
		//	{
		//		return (int)curRoomType +"_"+curRoomId;
		//	}
	
		private static long timeBeginSync = 0;
		public static string SID;
		public static long TimeBeginSync {
				get { return timeBeginSync; }
				set { timeBeginSync = value; }
		}
		public static float GetSecondPlayOnline (long _now)
		{
				float result = (_now - TimeBeginSync) / 1000.0f;
				return result;
		}
	
		public void ResetSocketClient ()
		{
				if (aClient != null)
						aClient.MakeDisconnect ();
		}
	
	
		public static void Init (FHClient c)
		{
				aClient = c;
				aClient.ClearAllMessHandler ();
		
				//game logic message
				aClient.RegisterHandler (MessagesFromServer.SubscribeResult, typeof(M_S_SubscribeResult));
				aClient.RegisterHandler (MessagesFromServer.RoomReady, typeof(M_S_RoomReady));
				aClient.RegisterHandler (MessagesFromServer.SycnReady, typeof(M_S_SyncReady));
				aClient.RegisterHandler (MessagesFromServer.ClientLogic, typeof(M_S_ClientLogic));
		
		
				//on network close handler
				aClient.OnDisconnected = OnNetworkClosed;
		
		}
	
		static bool isResume = false;
		static bool isPause = false;
		public static bool IsPause {
				get {
						return isPause;
				}
		}
	
		void OnApplicationPause (bool pause)
		{
				if (!pause) { //resume      
						isPause = false;
						if (Time.realtimeSinceStartup < 1) {
								Debug.LogWarning ("Ignoring OnApplicationPause " + isPause);
								return;
						}
			
						Debug.LogWarning ("Resume from Pause");
						isResume = true;
			
				} else {//pause
						isPause = true;
						isResume = false;
				}
		
		
		}
	
	
		static void OnNetworkClosed ()
		{
				Debug.LogWarning ("OnNetworkClosed");
//				GUIMessageDialog.Show (res => {
//						if (SceneManager.instance.GetCurrentScene () != FHScenes.Online)
//								GuiManager.HidePanel (GuiManager.instance.guiOnlinePlay);
//						return true;
//				}, FHLocalization.instance.GetString (FHStringConst.ONLINE_CONNECT_ERROR) + "_", FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
		}
	
	#region COMMON NETWORK FUNCTION
	
		public static void JoinPlay (int roomType, string roomname)
		{
				M_C_JoinPlay mRoom = new M_C_JoinPlay (roomType, roomname);
				mRoom.Send (aClient);
		}
	
		public static void JoinPlayAuto (int roomType, string roomname)
		{
				M_C_JoinPlayAuto mRoom = new M_C_JoinPlayAuto (roomType, roomname);
				mRoom.Send (aClient);
		}
	
		//	public static void JoinRoom(int id)
		//	{
		//		M_C_JoinRoom mRoom = new M_C_JoinRoom(id);
		//		mRoom.Send(aClient);
		//
		//	}
	
		//	public static void JoinMyHome()
		//	{
		//		FishPartyManager.Instance.callbackFunc += GotoHome;
		//		if(FishPartyManager.Instance.fishPartyInfo != null)
		//			FishPartyManager.Instance.LeaveFishingScene();
		//		else
		//			GotoHome();
		//	}
	
		//	public static void LeaveRoom()
		//	{
		////		AvUIManager.Instance.ShowLoading();
		//		M_C_LeaveRoom mRoom = new M_C_LeaveRoom();
		//		mRoom.Send(aClient);
		//
		//
		//		M_Chat_LeaveRoom mc = new M_Chat_LeaveRoom();
		//		mc.Send(aClient);
		//	}
	
		//	public static void ConnectGlobalChat()
		//	{
		//		M_Chat_Connect m = new M_Chat_Connect(FHPlayersManager.instance.PlayerMe.NickName, FHPlayersManager.instance.PlayerMe.Uid);
		//		m.Send(aClient);
		//	}
	
	
		public static void JoinChatRoom (string roomname)
		{
		
				M_Chat_JoinRoom mc = new M_Chat_JoinRoom (roomname);
				mc.Send (aClient);
		}
	
		public static void JoinChatParty (string partyName)
		{
				Debug.LogWarning ("Join chat party");
				M_Chat_JoinParty mp = new M_Chat_JoinParty (partyName);
				mp.Send (aClient);
		}
	
		public static void Connect (Action<object> callback)
		{
				Debug.LogWarning ("Start connect to " + GameServerUrl);
				aClient.Connect (GameServerUrl);
				aClient.RegisterHandlerWithCallback (MessagesFromServer.Connected, typeof(MC_S_Connected), callback, true);
		}
	
	#endregion
	
	#region Register cumstom message and callback
		public static void SendChatToServer (M_Chat_Base mSend)
		{
				mSend.Send (aClient);
		}
	
		public static void SendMessageToServer (M_C_Base mSend)
		{
				SendMessageToServerCallback (mSend, MessagesFromServer.None, null, null);
		}
		public static void SendMessageToServerCallback (M_C_Base mSend, MessagesFromServer messageType, Type responseType, Action<object> callback)
		{
				if (!AllowTransfer (mSend))
						return;
		
				if (mSend != null)
						mSend.Send (aClient);
		
				if (callback != null) {
						RegisterMessageCustom (messageType, responseType, callback);
				}
		}
		public static void RegisterMessageCustom (MessagesFromServer type, Type responseType, Action<object> callback)
		{
				aClient.RegisterHandlerWithCallback (type, responseType, callback);
		}
	#endregion
	
		public static void SendRequestToServer (BaseRequest req, Type respType, Action<BaseResponse> respCallback)
		{
				aClient.SendRequest (req, respType, respCallback);
		}
	
	#region NetworkTraffic
		public class TransferConfig
		{
				public float interval = 0;
				public float lastTransfer = Time.time;
				public M_C_Base stackMessage = null;
				public bool isWait = false;
		}
	
		static Dictionary<int, TransferConfig> _transferConfig = new Dictionary<int, TransferConfig> ();
	
		static void AddTransferConfig (int msgType, int time)
		{
				TransferConfig config = new TransferConfig ();
				config.interval = time;
				_transferConfig.Add (msgType, config);
		}
	
		static bool AllowTransfer (M_C_Base msg)
		{
				if (msg == null)
						return false;
		
				TransferConfig config = null;
				if (_transferConfig.TryGetValue (msg.type, out config)) {
						float allowTime = config.lastTransfer + config.interval;
						if (allowTime <= Time.time) {
								config.lastTransfer = Time.time;
								config.stackMessage = null;
								return true;
						}
			
						config.stackMessage = msg;
						if (!config.isWait)
								instance.StartTransfer (config);
			
						return false;
				}
		
				return true;
		}
	
		public void StartTransfer (TransferConfig config)
		{
				StartCoroutine (Transfer (config));
		}
	
		IEnumerator Transfer (TransferConfig config)
		{
				config.isWait = true;
				float waitTime = config.lastTransfer + config.interval - Time.time;
				if (waitTime < 0)
						waitTime = 0;
				else if (waitTime > config.interval)
						waitTime = config.interval;
				//UnityEngine.Debug.Log("wait time: " + waitTime);
				yield return new WaitForSeconds (waitTime);
				SendMessageToServer (config.stackMessage);
				config.isWait = false;
		}
	#endregion
	
		//	void OnGUI()
		//	{
		//		if(GUI.Button(new Rect(100,100,100,25), "Close"))
		//		{
		//			aClient.MakeDisconnect();
		//		}
		//	}
	
}