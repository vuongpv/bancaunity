using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

public class ConfigManager : Singleton<ConfigManager>
{
		public static ConfigGun configGun;

		public static ConfigFish configFish;
		public static ConfigFishGroup configFishGroup;

		public static ConfigLevel configLevel;

		public static ConfigRouteDesign configRouteDesign;
		public static ConfigSeason configSeason;

		public static ConfigPayPort configPayPort;
		public static ConfigGoldPack configGoldPack;
		public static ConfigPayPortBuild configPayPortBuild;

		public static ConfigQuest configQuest;

		public static ConfigPowerup configPowerup;

		public static ConfigCardType configCardType;
		public static ConfigCard configCard;

		public static ConfigFont configFont;

		private void LoadDataConfig<TConfigTable> (ref TConfigTable configTable, params string[] dataPaths) where TConfigTable : IConfigDataTable, new()
		{
				try {
						if (configTable == null) {
								configTable = new TConfigTable ();

								configTable.BeginLoadAppend ();
								foreach (var path in dataPaths) {
										configTable.LoadFromAssetPath (path);
								}
								configTable.EndLoadAppend ();
//								Logger.current.DebugFormat ("Config [{0}] loaded", configTable.GetName ());
						}
				} catch (System.Exception ex) {
						Debug.LogError ("=================LoadDataConfig: bugggggggggg: " + ex.ToString ());
//						Logger.current.ErrorFormat ("Load Config [{0}] Error = {1}", configTable.GetName (), ex.ToString ());
				}
		}

		public void Init ()
		{
				Init1 ();
				Init2 ();
				Init3 ();
				Init4 ();
		}

		public void Init1 ()
		{
				LoadDataConfig<ConfigGun> (ref configGun, "Config/ConfigGun");
		
				LoadDataConfig<ConfigFish> (ref configFish, "Config/ConfigFish");
				LoadDataConfig<ConfigFishGroup> (ref configFishGroup, "Config/ConfigFishGroup");
		}

		public void Init2 ()
		{
				LoadDataConfig<ConfigLevel> (ref configLevel, "Config/ConfigLevel");
		}

		public void Init3 ()
		{
				LoadDataConfig<ConfigSeason> (ref configSeason, "Config/ConfigSeason");
	

//				LoadDataConfig<ConfigPayPort> (ref configPayPort, "Config/ConfigPayPort");
//				LoadDataConfig<ConfigGoldPack> (ref configGoldPack, "Config/ConfigGoldPack");
//				LoadDataConfig<ConfigPayPortBuild> (ref configPayPortBuild, "Config/ConfigPayPortBuild");

				LoadDataConfig<ConfigPowerup> (ref configPowerup, "Config/ConfigPowerup");
		}

		public void Init4 ()
		{
//				LoadDataConfig<ConfigQuest> (ref configQuest, "Config/ConfigQuest");
//        
//				LoadDataConfig<ConfigCardType> (ref configCardType, "Config/ConfigCardType");
//				LoadDataConfig<ConfigCard> (ref configCard, "Config/ConfigCard");
//
//				LoadDataConfig<ConfigFont> (ref configFont, "Config/ConfigFont");
		}

		public void LoadFishConfigs (bool reload)
		{
				if (reload) {
						configFish = null;
						configFishGroup = null;
				}

				LoadDataConfig<ConfigFish> (ref configFish, "Config/ConfigFish");
				LoadDataConfig<ConfigFishGroup> (ref configFishGroup, "Config/ConfigFishGroup");
		}

		public void LoadRouteDesignConfig (string name)
		{
				configRouteDesign = null;
				LoadDataConfig<ConfigRouteDesign> (ref configRouteDesign, "Config/" + name);
		}

		public void UnLoad ()
		{
		}
}