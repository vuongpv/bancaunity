using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHFishSeasonManager : SingletonMono<FHFishSeasonManager>
{

		public	Camera mView;
		private int level = 1;
		public TextAsset fishLevel;
		public TextAsset fishInfor;
		public static float golds = 90;
		public static GameObject controlGold;
		private static UILabel numberGold;
		private string[][] infor_fishLevel, infor_Fish;
		public ShopDialog shopDialog;
		public SettingDialog settingDialog;
		private bool isGift = false;
		float countTimeGift = 0;
		Texture2D textureBlank ;
		public Gun gun;
		private bool isTap = true, isCapture = false;
		private int countCapture;
		public GameObject ButtonGift;
		private readonly int timeGift = 20;
		private readonly int GIFTGOLD = 50;
		public bool canStart = false;
		List<ConfigSeasonRecord> listSeason;
		List<int> listNormalSeasonID, listSpecialSeasonID;
		private FHFishSeason currentSeason;
		private int currentSeasonID;
		private string seasonOnlineName = "";

		public string SeasonOnlineName {
				get { return seasonOnlineName; }
				set { seasonOnlineName = value; }
		}
	
		private Dictionary<string, FHFishSeason> seasons = new Dictionary<string, FHFishSeason> ();
	
		void Start ()
		{
				LoadConfigManager ();

				infor_fishLevel = CSVReader.GetData (fishLevel.text);
				infor_Fish = CSVReader.GetData (fishInfor.text);


				//set Onclik
				shopDialog.SetCallBack (OnClickButton);
				settingDialog.SetCallBack (OnClickButton);
		
				//				LoadFish (level);
				controlGold = GameObject.Find ("BgMoney");
				numberGold = (controlGold.transform.FindChild ("NumberMoney")).GetComponent<UILabel> ();
				numberGold.text = FHFishSeasonManager.golds + "";
				textureBlank = new Texture2D (Screen.width, Screen.height);


				foreach (FHFishSeason child in GetComponentsInChildren<FHFishSeason>(false))
						GameObject.Destroy (child.gameObject);
		
				listSeason = ConfigManager.configSeason.GetAllSeason ();

				listNormalSeasonID = new List<int> ();
				listSpecialSeasonID = new List<int> ();
		
				if (listSeason != null) {
						for (int i = 0; i < listSeason.Count; i++) {
								if (!listSeason [i].scenes.Contains (Application.loadedLevelName))
										continue;
				
								if (IsSpecialSeason (listSeason [i].id)) {
										listSpecialSeasonID.Add (i);
								} else
										listNormalSeasonID.Add (i);
				
								string prefabName = listSeason [i].name;
								GameObject obj = GameObject.Instantiate (Resources.Load ("Prefabs/Others/season")) as GameObject;
								obj.transform.parent = this.transform;
								obj.name = prefabName;
				
								FHFishSeason season = obj.GetComponent<FHFishSeason> ();
								seasons [prefabName] = season;
				
								season.Setup (this, listSeason [i]);
						}
				}
		
				StartCoroutine (CheckStartSeason ());
				FHFishSeasonManager.instance.canStart = true;
		}
	
		IEnumerator CheckStartSeason ()
		{
				while (!canStart) {
						yield return new WaitForSeconds (0.1f);
				}
		
				SetSeasonStart ();
		}
	
		public void SetSeasonStart ()
		{
				currentSeasonID = -1;
				currentSeason = null;
		
				currentSeasonID = NextSeason ();
				SetSeason (listSeason [currentSeasonID].name);
		}
	
		void Update ()
		{
				if (currentSeason == null)
						return;
		
		
				if (currentSeason.finished) {
						currentSeasonID = NextSeason ();
			
						SetSeason (listSeason [currentSeasonID].name);
				} else
						currentSeason.OnSeasonUpdate ();

				UpdateGift ();
		}

		private void UpdateGift ()
		{
				if (!isGift) {
			
						countTimeGift += Time.deltaTime;
						if (countTimeGift >= timeGift) {
								isGift = true;
								countTimeGift = 0;
								ButtonGift.gameObject.SetActive (true);
						}
				}
		}
	
		void SetSeason (string name)
		{
				if (currentSeason != null)
						currentSeason.OnSeasonEnd ();
		
				if (!seasons.ContainsKey (name))
						return;
		
				currentSeason = seasons [name];
		
				if (currentSeason != null)
						currentSeason.OnSeasonStart ();
		}
	
		void SetSeason (FHFishSeason season)
		{
				if (currentSeason != null)
						currentSeason.OnSeasonEnd ();
		
				currentSeason = season;
		
				if (currentSeason != null)
						currentSeason.OnSeasonStart ();
		}
	
		public FHFishSeason GetCurrentSeason ()
		{
				return currentSeason;
		}
	
		int NextSeason ()
		{
				List<int> listSeasonID = null;
		
				if (Application.loadedLevelName == FishScenes.Online)
						listSeasonID = listNormalSeasonID;
				else {
						if (currentSeasonID == -1 || IsSpecialSeason (listSeason [currentSeasonID].id) || listSpecialSeasonID.Count <= 0) {
								listSeasonID = listNormalSeasonID;
						} else {
								listSeasonID = listSpecialSeasonID;
				
						}
				}
		
				if (listSeasonID.Count <= 0)
						return 0;
				else
						return listSeasonID [FHSystem.instance.randomGenerator.Next (listSeasonID.Count)];
		}
	
		bool IsSpecialSeason (int seasonID)
		{
				return (seasonID > 100);
		}

		void OnGUI ()
		{
				if (!isCapture)
						return;
				countCapture++;
				if (countCapture >= 5) {
						isCapture = false;
						countCapture = 0;
				}
		
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), textureBlank);
		}
	
		void OnTap (TapGesture gesture)
		{
				if (!isTap) {
						isTap = true;
						return;
				}
				if (!GameBoard.isShowDialog) {
						if (golds < gun.GetID ()) {
								GameBoard.ShowDialog (Constant.pathPrefabs + "Dialog/", "Warning", "You don't have enough money, you need to recharge.", "Close", ClickButton);
								return;
						} else {
								if (!gun.ChangeGun (gesture))
										gun.GunAction (gesture);
				
						}
				}
		}
	
		public void OnSettingClick ()
		{
				isTap = false;
				Debug.Log ("OnSettingClick");
				GameBoard.isShowDialog = true;
				GameBoard.ShowDialog (settingDialog);
//				settingDialog.gameObject.SetActive (true);
		
		}
	
		public void OnCaptureClick ()
		{
				isTap = false;
				isCapture = true;
				Debug.Log ("OnCaptureClick");
				Application.CaptureScreenshot (System.DateTime.Now + ".png");
		
		}
	
		public void CloseShop ()
		{
				isTap = false;
				Debug.LogError ("Close shop");
				GameBoard.CloseDialog (shopDialog, false);
		}
	
		public void CloseSetting ()
		{
				isTap = false;
				Debug.LogError ("Close shop");
				GameBoard.CloseDialog (settingDialog, false);
		}
	
		void ClickButton ()
		{
				isTap = false;
				if (UICamera.selectedObject.name.Equals ("Close")) {
						GameBoard.CloseDialog (GameplayOffline.currenDialog, true);
				}
		}
	
		public void OnShopClick ()
		{
				isTap = false;
				Debug.Log ("On ShopClick");
				
				GameBoard.ShowDialog (shopDialog);
		
		}

		public static void UpdateGold (float gold)
		{
				golds += gold;
				numberGold.text = golds + "";
		}
	
		public void OnClickGift ()
		{
				isTap = false;
				isGift = false;
				ButtonGift.gameObject.SetActive (false);
				Bullet.CreateGold (GIFTGOLD, ButtonGift.gameObject.transform.localPosition);
		}
	
		public void OnClickItem ()
		{
//				isShowDialog = true;
				GameBoard.ShowDialog (Constant.pathPrefabs + "Dialog/", "Warning", "The function is not open. Come back latter.", "Close", ClickButton);
		}
	
		public void OnClickButton ()
		{
				isTap = false;
				string nameObject = UICamera.selectedObject.name;
				if (name == null)
						return;	
		
				float g = 0;
		
				switch (nameObject) {
				case "ButtonBuy1":
						g = 200;
						CloseShop ();
						break;
				case "ButtonBuy2":
						g = 400;
						CloseShop ();
						break;
				case "ButtonBuy3":
						g = 600;
						CloseShop ();
						break;
				case "ButtonMenu":
						CloseSetting ();
						Application.LoadLevel ("MenuScreen");
						break;
				case "ButtonTutorial":
						CloseSetting ();
						break;
				case "ButtonClode":
						CloseSetting ();
						CloseShop ();
						break;
				}
		
				UpdateGold (g);
		}

		void LoadConfigManager ()
		{
				LoadingStep_1 ();
				LoadingStep_2 ();
				LoadingStep_3 ();
				LoadingStep_4 ();
				LoadingStep_5 ();
		}

		void LoadingStep_1 ()
		{
				ConfigManager.instance.Init1 ();
		}
	
		void LoadingStep_2 ()
		{
				ConfigManager.instance.Init2 ();
		}
	
		void LoadingStep_3 ()
		{
				ConfigManager.instance.Init3 ();
		}
	
		void LoadingStep_4 ()
		{
				ConfigManager.instance.Init4 ();
		}
	
		void LoadingStep_5 ()
		{
				// Cache HOTween for avoiding FPS decreasing when call HOTween for the firs time in game play        
				Holoville.HOTween.HOTween.EnableOverwriteManager (false);
				
//				 Calculate bound
//				FHSystem.instance.CalculateBound ();
		
				// Load routes assets
				FHRouteManager.instance.LoadRoutesAssets ();
		}
	


}

