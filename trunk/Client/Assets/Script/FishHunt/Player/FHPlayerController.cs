using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;
using System;
using Holoville.HOTween;


public class FHPlayerController : MonoBehaviour
{
		// GUI
		public FHGunHudPanel gunHudPanel;
		public FHPlayerHudPanel playerHudPanel;
		public FHGoldHudPanel goldHudPanel;
		public FHItems items;


		public GameObject gunAnchor;

		public Collider dragCollider;
		public GameObject pick;
		public bool isDragging;


		protected float initialAngle = 1;

		public float GetInitialSide ()
		{
				return initialAngle;
		}
		// Data
		public SoundableObject _soundable { get; protected set; }

		public string playerName = "";

		public FHGun currentGun;

		private FHPlayerProfile profile;

		public virtual int gold {
				get { return profile.gold; }
				set { profile.gold = value; }
		}

		public virtual int xp {
				get { return profile.xp; }
				set { profile.xp = value; }
		}

		public virtual int level {
				get { return profile.level; }
				set { profile.level = value; }
		}

		public virtual int lightning {
				get { return profile.lightning; }
				set { profile.lightning = value; }
		}

		public virtual int nuke {
				get { return profile.nuke; }
				set { profile.nuke = value; }
		}

		public Dictionary<long, long> powerups = new Dictionary<long, long> ();

		public int betMultiplierIndex = 0;

		protected JobScheduler scheduler = new JobScheduler ();
    
		protected ConfigLevelRecord currentLevel, nextLevel;

		protected int uncollectedGold;

		protected int targetGunID = -1;
		protected int lastNormalGunID;
		protected Vector3 gunAnchorPos;

		Dictionary<FHQuestParam, object> questParams = new Dictionary<FHQuestParam, object> ();

		void Awake ()
		{
				_soundable = GetComponent<SoundableObject> ();

				profile = FHPlayerProfile.instance;
		}

		public virtual void Start ()
		{
				// Load data
				currentLevel = ConfigManager.configLevel.GetLevel (level);
				nextLevel = ConfigManager.configLevel.GetLevel (level + 1);
				LoadPowerups ();

				// GUI
				playerHudPanel.UpdateXPProgress (xp - currentLevel.accountXP, currentLevel.xpPerLevel); 
        
				gunHudPanel.Init ();
				ConfigGunRecord configGun = ConfigManager.configGun.GetGunByID (1);
				_SetCurrentGun (configGun);

			

				// Scheduler
				scheduler.AddJob ("get_coin", OnCollectCoinInterval, 500, 500, null);

				FHFishSeasonManager.instance.canStart = true;
		}

		void Update ()
		{
				scheduler.Update ();

				goldHudPanel.UpdateGold ();

				
		}




	#region [ Input events ]

		public virtual void OnFingerDown (Vector3 hitPoint)
		{
//				if (items.OnFingerDown (hitPoint)) {
//						isDragging = true;
//						return;
//				}

				if (targetGunID != -1)
						return;
				isDragging = true;
				currentGun.SetTarget (hitPoint);

				if (currentGun.configGun.id < 100)
						currentGun.PullTrigger ();

				goldHudPanel.StartOutOfCoinNotify ();
		}



		public virtual void OnFingerUp ()
		{
				isDragging = false;

//				if (items.OnFingerUp ())
//						return;

				if (currentGun.configGun.id > 100) {
						if (targetGunID == -1)
								currentGun.FireBullet ();
				} else
						currentGun.ReleaseTrigger ();
		}


		public virtual void OnFingerMove (Vector3 hitPoint)
		{
//				if (items.OnFingerMove (hitPoint))
//						return;

				if (targetGunID != -1)
						return;

				currentGun.SetTarget (hitPoint);
		}

		
	#endregion

		public virtual void SubCoin (int subValue)
		{
				if (subValue < 0)
						return;

				gold -= subValue;

				if (gold < 0)
						gold = 0;

				ProfileUpdate (FHProfileProperty.GOLD);
		}

		public virtual void AddCoin (int addValue)
		{
				if (addValue < 0)
						return;

				gold += addValue;

				ProfileUpdate (FHProfileProperty.GOLD);
		}

		public virtual void OnShot (int gunID, Vector3 toward)
		{
		}

		public void AddXP (int addValue)
		{
				if (addValue < 0)
						return;

				xp += addValue;

				CheckLevelUp ();
		}

		public virtual void OnFireBullet (int gunID, int _betMultiplier, int _bulletPrice)
		{
				// Decrease coin & increase XP
				SubCoin (_betMultiplier * _bulletPrice);
				AddXP (_betMultiplier * _bulletPrice);
				IncreasePowerup (_betMultiplier, _bulletPrice);
		}

    #region Switch gun
		public virtual void SetCurrentGun (int ID)
		{
				if (targetGunID != -1)
						return;

				if (currentGun != null && currentGun.configGun.id == ID)
						return;

				if (ID > 100 || currentGun.configGun.id > 100) {
						SetCurrentGun_Special (ID);
						return;
				}

				ConfigGunRecord configGun = ConfigManager.configGun.GetGunByID (ID);

				_SetCurrentGun (configGun);
		}

		public void SetCurrentGun (string name)
		{
				ConfigGunRecord configGun = ConfigManager.configGun.GetGunByName (name);

				_SetCurrentGun (configGun);
		}

		protected void _SetCurrentGun (ConfigGunRecord configGun)
		{
				int lastCurrentGunID = -1;
				if (currentGun != null) {
						lastCurrentGunID = currentGun.configGun.id;
						FHGunManager.instance.DespawnGun (currentGun);
				}

				currentGun = FHGunManager.instance.SpawnGun (configGun, this);

				if (lastCurrentGunID != -1) {
						gunHudPanel.SpawnSwitchGunEffect ();
						FHAudioManager.instance.PlaySound (FHAudioManager.SOUND_SWITCH);
				}

				// UI
				if (configGun.id < 100)
						gunHudPanel.EnableBet ();

				gunHudPanel.UpdateUI ();

				// Set min coin nottifier
				goldHudPanel.SetMinCoin (FHGameConstant.BET_MULTIPLIERS [betMultiplierIndex] * currentGun.configGun.bulletPrice);
		}

		void SetCurrentGun_Special (int ID)
		{
				targetGunID = ID;
				gunAnchorPos = gunAnchor.transform.localPosition;

				FHAudioManager.instance.PlaySound (FHAudioManager.SOUND_SWITCH_SPECIAL);

				iTween.RotateTo (gunAnchor, iTween.Hash (
                iT.RotateTo.islocal, true,
                iT.RotateTo.rotation, Vector3.zero,
                iT.RotateTo.time, 0.25f,
                iT.RotateTo.oncomplete, "OnCompleteRotate",
                iT.RotateTo.oncompletetarget, this
				));
		}

		void _SetCurrentGun_Special (int ID)
		{
				if (ID >= 100 && currentGun != null && currentGun.configGun.id < 100)
						lastNormalGunID = currentGun.configGun.id;

				ConfigGunRecord configGun = ConfigManager.configGun.GetGunByID (ID);

				int lastCurrentGunID = -1;
				if (currentGun != null) {
						lastCurrentGunID = currentGun.configGun.id;
						FHGunManager.instance.DespawnGun (currentGun);
				}

				currentGun = FHGunManager.instance.SpawnGun (configGun, this);

				FHAudioManager.instance.PlaySound (FHAudioManager.SOUND_SWITCH_SPECIAL);

				// UI
				if (configGun.id < 100)
						gunHudPanel.EnableBet ();

				gunHudPanel.UpdateUI ();
		}

		protected void OnCompleteRotate ()
		{
				iTween.MoveTo (gunAnchor, iTween.Hash (
                iT.MoveTo.islocal, true,
                iT.MoveTo.z, -3.5f,
                iT.MoveTo.time, 0.5f,
                iT.MoveTo.oncomplete, "OnCompleteHide",
                iT.MoveTo.oncompletetarget, this
				));
		}

		protected void OnCompleteHide ()
		{
				_SetCurrentGun_Special (targetGunID);

				iTween.MoveTo (gunAnchor, iTween.Hash (
                iT.MoveTo.islocal, true,
                iT.MoveTo.position, gunAnchorPos,
                iT.MoveTo.time, 0.5f,
                iT.MoveTo.oncomplete, "OnCompleteShow",
                iT.MoveTo.oncompletetarget, this
				));
		}

		protected void OnCompleteShow ()
		{
				targetGunID = -1;
		}
    #endregion

		protected bool OnCollectCoinInterval (object param)
		{
				if (uncollectedGold == 0)
						return true;

				// Coin sound
				FHAudioManager.instance.PlaySoundCoin ();

				// Gold
				AddCoin (uncollectedGold);
		
				uncollectedGold = 0;

				return true;
		}

		public virtual void CalculateHitRate (FHGun gun, List<FHFish> fishes, int betMultiplier)
		{
				int totalPrice = 0;
				int totalGold = 0;

				foreach (var fish in fishes) {
						if (fish != null) {
								fish.hitTimes++;

								int powerupID;

								if (FHGamblingLogic.instance.FishWillBeDie (gun, fish, currentLevel, out powerupID)) {
										if (powerupID != -1) {
												IncreasePowerup (powerupID);
//												FHGuiCollectibleManager.instance.SpawnWorldPwrUp (powerupID, fish._transform.position, skillPanel);
										}

										FHFish _fish = fish;

//										FHQuestSystem.instance.UpdateProperty (FHQuestProperty.Fish, _fish.configFish.id);

										totalPrice += _fish.configFish.price;
										totalGold += _fish.configFish.price * betMultiplier;

										_fish.Die (() =>
										{
												FHGuiCollectibleManager.instance.SpawnWorldGold (_fish._transform.position, goldHudPanel, _fish.configFish.price * betMultiplier);
												uncollectedGold += _fish.configFish.price * betMultiplier;
										});
								}
						}
				}

				if (totalGold > 0) {
						questParams [FHQuestParam.GunID] = gun.configGun.id;
						questParams [FHQuestParam.BetMultiplier] = betMultiplier;
						questParams [FHQuestParam.NumberCoins] = totalGold;
//						FHQuestSystem.instance.UpdateProperty (FHQuestProperty.Coin, questParams);
				}

				if (totalPrice >= 50 && (GuiManager.instance.guiScore.status == GUIDialogBase.GUIPanelStatus.Hidden || GuiManager.instance.guiScore.status == GUIDialogBase.GUIPanelStatus.Invalid))
						GuiManager.ShowPanel (GuiManager.instance.guiScore, totalGold);
		}
	
		public int TurnBetMultiplier ()
		{
				if (currentGun == null)
						return FHGameConstant.BET_MULTIPLIERS [betMultiplierIndex];

				int orig_betMultiplierIndex = betMultiplierIndex;
				ConfigLevelRecord levelRecord = ConfigManager.configLevel.GetLevel (level);
        
				betMultiplierIndex++;
				if (betMultiplierIndex >= FHGameConstant.BET_MULTIPLIERS.Length ||
						FHGameConstant.BET_MULTIPLIERS [betMultiplierIndex] * currentGun.configGun.bulletPrice > gold ||
						FHGameConstant.BET_MULTIPLIERS [betMultiplierIndex] > levelRecord.maxBet)
						betMultiplierIndex = 0;

				if (betMultiplierIndex == orig_betMultiplierIndex)
						return FHGameConstant.BET_MULTIPLIERS [betMultiplierIndex];

				UpdatePowerupIcons ();

				// Set min coin nottifier
				goldHudPanel.SetMinCoin (FHGameConstant.BET_MULTIPLIERS [betMultiplierIndex] * currentGun.configGun.bulletPrice);

				return FHGameConstant.BET_MULTIPLIERS [betMultiplierIndex];
		}

		public int GetCurrentBetMultiplier ()
		{
				return FHGameConstant.BET_MULTIPLIERS [betMultiplierIndex];
		}

		protected virtual void CheckLevelUp ()
		{
				if (nextLevel != null) {
						if (xp >= nextLevel.accountXP) {
								level++;

								// @STATISTIC: Track level up
								switch (level) {
								case 2:
										FlurryBinding.SendEvent (StatisticDefine.REACH_LEVEL, "level", "2");
										break;

								case 5:
										FlurryBinding.SendEvent (StatisticDefine.REACH_LEVEL, "level", "5");
										break;

								case 10:
										FlurryBinding.SendEvent (StatisticDefine.REACH_LEVEL, "level", "10");
										break;
								}


//								FHQuestSystem.instance.UpdateQuestConfigs ();

								// Level up effect
								playerHudPanel.levelLabel.text = level.ToString ();
								GuiManager.ShowPanel (GuiManager.instance.guiLevelUp, new UILevelUpParams (level, currentLevel.gold));

								FHGuiCollectibleManager.instance.SpawnWorldGold (Vector3.zero, goldHudPanel, currentLevel.gold);
								uncollectedGold += currentLevel.gold;

								// Next level
								currentLevel = nextLevel;
								nextLevel = ConfigManager.configLevel.GetLevel (level + 1);
						}
				}

				//Debug.LogError(playerHudPanel.name);
				//Debug.LogError(currentLevel.accountXP);
				playerHudPanel.UpdateXPProgress (xp - currentLevel.accountXP, currentLevel.xpPerLevel);
		}

		void LoadPowerups ()
		{
				// powerup gun
				powerups.Clear ();

				if (profile.powerups != null) {
						foreach (KeyValuePair<string, object> pair in profile.powerups)
								powerups [int.Parse (pair.Key)] = (long)pair.Value;
				}

				ProfileUpdate (FHProfileProperty.POWER_UP);

				UpdatePowerupIcons ();
		}

		public void UpdatePowerupIcons ()
		{
				// powerup gun
				int multiplier = GetCurrentBetMultiplier ();
				int totalGold = ConfigManager.configPowerup.GetGoldLimitForMultiplier (multiplier);
				if (powerups.ContainsKey (multiplier)) {
						

						gunHudPanel.UpdatePowerupBar (powerups [multiplier], totalGold);
				} else {
						
						gunHudPanel.UpdatePowerupBar (0, totalGold);
				}

				
		}

		void IncreasePowerup (int multiplier, int bulletPrice)
		{
				if (powerups != null) {
						if (!powerups.ContainsKey (multiplier))
								powerups [multiplier] = 0;

						int totalGold = ConfigManager.configPowerup.GetGoldLimitForMultiplier (multiplier);

						powerups [multiplier] += multiplier * bulletPrice;

						if (powerups [multiplier] >= totalGold) {
								powerups [multiplier] = totalGold;
								
								FHAudioManager.instance.PlaySound (FHAudioManager.SOUND_ENABLE_POWERUP);
						}

						gunHudPanel.UpdatePowerupBar (powerups [multiplier], totalGold);

						ProfileUpdate (FHProfileProperty.POWER_UP);
				}
		}

		public void ResetPowerup (int multiplier)
		{
				// reset powerup
				if (powerups == null)
						return;

				powerups [multiplier] = 0;
				ProfileUpdate (FHProfileProperty.POWER_UP);

				
				gunHudPanel.UpdatePowerupBar (powerups [multiplier], 0);
				gunHudPanel.EnableBet ();

				RestoreNormalGun ();
		}

		public void IncreasePowerup (int powerupID)
		{
				switch (powerupID) {
				case FHGameConstant.LIGHTNING_GUN:
						lightning += 1;
						ProfileUpdate (FHProfileProperty.LIGHTNING);
						break;

				case FHGameConstant.NUKE_GUN:
						nuke += 1;
						ProfileUpdate (FHProfileProperty.NUKE);
						break;
				}
		}

		public void DecreasePowerup (int powerupID)
		{
				switch (powerupID) {
				case FHGameConstant.LIGHTNING_GUN:
						lightning -= 1;
						ProfileUpdate (FHProfileProperty.LIGHTNING);
						if (lightning <= 0)
								RestoreNormalGun ();
						break;

				case FHGameConstant.NUKE_GUN:
						nuke -= 1;
						ProfileUpdate (FHProfileProperty.NUKE);
						if (nuke <= 0)
								RestoreNormalGun ();
						break;
				}

				UpdatePowerupIcons ();
		}

		void RestoreNormalGun ()
		{
				SetCurrentGun (lastNormalGunID);
		}

		public void SpawnCollectGold (int _gold)
		{
				FHGuiCollectibleManager.instance.SpawnWorldGold (Vector3.zero, goldHudPanel, _gold);
				uncollectedGold += _gold;
		}

		public virtual void ProfileUpdate (string prop)
		{
				switch (prop) {
				case FHProfileProperty.POWER_UP:
						if (profile != null) {
								Dictionary<string, object> _powerups = null;

								if (profile.powerups != null)
										_powerups = profile.powerups;
								else
										_powerups = new Dictionary<string, object> (); 
                    
								foreach (KeyValuePair<long, long> pair in powerups)
										_powerups [pair.Key.ToString ()] = pair.Value;

								profile.SetProperty (FHProfileProperty.POWER_UP, _powerups);
						}
						break;
				}
		}
}