using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate void FHPlayerPanelToggleCallback (FHPlayerMultiController player,bool isShowed);

public class FHMultiPlayerManager : SingletonMono<FHMultiPlayerManager>
{
		public FHPlayerMultiController topPlayer;
		public FHPlayerMultiController bottomPlayer;
		public FHPlayerMultiController leftPlayer;
		public FHPlayerMultiController rightPlayer;

		FHPlayerMultiController[] players;

		public GameObject sharingCoinObj = null;

		FHPlayerProfile profile;

		int origLightning, origNuke;
		Dictionary<string, object> origPowerups;

		void Start ()
		{
				Init ();
		}

		void Init ()
		{
				profile = FHPlayerProfile.instance;

				players = new FHPlayerMultiController[4];
				players [0] = bottomPlayer;
				players [1] = leftPlayer;
				players [2] = topPlayer;
				players [3] = rightPlayer;

				// Init gui
				Setup (bottomPlayer, true);
				Setup (topPlayer, true);
				Setup (leftPlayer, false);
				Setup (rightPlayer, false);

				// Init touch zones
				FHTouchZoneManager.instance.Init ();
				FHCircleMenuManager.instance.Init ();

				// Store original items
				origLightning = profile.lightning;
				origNuke = profile.nuke;

				origPowerups = new Dictionary<string, object> ();
				foreach (KeyValuePair<string, object> pair in profile.powerups) {
						origPowerups [pair.Key] = pair.Value;
				}

				// Start season
				if (DateTime.Now.Ticks - profile.lastTimeShowMultiTut > TimeSpan.TicksPerDay) {
						GuiManager.ShowPanel (GuiManager.instance.guiHelp, true);
						profile.lastTimeShowMultiTut = DateTime.Now.Ticks;
				} else
						FHFishSeasonManager.instance.canStart = true;
		}

		public FHPlayerMultiController GetMainPlayer ()
		{
				return bottomPlayer;
		}

		public void Setup (FHPlayerMultiController player, bool isActive)
		{
				player.playerPanel.Setup (isActive);

				if (isActive)
						InitPlayer (player);
		}

		public void Show (FHPlayerMultiController player)
		{
				player.playerPanel.Toggle (player, ToggleCallback);
		}

		public void Hide (FHPlayerMultiController player)
		{
				if (player.isHostingPlayer)
						return;

				if (player.circleMenu.IsShowed ())
						player.circleMenu.Toggle ();

				player.touchZone.Hide ();
				player.playerPanel.Toggle (player, ToggleCallback);

				ResetPlayer (player);
		}

		void ToggleCallback (FHPlayerMultiController player, bool isShowed)
		{
				if (isShowed)
						InitPlayer (player);
		}

		void InitPlayer (FHPlayerMultiController player)
		{
				player.Show ();
				player.touchZone.Show ();

				player.isActive = true;
		}

		void ResetPlayer (FHPlayerMultiController player)
		{
				TakeBack (player, bottomPlayer);

				player.Hide ();

				player.isActive = false;
		}

		public void TakeBack (FHPlayerMultiController srcPlayer, FHPlayerMultiController dstPlayer)
		{
				dstPlayer.gold += srcPlayer.gold;
				if (srcPlayer.gold > 0) {
						FHGuiCollectibleManager.instance.SpawnUICoin (srcPlayer.goldHudPanel.coinAnchor.position, dstPlayer.goldHudPanel, srcPlayer.gold);
						FHGuiCollectibleManager.instance.SpawnUICoinTextMulti (dstPlayer.goldHudPanel.coinAnchor.transform, srcPlayer.gold);
				}
        
				dstPlayer.lightning += srcPlayer.lightning;
        
				dstPlayer.nuke += srcPlayer.nuke;
        
				for (int i = 0; i < FHGameConstant.BET_MULTIPLIERS.Length; i++) {
						int multiplier = FHGameConstant.BET_MULTIPLIERS [i];

						if (!srcPlayer.powerups.ContainsKey (multiplier) || srcPlayer.powerups [multiplier] < ConfigManager.configPowerup.GetGoldLimitForMultiplier (multiplier))
								continue;

						dstPlayer.powerups [multiplier] = srcPlayer.powerups [multiplier];
				}

				srcPlayer.ResetData ();

				// Save profile
				ProfileUpdate_Gold ();
				ProfileUpdate_Lightning ();
				ProfileUpdate_Nuke ();
				ProfileUpdate_Powerup ();

		}

		public void ShareCoin (FHPlayerMultiController srcPlayer, GameObject dstObj, Vector3 dropPos, int coinValue)
		{
				UITouchZone touchZone = (dstObj != null) ? dstObj.GetComponent<UITouchZone> () : null;
				if (touchZone != null) {
						FHPlayerMultiController dstPlayer = touchZone.player;

						if (dstPlayer != null && dstPlayer.isActive && srcPlayer.gold >= coinValue) {
								srcPlayer.SubCoin (coinValue);
								dstPlayer.AddCoin (coinValue);

								FHGuiCollectibleManager.instance.SpawnUICoin (dropPos, dstPlayer.goldHudPanel, coinValue);
								FHGuiCollectibleManager.instance.SpawnUICoinTextMulti (dstPlayer.goldHudPanel.coinAnchor.transform, coinValue);
						}
				}   
		}

		public void ProfileUpdate (string prop)
		{
				switch (prop) {
				case FHProfileProperty.GOLD:
						ProfileUpdate_Gold ();
						break;

				case FHProfileProperty.NUKE:
						ProfileUpdate_Nuke ();
						break;

				case FHProfileProperty.LIGHTNING:
						ProfileUpdate_Lightning ();
						break;

				case FHProfileProperty.POWER_UP:
						ProfileUpdate_Powerup ();
						break;
				}
		}

		void ProfileUpdate_Gold ()
		{
				int total = 0;
				for (int i = 0; i < players.Length; i++)
						if (players [i] != null && players [i].isActive)
								total += players [i].gold;

				profile.gold = total;
		}

		void ProfileUpdate_Nuke ()
		{
				int total = 0;

				for (int i = 0; i < players.Length; i++)
						if (players [i] != null && players [i].isActive)
								total += players [i].nuke;

				profile.nuke = origNuke + total;
		}

		void ProfileUpdate_Lightning ()
		{
				int total = 0;

				for (int i = 0; i < players.Length; i++)
						if (players [i] != null && players [i].isActive)
								total += players [i].lightning;

				profile.lightning = origLightning + total;
		}

		void ProfileUpdate_Powerup ()
		{
				Dictionary<string, object> powerups = null;

				//if (profile.powerups != null)
				//powerups = profile.powerups;
				//else
				//powerups = new Dictionary<string, object>();

				powerups = new Dictionary<string, object> (origPowerups);

				for (int i = 0; i < players.Length; i++)
						if (players [i] != null && players [i].isActive) {
								for (int j = 0; j < FHGameConstant.BET_MULTIPLIERS.Length; j++) {
										int multiplier = FHGameConstant.BET_MULTIPLIERS [j];

										if (!players [i].powerups.ContainsKey (multiplier) || players [i].powerups [multiplier] < ConfigManager.configPowerup.GetGoldLimitForMultiplier (multiplier))
												continue;

										if (!powerups.ContainsKey (multiplier.ToString ()) || (long)powerups [multiplier.ToString ()] < players [i].powerups [multiplier])
												powerups [multiplier.ToString ()] = players [i].powerups [multiplier];
								}
						}

				profile.SetProperty (FHProfileProperty.POWER_UP, powerups);
		}
}