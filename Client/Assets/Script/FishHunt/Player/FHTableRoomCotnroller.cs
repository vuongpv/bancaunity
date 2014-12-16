using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ModeOnline
{
		TabBetNormal=1,
		TabDiamond=2
}

public class FHTableRoomCotnroller : SingletonMono<FHTableRoomCotnroller>
{
		public UILabel m_NamePlayer, m_DiamondPlayer, m_GoldPlayer;
		private string SID;
		private GameObject[] tables, waitingJoinRoom;
		private UILabel[] numberPrice, nameTable;
		public static UISprite[] disableBG, goldIcons, diamondIcons, buttonEnableJoinRooms, buttonDisableJoinRooms;
		public GameObject prefabsTable;
		public GameObject waitingObject;
		public UIWrapContent wrapContent;
		private bool isReadyConnect = false;
		private float countDownTime = -5;
		private int modeOnline = (int)ModeOnline.TabBetNormal;
		private FHClient client;
		private List<PublicRoomData> managerRoom;
		private PublicRoomData roomJoin;
		private int position = -1;

		void Awake ()
		{
				waitingObject.SetActiveRecursively (true);
				managerRoom = new List<PublicRoomData> ();
				FHNetworkManager.Connect (obj => {
						MC_S_Connected result = obj as MC_S_Connected;
						
						FHNetworkManager.SendRequestToServer (new RequestLogin (SystemInfo.deviceUniqueIdentifier), typeof(ResponseLogin), resp => {
								ResponseLogin res = resp as ResponseLogin;	
								if (res.retCode == (int)ResultCode.OK) {
										string uid = res.uid;
										Debug.Log ("isMission: " + res.isMission);
										FHUsersManager.instance.CreatePlayerMe (uid);
//										Debug.Log ("Login === OK: " + uid);	
					
										FHNetworkManager.SendRequestToServer (new Request_Properties (uid), typeof(Response_Properties), respProperties => {
												Response_Properties resProperties = respProperties as Response_Properties;	
												if (resProperties.retCode == (int)ResultCode.OK) {
//														Debug.Log ("=========properties: " + resProperties.properties);
														ParseMyInformation (resProperties.properties);
												}
										});

										FHNetworkManager.SendRequestToServer (new Request_JoinWaittingRoom (uid), typeof(Response_JoinWaittingRoom), respJoinWaittingRoom => {
												Response_JoinWaittingRoom responseJoinWaittingRoom = respJoinWaittingRoom as Response_JoinWaittingRoom;
												if (responseJoinWaittingRoom.retCode == (int)ResultCode.OK) {
														if (!responseJoinWaittingRoom.result) {
																waitingObject.SetActiveRecursively (false);
																Application.LoadLevel (FHScenes.MainMenu);
																return;
														}
												}
										});
					
										FHNetworkManager.SendRequestToServer (new Request_Rooms (uid), typeof(Response_Rooms), ret => {
												Response_Rooms infor = ret as Response_Rooms;
												managerRoom = infor.rooms;

												LoadTables (managerRoom);
												RefeshPosition (infor.positions);

												waitingObject.SetActiveRecursively (false);
										});
								}
						});
				});
		}



		private void LoadTables (List<PublicRoomData> managerRoom)
		{
//				Transform parent = GameObject.Find ("Tables").transform;
				Transform parent = GameObject.Find ("UIWrap Content").transform;
				numberPrice = new UILabel[managerRoom.Count];
				nameTable = new UILabel[managerRoom.Count];
				disableBG = new UISprite[managerRoom.Count];
				tables = new GameObject[managerRoom.Count];
				waitingJoinRoom = new GameObject[managerRoom.Count];
				goldIcons = new UISprite[managerRoom.Count];
				diamondIcons = new UISprite[managerRoom.Count];
				buttonEnableJoinRooms = new UISprite[3 * managerRoom.Count];
				buttonDisableJoinRooms = new UISprite[3 * managerRoom.Count];

				BoxCollider box = null;
				string nameJoin = "", nameDisable = "";
				for (int i=0; i<managerRoom.Count; i++) {
						tables [i] = Instantiate (prefabsTable) as GameObject;
						tables [i].SetActive (true);
						tables [i].transform.parent = parent;
						tables [i].transform.localScale = Vector3.one;
						
						waitingJoinRoom [i] = tables [i].transform.FindChild ("Waiting").gameObject as GameObject;

						goldIcons [i] = tables [i].transform.FindChild ("IconGold").GetComponent<UISprite> ();


						disableBG [i] = tables [i].transform.FindChild ("TableDis").GetComponent<UISprite> ();


						diamondIcons [i] = tables [i].transform.FindChild ("IconDiamond").GetComponent<UISprite> ();

						if (modeOnline == (int)ModeOnline.TabBetNormal) {
								LoadTableNormal (i);
						} else {
								LoadTableDiamond (i);
						}

						numberPrice [i] = tables [i].transform.FindChild ("NumberGold").GetComponent<UILabel> ();
						numberPrice [i].text = managerRoom [i].price + "";

						nameTable [i] = tables [i].transform.FindChild ("TableEnable").transform.FindChild ("NameTable").GetComponent<UILabel> ();
						if (managerRoom [i].skintype == 0) {
								nameTable [i].text = "Player: " + managerRoom [i].maxUser;
						} else {
								nameTable [i].text = "Auto";
						}
//						nameTable [i].text = managerRoom [i].name + "";
						if (box == null) {
								box = tables [i].GetComponent<BoxCollider> ();
						}
						tables [i].transform.localPosition = new Vector3 (i * box.size.x, 0, 0);
				}

//				int _row = 2;
//				int _col = _roomPrices.Length / _row;
//				float xStart = (Screen.width - _col * (box.size.x + 10)) / 2;
//				float yStart = GameObject.Find ("languages").transform.position.y;
//				for (int i=0; i<_row; i++) {
//						for (int j=0; j<_col; j++) {
//								tables [(i * _col) + j].transform.localPosition = new Vector3 (xStart + j * (box.size.x + 10) - box.size.x / 2, yStart + box.size.y / 2 + -i * (box.size.y + 20), 0);
//						}
//				}

		}

		void LoadTableDiamond (int i)
		{

				for (int k=0; k<3; k++) {

						string nameJoin = "Diamond_P" + (k + 1) + "_0";
						string nameDisable = "Diamond_P" + (k + 1) + "_1";
						
						buttonEnableJoinRooms [i * 3 + k] = tables [i].transform.FindChild ("P" + (k + 1) + "_0").GetComponent<UISprite> ();
						buttonEnableJoinRooms [i * 3 + k].name = i + nameJoin;
						buttonDisableJoinRooms [i * 3 + k] = tables [i].transform.FindChild ("P" + (k + 1) + "_1").GetComponent<UISprite> ();
						buttonDisableJoinRooms [i * 3 + k].name = i + nameDisable;
				}
				goldIcons [i].gameObject.SetActive (false);
				diamondIcons [i].gameObject.SetActive (true);
				if (float.Parse (m_DiamondPlayer.text) < managerRoom [i].price) {
						disableBG [i].gameObject.SetActive (true);	
				} else {
						disableBG [i].gameObject.SetActive (false);	
				}
		}

		void LoadTableNormal (int i)
		{
				for (int k=0; k<3; k++) {
			
						string nameJoin = "P" + (k + 1) + "_0";
						string nameDisable = "P" + (k + 1) + "_1";
						buttonEnableJoinRooms [i * 3 + k] = tables [i].transform.FindChild ("P" + (k + 1) + "_0").GetComponent<UISprite> ();
						buttonEnableJoinRooms [i * 3 + k].name = i + nameJoin;
						buttonDisableJoinRooms [i * 3 + k] = tables [i].transform.FindChild ("P" + (k + 1) + "_1").GetComponent<UISprite> ();
						buttonDisableJoinRooms [i * 3 + k].name = i + nameDisable;
				}
				goldIcons [i].gameObject.SetActive (true);
				diamondIcons [i].gameObject.SetActive (false);
				if (float.Parse (m_GoldPlayer.text) < managerRoom [i].price) {
						disableBG [i].gameObject.SetActive (true);	
				} else {
						disableBG [i].gameObject.SetActive (false);	
				}
		}

		private void ParseMyInformation (string infor)
		{
				string[] subString = new string[] { "$$" };
				string[] infors = infor.Split (subString, StringSplitOptions.RemoveEmptyEntries);
				m_NamePlayer.text = infors [0];
				m_GoldPlayer.text = infors [2];
				m_DiamondPlayer.text = infors [3];

				FHPlayerProfile.instance.gold = int.Parse (infors [2]);
				FHPlayerProfile.instance.diamond = int.Parse (infors [3]);
		}

		public  void RefeshPosition (String positions)
		{
				string[] subString = new string[] { "$$" };
				string[] infors = positions.Split (subString, StringSplitOptions.RemoveEmptyEntries);
				
				for (int i=0; i<buttonEnableJoinRooms.Length; i++) {
						if (infors [i].Equals ("1")) {
								buttonEnableJoinRooms [i].gameObject.SetActive (false);
								buttonDisableJoinRooms [i].gameObject.SetActive (true);
						} else {
								buttonEnableJoinRooms [i].gameObject.SetActive (true);
								buttonDisableJoinRooms [i].gameObject.SetActive (false);
						}
						
				}
		}
		public  void OnselectPositionFail ()
		{
				for (int i=0; i<waitingJoinRoom.Length; i++) {
						if (waitingJoinRoom [i] == null)
								continue;

						waitingJoinRoom [i].SetActiveRecursively (false);
				}

				StopCoroutine ("StartJoinFishRoom");
				StopCoroutine ("TimeOutJoinRoom");
		}

		public void OnClickJoinFishRoom ()
		{
				int indexroom = 0;
				string nameButton = UICamera.selectedObject.name;
				Debug.Log ("=========================OnClickJoinFishRoom: " + nameButton);
				if (nameButton == null || nameButton.Trim ().Equals (""))
						return;
				string _indexRoom = "";
				for (int i=0; i<nameButton.Length; i++) {
						if (nameButton [i] == 'P')
								break;
						_indexRoom += nameButton [i];
				}


				position = int.Parse (nameButton [nameButton.IndexOf ("P") + 1] + "") - 1;

				indexroom = int.Parse (_indexRoom);

				OnBetItemClick (indexroom);
		}

		public void OnClickButton ()
		{
				string nameButton = UICamera.selectedObject.name;
				Debug.Log ("=========================Name button: " + nameButton);
				
				switch (nameButton) {
				case "BtnClose":
						{
								OnCloseFunc ();
						}
						break;
				case "ButtonOnline":
//						GuiManager.ShowPanel (GuiManager.instance.guiOnlinePlay, UIOnlinePlay.UIOnlineTab.TabBetNormal);
				
						break;
				case "BackBtn":

						FHNetworkManager.SendMessageToServer (new M_C_LeaveWaitingRoom (FHUsersManager.instance.UserMe.ClientId));
						FHNetworkManager.SendMessageToServer (new M_C_LeaveCurrentRoom (FHUsersManager.instance.UserMe.ClientId));
						break;
				
				}
		}

		public  void OnBeginShow (object parameter)
		{
				#if UNITY_EDITOR
				Application.runInBackground = true;
				#endif
				client = FHNetworkManager.Client ();
				client.networkError += OnNetworkError;
				client.checkedJoinRoomFailed += OnServerFull;
				isReadyConnect = false;
				OnOpen ();
		}
	
		public  void OnBeginHide (object parameter)
		{
				if (client != null) {
						client.networkError -= OnNetworkError;
						client.checkedJoinRoomFailed -= OnServerFull;
				}
		}

		public void OnNetworkError (bool result)
		{
				GUIMessageDialog.Show (OnErrorClose, FHLocalization.instance.GetString (FHStringConst.ONLINE_CONNECT_ERROR) + "_", FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
		}

		public void OnServerFull (bool result)
		{
				GUIMessageDialog.Show (OnErrorClose, FHLocalization.instance.GetString (FHStringConst.ONLINE_SERVER_FULL), FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
		}

		public bool OnErrorClose (FH.MessageBox.DialogResult result)
		{
				if (SceneManager.instance.GetCurrentScene () != FHScenes.Online) {
						OnClose ();
				}
				return true;
		}

		public void OnOpen ()
		{
				StopCoroutine ("StartJoinRoomAI");
				StopCoroutine ("TimeOutJoinRoom");
				gameObject.SetActiveRecursively (true);
		
				FHNetworkManager.Connect (obj => {
						MC_S_Connected result = obj as MC_S_Connected;
						FHNetworkManager.SendRequestToServer (new RequestLogin (SystemInfo.deviceUniqueIdentifier), typeof(ResponseLogin), resp => {
								ResponseLogin res = resp as ResponseLogin;
								if (res.retCode == (int)ResultCode.OK) {
										string uid = res.uid;
										FHUsersManager.instance.CreatePlayerMe (uid);
								}});
				});
		
				for (int i=0; i<waitingJoinRoom.Length; i++) {
						waitingJoinRoom [i].SetActiveRecursively (false);
				}
		}
	
		public void ShowWaiting (int idx)
		{
				waitingJoinRoom [idx].SetActiveRecursively (true);
//				GuiManager.ShowPanel (GuiManager.instance.guiChat);
		}
	
		public void OnBetItemClick (int index)
		{
				for (int i=0; i<waitingJoinRoom.Length; i++) {
						if (waitingJoinRoom [i] == null || !waitingJoinRoom [i].activeSelf)
								continue;
						return;
				}
				if (!managerRoom [index].status) {
						Debug.LogWarning ("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ Nta dang danh");
						return;
				}
				roomJoin = managerRoom [index];
				if (managerRoom [index].price > FHPlayerProfile.instance.gold) {
						return;
				}
				if (!isReadyConnect) {
						isReadyConnect = true;
				} else {
						return;
				}
				
				float now = Time.realtimeSinceStartup;
		
				ShowWaiting (index);
		
				// stop old coroutine
				StopCoroutine ("StartJoinRoom");
				StopCoroutine ("StartJoinRoomAI");
				StopCoroutine ("TimeOutJoinRoom");
				StopCoroutine ("StartJoinFishRoom");

				StartCoroutine (StartJoinFishRoom ());
		
		}

		

		public void OnBetDiamondItemClick (int index)
		{
				if (managerRoom [index].price > FHPlayerProfile.instance.diamond) {
						//GUIMessageDialog.Show(null,"You don't have enough money for this bet", "Information",FH.MessageBox.MessageBoxButtons.OK);
						return;
				}
				if (!isReadyConnect) {
						isReadyConnect = true;
				} else {
						return;
				}
				
				float now = Time.realtimeSinceStartup;
		
		
				ShowWaiting (index);
		
				// stop old coroutine
				StopCoroutine ("StartJoinRoom");
				StopCoroutine ("StartJoinRoomAI");
				StopCoroutine ("TimeOutJoinRoom");
				StartCoroutine (StartJoinFishRoom ());
//				StartCoroutine (StartJoinRoomDiamond ());
				//        GoogleAnalyticsBinding.SendEvent("OnlineMode", "JoinRoomDiamond", "online", 1);
		}
	

		public IEnumerator StartJoinFishRoom ()
		{
				yield return new WaitForSeconds (1);
				FHNetworkManager.JoinFishRoom (roomJoin.id, FHUtils.GetPlayerName (), position);
		}



		public IEnumerator TimeOutJoinRoom ()
		{
				yield return new WaitForSeconds (FHUtils.WAIT_TIME_OUT);
				GUIMessageDialog.Show (OnCloseResult, FHLocalization.instance.GetString (FHStringConst.ONLINE_CONNECT_ERROR) + ".", FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
		
		}

		public void OnCloseFunc ()
		{
				StopCoroutine ("ChangeTextWaiting");
				GuiManager.HidePanel (GuiManager.instance.guiOnlinePlay);
				GameObject.Destroy (client);
		}

		public void OnClose ()
		{
				//        client.Close();
				StopCoroutine ("StartJoinRoom");
				StopCoroutine ("StartJoinRoomAI");
				StopCoroutine ("ChangeTextWaiting");
				StopCoroutine ("TimeOutJoinRoom");
				GuiManager.HidePanel (GuiManager.instance.guiOnlinePlay);
				GameObject.Destroy (client);
		}

		private bool OnCloseResult (FH.MessageBox.DialogResult result)
		{
				if (result == FH.MessageBox.DialogResult.Yes || result == FH.MessageBox.DialogResult.Ok) {
						OnClose ();
				}
				return true;
		}
	
		
}
