using UnityEngine;
using System.Collections;
using System.Linq;

public class GameplayOffline : GameBoard
{
		public	Camera mView;
		private int level = 1;
		public TextAsset fishLevel;
		public TextAsset fishInfor;
		public  float golds = 90;
		public  GameObject controlGold;
		private  UILabel numberGold;
		private string[][] infor_fishLevel, infor_Fish;
		public ShopDialog shopDialog;
		public SettingDialog settingDialog;
		private bool isGift = false;
		float countTimeGift = 0;
		Texture2D textureBlank ;
		public GameObject ButtonGift;
		public Gun gun;
		private bool isTap = true, isCapture = false;
		private int countCapture;
		private readonly int timeGift = 20;

		void Start ()
		{
				infor_fishLevel = CSVReader.GetData (fishLevel.text);
				infor_Fish = CSVReader.GetData (fishInfor.text);

				LoadFish (level);
				controlGold = GameObject.Find ("BgMoney");
				numberGold = (controlGold.transform.FindChild ("NumberMoney")).GetComponent<UILabel> ();
				numberGold.text = golds + "";
				textureBlank = new Texture2D (Screen.width, Screen.height);
		}

		void Update ()
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

		public  void UpdateGold (float gold)
		{
				golds += gold;
				numberGold.text = golds + "";
		}

		private void LoadFish (int level)
		{
				Debug.Log ("Loadfish level:  " + level);
				string[] data = infor_fishLevel [level];
				for (int i=(int)INDEX_FISH_LEVEL.F1; i<=(int)INDEX_FISH_LEVEL.F17; i++) {
						if (data [i] == null)
								continue;

						for (int j=0; j<int.Parse(data[i]); j++) {
								string[] datafish = infor_Fish [i - 3];
								CreateFish (i - 3, datafish);
						}
				}
		}

		private Fish CreateFish (int id, string[] data)
		{

//				string tt = "ID: " + id + "|| ";
//				foreach (string e in data) {
//						tt += e;
//				}
//				Debug.LogError (tt);

				Fish f = null;

				switch (id) {
				case 7:
						f = (Instantiate (Resources.Load (Constant.pathPrefabs + "Fishs/Fish07")) as GameObject).GetComponent<Fish07> ();
						f.SetCamera (mView);
						f.transform.parent = transform.FindChild ("Object").transform.FindChild ("Fishs").transform;
						f.Init (id);
						break;
				case 10:
				case 6:
						f = (Instantiate (Resources.Load (Constant.pathPrefabs + "Fishs/Jellyfish")) as GameObject).GetComponent<Jellyfish> ();
						f.SetCamera (mView);
						f.transform.parent = transform.FindChild ("Object").transform.FindChild ("Fishs").transform;
						f.Init (id);
						break;
				case 15:
				case 16:
				case 17:
						f = (Instantiate (Resources.Load (Constant.pathPrefabs + "Fishs/Mermaid")) as GameObject).GetComponent<Mermaid> ();
						f.SetCamera (mView);
						f.transform.parent = transform.FindChild ("Object").transform.FindChild ("Fishs").transform;
						f.Init (id);
						break;
//		case 1:
//		case 2:
//			f=(Instantiate (Resources.Load (Constant.pathPrefabs+"Fishs/FishForum")) as GameObject).GetComponent<FishForum>();
//			f.SetCamera( mView);
//			f.transform.parent =transform.FindChild("Object").transform.FindChild("Fishs").transform;
//			f.Init(id);

						break;
				default:
						f = (Instantiate (Resources.Load (Constant.pathPrefabs + "Fishs/Fish")) as GameObject).GetComponent<Fish> ();
						f.SetCamera (mView);
						f.transform.parent = transform.FindChild ("Object").transform.FindChild ("Fishs").transform;
						f.Init (id);
						break;
				}

				f.SetPrice (float.Parse (data [5]));


				return f;
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
				if (!isShowDialog) {
						if (golds < gun.GetID ()) {
								GameplayOffline.ShowDialog (Constant.pathPrefabs + "Dialog/", "Warning", "You don't have enough money, you need to recharge.", "Close", ClickButton);
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
				isShowDialog = true;
				settingDialog.Show (OnClickButton);
				settingDialog.gameObject.SetActive (true);
		
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
				shopDialog.gameObject.SetActive (false);
				isShowDialog = false;
		}

		public void CloseSetting ()
		{
				isTap = false;
				Debug.LogError ("Close shop");
				settingDialog.gameObject.SetActive (false);
				isShowDialog = false;
		}
	
		void ClickButton ()
		{
				isTap = false;
				if (UICamera.selectedObject.name.Equals ("Close")) {
						GameplayOffline.CloseDialog (GameplayOffline.currenDialog);
				}
		}

		public void OnShopClick ()
		{
				isTap = false;
				Debug.Log ("On ShopClick");
				isShowDialog = true;
				shopDialog.Show (OnClickButton);
				shopDialog.gameObject.SetActive (true);
			
		}

		public void OnClickGift ()
		{
				isTap = false;
				UpdateGold (50);
				isGift = false;
				ButtonGift.gameObject.SetActive (false);
		}

		public void OnClickItem ()
		{
				ShowDialog (Constant.pathPrefabs + "Dialog/", "Warning", "The function is not open. Come back latter.", "Close", ClickButton);
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
	

}

/**
 *index trong data 
 */
enum INDEX_FISH_LEVEL
{
		LEVEL=0,
		TOTAL_GOLD=1,
		REWARD_GOLD=2,
		TOTAL_FISH=3,
		F1=4,
		F2=5,
		F3=6,
		F4=7,
		F5=8,
		F6=9,
		F7=10,
		F8=11,
		F9=12,
		F10=13,
		F11=14,
		F12=15,
		F13=16,
		F14=17,
		F15=18,
		F16=19,
		F17=20,
};
