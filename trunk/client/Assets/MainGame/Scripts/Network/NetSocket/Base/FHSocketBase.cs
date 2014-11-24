using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using UnityEngine;

using WebSocket4Net;
using FHNetSocket;


/// <summary>
/// This is abstract class, don't using this class or modifier any function if not necessary
/// Using FHSocketClient instead
/// </summary>
public class FHSocketBase : MonoBehaviour
{
    #region variable

		public string serverIP = "127.0.0.1";// server IP
		public int serverPort = 8080;// server port
		private string handShakeSID = "";// Hand shake SID from socketIO
		[HideInInspector]
		public bool
				isJoinRoom = false;// check already join Room
		public int roomType = -1;// check already join Room
		public string roomName = "";// check already join Room
		// Update scheduler
		protected GFramework.JobScheduler jobScheduler = new GFramework.JobScheduler ();
		public int pingTimeInterval = 2000;// 5 FPS
		protected int updateLogicInterval = 100;//10FPS
		protected int updateMessageInterval = 50;//30FPS

		protected ConcurrentQueue<string> outboundQueue;// queue message control

		protected int retryConnectionCount = 0;
		protected int retryConnectionAttempts = 5;

		protected Uri uri;// URL of WebSocket server

		protected WebSocket wsClient;// main WebSocket implementation
		protected WebSocketVersion socketVersion = WebSocketVersion.Rfc6455;//By Default, use WebSocketVersion.Rfc6455

		protected readonly static object fhClientLock = new object (); // allow one connection attempt at a time

		public string LastErrorMessage = "";// Value of the last error message text  
//		public e vent Action<bool> networkError = null;

		public static string ipServer = "";
		/// <summary>
		/// RegistrationManager for dynamic events
		/// </summary>
		protected RegistrationManager registrationManager;  // allow registration of dynamic events (event names) for client actions


    #region Socket event handler

		/// <summary>
		/// Event Opened event comes from the underlying websocket client connection being opened. 
		/// </summary>
		public event EventHandler Opened;
		public event EventHandler<MessageEventArgs> Message;
		public event EventHandler ConnectionRetryAttempt;

		/// <summary>
		/// The Socket.IO service may have closed the connection due to a Ping timeout
		/// or the connection was just broken
		/// </summary>
		public event EventHandler SocketConnectionClosed;
		public event EventHandler<ErrorEventArgs> Error;

    #endregion


		/// <summary>
		/// Number of retry connection
		/// </summary>
		public int RetryConnectionAttempts {
				get { return this.retryConnectionAttempts; }
				set { this.retryConnectionAttempts = value; }
		}

		/// <summary>
		/// SYS section ID
		/// </summary>
		public string SID {
				get {
						return handShakeSID;
				}
		}

		/// <summary>
		/// Returns boolean of ReadyState == WebSocketState.Open
		/// </summary>
		public bool IsConnected {
				get {
						return this.ReadyState == WebSocketState.Open;
				}
		}

		/// <summary>
		/// Connection state of websocket client: None, Connecting, Open, Closing, Closed
		/// </summary>
		public WebSocketState ReadyState {
				get {
						if (this.wsClient != null)
								return this.wsClient.State;
						else
								return WebSocketState.None;
				}
		}
    #endregion

		void Start ()
		{
				//Init(@"http://"+serverIP + ":" + serverPort, WebSocketVersion.Rfc6455);

		}
		void Update ()
		{
				//jobScheduler.Update();
		}
		void OnDestroy ()
		{
				this.Close ();
		}
		public void InitialNetwork ()
		{
				Init (@"http://" + serverIP + ":" + serverPort, WebSocketVersion.Rfc6455);
		}
		public void Init (string url, WebSocketVersion socketVersion)
		{
				if (IsConnected) {
						Debug.Log ("Already connected");
						return;
				}
				this.uri = new Uri (url);

				this.socketVersion = socketVersion;


				this.registrationManager = new RegistrationManager ();
				//this.outboundQueue = (new ConcurrentQueue<string>());   

				//  Todo
				if (ipServer.Length < 1)
						ipServer = gethostbyname (uri.Host);
				HandShake ();

		}

		/// <summary>
		/// Initiate handshake & authorization connection with Socket.IO service
		/// </summary>
		public void HandShake ()
		{
				//lock (fhClientLock)
				{
						if (!(this.ReadyState == WebSocketState.Connecting || this.ReadyState == WebSocketState.Open)) {
								//Debug.LogError(uri.Host + "," + uri.Port);
								string strURL = string.Format ("http://{0}:{1}/", uri.Host, uri.Port);
								Debug.Log ("Start Shakehand:" + strURL);
								WWW wwwShakehand = new WWW (strURL);
								this.StartCoroutine (WaitForHandShake (wwwShakehand));
						}
				}
		}
		IEnumerator WaitForHandShake (WWW www)
		{
				yield return www;
				Debug.Log ("WaitForShakeHand Begin:");
				yield return www;
				if (www.error != null) {
						Debug.LogError ("WaitForShakeHand Error: " + www.error);
//						SetNetworkException ();
						yield break;
				}
				if (IsConnected) {
						Debug.Log ("Already connected");
						yield break;
				}
				string dataInfo = www.text;
				Debug.Log ("Shakehand, result=" + dataInfo);
				string[] split = new string[] { ":", "<" };
				string[] sub = dataInfo.Split (split, StringSplitOptions.RemoveEmptyEntries);
				string urlSocket = "ws://" + serverIP + ":" + serverPort + "/socket.io/1/websocket/" + sub [0];
				handShakeSID = sub [0];
				Connect (handShakeSID);
		}
//		public void SetNetworkException ()
//		{
//				if (networkError != null) {
//						networkError (true);
//				}
//		}
		public void Connect ()
		{
				Connect (handShakeSID);
		}
		public static string gethostbyname (string aURL)
		{
				IPHostEntry Hosts = Dns.GetHostEntry (aURL);
				return Hosts.AddressList [0].ToString ();
				return aURL;
		}
		public void Connect (string _handShakeSID)
		{
				lock (fhClientLock) {
						if (!(this.ReadyState == WebSocketState.Connecting || this.ReadyState == WebSocketState.Open)) {
								try {
										string _host = ipServer;
										string wsScheme = (uri.Scheme == Uri.UriSchemeHttps ? "wss" : "ws");
										//string url = "ws://multi.fishhunt.g6-mobile.zing.vn:443/socket.io/1/websocket/"+handShakeSID;
										//string url = "ws://120.138.76.38:443/socket.io/1/websocket/"+handShakeSID;
										string url = string.Format ("{0}://{1}:{2}/socket.io/1/websocket/{3}", wsScheme, _host, uri.Port, _handShakeSID);
										Debug.LogWarning ("Connect to: " + _host + "," + uri.Host + "full URL=" + url);
										this.wsClient = new WebSocket (
                        url,
                        string.Empty,
                        this.socketVersion);

										this.wsClient.AllowUnstrustedCertificate = true;
										//this.wsClient.EnableAutoSendPing = true; // #4 tkiley: Websocket4net client library initiates a websocket M_Heartbeat, causes delivery problems
										this.wsClient.Opened += this.wsClient_OpenEvent;
										this.wsClient.MessageReceived += this.wsClient_MessageReceived;
										this.wsClient.Error += this.wsClient_Error;

										this.wsClient.Closed += wsClient_Closed;

										this.wsClient.Open ();
										this.isJoinRoom = false;
								} catch (Exception ex) {
										Debug.LogError ("Exception connect socket:" + ex.Message);
//										SetNetworkException ();
								}
						}
				}
		}

		private IEnumerator ReConnect ()
		{
				this.retryConnectionCount++;

				this.OnConnectionRetryAttemptEvent (this, EventArgs.Empty);

				this.StopPingSchedule (); // stop the M_Heartbeat time
				this.CloseWebSocketClient ();// stop websocket

				this.Connect ();

				yield return new WaitForSeconds (5.0f);// wait 5s;
				bool connected = ReadyState == WebSocketState.Open ? true : false;
				if (connected) {
						Debug.LogWarning ("NetSocket: Reconnect successful");
						this.retryConnectionCount = 0;
				} else {	// Didn't connect - try again until exhausted
						if (this.retryConnectionCount < this.RetryConnectionAttempts) {
								this.ReConnect ();
						} else {
								this.Close ();
								this.OnSocketConnectionClosedEvent (this, EventArgs.Empty);
						}
				}
		}

		/// <summary>
		/// <para>Asynchronously calls the action delegate on event message notification</para>
		/// <para>Mimicks the Socket.IO client 'socket.on('name',function(data){});' pattern</para>
		/// <para>Reserved socket.io event names available: connect, disconnect, open, close, error, retry, reconnect  </para>
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="action"></param>
		/// <example>
		/// client.On("testme", (data) =>
		///    {
		///        Debug.WriteLine(data.ToJson());
		///    });
		/// </example>
		public virtual void On (
        string eventName,
        Action<M_Message> action)
		{
				this.registrationManager.AddOnEvent (eventName, action);
		}
		public virtual void On (
        string eventName,
        string endPoint,
        Action<M_Message> action)
		{

				this.registrationManager.AddOnEvent (eventName, endPoint, action);
		}
		/// <summary>
		/// <para>Asynchronously sends payload using eventName</para>
		/// <para>payload must a string or Json Serializable</para>
		/// <para>Mimicks Socket.IO client 'socket.emit('name',payload);' pattern</para>
		/// <para>Do not use the reserved socket.io event names: connect, disconnect, open, close, error, retry, reconnect</para>
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="payload">must be a string or a Json Serializable object</param>
		/// <remarks>ArgumentOutOfRangeException will be thrown on reserved event names</remarks>
		public void Emit (string eventName, System.Object payload, string endPoint, Action<System.Object> callback)
		{

				string emit_eventName = eventName.ToLower ();
				M_Message msg = null;
				switch (emit_eventName) {
				case "message":
						if (payload is string)
								msg = new M_TextMessage () { MessageText = payload.ToString() };
						else
								msg = new M_JSONMessage (payload);
						this.Send (msg);
						break;
				case "connect":
				case "disconnect":
				case "open":
				case "close":
				case "error":
				case "retry":
				case "reconnect":
						Debug.LogError (eventName + " : Event name is reserved by socket.io, and cannot be used by clients or servers with this message type");
						break;
				default:
						if (!string.IsNullOrEmpty (endPoint) && !endPoint.StartsWith ("/"))
								endPoint = "/" + endPoint;
						msg = new M_EventMessage (eventName, payload, endPoint, callback);

                //if (callback != null)
                //    this.registrationManager.AddCallBack(msg);

						this.Send (msg);
						break;
				}
		}

		/// <summary>
		/// <para>Asynchronously sends payload using eventName</para>
		/// <para>payload must a string or Json Serializable</para>
		/// <para>Mimicks Socket.IO client 'socket.emit('name',payload);' pattern</para>
		/// <para>Do not use the reserved socket.io event names: connect, disconnect, open, close, error, retry, reconnect</para>
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="payload">must be a string or a Json Serializable object</param>
		public void Emit (string eventName, System.Object payload)
		{
				this.Emit (eventName, payload, string.Empty, null);
		}

		/// <summary>
		/// Send message to server
		/// </summary>
		/// <param name="msg"></param>
		public void Send (M_Message msg)
		{
				//if (this.outboundQueue != null)
				//    this.outboundQueue.Enqueue(msg.Encoded);
				if (this.ReadyState == WebSocketState.Open) {
						//print("Send Message:" + msg.Encoded);
						this.wsClient.Send (msg.Encoded);
				} else {
						print ("Send Message Error");
				}
		}
		public void Send (string msg)
		{
				M_Message message = new M_TextMessage () { MessageText = msg };
				Send (message);
		}

		/// <summary>
		/// Message Event
		/// </summary>
		/// <param name="msg"></param>
		protected virtual void OnMessageEvent (M_EventMessage msg)
		{
				Debug.Log ("OnMessageEvent");
		}
		protected virtual void OnMessageJson (M_JSONMessage msg)
		{
				Debug.Log ("OnMessageJson");
		}
		protected virtual void OnMessageDisconnect (M_DisconnectMessage msg)
		{
				Debug.Log ("OnMessageDisconnect");
		}
		protected virtual void OnMessageText (M_TextMessage msg)
		{
				Debug.Log ("OnMessageText");
		}
		protected virtual void OnMessageError (M_ErrorMessage msg)
		{
				Debug.Log ("OnMessageError");
		}
		/// <summary>
		/// Close SocketIO.Client,clear all event registrations 
		/// </summary>
		public void Close ()
		{
				this.retryConnectionCount = 0; // reset for next connection cycle
				// stop the M_Heartbeat time
				this.StopPingSchedule ();

				// stop outbound messages
				//this.ResetBoundQueue();

				this.CloseWebSocketClient ();

				this.isJoinRoom = false;
				//if (this.registrationManager != null)
				//{
				//    this.registrationManager.Dispose();
				//    this.registrationManager = null;
				//}

		}


		private void CloseWebSocketClient ()
		{
				if (this.wsClient != null) {
						// unwire events
						this.wsClient.Closed -= this.wsClient_Closed;
						this.wsClient.MessageReceived -= wsClient_MessageReceived;
						this.wsClient.Error -= wsClient_Error;
						this.wsClient.Opened -= this.wsClient_OpenEvent;

						if (this.wsClient.State == WebSocketState.Connecting || this.wsClient.State == WebSocketState.Open) {
								try {
										Debug.LogWarning ("close Web socket");
										this.wsClient.Close ();
								} catch {
										print ("exception raised trying to close websocket: can safely ignore, socket is being closed");
								}
						}
						this.wsClient = null;
				}
		}

    #region Web socket Client Events
		// websocket client events - open, messages, errors, closing
		protected void wsClient_OpenEvent (System.Object sender, EventArgs e)
		{
				print ("wsClient_OpenEvent");

				jobScheduler.AddJob ("PingSchedule", OnPingSchedule, pingTimeInterval, pingTimeInterval, null);
				jobScheduler.AddJob ("DispatchMessage", OnDispatchMessage, updateMessageInterval, updateMessageInterval, null);

				//Debug.LogError("AAAAAAAAAAAAAAAAA");
				if (this.Opened != null) {
						try {
								this.Opened (this, EventArgs.Empty);
						} catch (Exception ex) {
								print (ex);
						}
				}

		}
		/// <summary>
		/// Raw websocket messages from server - convert to message types and call subscribers of events and/or callbacks
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void wsClient_MessageReceived (System.Object sender, MessageReceivedEventArgs e)
		{

				M_Message iMsg = FHNetSocket.M_Message.Factory (e.Message);
				//print("wsClient_MessageReceived: "+e.Message);
				if (iMsg.Event == "responseMsg")
						print (string.Format ("InvokeOnEvent: {0}", iMsg.RawMessage));
				switch (iMsg.MessageType) {
				case SocketIOMessageTypes.Disconnect:
						Debug.LogError ("a player disconnect");
						if (string.IsNullOrEmpty (iMsg.Endpoint)) // Disconnect the whole socket
								this.Close ();
						break;
				case SocketIOMessageTypes.M_Heartbeat:
                //print("Client 435 Ping:" + SocketIOMessageTypes.M_Heartbeat);
						SendPingToServer ();

						break;
				case SocketIOMessageTypes.Connect:
						break;
				case SocketIOMessageTypes.Message:
						break;
				case SocketIOMessageTypes.JSONMessage:
						this.OnMessageJson ((M_JSONMessage)iMsg);
						break;
				case SocketIOMessageTypes.Event:
						this.OnMessageEvent ((M_EventMessage)iMsg);
						break;
				case SocketIOMessageTypes.Error:
						this.OnMessageError ((M_ErrorMessage)iMsg);
						break;
						break;
				case SocketIOMessageTypes.ACK:
                //this.registrationManager.InvokeCallBack(iMsg.AckId, iMsg.Json);
						break;
				default:
						print ("unknown wsClient message Received...");
						break;
				}
		}

		/// <summary>
		/// websocket has closed unexpectedly - retry connection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void wsClient_Closed (System.Object sender, EventArgs e)
		{
				if (this.retryConnectionCount < this.RetryConnectionAttempts) {
						this.ReConnect ();
				} else {
						this.Close ();
						this.OnSocketConnectionClosedEvent (this, EventArgs.Empty);
				}
		}

		private void wsClient_Error (System.Object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
		{
//				if (networkError != null) {
//						networkError (true);
//				}
				//this.OnErrorEvent(sender, new ErrorEventArgs("SocketClient error", e.Exception));
		}

		private void OnErrorEvent (System.Object sender, ErrorEventArgs e)
		{
				this.LastErrorMessage = e.Message;
				if (this.Error != null) {
						try {
								this.Error.Invoke (this, e);
						} catch {
						}
				}
				print (string.Format ("Error Event: {0}\r\n\t{1}", e.Message, e.Exception));
		}
		private void OnSocketConnectionClosedEvent (System.Object sender, EventArgs e)
		{
				if (this.SocketConnectionClosed != null) {
						try {
								this.SocketConnectionClosed (sender, e);
						} catch {
						}
				}
				print ("SocketConnectionClosedEvent");
		}
    #endregion

		private void OnConnectionRetryAttemptEvent (System.Object sender, EventArgs e)
		{
				if (this.ConnectionRetryAttempt != null) {
						try {
								this.ConnectionRetryAttempt (sender, e);
						} catch (Exception ex) {
								Debug.LogError (ex);
						}
				}
				Debug.LogError (string.Format ("Attempting to reconnect: {0}", this.retryConnectionCount));
		}

		/// <summary>
		/// Ping Schedule interval
		/// </summary>
		protected void StopPingSchedule ()
		{
				//Debug.LogError("StopPingSchedule");
				jobScheduler.RemoveJob ("PingSchedule");
		}

		protected bool OnPingSchedule (object param)
		{
				//Debug.Log("OnPingSchedule");
				SendPingToServer ();
				return true;
		}
		protected virtual bool OnDispatchMessage (object param)
		{
				return true;
		}
		protected void SendPingToServer ()
		{
				if (this.ReadyState == WebSocketState.Open) {
						M_Message msg = new M_Heartbeat ();
						try {
								Send (msg);
						} catch (Exception ex) {
								Debug.LogError (string.Format ("OnM_HeartbeatTimerCallback Error Event: {0}\r\n\t{1}", ex.Message, ex.InnerException));
						}
				}
		}


		protected void ResetBoundQueue ()
		{
				jobScheduler.RemoveJob ("OnConcurrentQueueSchedule");
				if (this.outboundQueue != null) {
						//this.outboundQueue.TryDequeue(); // remove adding any more items;
						this.outboundQueue = null;
				}
		}
		protected bool OnConcurrentQueueSchedule (object param)
		{

				if (this.ReadyState == WebSocketState.Open) {
						string msgString;
						try {
								if (this.outboundQueue.TryDequeue (out msgString)) {
										this.wsClient.Send (msgString);
								}
						} catch (Exception ex) {
								Debug.LogError ("The outboundQueue is no longer open...");
						}
				}
				return true;
		}

		// abstract game function
//    public virtual void JoinRoom(SocketJoinRoomType roomtype, string userName)// data is a package message
//    {
//
//    }
		//public virtual void LeaveRoom(string roomName,object data)// data is a package message
		//{

		//}
}

