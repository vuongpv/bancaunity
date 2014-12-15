using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHQuestPanel : MonoBehaviour
{
		public FHPlayerController controller;

		public GameObject contentContainer;
    
		public UILabel message;

		public UILabel time;

		public UISprite guns;
		public UISprite fishes;
		public UISprite multipliers;

		public GameObject resultContainer;

		public UILabel coin;

		public GameObject hint;

		void OnClick ()
		{
				GameObject obj = UICamera.selectedObject;

				switch (obj.name) {
				case "Collect":
						Debug.LogError ("================ Collect");
						OnBtnCollect ();
						break;

				case "Content":
						Debug.LogError ("================ content");
//						if (FHQuestSystem.instance != null && FHQuestSystem.instance.GetActiveQuest () != null)
//								GuiManager.ShowPanel (GuiManager.instance.guiQuestDetail, FHQuestSystem.instance.GetActiveQuest ());
						break;
				}
		}

		void OnBtnCollect ()
		{
//				FHQuestSystem.instance.ReceiveAward ();
		}

		public void UpdateTime (float seconds)
		{
				if (seconds < 0) {
						time.text = "";
						return;
				}

				int _seconds = (int)seconds;
				time.text = string.Format ("{0:00}:{1:00}", _seconds / 60, _seconds % 60);
		}

		public void UpdateMessage (string content)
		{
				message.text = content;
		}

		public void ShowContent ()
		{
				resultContainer.SetActiveRecursively (false);

				UIHelper.EnableWidget (contentContainer);

				UIHelper.DisableWidget (guns.gameObject);
				UIHelper.DisableWidget (fishes.gameObject);
				UIHelper.DisableWidget (multipliers.transform.parent.gameObject);

				hint.SetActiveRecursively (true);
				hint.GetComponent<FHTextFadeOut> ().StartEffect ();
		}

		public void ShowFish (int fishID)
		{
				UIHelper.EnableWidget (fishes.gameObject);

				ConfigFishRecord record = ConfigManager.configFish.GetFishByID (fishID);
				fishes.spriteName = record.name;
				fishes.MakePixelPerfect ();
		}

		public void ShowGun (int gunID)
		{
				UIHelper.EnableWidget (guns.gameObject);

				ConfigGunRecord record = ConfigManager.configGun.GetGunByID (gunID);
				guns.spriteName = record.name;
				guns.MakePixelPerfect ();
		}
    
		public void ShowMultiplier (int _multiplier)
		{
				UIHelper.EnableWidget (multipliers.transform.parent.gameObject);

				multipliers.spriteName = "multiplier_" + _multiplier.ToString ();
				multipliers.MakePixelPerfect ();
		}

		public void ShowResult (int _coin)
		{
				UIHelper.DisableWidget (contentContainer);

				resultContainer.SetActiveRecursively (true);
				coin.text = _coin.ToString ();
				UIHelper.EnableWidget (resultContainer);
		}

		public void HideResult ()
		{
				resultContainer.SetActiveRecursively (false);
		}

		public void Hide ()
		{
				resultContainer.SetActiveRecursively (false);
				UIHelper.DisableWidget (contentContainer);
		}
}