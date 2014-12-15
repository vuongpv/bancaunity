using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class FHPlayerOnline : FHPlayerController
{
	
		[HideInInspector]
		public FHLobbyGame.PlayerInfo
				playerAdhoc; // for adhoc
		[HideInInspector]
		public FHUserOnlinePlay
				playerOnline; // for online
	
		[HideInInspector]
		public FHNetEvent_Capture
				FHNetEvent_Capture = new FHNetEvent_Capture ();// for adhoc & online
	
		public UIAnchor anchorGun;
		public bool isMainPlayer = false;
		[HideInInspector]
		public bool
				isAutoPlay = false;
		[HideInInspector]
		public bool
				isStopScore = false;
	
		private Camera uiCam;
		private int goldOnline = 1000;
		private int xpOnline = 0;
		private int levelOnline = 1;
		private int lightingOnline = 0;
		private int nukeOnline = 0;
	
		private float autoAIScalar = 1;
		private bool isDiamondMode = false;
	
		public void SetAutoAIScalar (float scalar)
		{
				autoAIScalar = scalar;
		}
	
		void Awake ()
		{
				_soundable = GetComponent<SoundableObject> ();
		
				uiCam = UICamera.currentCamera;
		}
		void Start ()
		{
				currentLevel = ConfigManager.configLevel.GetLevel (level);
				nextLevel = ConfigManager.configLevel.GetLevel (level + 1);
				gunHudPanel.Init ();
				ConfigGunRecord configGun = ConfigManager.configGun.GetGunByID (1);
				_SetCurrentGun (configGun);
		
				goldHudPanel.UpdateUI (gold, playerName);
				// Scheduler
				scheduler.AddJob ("get_coin", OnCollectCoinInterval, 500, 500, null);
				isStopScore = false;
		}
		void Update ()
		{
				scheduler.Update ();
		}
	
		public void SetupClient ()
		{
				initialAngle = -1;
//				gameObject.transform.RotateAroundLocal (new Vector3 (0, 1, 0), Mathf.PI);
		}
	
	#region Using for Adhoc play
		public void SetUpAdhoc (FHLobbyGame.PlayerInfo info)
		{
				playerAdhoc = info;
				//Debug.LogError(info.username);
				playerName = info.username;
				if (goldHudPanel != null)
						goldHudPanel.UpdateUI (gold, playerName);
				FHNetEvent_Capture.Reset ();
		}
		public void CheckSyncDataAdhoc ()
		{
		
				Dictionary<int, FHNetEvent_Logic> list = FHNetEvent_Capture.listEvent;
				if (list.Count > 0) {
						foreach (KeyValuePair<int, FHNetEvent_Logic> pair in list) {
								FHLanCapturePackage package = new FHLanCapturePackage (playerAdhoc.player, pair.Value);
								FHLanNetwork.instance.SendSycnData (package);
						}
						list.Clear ();
				}
		}
	#endregion
	
	#region Using for Online Mode Play
		public void SetupOnline (FHUserOnlinePlay user, bool _isDiamondMode)
		{
				playerOnline = user;
				playerName = user.userName;
				if (goldHudPanel != null)
						goldHudPanel.UpdateUI (gold, playerName);
				isDiamondMode = _isDiamondMode;
		}
	
		public void ResetSyncDataOnline ()
		{
				FHNetEvent_Capture.listEvent.Clear ();
		}
	#endregion
	
	#region [ Input events ]
		public override void OnFingerDown (Vector3 hitPoint)
		{
				if (isMainPlayer) {
						base.OnFingerDown (hitPoint);
						//FHNetEvent_Capture.AddEventFigure(currentGun.angleRotate);
						//write log event
				}
		}
	
		public float GetAngleFromTarget (Vector3 target)
		{
				return currentGun.GetAngleRotateFromTarget (target);
		}
	
		public float DistanceFromTarget (Vector3 target)
		{
				return Vector3.Distance (currentGun.transform.position, target);
		}
	
		public override void OnFingerUp ()
		{
				if (isMainPlayer) {
						isDragging = false;
						currentGun.ReleaseTrigger ();
			
						//write log event
//						FHNetEvent_Capture.AddEventStopShot();
				}
		}
	
	
		public override void OnFingerMove (Vector3 hitPoint)
		{
				if (isMainPlayer) {
						currentGun.SetTarget (hitPoint);
						//Debug.LogError(currentGun.angleRotate);
						//FHNetEvent_Capture.AddEventFigure(currentGun.angleRotate);
						//write log event
				}
		}
	#endregion
	
	#region override function
		public override int gold {
				get { return goldOnline; }
				set { goldOnline = value; }
		}
	
		public override int xp {
				get { return xpOnline; }
				set { xpOnline = value; }
		}
	
		public override int level {
				get { return levelOnline; }
				set { levelOnline = value; }
		}
	
		public override int lightning {
				get { return lightingOnline; }
		}
	
		public override int nuke {
				get { return nukeOnline; }
		}
	
		protected override void CheckLevelUp ()
		{    
		}
	
		public override void OnShot (int gunID, Vector3 toward)
		{
				//if (isMainPlayer)
				//{
				//    FHNetEvent_Capture.AddEventShot(gunID,0,toward);
				//}
		}
		public override void SubCoin (int subValue)
		{
				if (isStopScore) {
						return;
				}
				if (isMainPlayer) {
						if (subValue < 0)
								return;
			
						gold -= subValue;
			
						if (gold < 0)
								gold = 0;
						goldHudPanel.UpdateUI (gold, playerName);
						FHNetEvent_Capture.AddEventChangeScore (gold);
				}
		}
		public override void AddCoin (int addValue)
		{
				if (isStopScore) {
						return;
				}
				if (isMainPlayer) {
						base.AddCoin (addValue);
						goldHudPanel.UpdateUI (gold, playerName);
						FHNetEvent_Capture.AddEventChangeScore (gold);
				}
		}
		public override void OnFireBullet (int _gunID, int _betMultiplier, int _bulletPrice)
		{
				if (isMainPlayer) {
						base.OnFireBullet (_gunID, _betMultiplier, _bulletPrice);
						FHNetEvent_Capture.AddEventShot (_gunID, 0, currentGun.angleRotate);
				}
		}
		public override void SetCurrentGun (int ID)
		{
				base.SetCurrentGun (ID);
				FHNetEvent_Capture.AddEventChangeGun (ID);
		}
	
		public override void CalculateHitRate (FHGun gun, List<FHFish> fishes, int betMultiplier)
		{
				if (isStopScore) {
						return;
				}
				if (isMainPlayer || isAutoPlay) {
						List<int> fishData = new List<int> ();
						foreach (var fish in fishes) {
								if (fish != null) {
										fish.hitTimes++;
					
										int powerupID;
										bool resultShot = false;
					
										if (!isAutoPlay) {
												resultShot = FHGamblingLogic.instance.FishWillBeDie (gun, fish, FHGameConstant.ONLINE_PAYOUT_RATE, 1, out powerupID);
										} else {
												resultShot = FHGamblingLogic.instance.FishWillBeDie (gun, fish, FHGameConstant.ONLINE_PAYOUT_RATE, autoAIScalar, out powerupID);
										}
										if (resultShot) {
												FHFish _fish = fish;
												_fish.Die (() =>
												{
														if (!isDiamondMode) {
																FHGuiCollectibleManager.instance.SpawnWorldGold (_fish._transform.position, goldHudPanel, _fish.configFish.price * betMultiplier);
														} else {
																FHGuiCollectibleManager.instance.SpawnWorldDiamond (_fish._transform.position, goldHudPanel, _fish.configFish.price * betMultiplier);
														}
														uncollectedGold += _fish.configFish.price * betMultiplier;
												});
												fishData.Add (_fish.fishIdentify);
										}
								}
						}
						if (!isAutoPlay) {// AI auto play
								if (uncollectedGold > 0) {
										FHNetEvent_Capture.AddEventChangeScore (gold + uncollectedGold);
								}
								//if (fishData.Count>0)
								int[] _fishDie = new int[fishData.Count];
								for (int i = 0; i < fishData.Count; i++) {
										_fishDie [i] = fishData [i];
								}
								FHNetEvent_Capture.AddEventHitFish (gun.id, 0, gun.bulletPrefab.transform.position, _fishDie);
						} else {
								ProcessLanChangeGold (gold += uncollectedGold);
						}
				} else {
						foreach (var fish in fishes) {
								if (fish != null) {
										fish.hitTimes++;
								}
						}
				}
		}
	#endregion
	
	
	#region Network
		public void ProcessLanChangeGold (int totalgold)
		{
				//Debug.LogError("ProcessLanChangeGold");
				//uncollectedGold = totalgold - gold;
				gold = totalgold;
				goldHudPanel.UpdateUI (gold, playerName);
		}
		public void ProcessLanFigureMove (float rotateAngle)
		{
				//Debug.LogWarning("Move rotateAngle:" + rotateAngle);
				if (currentGun != null) {
						currentGun.SetTargetAndShotDelay (rotateAngle, false);
				} else {
						Debug.LogWarning ("Current Gun null");
				}
		}
		public void ProcessLanCurrentGun (int ID)
		{
				//Debug.LogError("ProcessLanCurrentGun");
				ConfigGunRecord configGun = ConfigManager.configGun.GetGunByID (ID);
				_SetCurrentGun (configGun);
		
		}
		public void ProcessLanShot (int ID, long guidBullt, float rotateAngle)
		{
				//ProcessLanCurrentGun(ID);
				//Debug.LogError("Hit rotateAngle:" + rotateAngle);
				currentGun.SetTargetAndShotDelay (rotateAngle, true);
		}
	
		public void ProcessLanStopShot ()
		{
				//currentGun.ReleaseTrigger();
		}
	
		public void ProcessLanHitFish (int ID, long guidBullet, int[] fishDie)
		{
				List<FHFish> fishResult = GetFishByID (fishDie);
				for (int i=0; i<fishResult.Count; i++) {
						FHFish _fish = fishResult [i];
						fishResult [i].Die (() =>
						{
								//todo
								//Debug.LogWarning("Die from server send");
								if (!isDiamondMode) {
										FHGuiCollectibleManager.instance.SpawnWorldGold (_fish._transform.position, goldHudPanel, _fish.configFish.price /** betMultiplier*/);
								} else {
										FHGuiCollectibleManager.instance.SpawnWorldDiamond (_fish._transform.position, goldHudPanel, _fish.configFish.price /** betMultiplier*/);
								}
						});
				}
		
		}
	#endregion
	
	#region  Util
		private List<FHFish> GetFishByID (int[] fishIDs)
		{
				HashSet<FHFish> activeFishes = FHFishManager.instance.GetActiveFishes ();
				List<FHFish> fishResult = new List<FHFish> ();
				if (fishIDs != null) {
						for (int i = 0; i < fishIDs.Length; i++) {
								foreach (var fish in activeFishes) {
										if (fish.fishIdentify == fishIDs [i]) {
												fishResult.Add (fish);
										}
								}
						}
				}
				return fishResult;
		}
	
	#endregion
}
