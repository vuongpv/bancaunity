using UnityEngine;
using System.Collections;

public class MenuScreen : GameBoard
{
	
		public void OnClick ()
		{
			string nameObject = UICamera.selectedObject.name;
			if (nameObject.Equals ("Offline")) {
				Application.LoadLevel ("GamePlay");
			} else if (nameObject.Equals ("Online")) {
				ShowDialog(Constant.pathPrefabs + "Dialog/","Warning","Not support","Close",OnClickDialog);
			}
		}

	public void OnClickDialog()
	{
		CloseDialog (currenDialog);
	}
}
