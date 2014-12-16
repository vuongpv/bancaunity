using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FHSceneOnlineController : SingletonMono<FHSceneOnlineController>
{
		public UIOnlineFinalScore onlineFinalResult;
		public FHPlayerOnline[] players;
		public GameObject[] buttonDec, buttonInc, panelPlayers;
		public GameObject WaitMenu;
		public UILabel lblTimePlay;

		public UISprite coinOver1;
		public UISprite coinOver2;
		public UISprite coinOver3;
		public UISprite coinIcon1;
		public UISprite coinIcon2;
		public UISprite coinIcon3;

		public float timeBeginAwake = 0;
		private float timeUpdateSequence = 0;
		private float timeReadyStart = 0;
		private float sequenceTime = 0.1f;
		private float checkSendCount = 0;
		private float sequenceTimeSendData = 0.1f;// max =10 in second
		private float timeForPlay = 500;
		private bool isAccessGame;
		private int countCheckReady = 0;
		[HideInInspector]
		public FHUserOnlinePlay
				playerInfoMe;
		private bool isGameEnd = false;

		
		private FHRoomOnlinePlay roomData;
	
		private FHClient client;
	
		// GUI for final result
		private bool isTimeUp = false;
		// reconnect
		private bool isReconnectSuccess = true;
		private int reConnectCount = 0;
		private bool isSetResult = false;
	
		void Start ()
		{
				isReconnectSuccess = true;
				reConnectCount = 0;
				timeBeginAwake = Time.realtimeSinceStartup;
				timeReadyStart = Time.realtimeSinceStartup;
				timeUpdateSequence = 0;
				isSetResult = false;
				isTimeUp = false;
				StartCoroutine (CheckReady ());
		
		
				client = FHNetworkManager.Client ();
				client.checkedUserOnlineReady += CheckReadyToPlay;
				client.logClientLogic += ActionGameLog;
				client.getFinalResult += GetFinalResult;
				client.getReconnectResult += GetReconnectResult;
				client.networkError += OnNetworkError;
				
				roomData = client.fhRoomOnlinePlay;
				client.roomName = roomData.roomName;
				client.roomType = roomData.roomType;
				sequenceTimeSendData = roomData.timeSequenceUpdate;
				sequenceTime = sequenceTimeSendData;
				timeForPlay = roomData.timePlay;
				
				Debug.Log (roomData.timePlay + "," + sequenceTimeSendData);

		
				FHFishSeasonManager.instance.SeasonOnlineName = FHUtils.FH_PATH_ROUTE_ONLINE + roomData.routeID;

				for (int i = 0; i < roomData.listPlayer.Count; i++) {// tam thoi chi cho 3 nguoi choi
						FHUserOnlinePlay user = roomData.listPlayer [i];
						
						if (user.uid.Equals (FHUsersManager.instance.UserMe.ClientId)) {// player me
								players [user.location].gameObject.SetActive (true);
								panelPlayers [user.location].gameObject.SetActive (true);
								players [user.location].SetupOnline (user, roomData.isDiamondRoom);
								playerInfoMe = user;
				
								BoxCollider box = players [user.location].GetComponent<BoxCollider> ();
								box.gameObject.SetActive (true);
								box.enabled = true;

								players [user.location].isMainPlayer = true;
								buttonDec [user.location].GetComponent<BoxCollider> ().enabled = true;
								buttonInc [user.location].GetComponent<BoxCollider> ().enabled = true;
								
						} else { // setup for another player
								players [user.location].gameObject.SetActive (true);
								panelPlayers [user.location].gameObject.SetActive (true);
								players [user.location].isMainPlayer = false;
								players [user.location].SetupOnline (user, roomData.isDiamondRoom);
						}
				}


				for (int i=0; i<players.Length; i++) {
						if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null)
								continue;
						players [i].SetupClient ();
				}
			
				if (roomData.isAutoPlay) {

						FHAIAutoPlay.instance.isActiveAuto = false;
						StartCoroutine (FHAIAutoPlay.instance.SetDelayActive (3));
						float ran = FHUtils.rand.Next (0, 100) / 40.0f;
						
						for (int i=0; i<players.Length; i++) {
								if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null)
										continue;
								if (players [i].playerOnline.uid != playerInfoMe.uid) {
										players [i].isAutoPlay = true;
								}
						}
				} else {
						for (int i=0; i<players.Length; i++) {
								if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null)
										continue;
								if (players [i].playerOnline.uid != playerInfoMe.uid) {
										players [i].isAutoPlay = false;
								}
						}
						FHAIAutoPlay.instance.isActiveAuto = false;
				}
				if (roomData.isDiamondRoom) {
						coinOver1.spriteName = FHUtils.NAME_DIAMOND_ICON;
						coinOver2.spriteName = FHUtils.NAME_DIAMOND_ICON;
						coinOver3.spriteName = FHUtils.NAME_DIAMOND_ICON;

						coinIcon1.spriteName = FHUtils.NAME_DIAMOND_ICON;
						coinIcon2.spriteName = FHUtils.NAME_DIAMOND_ICON;
						coinIcon3.spriteName = FHUtils.NAME_DIAMOND_ICON;
				}
				isGameEnd = false;
				GuiManager.HidePanel (GuiManager.instance.guiOnlinePlay);
				#if UNITY_EDITOR
				Application.runInBackground = true;
				#endif
		}
	
		void OnDestroy ()
		{
		
		

		}

		public void DestroyClient ()
		{
				if (client != null) {
						client.checkedUserOnlineReady -= CheckReadyToPlay;
						client.logClientLogic -= ActionGameLog;
						client.getFinalResult -= GetFinalResult;
						client.getReconnectResult -= GetReconnectResult;

				}
		}

		public IEnumerator CheckReady ()
		{
				yield return new WaitForSeconds (2);
				FHNetworkManager.SendMessageToServer (new M_C_SycnReady (FHUsersManager.instance.UserMe.ClientId));
		}

		public IEnumerator HidePanel (float percent)
		{
				yield return new WaitForSeconds (0.05f);
				gameObject.GetComponent<UIPanel> ().alpha = percent;
				percent -= 0.05f;
				if (percent < 0) {
						WaitMenu.SetActiveRecursively (false);
						StopCoroutine ("HidePanel");
				} else {
						StartCoroutine (HidePanel (percent));
				}
		}

		public IEnumerator ShowPanel (float percent)
		{
				yield return new WaitForSeconds (0.05f);
				if (percent >= 1)
						StopCoroutine ("ShowPanel");
				else {
						percent += 0.05f;
						gameObject.GetComponent<UIPanel> ().alpha = percent;
						StartCoroutine (ShowPanel (percent));
				}
		}

		public void CheckReadyToPlay (int countUser)
		{
				Debug.LogError ("CheckReadyToPlay: " + countUser + " --- " + roomData.listPlayer.Count);
				if (countUser == roomData.listPlayer.Count) {
						isAccessGame = true;
						timeReadyStart = Time.realtimeSinceStartup;
						StartCoroutine (HidePanel (1));
			
						FHFishSeasonManager.instance.canStart = true;
						// tru tien
						if (!roomData.isDiamondRoom) {
								int betGold = (int)roomData.price;
								if (betGold > 0) {
										FHPlayerProfile.instance.gold -= betGold;
										FHPlayerProfile.instance.ForceSave ();
								}
						}
						if (roomData.isAutoPlay) {
								GoogleAnalyticsBinding.SendEvent ("AutoPlay AI", "ReadyPlay", "online", 1);
						} else {
								//                if (!roomData.isDiamondRoom)
								//                {
								//                    GoogleAnalyticsBinding.SendEvent("PVP Gold Play", "ReadyPlay", "online", 1);
								//                }
								//                else
								//                {
								//                    GoogleAnalyticsBinding.SendEvent("PVP Diamond Play", "ReadyPlay", "online", 1);
								//                }
						}
				}
		}

		public void ActionGameLog (string listEvent, long timeServer = 0)
		{
				// message just for nother player
				FHNetEvent_ClientLogicEvent logicClient = null;
				#if UNITY_IPHONE  || IOS
		logicClient = new FHNetEvent_ClientLogicEvent();
		if (!logicClient.Deserialize(SimpleJSON.JSONNode.Parse(listEvent)))
		{
			Debug.LogError("FH Message event client logic error");
			return;
		}      
				#else
				try {
						logicClient = FHUtils.ToObject<FHNetEvent_ClientLogicEvent> (listEvent);
				} catch (Exception ex) {
						Debug.LogError ("FH Message event client logic error");
						return;
				}
				#endif
		
				//        if (logicClient == null)
				//        {
				//            Debug.LogError("FH Message event client logic error");
				//        }
				//        if (playerInfoMe==null)
				//        {
				//            Debug.LogError("FH Message event player me null");
				//        }
				//        if (playerInfoMe.sid == logicClient.SID)// tam thoi choi chi co 2 nguoi
				//        {
				//            Debug.Log("Except me");
				//            return;
				//        }
		
				
				float _time = FHNetworkManager.GetSecondPlayOnline (timeServer);
				FHFishSeason season = FHFishSeasonManager.instance.GetCurrentSeason ();
				if (season != null) {
						
						season.SyncTimeSpawn (_time);
				}
		
				Dictionary<int, FHNetEvent_Logic> listEvts = FHOnlineCapturePackage.Deserialize (listEvent);
				foreach (KeyValuePair<int, FHNetEvent_Logic> pair in listEvts) {
						//Debug.LogError(pair.Value.eventName);
						FHNetEvent_Logic evt = pair.Value;
						switch (evt.eventName) {
						case (int)FHMessageProperties.ChangeMoney:
								{
										FHNetEvent_ChangeMoney _event = (FHNetEvent_ChangeMoney)evt;

										for (int i=0; i<players.Length; i++) {
												if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null || players [i].playerOnline.uid == playerInfoMe.uid)
														continue;
												if (FHNetworkManager.UID == (players [i].playerOnline.uid)) {
						
														players [i].ProcessLanChangeGold (_event.cMoney);
												}
										}


								}
								break;
						case (int)FHMessageProperties.FigureMove:
								{
										FHNetEvent_Figure _event = (FHNetEvent_Figure)evt;

										for (int i=0; i<players.Length; i++) {
												if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null || players [i].playerOnline.uid == playerInfoMe.uid)
														continue;
												if (FHNetworkManager.UID == (players [i].playerOnline.uid)) {
														players [i].ProcessLanFigureMove (_event.rA);
												}
										}

								}
								break;
						case (int)FHMessageProperties.UpgradeGun:
								{

										FHNetEvent_ChangeGun _event = (FHNetEvent_ChangeGun)evt;
										for (int i=0; i<players.Length; i++) {
												if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null || players [i].playerOnline.uid == playerInfoMe.uid)
														continue;
												if (FHNetworkManager.UID == (players [i].playerOnline.uid)) {
														players [i].ProcessLanCurrentGun (_event.gID);
												}
										}

								}
								break;
						case (int)FHMessageProperties.Shot:
								{
										FHNetEvent_Shot _event = (FHNetEvent_Shot)evt;
										for (int i=0; i<players.Length; i++) {
												if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null || players [i].playerOnline.uid == playerInfoMe.uid)
														continue;
												if (FHNetworkManager.UID == (players [i].playerOnline.uid)) {
														players [i].ProcessLanShot (_event.gID, 0, _event.rA);
												}
										}

								}
								break;
						case (int)FHMessageProperties.StopShot:
								{
										FHNetEvent_StopShot _event = (FHNetEvent_StopShot)evt;

										for (int i=0; i<players.Length; i++) {
												if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null || players [i].playerOnline.uid == playerInfoMe.uid)
														continue;
												if (FHNetworkManager.UID == (players [i].playerOnline.uid)) {
														players [i].ProcessLanStopShot ();
												}
										}


								}
								break;
						case (int)FHMessageProperties.HitFish:
								{
										FHNetEvent_HitFish _event = (FHNetEvent_HitFish)evt;

										for (int i=0; i<players.Length; i++) {
												if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null || players [i].playerOnline.uid == playerInfoMe.uid)
														continue;
												if (FHNetworkManager.UID == (players [i].playerOnline.uid)) {
														players [i].ProcessLanHitFish (0, 0, _event.fDieID);
												}
										}

								}
				
								break;
						}
				}
		
		}
	
		public void GetFinalResult (string jsonObject)
		{
				FHNetEvent_FinalResult _result = null;
				#if UNITY_IPHONE  || IOS
		_result = new FHNetEvent_FinalResult();
		if (!_result.Deserialize(SimpleJSON.JSONNode.Parse(jsonObject)))
		{
			Debug.LogError("FH Message event client logic error");
			return;
		}
				#else
				try {
						_result = FHUtils.ToObject<FHNetEvent_FinalResult> (jsonObject);
				} catch (Exception ex) {
						Debug.LogError ("FH Message event client logic error");
						return;
				}
				#endif

				for (int i=0; i<players.Length; i++) {
						if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null)
								continue;
						if (_result.UID == players [i].playerOnline.uid) {
								players [i].gold = _result.score;
						}
				}
		}
		// Update is called once per frame
		void Update ()
		{
				if (!isAccessGame) {
						float now = Time.realtimeSinceStartup;
						if (now - timeBeginAwake > 12) {
								timeBeginAwake = now;
								//isAccessGame = true;
								GUIMessageDialog.Show (OnErrorFunction, FHLocalization.instance.GetString (FHStringConst.ONLINE_ERROR_ACCESS_FAIL), FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
						}
				} else {
						float now = Time.realtimeSinceStartup;
						if (!isGameEnd) {
								if (!FHNetworkManager.IsConnected) {
										OnNetworkError (true);
								}

								for (int i=0; i<players.Length; i++) {
										if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null)
												continue;

										if (players [i].playerOnline.uid != playerInfoMe.uid) 
												continue;


										if (players [i].FHNetEvent_Capture.listEvent.Count > 0) {
												if (now - timeUpdateSequence > sequenceTime) {
														//Debug.LogError("Check routine");
														checkSendCount++;
														if (checkSendCount > 100) {
																sequenceTime = 1;
																checkSendCount = 0;
														} else {
																sequenceTime = sequenceTimeSendData;
														}
														timeUpdateSequence = now;
														if (players [i].FHNetEvent_Capture.listEvent.Count > 0) {
																FHOnlineCapturePackage package = new FHOnlineCapturePackage (playerInfoMe.uid, players [i].FHNetEvent_Capture.listEvent);
																//Debug.Log(package.jsonMsgs);
																if (package.jsonMsgs.Length > 0) {
#if UNITY_IPHONE  || IOS
#else
																		package.jsonMsgs = package.jsonMsgs.Replace ("\\\"", "\"");
#endif
																		Debug.LogWarning ("Send_ClientLogicEvent cho nay ne: " + package.UID + " --- " + package.jsonMsgs);
																		//                                client.Send_ClientLogicEvent(package.SID, package.jsonMsgs,fhPlayer01.gold);
									
																		FHNetworkManager.SendMessageToServer (new M_C_CaptureLogic (package.UID, package.jsonMsgs, players [i].gold));
																}
																players [i].ResetSyncDataOnline ();
														}
												}
										} else {
												if (now - timeUpdateSequence > 1) {// reset lock message
														checkSendCount = 0;
														sequenceTime = sequenceTimeSendData;
												}
										}
										
								}

								if (now - timeReadyStart > timeForPlay - 3) {
										for (int i=0; i<players.Length; i++) {
												players [i].isStopScore = true;
										}

										SetupFinalResult ();
								}
								if (now - timeReadyStart > timeForPlay) {
										if (!isTimeUp) {
												isTimeUp = true;
												isGameEnd = true;
												FHAIAutoPlay.instance.isActiveAuto = false;
												SetFinalResult ();
										}
								}
						}
			
						int _timeRemain = (int)timeForPlay - (int)(now - timeReadyStart);
						if (_timeRemain < 0) {
								_timeRemain = 0;
						} else {
								int _minute = _timeRemain / 60;
								int _second = _timeRemain % 60;
								string strTime = "";
								strTime = _minute >= 10 ? _minute.ToString () : ("0" + _minute.ToString ());
								strTime += ":" + (_second >= 10 ? _second.ToString () : ("0" + _second.ToString ()));
								lblTimePlay.text = strTime;
						}
				}
		}

		public bool OnErrorFunction (FH.MessageBox.DialogResult result)
		{
				FHNetworkManager.SendMessageToServer (new M_C_LeaveWaitingRoom (FHUsersManager.instance.UserMe.ClientId));
				FHNetworkManager.SendMessageToServer (new M_C_LeaveCurrentRoom (FHUsersManager.instance.UserMe.ClientId));
				SceneManager.instance.LoadScene ("MainMenu");
				return true;
		}

		public void OnGameEndFunction (FH.MessageBox.DialogResult result)
		{
				FHNetworkManager.SendMessageToServer (new M_C_LeaveWaitingRoom (FHUsersManager.instance.UserMe.ClientId));
				FHNetworkManager.SendMessageToServer (new M_C_LeaveCurrentRoom (FHUsersManager.instance.UserMe.ClientId));
				SceneManager.instance.LoadScene ("MainMenu");
		}
	
		public void OnClick ()
		{
				switch (UICamera.selectedObject.name) {
				case "BtnResultClose":
						{
								FHNetworkManager.SendMessageToServer (new M_C_LeaveWaitingRoom (FHUsersManager.instance.UserMe.ClientId));
								FHNetworkManager.SendMessageToServer (new M_C_LeaveCurrentRoom (FHUsersManager.instance.UserMe.ClientId));
								DestroyClient ();
								SceneManager.instance.LoadScene ("MainMenu");
								//client.
						}
						break;
				case "BackBtn":
						if (isGameEnd) {
								FHNetworkManager.SendMessageToServer (new M_C_LeaveWaitingRoom (FHUsersManager.instance.UserMe.ClientId));
								FHNetworkManager.SendMessageToServer (new M_C_LeaveCurrentRoom (FHUsersManager.instance.UserMe.ClientId));
								break;
						}

						GUIMessageDialog.Show (UserClose, FHLocalization.instance.GetString (FHStringConst.ONLINE_USER_DISCONNECT), FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.YesNo);
						break;
				}
		}
	
		public void SetupFinalResult ()
		{
				if (!isSetResult) {
						isSetResult = true;
						StartCoroutine (ShowPanel (0f));
						Debug.LogWarning ("Send_FinalResultEvent cho nay ne");
						//            client.Send_FinalResultEvent(playerInfoMe.sid, fhPlayer01.gold);
						if (roomData.isAutoPlay) {
								//                GoogleAnalyticsBinding.SendEvent("AutoPlay AI", "FinishPlay", "online", 1);
								Debug.Log ("FinalResult AutoPlay");
						} else {
								//                if (!roomData.isDiamondRoom)
								//                {
								//                    GoogleAnalyticsBinding.SendEvent("PVP Gold Play", "FinishPlay", "online", 1);
								//                }
								//                else
								//                {
								//                    GoogleAnalyticsBinding.SendEvent("PVP Diamond Play", "FinishPlay", "online", 1);
								//                }
						}
				}
		}

		/**
	 * 1 : max
	 * 0 : the same
	 * -1: min
	 */
		private int  MaxNumber (int max, int[] numbers)
		{
				if (numbers == null)
						return 1;
				int result = 1;

				for (int i=0; i<numbers.Length; i++) {
						if (max < numbers [i])
								return -1;
						else if (max == numbers [i])
								result = 0;
						else
								result = 1;
				}
				return result;
		}

		public void SetFinalResult ()
		{
				int reward = 0;
				if (!roomData.isDiamondRoom) {
						reward = (int)roomData.price;
				} else {
						reward = (int)roomData.price;
				}
				int meScore = 0;
				int[] friendScores = new int[]{0,0};
				int indexTemp = 0;
				for (int i=0; i<players.Length; i++) {
						if (players [i] == null || players [i].playerOnline == null || players [i].playerOnline.uid == null)
								continue;
						if (players [i].playerOnline.uid == playerInfoMe.uid) {
								meScore = players [i].gold;
						} else {
								friendScores [indexTemp] = players [i].gold;
								indexTemp++;
						}
				}

				UIOnlineFinalScore.MatchOnlineResult matchResult = UIOnlineFinalScore.MatchOnlineResult.WIN;
				int _finalreward = 0;
				switch (MaxNumber (meScore, friendScores)) {
				case 1://Win
						if (!roomData.isDiamondRoom) {
								FHPlayerProfile.instance.gold += reward * 2;//cong lai gold + bonus voi tax
								FHPlayerProfile.instance.ForceSave ();
						} else {
								_finalreward = (int)(reward * roomData.taxPercent);
						}
						matchResult = UIOnlineFinalScore.MatchOnlineResult.WIN;
						break;
				case 0://DEUCE
						if (!roomData.isDiamondRoom) {
								FHPlayerProfile.instance.gold += reward;// tra laigold
								FHPlayerProfile.instance.ForceSave ();
						}
			
			
						matchResult = UIOnlineFinalScore.MatchOnlineResult.DEUCE;
						break;
				case -1://lose
						matchResult = UIOnlineFinalScore.MatchOnlineResult.LOSE;
						_finalreward = -reward;
						break;
				}
				
				onlineFinalResult.Show (matchResult, _finalreward);

				FHNetworkManager.SendMessageToServer (new M_C_UpdateProperties (playerInfoMe.uid, 0, FHPlayerProfile.instance.gold, FHPlayerProfile.instance.diamond));
				FHNetworkManager.SendMessageToServer (new M_C_LeaveWaitingRoom (playerInfoMe.uid));
				FHNetworkManager.SendMessageToServer (new M_C_LeaveCurrentRoom (playerInfoMe.uid));
		
		}
	
		public void OnNetworkError (bool result)
		{
				//        GoogleAnalyticsBinding.SendEvent("OnlineCustom", "Custom", "online", 1);
				if (isGameEnd) {
						return;
				}
				float now = Time.realtimeSinceStartup;
				if (now - timeReadyStart < FHUtils.TIME_IGNORE_DISCONNECT) {// chi disconnect 60s dau
						Debug.LogError ("FH SceneOnlineController: OnNetworkError");
						GUIMessageDialog.Show (OnErrorClose, FHLocalization.instance.GetString (FHStringConst.ONLINE_ERROR_DISCONNECT), FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
						FHAIAutoPlay.instance.isActiveAuto = false;
						isGameEnd = true;
				}
				/* if (isGameEnd)// reconnect
            {
                return;
            }
            float now = Time.realtimeSinceStartup;
            if (now - timeReadyStart> timeForPlay-10)//gan het time bo qua loi
            {
                return;
            }
            if (isReconnectSuccess)//reconnect
            {
                isReconnectSuccess = false;
                TryReconnect();
            }
        */
		}

		public bool OnErrorClose (FH.MessageBox.DialogResult result)
		{
				DestroyClient ();
				SceneManager.instance.LoadScene ("MainMenu");
				FHAIAutoPlay.instance.isActiveAuto = false;
				return true;
		}
	
		public bool UserClose (FH.MessageBox.DialogResult result)
		{
				if (result == FH.MessageBox.DialogResult.Yes) {
						FHNetworkManager.SendMessageToServer (new M_C_LeaveWaitingRoom (FHUsersManager.instance.UserMe.ClientId));
						FHNetworkManager.SendMessageToServer (new M_C_LeaveCurrentRoom (FHUsersManager.instance.UserMe.ClientId));
						isGameEnd = true;
						DestroyClient ();
						SceneManager.instance.LoadSceneWithLoading ("MainMenu");
						FHAIAutoPlay.instance.isActiveAuto = false;
				}
				return true;
		}
	
	#region retry reconnect to server when disconnect
		public void TryReconnect ()
		{
				DestroyClient ();
				StartCoroutine (CoroutinReconnect ());
		}

		public IEnumerator CoroutinReconnect ()
		{
				yield return new WaitForSeconds (0.5f);
				if (!FHNetworkManager.IsConnected) {
						FHNetworkManager.Connect (obj => {
								MC_S_Connected result = obj as MC_S_Connected;
								StartCoroutine (ReJoinRoom (0));
						});
				}
		
//				if (!FHNetworkManager.IsConnected) {
//				            client.InitialNetwork();
//				}
//				StartCoroutine (ReJoinRoom (0));
		
		}

		public IEnumerator ReJoinRoom (int count)
		{
				yield return new WaitForSeconds (0.5f);
				if (!FHNetworkManager.IsConnected) {
						if (count < 4) {
								StartCoroutine (ReJoinRoom (count));
						} else {
								isGameEnd = true;
								Debug.LogWarning ("FH SceneOnlineController: Can not reconnect");
								GUIMessageDialog.Show (OnErrorClose, FHLocalization.instance.GetString (FHStringConst.ONLINE_ERROR_DISCONNECT), FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
								FHAIAutoPlay.instance.isActiveAuto = false;
						}
						count++;
				} else {
						client.checkedUserOnlineReady += CheckReadyToPlay;
						client.logClientLogic += ActionGameLog;
						client.getFinalResult += GetFinalResult;
						client.getReconnectResult += GetReconnectResult;
						Debug.LogError ("ReConnect:" + roomData.roomName);
						//client.ReConnect(roomData.roomName);
				}
		}

		public IEnumerator ReJoinRoomTimeOut ()
		{
				yield return new WaitForSeconds (2.0f);
				if (!isReconnectSuccess || !isGameEnd) {
						isGameEnd = true;
						Debug.LogWarning ("FH SceneOnlineController:  reconnect failed");
						GUIMessageDialog.Show (OnErrorClose, FHLocalization.instance.GetString (FHStringConst.ONLINE_ERROR_DISCONNECT), FHLocalization.instance.GetString (FHStringConst.ONLINE_MESSAGE_CAPTION), FH.MessageBox.MessageBoxButtons.OK);
						FHAIAutoPlay.instance.isActiveAuto = false;
				}
		}

		public void GetReconnectResult (bool result)
		{
				Debug.LogError ("GetReconnectResult");
				isReconnectSuccess = true;
				//todo
		}
	#endregion
	
}
