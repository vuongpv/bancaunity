using UnityEngine;
using System.Collections;
using SimpleJSON;

public class UIDailyGift : UIBaseDialogHandler
{
	UIPanel[] panels;

	public GameObject content;
	public GameObject error;

	public UIDailyGiftItem[] items;


	public override void OnInit()
	{
		panels = GetComponentsInChildren<UIPanel>();
		//foreach (var panel in panels)
		//	panel.alpha = 0;
	}

	public override void OnBeginShow(object parameter)
	{
		//foreach (var panel in panels)
			//panel.alpha = 0;

		content.SetActiveRecursively(false);
		error.SetActiveRecursively(true);
		error.GetComponentInChildren<UILabel>().text = FHLocalization.instance.GetString(FHStringConst.DAILYGIFT_CONNECTING);

		FHHttpClient.PeakDailyGift((code, json) => {
			Refresh(false, code, json);
		});
	}

	void OnClick()
	{
		switch (UICamera.selectedObject.name)
		{
			case "BtnDay1":
			case "BtnDay2":
			case "BtnDay3":
			case "BtnDay4":
			case "BtnDay5":
			case "BtnDay6":
			case "BtnDay7":
				FHHttpClient.CollectDailyGift((code, json) => {
					// Okie
					if (code == FHResultCode.OK)
					{
						JSONNode gold = json["gifts"]["gold"];
						JSONNode pwup = json["gifts"]["pwup"];

						if (gold != null)
						{
							FHPlayerProfile.instance.gold += gold.AsInt;
							FHPlayerProfile.instance.ForceSave();
						}

						if (FHGoldHudPanel.instance != null)
							FHGoldHudPanel.instance.UpdateGold();
						//if( pwup != null )
							//FHPlayerProfile.instance.gold += gold.AsInt;
					}

					Refresh(true, code, json);
				});
				break;

			case "BtnClose":
				GuiManager.HidePanel(GuiManager.instance.guiDailyGift);
				break;
		}
	}

	void Refresh(bool isCollect, int code, JSONNode json)
	{
		if (code == FHResultCode.NOT_CONNECT || code == FHResultCode.HTTP_ERROR)
		{
			content.SetActiveRecursively(false);
			error.SetActiveRecursively(true);
			if( code == FHResultCode.NOT_CONNECT )
				error.GetComponentInChildren<UILabel>().text = FHLocalization.instance.GetString(FHStringConst.DAILYGIFT_NO_CONNECTION);
			else
				error.GetComponentInChildren<UILabel>().text = FHLocalization.instance.GetString(FHStringConst.DAILYGIFT_CONNECT_FAILED);
			return;
		}
		else
		{
			content.SetActiveRecursively(true);
			error.SetActiveRecursively(false);
		}

		// Get day can be collect
		int nDay = json["day"].AsInt;
		for (int i = 0; i < 7; i++)
		{
			if (!isCollect && code == FHResultCode.OK)
			{
				items[i].SetCurrent(i == nDay);
				items[i].Enable(i <= nDay);
				items[i].collider.enabled = (i == nDay);
			}
			else
			{
				items[i].SetCurrent(false);
				items[i].Enable(i < nDay);
				items[i].collider.enabled = false;
			}
			items[i].SetCollected(i < nDay);
		}
	}
}
