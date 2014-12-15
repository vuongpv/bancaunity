using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHGunHudPanel : MonoBehaviour
{
		public float progressBarMax = 0.2f;

		public GameObject bet;
		public UILabel betLabel;

		public UISprite progressBar;

		public FHPlayerController controller;

		public GameObject touchArea;

		private ConfigGunRecord[] listGuns;

		private Dictionary<string, FHGun> guns = new Dictionary<string, FHGun> ();

		SpawnPool gunEffectPool;
		GameObject effSwitchGunPrefab;

		public void Init ()
		{
				listGuns = ConfigManager.configGun.records.ToArray ();

				gunEffectPool = PoolManager.Pools ["guneffects"];
				effSwitchGunPrefab = (GameObject)Resources.Load ("Prefabs/Effect/id_eff_switch_gun", typeof(GameObject));
		}

		public void OnClick ()
		{
				switch (UICamera.selectedObject.name) {
				case "DecBtn":
						SelectPrevGun ();
						UpdateUI ();
						break;

				case "IncBtn":
						SelectNextGun ();
						UpdateUI ();
						break;

				case "BetBtn":
						TurnBetLabel ();
						break;

				case "SkillBtn":
						((FHPlayerMultiController)controller).ToggleCircleMenu (FHCircleMenuType.Skills);
						break;
				}
		}

		public void UpdateUI ()
		{
				try {

						SetBetLabel (controller.GetCurrentBetMultiplier ());
				} catch {

				}
		}

		void SelectNextGun ()
		{
				for (int i = 0; i < listGuns.Length; i++) {
						if (listGuns [i].id == controller.currentGun.id) {
								do {
										i++;
										if (i >= listGuns.Length)
												i = 0;
								} while (listGuns[i].id > 100);

								controller.SetCurrentGun (listGuns [i].id);
								break;
						}
				}
		}

		void SelectPrevGun ()
		{
				for (int i = 0; i < listGuns.Length; i++) {
						if (listGuns [i].id == controller.currentGun.id) {
								do {
										i--;
										if (i < 0)
												i = listGuns.Length - 1;
								} while (listGuns[i].id > 100);

								controller.SetCurrentGun (listGuns [i].id);
								break;
						}
				}
		}

		void TurnBetLabel ()
		{
				SetBetLabel (controller.TurnBetMultiplier ());
		}

		void SetBetLabel (int betMultiplier)
		{
				if (betLabel != null) {
						betLabel.text = "X " + betMultiplier.ToString ();
				}
		}

		public void UpdatePowerupBar (long current, int total)
		{
				if (current == 0)
						progressBar.fillAmount = 0.0f;
				else
						progressBar.fillAmount = (float)current / (float)total * progressBarMax;
		}

		public void SpawnSwitchGunEffect ()
		{
				Transform eff = gunEffectPool.Spawn (effSwitchGunPrefab.transform);
				eff.gameObject.GetComponent<FHSwitchGun> ().Setup (this);
		}

		public void DespawnSwitchGunEffect (Transform eff)
		{
				gunEffectPool.Despawn (eff);
		}

		public void DisableBet ()
		{
				UISprite sprite = bet.GetComponentInChildren<UISprite> ();
				if (FHSystem.instance.GetCurrentPlayerMode () == FHPlayerMode.Single)
						sprite.spriteName = "gun_multiplier_dis";
				else
						sprite.spriteName = "multi_multiplier_btn_dis";
				sprite.MakePixelPerfect ();

				bet.collider.enabled = false;
		}

		public void EnableBet ()
		{
//				UISprite sprite = bet.GetComponentInChildren<UISprite> ();
//				if (sprite == null) {
//						//Debug.Log("Enable bet null");
//						return;
//				}
//				if (FHSystem.instance.GetCurrentPlayerMode () == FHPlayerMode.Single)
//						sprite.spriteName = "gun_multiplier";
//				else
//						sprite.spriteName = "multi_multiplier_btn";
//				sprite.MakePixelPerfect ();
//
//				bet.collider.enabled = true;
		}
}