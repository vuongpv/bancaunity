using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Messages;
using System.Net;
using System.Linq;
using Pathfinding.Serialization.JsonFx;
 
class CallBackData
{
		public Type type;
		public Action<object> callBack;
		public bool removeAfterCallBack;
	
		public CallBackData (Type t, Action<object> cb, bool remove)
		{
				type = t;
				callBack = cb;
				removeAfterCallBack = remove;
		}
	
}

//Them timeout
class RequestUnit
{
		public bool isReceivedData;
		public string receivedData;
	
		public Action<BaseResponse> respCallback;
		public Type respType;

	
		public void Request (IEndPointClient client, BaseRequest req, Type respType, Action<BaseResponse> cb)
		{
				isReceivedData = false;
				this.respType = respType;
				this.respCallback = cb;

//		if(req.GetType() != typeof(RequestPing))
//			Debug.LogWarning("Send request " + req + ":" + JsonWriter.Serialize(req));

				if (client == null) {
						Debug.LogError ("endpoint client is NULL???");
//			GUIMessageDialog.Show(res=>{
//				AvSceneManager.Instance.LoadToLoginScene();
//				return true;
//			}, AvLocalizationManager.GetString(9015),AvLocalizationManager.GetString(9014));
						return;
				}

				client.Emit ("request", req, obj => {
						try {
								JsonEncodedEventMessage m = obj as JsonEncodedEventMessage;
								//Debug.Log("Received Response " + m.ToJsonString());
								receivedData = m.args.FirstOrDefault ().ToString ();
						} catch (Exception e) {
								Debug.LogError ("Loi response cmnr " + e.Message);
								receivedData = null;
				
						}
						isReceivedData = true;
				});
		}
	
		public void Response ()
		{
				try {
//			if(respType != typeof(ResponsePing))
//				Debug.LogWarning("Response of " + respType + ":" + receivedData);

						if (receivedData != null) {
								BaseResponse resp = JsonReader.Deserialize (receivedData, respType) as BaseResponse;
								if (resp != null)
										respCallback (resp);
								else
										Debug.LogError ("Response NULL");
						} else
								Debug.LogError ("Response NULL");
				} catch (Exception ex) {
						Debug.LogError ("Error at Response:" + ex.Message + "\n" + ex.StackTrace);
				}
		}
}

public class FHClient : MonoBehaviour
{
		private bool isDisconnected = false;//for check connection
		public Action OnDisconnected = null;

		private Client client;

		public Client Client {
				get {
						return client;
				}
		}

		public float pingTimeOut = 10.0f;

		private IEndPointClient gameClient;

		public IEndPointClient GameClient {
				get {
						return gameClient;
				}
		}

		private IEndPointClient chatClient;
	
		public IEndPointClient ChatClient {
				get {
						return chatClient;
				}
		}

		protected readonly static object lockMessage = new object (); // allow one connection attempt at a time
		protected readonly static object lockRequest = new object ();

		private List<SocketIOClient.Messages.IMessage> listReceivedMess = new List<IMessage> ();

		private Dictionary<string, Type> listMessageHandler = new Dictionary<string, Type> ();

		private Dictionary<string, CallBackData> listMessageCallback = new Dictionary<string, CallBackData> ();

		private List<RequestUnit> listRequests = new List<RequestUnit> ();

		public event Action<bool> networkError = null;
		public event Action<int> checkedUserOnlineReady = null;
		public event Action<bool> checkedJoinRoomFailed = null;
		public event Action<string, long> logClientLogic = null;
		public event Action<string> getFinalResult = null;
		public event Action<bool> getReconnectResult = null;
		public FHRoomOnlinePlay fhRoomOnlinePlay = new FHRoomOnlinePlay ();
	
		[HideInInspector]
		public bool
				isJoinRoom = false;// check already join Room
	
		[HideInInspector]
		public int
				roomType = -1;// check already join Room
	
		[HideInInspector]
		public string
				roomName = "";// check already join Room

		// Use this for initialization
		void Start ()
		{

		}

		void FixedUpdate ()
		{
				CheckRecvMess ();
				CheckRecvResponse ();
				CheckNetwork ();
		}

		public void StartJobs ()
		{
				InvokeRepeating ("PingServer", 0, pingTimeOut);
				lastPingTime = 0;
				timeOutCount = 0;
		}

		float lastPingTime = 0;
		int timeOutCount = 0;

		void PingServer ()
		{
				if (client != null && client.IsConnected && !isDisconnected) {
						//check timeout cai ne
						if (lastPingTime != 0) {
								timeOutCount++;
								Debug.LogWarning ("TIMEOUT CMNR " + timeOutCount);
								if (timeOutCount > 1) {//cho phep timeout them 1 lan
										timeOutCount = 0;
										lastPingTime = 0;
										MakeDisconnect ();
										return;
								}
						}

						lastPingTime = Time.realtimeSinceStartup;
						SendRequest (new RequestPing (), typeof(ResponsePing), res => {
								//nhan dc response -> reset ca 2
								lastPingTime = 0;
								timeOutCount = 0;
						});
				}
		}

		List<RequestUnit> listToResponse = new List<RequestUnit> ();
		void CheckRecvResponse ()
		{
				lock (lockRequest) {
						listToResponse.Clear ();
						if (listRequests.Count > 0) {//Them timeout
								//Debug.Log("num of request " + listRequests.Count);
								foreach (var req in listRequests) {
										if (req.isReceivedData) {
												listToResponse.Add (req);
										}
								}
			
			
								foreach (var reqResp in listToResponse) {
										reqResp.Response ();
										listRequests.Remove (reqResp);
								}
								//Debug.Log("num of request after " + listRequests.Count);
						}
				}
		}

		// Update is called once per frame
		void CheckRecvMess ()
		{
				lock (lockMessage) {
						if (listReceivedMess.Count > 0) {
								foreach (var mess in listReceivedMess) {
										ProcessRecvMess (mess);
								}
								//Debug.Log(listReceivedMess.Count);
								listReceivedMess.Clear ();
						}
				}
		}


		void CheckNetwork ()
		{
				if (isDisconnected) {
						/*isDisconnected = false;
			if(OnDisconnected != null)
				OnDisconnected();
			OnDestroy();*/
						MakeDisconnect ();

				}
		}
		public void MakeDisconnect ()
		{
				isDisconnected = false;
				OnDestroy ();
				if (OnDisconnected != null)
						OnDisconnected ();
        
		}

		public void SetNetworkException ()
		{
				if (networkError != null)
						networkError (true);
		}

		//gui request lay thong tin server url. check them client version = query
		Uri RequestServerUri (string url)
		{
				Uri uri = new Uri (url);
		
				//rebuild uri for loadbalancer
//		using(WebClient client = new WebClient())
//		{
//			try{
//				client.Encoding = Encoding.UTF8;//load utf-8 string
//				string value = client.DownloadString(url + "/?v=" + GameConst.FULL_VERSION);
//				Debug.LogWarning("received string:" + value);	

				//var serverInfo = Pathfinding.Serialization.JsonFx.JsonReader.Deserialize<ServerInfo>(value);
//				var serverInfo = JsonReader.Deserialize<ServerInfo>(value);
				//Debug.LogError("CODE: " + serverInfo.code + " |IP: " + serverInfo.ip + " |MESSAGE: " + serverInfo.mess);
//				if(serverInfo.code == -1)//version problem
//				{
//					Debug.LogWarning("version problem: " + value);
//					AvUIManager.Instance.HideLoading();
//					GUIMessageDialog.Show(res => {
//						Application.OpenURL(GameConst.ITUNE_URL);
//						Application.Quit();
//						return true;
//					}, AvLocalizationManager.GetString(9013), AvLocalizationManager.GetString(9012), MessageBox.Buttons.OK);
//					return null;
//				}
//				else if(serverInfo.code == -2)//maintance
//				{
//					Debug.LogWarning("maintance server");
//					AvUIManager.Instance.HideLoading();
//					GUIMessageDialog.Show(res => {
//						Application.Quit();
//						return true;
//					}, serverInfo.mess, AvLocalizationManager.GetString(9010), MessageBox.Buttons.OK);
//					return null;
//				}
//				else
//				{
//					IPAddress[] addresslist = Dns.GetHostAddresses(url);			
//					if(addresslist != null && addresslist.Length > 0)
//					{
//						UriBuilder builder = new UriBuilder(uri);
//						builder.Host = addresslist[0].ToString();//chi update lai url
//						builder.Port = uri.Port;//port giu nguyen tu client
//						uri = builder.Uri;
//					}	   
//				}
//			}
//			catch (Exception ex)
//			{
////				AvUIManager.Instance.HideLoading();
//				Debug.LogError("Cannot download string with error: " + ex.Message);
////				GUIMessageDialog.Show(null,AvLocalizationManager.GetString(9015),AvLocalizationManager.GetString(9014));
//				return null;
//			}
//		}

				return uri;
		}

		public void Connect (string serverUrl)
		{
				if (Application.internetReachability == NetworkReachability.NotReachable) {
						Debug.LogError ("No internet connection");
						isDisconnected = true;
//			GUIMessageDialog.Show(null,AvLocalizationManager.GetString(9015),AvLocalizationManager.GetString(9014));
						GUIMessageDialog.Show (null, FHLocalization.instance.GetString (FHStringConst.ONLINE_CONNECT_ERROR) + "_", FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
						return;
				}

		
//		Uri u = RequestServerUri(serverUrl);
				Uri u = new Uri (serverUrl);
				if (u == null) {
						return;
				}

				Debug.LogWarning ("received server URL " + u.AbsoluteUri);

				//huy truoc da
				if (client != null) {
						client.Dispose ();
						client = null;
				}

				client = new Client (u.AbsoluteUri);
				client.RetryConnectionAttempts = 0;

				client.Opened += (object sender, System.EventArgs e) => {

						Debug.Log ("socket opened");

						isDisconnected = false;
						lastPingTime = 0;
			
						gameClient = client.Connect ("/gameserver");
						chatClient = client.Connect ("/chatserver");


				};

				//regist message to receive
				client.Message += (object sender, MessageEventArgs e) => {
						//Debug.Log("receive message " + e.Message.RawMessage);
						lock (lockMessage) {
								listReceivedMess.Add (e.Message);
						}
				};



				client.SocketConnectionClosed += ((object sender, System.EventArgs e) => {
						Debug.LogWarning ("socket closed");
						isDisconnected = true;
				});
		
				client.ConnectionRetryAttempt += (object sender, System.EventArgs e) => {
						Debug.LogWarning ("connection retry " + client.RetryConnectionAttempts);
						isDisconnected = true;
				};
		
				client.Error += (object sender, ErrorEventArgs e) => {
						Debug.LogError ("connection error " + e.Message);
						GUIMessageDialog.Show (null, FHLocalization.instance.GetString (FHStringConst.ONLINE_CONNECT_ERROR) + "_", FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
						isDisconnected = true;
				};

				client.Connect ();
		}

		void ProcessRecvMess (SocketIOClient.Messages.IMessage mess)
		{
				System.Type handlerType;
				bool flag = false;

				//server send diconnect
				if (mess.Event == "disconnect") {
						Debug.LogError ("disconnected");
						isDisconnected = true;
						return;
				}

//		Debug.LogWarning("raw message:" + mess.MessageText);	
				if (listMessageHandler.TryGetValue (mess.Event, out handlerType)) {
						if (handlerType != null) {
								M_S_Base handler = mess.Json.GetFirstArgAs (handlerType) as M_S_Base;
								if (handler == null) {
										handler = System.Activator.CreateInstance (handlerType) as M_S_Base;
								}
				
								if (handler != null) {
										try {
												string data = JsonWriter.Serialize (handler);//doan nay hoi tao lao, do jsonfx convert object -> dictionary
//						Debug.LogWarning("process message:" + mess.Event + "-" + data);	
												handler.Process ();
												flag = true;
										} catch (Exception ex) {
												Debug.LogError ("Error when process message " + handlerType + ":" + ex.Message + "\n" + ex.StackTrace);
										}
								}
						}
				}

				CallBackData callBackObj; 
				if (listMessageCallback.TryGetValue (mess.Event, out callBackObj)) {
						if (callBackObj != null && callBackObj.callBack != null) {
								try {
										object a = mess.Json.GetFirstArgAs (callBackObj.type);
					

										string data = JsonWriter.Serialize (a);//doan nay hoi tao lao, do jsonfx convert object -> dictionary
										Debug.LogWarning ("process message:" + mess.Event + "-" + data);	

										callBackObj.callBack (a);
										flag = true;
								} catch (Exception ex) {
										Debug.LogError ("Error when process message " + callBackObj.type + "\n" + ex.StackTrace);
								}

								if (callBackObj.removeAfterCallBack)
										listMessageCallback.Remove (mess.Event);//need to review
						}
				}

				if (!flag) {
						if (mess.Event != "open" && mess.Event != "connect")
								Debug.LogWarning ("unprocessed message  " + mess.Event + "-" + mess.MessageText);
				}


		}

		void OnDestroy ()
		{
				if (client != null) {
						Debug.LogWarning ("OnDestroyClient");
						client.Dispose ();
						client = null;
				}
				gameClient = null;
				chatClient = null;
		}

		public void ClearAllMessHandler ()
		{
				listMessageHandler.Clear ();
				listMessageCallback.Clear ();
				listRequests.Clear ();
		}

		public void RegisterHandler (MessagesFromServer type, System.Type handlerType)
		{
				try {
						listMessageHandler.Add (((int)type).ToString (), handlerType);
				} catch (Exception e) {
						Debug.LogError ("Cannot register handler for " + type + " error:" + e.Message);
				}
		}

		public void RegisterHandlerWithCallback (MessagesFromServer type, Type responseType, Action<object> callBack, bool removeAfterCallBack = false)
		{
				string strType = ((int)type).ToString ();
				CallBackData obj = new CallBackData (responseType, callBack, removeAfterCallBack); 
				listMessageCallback [strType] = obj;//use new one
		}

		public void SendRequest (BaseRequest req, Type respType, Action<BaseResponse> respCallback)
		{
				lock (lockRequest) {
						//Debug.Log("client send request " + req);
						RequestUnit reqUnit = new RequestUnit ();
						listRequests.Add (reqUnit);
						reqUnit.Request (gameClient, req, respType, respCallback);
				}
		}
	
		public void OnJoinPlayResult (string _roomName, int _roomType, bool _result)
		{
				if (_result == false) {
						if (checkedJoinRoomFailed != null) {
								checkedJoinRoomFailed (true);
						}
				} else {
						isJoinRoom = true;
						roomType = _roomType;
						roomName = _roomName;
				} 
		}
	
		public void OnSycnUserReady (int countReady)
		{
				if (checkedUserOnlineReady != null)
						checkedUserOnlineReady (countReady);
		}
	
		public void OnCaptureClientLogic (string json, long timeServer)
		{
				if (logClientLogic != null)
						logClientLogic (json, timeServer);
		}
}


