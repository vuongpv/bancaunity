using UnityEngine;
using System.Collections;

public class MenuScreen : GameBoard
{
	
		public void OnClick ()
		{
				string nameObject = UICamera.selectedObject.name;
				if (nameObject.Equals ("Offline")) {
						Application.LoadLevel (FishScenes.Single);
				} else if (nameObject.Equals ("Online")) {
						ShowDialog (Constant.pathPrefabs + "Dialog/", "Warning", "The function is not open. Come back latter.", "Close", OnClickDialog);
				}
		}

		public void OnClickDialog ()
		{
				CloseDialog (currenDialog, true);
		}
}
