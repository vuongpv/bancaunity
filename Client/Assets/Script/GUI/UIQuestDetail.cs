using UnityEngine;
using System.Collections;
using SimpleJSON;

public class UIQuestDetail : UIBaseDialogHandler
{
		public UILabel title;
		public UILabel status;
		public UIQuestRemainTime remainTime;
		public UILabel award;
		public UILabel description;
		public UISprite icon;

		FHQuest quest;

		public override void OnInit ()
		{
		}

		public override void OnBeginShow (object parameter)
		{
				quest = parameter as FHQuest;
				ShowDetail ();
		}

		void ShowDetail ()
		{
				if (quest == null)
						return;

				Debug.LogError ("=======================Show ShowDetail Quest");

				ConfigQuestRecord questConfig = ConfigManager.configQuest.GetQuestByID (quest.configID);
				if (questConfig == null)
						return;

				title.text = FHLocalization.instance.GetString (questConfig.titleID);

				status.text = quest.GetStatus ();

				remainTime.Setup (quest);

				award.text = questConfig.award.ToString ();

				description.text = FHLocalization.instance.GetString (questConfig.descriptionID);
				SetIcon ();
		}

		void SetIcon ()
		{
				string spriteName = "";

				switch (quest.type) {
				case FHQuestType.HuntFish:
						ConfigFishRecord fish = ConfigManager.configFish.GetFishByID ((quest as FHQuest_HuntFish).fishID);
						spriteName = fish.name;
						break;

				case FHQuestType.UseGunCollectCoin:
						ConfigGunRecord gun = ConfigManager.configGun.GetGunByID ((quest as FHQuest_UseGunCollectCoin).gunID);
						spriteName = gun.name;
						break;

				case FHQuestType.CollectCoinWithBet:
						spriteName = "multiplier_" + (quest as FHQuest_CollectCoinWithBet).betMultiplier.ToString ();
						break;
				}

				icon.spriteName = spriteName;
				icon.MakePixelPerfect ();
		}

		void OnClick ()
		{
				switch (UICamera.selectedObject.name) {
				case "BtnClose":
						GuiManager.HidePanel (GuiManager.instance.guiQuestDetail);
						break;
				}
		}
}
