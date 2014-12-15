using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FHUtils
{
		public static float WAIT_PLAY_AUTO = 1;

		public static float WAIT_TIMEOUT_DIAMOND = 30;
		public static float WAIT_TIME_OUT = 10;
		public static float MAX_COUNT_RECONNECT = 1;
		public static float TIME_IGNORE_DISCONNECT = 60.0f;
		public static System.Random rand = new System.Random ();
		public static string FH_PATH_ROUTE_ONLINE = @"OnlineRoutes/ConfigRouteDesign";
 
//		public static int[] BET_VALUE_NORMAL = new int[] { 0,100, 200, 300, 500, 1000, 5000 };
//		public static int[] BET_VALUE_DIAMOND = new int[] { 0,100, 200,500};
		public static string NAME_DIAMOND_ICON = "kimcuong_icon_highlight";
		public static string NAME_LANGUAGESAVE = "LANGAUGE_SAVE_NAME";
		public static string GetPlayerName ()
		{
				string _name = PlayerPrefs.GetString ("FisherName", "");
				if (_name.Length < 1) {
						string _nameDefault = "Fisher " + (rand.Next (100000)).ToString ();
						FHUtils.SetPlayerName (_nameDefault);
						_name = PlayerPrefs.GetString ("FisherName", "");
				}
				return _name;

		}
    
		public static string GetRawPlayerName ()
		{
				string _name = PlayerPrefs.GetString ("FisherName", "");
				return _name;

		}
    
		public static void SetPlayerName (string _name)
		{
				_name = _name.Replace ("{", "").Replace ("\"", "").Replace (",", "").Replace (":", "").Replace ("$", "");
				PlayerPrefs.SetString ("FisherName", _name);
		}

		public static T ToObject<T> (string jsonText)
		{
				try {
						return SimpleJson.SimpleJson.DeserializeObject<T> (jsonText);
				} catch (Exception ex) {
						throw;
				}
		}

		public static void OpenAppStore (string appIdentifier)
		{
#if UNITY_ANDROID
				try {
						if (appIdentifier.Contains ("market://"))
								Application.OpenURL (appIdentifier);
						else
								Application.OpenURL ("market://details?id=" + appIdentifier);
				} catch (System.Exception ex) {
						if (appIdentifier.Contains ("http://"))
								Application.OpenURL (appIdentifier);
						else
								Application.OpenURL ("http://play.google.com/store/apps/details?id=" + appIdentifier);
				}
#elif UNITY_IPHONE
		try
		{
			Application.OpenURL("itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=690361840");
		}
		catch (System.Exception ex)
		{
			Application.OpenURL("https://itunes.apple.com/vn/app/fish-hunt-online/id690361840");
		}
#endif
		}

		public static void QuitGame ()
		{
				try {
						Application.Quit ();
				} catch (System.Exception ex) {

				}
		}
}

/// <summary>
/// Definition all room type user joining
/// </summary>
public enum SocketJoinRoomType
{
		// note : the number sync with server
		none         = 0,
		roomTypeBet = 1,// chicken class type 1: bet 50....
		roomTypeDiamond = 2,
//		roomTypeBet3 = 3,
//		roomTypeBet4 = 4,
//		roomTypeBet5 = 5,
//		roomTypeBet6 = 6,
//
//		roomTypeDiamond1    = 7,
//		roomTypeDiamond2    = 8,
//		roomTypeDiamond3    = 9,
//		roomTypeDiamondStart = 7,// START ROOM DIAMOND
//		roomTypeDiamondEnd  = 9// END ROOM DIAMOND
}