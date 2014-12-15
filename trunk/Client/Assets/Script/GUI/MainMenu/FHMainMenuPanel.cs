using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHMainMenuPanel : MonoBehaviour
{

		public GameObject dailyGiftBtn;

		void OnClick ()
		{
				Debug.Log (UICamera.selectedObject.name);
				switch (UICamera.selectedObject.name) {
				case "SingleModeBtn":
						SceneManager.instance.LoadSceneWithLoading (FHScenes.Single);
						break;

				case "MultiModeBtn":
						SceneManager.instance.LoadSceneWithLoading (FHScenes.Multi);
						break;

				case "OnlineModebtn":
						OnOnlinePlayClick ();
						break;
				case "DiamondModebtn":
						OnOnlineDiamondPlayClick ();
						break;
            
				case "DailyGiftBtn":
						{
								GuiManager.ShowPanel (GuiManager.instance.guiDailyGift);
								//FacebookBinding.PostNewFeed("Name ne", "caption ne", "Decs ne", "http://extremelifechanger.com/web_images/avatar-sam09-8-251.jpg", "http://google.com");

								// Make request params
								//Dictionary<string, object> @params = new Dictionary<string, object>();
								//@params["deviceID"] = SystemHelper.deviceUniqueID;
								//@params["product"] = "test product";
								//@params["money"] = 0;
								//FHHttpClient.RequestTransaction(@params, "card", (code, json) =>
								//{
								//    string payID = (string)json["payID"];
								//    SO6PaymentBinding.SendCard(SystemHelper.deviceUniqueID, "FH", "VNP", "12345678912", "123456789", payID);
								//});
					
						}
						break;
				}
		}

		public void OnMultiPlayClick ()
		{
				GuiManager.ShowPanel (GuiManager.instance.guiOnlinePlay);
		}

		public void OnOnlinePlayClick ()
		{
				SceneManager.instance.LoadSceneWithLoading (FHScenes.Tables);
//        GuiManager.ShowPanel(GuiManager.instance.guiOnlinePlay,UIOnlinePlay.UIOnlineTab.TabBetNormal);

		}

		public void OnOnlineDiamondPlayClick ()
		{
//				GuiManager.ShowPanel (GuiManager.instance.guiOnlinePlay, UIOnlinePlay.UIOnlineTab.TabDiamond);
		}
}