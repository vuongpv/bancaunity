using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;
using System;
using Holoville.HOTween;

public class FHPlayerMultiController : FHPlayerController
{
		public FHMutliPlayerPanel playerPanel;

		public FHCircleMenu circleMenu;

		public UITouchZone touchZone;

		public bool isHostingPlayer;

		public override int gold { get; set; }

		public override int xp { get; set; }

		public override int lightning { get; set; }

		public override int nuke { get; set; }

		public bool isActive;

		public virtual void Start ()
		{
		}

		public void Show ()
		{
				// Init data
				ResetData ();

				// GUI
				//playerHudPanel.UpdateXPProgress(xp - currentLevel.accountXP, currentLevel.xpPerLevel);

				gunHudPanel.Init ();
				ConfigGunRecord configGun = ConfigManager.configGun.GetGunByID (1);
				_SetCurrentGun (configGun);
				gunAnchor.transform.localEulerAngles = Vector3.zero;

				// Scheduler
				scheduler.AddJob ("get_coin", OnCollectCoinInterval, 500, 500, null);

				UpdatePowerupIcons ();

				
		}

		public void Hide ()
		{
				if (currentGun != null) {
						FHGunManager.instance.DespawnGun (currentGun);
						currentGun = null;
				}

				scheduler.RemoveJob ("get_coin");
		}

		public void ResetData ()
		{
				currentLevel = ConfigManager.configLevel.GetLevel (level);

				xp = 0;

				if (isHostingPlayer)
						gold = FHPlayerProfile.instance.gold;
				else
						gold = 0;

				nuke = 0;
        
				lightning = 0;

				powerups.Clear ();
		}

		public override void OnFingerDown (Vector3 hitPoint)
		{
				if (!isActive)
						return;

				if (circleMenu.IsShowed ())
						circleMenu.Toggle ();

				base.OnFingerDown (hitPoint);

				touchZone.StopNotify ();
		}

		public override void OnFingerUp ()
		{
				if (!isActive)
						return;

				base.OnFingerUp ();

				touchZone.CheckNotify ();
		}

		public override void OnFingerMove (Vector3 hitPoint)
		{
				if (!isActive)
						return;

				base.OnFingerMove (hitPoint);
		}

		public override void CalculateHitRate (FHGun gun, List<FHFish> fishes, int betMultiplier)
		{
				int totalPrice = 0;
				int totalGold = 0;

				foreach (var fish in fishes) {
						if (fish != null) {
								fish.hitTimes++;

								int powerupID;

								if (FHGamblingLogic.instance.FishWillBeDie (gun, fish, FHGameConstant.MULTI_PAYOUT_RATE, 1, out powerupID)) {
										if (powerupID != -1) {
												IncreasePowerup (powerupID);
//                        FHGuiCollectibleManager.instance.SpawnWorldPwrUp(powerupID, fish._transform.position, skillPanel);
										}

										FHFish _fish = fish;

										totalPrice += _fish.configFish.price;
										totalGold += _fish.configFish.price * betMultiplier;

										_fish.Die (() =>
										{
												FHGuiCollectibleManager.instance.SpawnWorldGoldMulti (goldHudPanel.coinAnchor, _fish._transform.position, goldHudPanel, _fish.configFish.price * betMultiplier);
												uncollectedGold += _fish.configFish.price * betMultiplier;
										});
								}
						}
				}

				if (totalPrice >= 50 && (GuiManager.instance.guiScore.status == GUIDialogBase.GUIPanelStatus.Hidden || GuiManager.instance.guiScore.status == GUIDialogBase.GUIPanelStatus.Invalid))
						GuiManager.ShowPanel (GuiManager.instance.guiScore, totalGold);
		}

		protected override void CheckLevelUp ()
		{
		}

		public override void ProfileUpdate (string prop)
		{
				FHMultiPlayerManager.instance.ProfileUpdate (prop);

				
		}

		

		public void ToggleCircleMenu (FHCircleMenuType type)
		{
				circleMenu.Toggle (type);
		}
}