using UnityEngine;
using System.Collections;
using System.Linq;

public class Gameplay : GameBoard
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

		public Gun gun;
		private bool isTap = true;

		void Start ()
		{
				infor_fishLevel = CSVReader.GetData (fishLevel.text);
				infor_Fish = CSVReader.GetData (fishInfor.text);

				LoadFish (level);
				controlGold = GameObject.Find ("BgMoney");
				numberGold = (controlGold.transform.FindChild ("NumberMoney")).GetComponent<UILabel> ();
				numberGold.text = Gameplay.golds + "";
		}

		public static void UpdateGold (float gold)
		{
				golds += gold;
				numberGold.text = Gameplay.golds + "";
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

		void OnTap (TapGesture gesture)
		{
				if (!isTap) {
						isTap = true;
						return;
				}
				if (!isShowDialog) {
						if (golds < gun.GetID ()) {
								Gameplay.ShowDialog (Constant.pathPrefabs + "Dialog/", "Warning", "No Money", "Close", ClickButton);
								return;
						} else {
								gun.GunAction (gesture);

						}
				} else {
//						if (shopDialog != null) {
//								if (shopDialog.gameObject.activeSelf) {
//										CloseShop ();
//								}
//						}else if(sett)
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
						Gameplay.CloseDialog (Gameplay.currenDialog);
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

		public void OnClickButton ()
		{
				isTap = false;
				string nameObject = UICamera.selectedObject.name;
				if (name == null)
						return;	

				float g = 0;
		
				if (nameObject.Equals ("ButtonBuy1")) {
						g = 200;
						CloseShop ();
				} else if (nameObject.Equals ("ButtonBuy2")) {
						g = 500;
						CloseShop ();
				} else if (nameObject.Equals ("ButtonBuy3")) {
						g = 600;
						CloseShop ();
				} else if (nameObject.Equals ("ButtonMenu")) {
						CloseSetting ();

				} else if (nameObject.Equals ("ButtonTutorial")) {
						CloseSetting ();
				} else if (nameObject.Equals ("ButtonClode")) {
						CloseSetting ();
						CloseShop ();
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
