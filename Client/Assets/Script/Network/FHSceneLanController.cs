using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHSceneLanController : MonoBehaviour
{

		public FHPlayerOnline fhPlayer01;
		public FHPlayerOnline fhPlayer02;
		public FHPlayerOnline fhPlayer03;
		public GameObject WaitMenu;
		public UILabel serverName;
		public UILabel listPlayerName;
		public UILabel timePlay;
		public Camera mainCamera;
		public float timeBeginAwake = 0;
		private float timeUpdateSequence = 0;
		private float timeReadyStart = 0;
		public bool isAccessGame;
		private FHLobbyGame fhLobbyGame;
		private int countCheckReady = 0;
		private FHLobbyGame.PlayerInfo playerInfoMe;
		private bool isGameEnd = false;


		void Start ()
		{

				Debug.LogError ("=============================LANLANLAN");

				Debug.LogError (gameObject.name + gameObject.GetInstanceID ());
				timeBeginAwake = Time.realtimeSinceStartup;
				timeUpdateSequence = 0;
				fhLobbyGame = FHLanNetwork.instance.fhLobbyGame;
				fhLobbyGame.checkedReadyIngame += CheckReadyToPlay;
				FHLanNetwork.instance.actionGameLog += ActionGameLog;
				FHLanNetwork.instance.actionStartGame += ActionStartGame;
				serverName.text = fhLobbyGame.serverName;
				listPlayerName.text = "";
				StartCoroutine (CheckReady ());
				fhPlayer01.SetUpAdhoc (new FHLobbyGame.PlayerInfo ());
				fhPlayer02.SetUpAdhoc (new FHLobbyGame.PlayerInfo ());
				fhPlayer03.SetUpAdhoc (new FHLobbyGame.PlayerInfo ());
				Debug.LogError (fhLobbyGame.playerList.Count);
				for (int i = 0; i < fhLobbyGame.playerList.Count&&i<2; i++) {
						listPlayerName.text += i.ToString () + "," + fhLobbyGame.playerList [i].username + "\n";

						FHLobbyGame.PlayerInfo playerAdhoc = fhLobbyGame.playerList [i];
						Debug.LogError (playerAdhoc.player.ipAddress + "," + Network.player.ipAddress + "," + playerAdhoc.username);
						if (playerAdhoc.player.ipAddress == Network.player.ipAddress && playerAdhoc.player.port == Network.player.port) {
								Debug.LogError ("Me:" + playerAdhoc.username);
								fhPlayer01.SetUpAdhoc (playerAdhoc);
								playerInfoMe = playerAdhoc;
						} else {
								Debug.LogError ("friend:" + playerAdhoc.username);
								fhPlayer02.SetUpAdhoc (playerAdhoc);
								fhPlayer03.SetUpAdhoc (playerAdhoc);
						}
				}
				if (Network.isClient) {
						Debug.LogError ("Here");
						mainCamera.transform.RotateAroundLocal (new Vector3 (0, 1, 0), Mathf.PI);
						fhPlayer01.SetupClient ();
						fhPlayer02.SetupClient ();
						fhPlayer03.SetupClient ();
				} else {
						//mainCamera.transform.RotateAroundLocal(new Vector3(0, 1, 0), 90);
				}
        
				isGameEnd = false;
		}
		void OnDestroy ()
		{
				fhLobbyGame.checkedReadyIngame -= CheckReadyToPlay;
				FHLanNetwork.instance.actionGameLog -= ActionGameLog;
				FHLanNetwork.instance.actionStartGame -= ActionStartGame;
		}
		public IEnumerator CheckReady ()
		{
				yield return new WaitForSeconds (2);
				fhLobbyGame.ReadyToPlay ();
				/// GUIMessageDialog.Show(Test, "call check port", "aaa", FH.MessageBox.MessageBoxButtons.OK);
       
		}
		public IEnumerator HidePanel (float percent)
		{
				yield return new WaitForSeconds (0.1f);
				gameObject.GetComponent<UIPanel> ().alpha = percent;
				percent -= 0.05f;
				if (percent < 0) {
						WaitMenu.SetActiveRecursively (false);
						StopCoroutine ("HidePanel");
				} else {
						StartCoroutine (HidePanel (percent));
				}
		}
		public void CheckReadyToPlay (bool result)
		{
				countCheckReady++;
				Debug.LogError ("CheckReadyToPlay:" + countCheckReady);
				//GUIMessageDialog.Show(Test, "check port:" + countCheckReady, "aaa", FH.MessageBox.MessageBoxButtons.OK);
       
				if (countCheckReady > 1 || countCheckReady == fhLobbyGame.playerList.Count) {
						isAccessGame = true;
						FHLanNetwork.SetNetworkTime ();
						timeReadyStart = Time.realtimeSinceStartup;
						//Debug.Log("Game Ready Start");
						if (Network.isServer) {
								FHLanNetwork.instance.CallStartGame ();
						}
				}
		}
		public void ActionStartGame (bool result)
		{
				if (gameObject == null)
						return;
				StartCoroutine (HidePanel (1));
				FHLanNetwork.SetNetworkTime ();
				FHFishSeasonManager.instance.SetSeasonStart ();
				timeReadyStart = Time.realtimeSinceStartup;

		}
		public void Test (FH.MessageBox.DialogResult result)
		{

		}
		public void ActionGameLog (FHLanCapturePackage packake)
		{
				//Debug.LogError(packake.player.ipAddress+","+packake.data.Length);
				FHNetEvent_Logic evt = FHLanCapturePackage.Deserialize (packake.data);
				//Debug.LogError(evt.eventName);
				switch (evt.eventName) {
				case (int)FHMessageProperties.ChangeMoney:
						{
								FHNetEvent_ChangeMoney _event = (FHNetEvent_ChangeMoney)evt;
								fhPlayer02.ProcessLanChangeGold (_event.cMoney);
								fhPlayer03.ProcessLanChangeGold (_event.cMoney);
						}
						break;
				case (int)FHMessageProperties.FigureMove:
						{
								FHNetEvent_Figure _event = (FHNetEvent_Figure)evt;
								fhPlayer02.ProcessLanFigureMove (_event.rA);
								fhPlayer03.ProcessLanFigureMove (_event.rA);
						}
						break;
				case (int)FHMessageProperties.UpgradeGun:
						{
								FHNetEvent_ChangeGun _event = (FHNetEvent_ChangeGun)evt;
								fhPlayer02.ProcessLanCurrentGun (_event.gID);
								fhPlayer03.ProcessLanCurrentGun (_event.gID);
						}
						break;
				case (int)FHMessageProperties.Shot:
						{
								FHNetEvent_Shot _event = (FHNetEvent_Shot)evt;
								//Debug.LogError("SHOT lan:" + _event.rotateAngle);
								fhPlayer02.ProcessLanShot (_event.gID, 0, _event.rA);
								fhPlayer03.ProcessLanShot (_event.gID, 0, _event.rA);
						}
						break;
				case (int)FHMessageProperties.StopShot:
						{
								FHNetEvent_StopShot _event = (FHNetEvent_StopShot)evt;
								fhPlayer02.ProcessLanStopShot ();
								fhPlayer03.ProcessLanStopShot ();
						}
						break;
				case (int)FHMessageProperties.HitFish:
						{
								FHNetEvent_HitFish _event = (FHNetEvent_HitFish)evt;
								fhPlayer02.ProcessLanHitFish (0, 0, _event.fDieID);
								fhPlayer03.ProcessLanHitFish (0, 0, _event.fDieID);
						}
						break;
				}
		}

		// Update is called once per frame
		void Update ()
		{
				if (!isAccessGame) {
						float now = Time.realtimeSinceStartup;
						if (now - timeBeginAwake > 40) {
								timeBeginAwake = now;
								isAccessGame = true;
								GUIMessageDialog.Show (OnErrorFunction, "Can not access game now, please try again", "Information", FH.MessageBox.MessageBoxButtons.OK);
						}
				} else {
						float now = Time.realtimeSinceStartup;
						if (!isGameEnd) {
								if (now - timeUpdateSequence > 0.1f) {
										//Debug.LogError("Check routine");
										timeUpdateSequence = now;
										fhPlayer01.CheckSyncDataAdhoc ();
								}
								if (now - timeReadyStart > FHLanNetwork.TIME_PLAYING_ONLAN) {
										//timeReadyStart = now;
										isGameEnd = true;
										string str = "";
										if (fhPlayer01.gold > fhPlayer02.gold) {
												str = "Timeup ! You win " + fhPlayer02.playerAdhoc.username;
										} else if (fhPlayer01.gold < fhPlayer02.gold) {
												str = "Timeup ! You lose " + fhPlayer02.playerAdhoc.username;
										} else {
												str = "Timeup ! Deuce";
										}
                    
										str += "\nYour score: " + fhPlayer01.gold.ToString ();
										str += "\n" + fhPlayer02.playerAdhoc.username + " score: " + fhPlayer02.gold.ToString ();
										fhLobbyGame.LeaveLobbyGame ();
                    
                    
										GUIMessageDialog.Show (OnGameEndFunction, str, "Game Result", FH.MessageBox.MessageBoxButtons.OK);
								}
								if (now - timeReadyStart > FHLanNetwork.TIME_PLAYING_ONLAN - 10) {// time allow disconnect remain playing
										FHLanNetwork.instance.SetGameState (true);
								}
								int _timeRemain = (int)FHLanNetwork.TIME_PLAYING_ONLAN - (int)(now - timeReadyStart);
								if (_timeRemain < 0)
										_timeRemain = 0;
								int _minute = _timeRemain / 60;
								int _second = _timeRemain % 60;
								string strTime = "";
								strTime = _minute > 10 ? _minute.ToString () : ("0" + _minute.ToString ());
								strTime += ":" + (_second > 10 ? _second.ToString () : ("0" + _second.ToString ()));
								timePlay.text = strTime;
						}
				}
		}
		public bool OnErrorFunction (FH.MessageBox.DialogResult result)
		{
				GuiManager.ShowPanel (GuiManager.instance.guiLanMultiPlayer);
				SceneManager.instance.LoadScene ("MainMenu");
				return true;
		}
		public bool OnGameEndFunction (FH.MessageBox.DialogResult result)
		{
				//FHLanNetwork.instance.fhLobbyGame.LeaveLobbyGame();
				SceneManager.instance.LoadScene ("MainMenu");
				return true;
		}
}
