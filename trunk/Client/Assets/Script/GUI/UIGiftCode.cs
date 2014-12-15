using UnityEngine;
using System.Collections;
using SimpleJSON;

public class UIGiftCode : UIBaseDialogHandler
{
	public UIInput codeInput;


	public override void OnInit()
	{
		
	}

	public override void OnBeginShow(object parameter)
	{
		
	}

	void OnClick()
	{
		switch (UICamera.selectedObject.name)
		{
			case "Submit":
				OnSubmit();
				break;

			case "BtnClose":
				GuiManager.HidePanel(GuiManager.instance.guiGiftCode);
				break;
		}
	}

	void OnSubmit()
	{
		if (!FHHttpClient.isInternetReachable)
		{
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.ENABLE_NET), FHLocalization.instance.GetString(FHStringConst.GIFCODE_TITLE), FH.MessageBox.MessageBoxButtons.OK);
			return;
		}

		string code = codeInput.text;
		if( string.IsNullOrEmpty(code) )
		{
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.GIFTCODE_USED), FHLocalization.instance.GetString(FHStringConst.GIFCODE_TITLE), FH.MessageBox.MessageBoxButtons.OK);
			return;
		}

		FHHttpClient.DoActionCode(code, (r, json) => 
		{
			if (r == FHResultCode.EXPIRED)
			{
				GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.GIFTCODE_EXPIRED), FHLocalization.instance.GetString(FHStringConst.GIFCODE_TITLE), FH.MessageBox.MessageBoxButtons.OK);
			}
			else if (r == FHResultCode.FAILED)
			{
				GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.GIFTCODE_USED), FHLocalization.instance.GetString(FHStringConst.GIFCODE_TITLE), FH.MessageBox.MessageBoxButtons.OK);
			}
			else if (r == FHResultCode.OK)
			{
				string action = json["action"];
				Debug.LogError("Revceie " + json["action"] + " " + action.ToString());
				if (action.Equals("addgold"))
				{
					int value = json["value"].AsInt;

					FHPlayerProfile.instance.gold += value;
					FHPlayerProfile.instance.ForceSave();

					if (FHGoldHudPanel.instance != null)
						FHGoldHudPanel.instance.UpdateGold();


					GUIMessageDialog.Show((dr) => 
						{
							GuiManager.HidePanel(GuiManager.instance.guiGiftCode);
							return true;
						}, 
						string.Format(FHLocalization.instance.GetString(FHStringConst.GIFTCODE_OK), value), 
						FHLocalization.instance.GetString(FHStringConst.GIFCODE_TITLE), FH.MessageBox.MessageBoxButtons.OK);
				}
			}
		});
	}

	
}
