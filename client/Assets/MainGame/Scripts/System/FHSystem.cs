using UnityEngine;
using System;
using System.Collections;
using SimpleJSON;

public enum FHVideoSetting
{
		SD = 0,
		HD = 1,
}


public enum FHPlayerMode
{
		None = -1,
		Single = 0,
		Multi = 1,
		Online = 2
}

public enum FHPayportShop
{
		PlayStore = 0,
		AppleStore = 2,
}


public class FHSystem : SingletonMono<FHSystem>
{
		public System.Random randomGenerator = new System.Random ((int)DateTime.Now.Ticks & 0x0000FFFF);
	
		public string appIdentifier;
	
		public string KeyGameID = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxY+ApErqB96F4QxiOmw86EzDzcLvi7JX35rD007hmUVIByTAkzctu7MUod9LFfdN48ozdOcxlNviE8Sn/aRICWSvjglVY4nhfmnUZp0pD6Mp1bPYdXTRIFqjyrv6nrIQP9ciE9sdjpi3t5yZvl9/uBbaKEPm7yFTVbzoLza4scXykb9GSjIEJE9D7cAW6oNvVZBKJI1ktBe1fxFYr4UnjCPNtywh1yjSkOo+sB15t7mFfk8qCh+zaCy/uzBEoQFELPLIXUUKQuc78GetN/xWJKyrUaeTutU6j8h2jdYa3xXfGKeuop1+9StmETZ1+sd66Sq+zlj9pj17/agf88GDkwIDAQAB";
	
		public string installSource = "";
	
		public FHPayportShop payportShop = FHPayportShop.PlayStore;
	
		public FHVideoSetting videoSetting;
	
		private bool enableCheat = false;
	
		public float boundBottom = 0.0f;
		public float boundLeft = 0.0f;
		public float boundTop = 0.0f;
		public float boundRight = 0.0f;
	
		public bool enableLocalPayment;
		public JSONNode shopConfig;
	
		void Awake ()
		{
				#if UNITY_IPHONE
		payportShop = FHPayportShop.AppleStore;
				#elif UNITY_ANDROID
		payportShop = FHPayportShop.PlayStore;
				#endif
		
				Application.targetFrameRate = 60;
				Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
	
		public void CalculateBound ()
		{
				Vector3 bottomLeft = Camera.main.ViewportToWorldPoint (new Vector3 (0.0f, 0.0f, Camera.main.transform.position.y));
				boundBottom = bottomLeft.z;
				boundLeft = bottomLeft.x;
		
				Vector3 topRight = Camera.main.ViewportToWorldPoint (new Vector3 (1.0f, 1.0f, Camera.main.transform.position.y));
				boundTop = topRight.z;
				boundRight = topRight.x;
		}
	
		public bool IsEnableCheat ()
		{
				return enableCheat;
		}
	
		public FHPlayerMode GetCurrentPlayerMode ()
		{
				switch (Application.loadedLevelName) {
				case FishScenes.Single:
						return FHPlayerMode.Single;
			
//				case FHScenes.Multi:
//						return FHPlayerMode.Multi;
			
				case FishScenes.Online:
						return FHPlayerMode.Online;
				}
		
				return FHPlayerMode.None;
		}
	
		public void StorePlayerData (string targetApp, Action<string> successCallback, Action failCallback)
		{
//				FHHttpClient.StoreProfile (FHPlayerProfile.instance.gold, FHPlayerProfile.instance.level, (code, json) =>
//				{
//						if (code == FHResultCode.OK) {
//								FHPlayerProfile.instance.targetApp = targetApp;
//								FHPlayerProfile.instance.gold = 0;
//								FHPlayerProfile.instance.level = 1;
//								FHPlayerProfile.instance.ForceSave ();
//				
//								if (successCallback != null)
//										successCallback (targetApp);
//						} else {
//								if (failCallback != null)
//										failCallback ();
//						}
//				});
		}
	
		public void RestorePlayerData (Action successCallback, Action failCallback)
		{
//				FHHttpClient.RestoreProfile ((code, json) =>
//				{
//						if (code == FHResultCode.OK) {
//								int restoreGold = json ["restoreGold"].AsInt;
//								int restoreLevel = json ["restoreLevel"].AsInt;
//				
//								FHPlayerProfile.instance.gold += restoreGold;
//								if (FHPlayerProfile.instance.level < restoreLevel) {
//										ConfigLevelRecord level = ConfigManager.configLevel.GetLevel (restoreLevel);
//										FHPlayerProfile.instance.level = restoreLevel;
//										FHPlayerProfile.instance.xp = level.accountXP;
//								}
//				
//								FHPlayerProfile.instance.dataRestored = true;
//				
//								FHPlayerProfile.instance.ForceSave ();
//				
//								if (successCallback != null)
//										successCallback ();
//						} else {
//								if (failCallback != null)
//										failCallback ();
//						}
//				});
		}
	
		public string GetFullAppID ()
		{
				#if UNITY_IPHONE
		return ("ios." + appIdentifier);
				#elif UNITY_ANDROID
		return ("android." + appIdentifier);
				#endif
				return appIdentifier;
		}
}