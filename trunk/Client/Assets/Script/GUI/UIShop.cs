using UnityEngine;
using System.Collections;

public class UIShop : MonoBehaviour
{

		public UILabel numberGold1, numberGold2, numberGold3;
		
		public void Onclick ()
		{
				switch (UICamera.selectedObject.name) {
				case "Button1":
						FHPlayerProfile.instance.gold += int.Parse (numberGold1.text.ToString ());
						OnClose ();
						break;

				case "Button2":
						FHPlayerProfile.instance.gold += int.Parse (numberGold2.text.ToString ());
						OnClose ();
						break;

				case "Button3":
						FHPlayerProfile.instance.gold += int.Parse (numberGold3.text.ToString ());
						OnClose ();
						break;
				case "OutOfCoin":
				case "BtClose":
						OnClose ();
						break;
				}
		}

		protected void OnClose ()
		{
				GuiManager.HidePanel (GuiManager.instance.guiShop);
		}
}
