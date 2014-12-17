using UnityEngine;
using System.Collections;

public class FHButton : MonoBehaviour
{
//		public GameObject outerArea;

		public void OnClick ()
		{
				switch (UICamera.selectedObject.name) {
				case "ButtonCamera":
						OnCaptureScreen ();
						break;
				case "ButtonShop":
						OnShowShop ();
						
						break;
				case "ButtonBack":
						OnPressBackBtn (true);
						
						break;
				case "ButtonInfor":
						GuiManager.ShowPanel (GuiManager.instance.guiHelp);
						
						break;
				case "ButtonSetting":
						GuiManager.ShowPanel (GuiManager.instance.guiOptionDialogHandler);
						
						break;
				}
		}

		public void OnCaptureScreen ()
		{
				Application.CaptureScreenshot (System.DateTime.Now + ".png");
		}

		public void OnShowShop ()
		{
				GuiManager.ShowPanel (GuiManager.instance.guiShop);
		}

		
		public void OnPressBackBtn (bool prompt)
		{
				if (prompt)
						GUIMessageDialog.Show (OnBackToMMResult, FHLocalization.instance.GetString (FHStringConst.CONFIRM_BACK_TO_MM_QUESTION), "MENU", FH.MessageBox.MessageBoxButtons.YesNo);
				else {
						SceneManager.instance.BackToMM ();
							
				}

		}
		bool OnBackToMMResult (FH.MessageBox.DialogResult result)
		{
				if (result == FH.MessageBox.DialogResult.Yes || result == FH.MessageBox.DialogResult.Ok) {
						if (FHSystem.instance.GetCurrentPlayerMode () == FHPlayerMode.Multi)
								GuiManager.ShowPanel (GuiManager.instance.guiMultiSummary);
						else
								SceneManager.instance.BackToMM ();
				}
		
				return true;
		}

	
		
}
