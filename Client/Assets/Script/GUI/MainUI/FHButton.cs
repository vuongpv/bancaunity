using UnityEngine;
using System.Collections;

public class FHButton : MonoBehaviour
{
		
		public void OnClick ()
		{
				switch (UICamera.selectedObject.name) {
				case "ButtonCamera":
						OnCaptureScreen ();
						break;
				case "ButtonShop":
						OnShowShop ();
						break;
				}
		}

		public void OnCaptureScreen ()
		{
			
		}

		public void OnShowShop ()
		{
				GuiManager.ShowPanel (GuiManager.instance.guiShop);
		}
}
