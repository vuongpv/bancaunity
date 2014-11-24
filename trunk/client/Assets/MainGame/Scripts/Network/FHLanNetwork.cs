using UnityEngine;
using System;
using System.Collections;

public class FHLanNetwork : SingletonMono<FHLanNetwork>
{
    #region Global variable
		public static string GAME_NAME_INLAN = "GSS_FISHHURT";
		public static float LIMIT_TIME_DELAY = 0.05f;// by second
		public static float TIME_PLAYING_ONLAN = 100;// by second
		public static int MINIMUM_PLAYER_TOPLAY = 1;
		public static int GAME_SERVER_PORT = 45672;
		public static System.Random rand = new System.Random ();
		public static int GetRandomPort ()
		{
				return rand.Next (10000) + 10000;
		}
   
    #endregion

		public FHLobbyChat fhLobbyChat;
		public FHLobbyGame fhLobbyGame;
		public bool isGameFinish = false;
		public void SetGameState (bool isstate)
		{
				isGameFinish = isstate;
		}

		private static float networkTime = 0;
		private static float timeBeginNetwork = 0;
		public static void SetNetworkTime ()
		{
				timeBeginNetwork = Time.realtimeSinceStartup;
				networkTime = 0;
		}
		public static float GetNetworkTime ()
		{
				networkTime = Time.realtimeSinceStartup - timeBeginNetwork;
				return networkTime;
		}

		private bool lastForceNat = false;// last MS Connection Attempt Forced Nat
		private bool natOptionTest = false;//NAT option Was Switched For Testing

		private int[] remotePort = new int[3];
		private string[] remoteIP = new string[3];
		private bool tryingToConnectquickplay = false;
		private int tryingToConnectquickplayNumber = 0;
		public void Initial ()
		{
				remoteIP [0] = "127.0f.0.1f";
				remotePort [0] = FHLanNetwork.GAME_SERVER_PORT;
				if (fhLobbyChat == null) {
						fhLobbyChat = gameObject.GetComponent<FHLobbyChat> ();
				}
				if (fhLobbyGame == null) {
						fhLobbyGame = gameObject.GetComponent<FHLobbyGame> ();
//						fhLobbyGame.checkDisconnectHost += DisconnectHost;
				}
        
		}
		// Use this for initialization
		//void Start()
		//{

		//}

		// Update is called once per frame
		void Update ()
		{
		}

		public HostData[] GetHostServer ()
		{
				MasterServer.RequestHostList (FHLanNetwork.GAME_NAME_INLAN);
				HostData[] data = MasterServer.PollHostList ();
				return data;
		}
		public void Reset ()
		{
				SetGameState (false);
				MasterServer.UnregisterHost ();
				Network.Disconnect ();
    
		}
		public NetworkConnectionError ConnectWithPassword (string[] serverIp, int port, string password)
		{
				remoteIP = serverIp;
				remotePort [0] = port;
				Reset ();
				NetworkConnectionError error = Network.Connect (serverIp, port, password);
				if (error == NetworkConnectionError.NoError) {
						fhLobbyGame.EnableLobby ();
				}
				return error;
		}
		public NetworkConnectionError Connect (string[] serverIp, int port)
		{
				remoteIP = serverIp;
				remotePort [0] = port;
				Reset ();
				NetworkConnectionError error = Network.Connect (serverIp, port);
				if (error == NetworkConnectionError.NoError) {
						fhLobbyGame.EnableLobby ();
				}
				return error;
		}

//		void OnFailedToConnect (NetworkConnectionError info)
//		{
//
//				Debug.LogError ("Failed To Connect info: " + info);
//
//				bool invalidPass = false;
//				if (!isGameFinish) {
//						if (info == NetworkConnectionError.InvalidPassword) {
//								invalidPass = true;
//						}
//						if (invalidPass) {
//								GUIMessageDialog.Show (OnDisconnectProcess, "Password is not correct", "Notice", FH.MessageBox.MessageBoxButtons.OK);
//						} else {
//								GUIMessageDialog.Show (OnDisconnectProcess, "Connect server failed", "Notice", FH.MessageBox.MessageBoxButtons.OK);
//						}
//				}
//				//FailedConnRetry(invalidPass);
//		}
//    void DisconnectHost(bool disconnected)
//    {
//        if (!isGameFinish)
//        {
//            GUIMessageDialog.Show(OnDisconnectResult, "Disconnect with server!", "Notice", FH.MessageBox.MessageBoxButtons.OK);
//        }
//    }
//    public bool OnDisconnectResult(FH.MessageBox.DialogResult result)
//    {
//        if (result == FH.MessageBox.DialogResult.Ok)
//        {
//            GuiManager.HidePanel(GuiManager.instance.guiJoinRoomHandler);
//            GuiManager.ShowPanel(GuiManager.instance.guiLanMultiPlayer);
//            if (SceneManager.instance.GetCurrentScene()!="MainMenu")
//            {
//                SceneManager.instance.LoadScene("MainMenu");
//            }
//        }
//
//		return true;
//    }
//    public bool OnDisconnectProcess(FH.MessageBox.DialogResult result)
//    {
//        if (result == FH.MessageBox.DialogResult.Ok)
//        {
//            GuiManager.HidePanel(GuiManager.instance.guiJoinRoomHandler);
//            GuiManager.ShowPanel(GuiManager.instance.guiLanMultiPlayer);
//            if (SceneManager.instance.GetCurrentScene() != "MainMenu")
//            {
//                SceneManager.instance.LoadScene("MainMenu");
//            }
//        }
//
//		return true;
//    }

		// game fishurt lan logic
//		public e vent Action<FHLanCapturePackage> actionGameLog=null;
//		[RPC]
//		void SycnData (NetworkPlayer player, byte[] data)
//		{
//				if (actionGameLog != null) {
//						actionGameLog (new FHLanCapturePackage (player, data));
//				}
//		}
//		public void SendSycnData (FHLanCapturePackage package)
//		{
//				networkView.RPC ("SycnData", RPCMode.Others, package.player, package.data);
//		}
//
//		public e vent Action<bool> actionStartGame = null;
//		[RPC]
//		void StartGame ()
//		{
//				if (actionStartGame != null) {
//						actionStartGame (true);
//				}
//		}
		public void CallStartGame ()
		{
				networkView.RPC ("StartGame", RPCMode.All);
		}
		public bool isConnect ()
		{
				if (Network.isClient || Network.isServer) {
						return true;
				}
				return false;
		}

}
