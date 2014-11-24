using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FHRouteManager : SingletonMono<FHRouteManager>
{
		Dictionary<int, FHRoute> routes = new Dictionary<int, FHRoute> ();
		Dictionary<string, Dictionary<int, List<int>>> fishRoutes = new Dictionary<string, Dictionary<int, List<int>>> ();
		Dictionary<string, FHRoutesAsset> routesAssets = new Dictionary<string, FHRoutesAsset> ();
		System.Random randomGenerator = new System.Random ((int)DateTime.Now.Ticks & 0x0000FFFF);
		int numberNormalSeasons = 0;

		public void LoadRoutesAssets ()
		{
				foreach (ConfigSeasonRecord season in ConfigManager.configSeason.records) {
						routesAssets [season.name] = Resources.Load ("ScriptableObjects/Routes/" + season.name) as FHRoutesAsset;
						if (season.id < 100)
								CalculateFishRoutes (season);

				}
		}

		public void SpawnRoutes (Transform routeRoot, ConfigSeasonRecord season)
		{
				FHRoutesAsset asset = routesAssets [season.name];

				int numberRoutes = asset.routes.Length;
				for (int i = 0; i < numberRoutes; i++)
						SpawnRoute (routeRoot, asset.routes [i]);
		}

		public void DespawnRoutes (Transform routeRoot)
		{
				GameObject.Destroy (routeRoot.gameObject);

				routes.Clear ();
		}

		void SpawnRoute (Transform routeRoot, FHRouteAsset routeAsset)
		{
				if (routeAsset == null)
						return;

				GameObject go = new GameObject ();

				string name = "";
				if (routeAsset.isGroup) {
						ConfigFishGroupRecord group = ConfigManager.configFishGroup.GetFishGroupByID (routeAsset.fishGroupID);
						if (group != null)
								name = group.name;
						else
								routeAsset.isGroup = false;
				}

				if (!routeAsset.isGroup) {
						name = ConfigManager.configFish.GetFishByID (routeAsset.fishID).name;
				}

				go.name = WrapRouteID (routeAsset.id) + "_" + name;
				go.transform.parent = routeRoot;
				go.transform.position = routeAsset.splinePos;


				FHFishData data = go.AddComponent<FHFishData> ();
				data.isGroup = routeAsset.isGroup;
				data.fishID = routeAsset.fishID;
				data.fishGroupID = 0;
				data.fishGroupID = routeAsset.fishGroupID;

				Spline spline = go.AddComponent<Spline> ();
				spline.interpolationMode = Spline.InterpolationMode.BSpline;

				FHRoute route = go.AddComponent<FHRoute> ();

				spline.AddSplineNodes (routeAsset.splineNodes);

				route.CalculateSegments ();

				routes [routeAsset.id] = route;
		}

		public FHRoute GetRoute (int routeID)
		{
				if (routes.ContainsKey (routeID))
						return routes [routeID];
				else
						return null;
		}

		string WrapRouteID (int routeID)
		{
				string result = routeID.ToString ();

				while (result.Length < 3)
						result = "0" + result;

				return result;
		}

    #region Generate route desgin
		static readonly int[] GROUP1 = { 4, 13, 9, 12, 1 };
		static readonly int[] GROUP2 = { 10, 5, 8, 14, 2 };
		static readonly int[] GROUP3 = { 6, 7, 15, 3, 11 };
		static readonly int[] FIREWORK_GROUP1 = {2,6,10,11};//bg3
		static readonly int[] FIREWORK_GROUP2 = {3,9,13,15};//bg6
    
		static readonly int GOLDEN_TURTLE_ID = 16;
		static readonly int GOLDEN_TURTLE_CYCLE = 3;

		void CalculateFishRoutes (ConfigSeasonRecord season)
		{
				FHRoutesAsset asset = routesAssets [season.name];

				fishRoutes [season.name] = new Dictionary<int, List<int>> ();

				int numberRoutes = asset.routes.Length;
				for (int i = 0; i < numberRoutes; i++) {
						if (!fishRoutes [season.name].ContainsKey (asset.routes [i].fishID))
								fishRoutes [season.name] [asset.routes [i].fishID] = new List<int> ();

						fishRoutes [season.name] [asset.routes [i].fishID].Add (asset.routes [i].id);
				}
		}

		public void GenerateRoutesDuringTime (ConfigSeasonRecord season, ref Dictionary<int, List<int>> routesDuringTime)
		{
				int routeInterval = 1, numberRoutes1 = 0, numberRoutes2 = 0, numberRoutes3 = 0;
        
				switch (FHSystem.instance.videoSetting) {
				case FHVideoSetting.SD:
						Debug.LogWarning ("GenerateRoutesDuringTime: FHVideoSetting.SD");
						routeInterval = 10;
						numberRoutes1 = 2;
						numberRoutes2 = 3;
						numberRoutes3 = 2;
						break;

				case FHVideoSetting.HD:
						Debug.LogWarning ("GenerateRoutesDuringTime: FHVideoSetting.HD");
						routeInterval = 12;
						numberRoutes1 = 3;
						numberRoutes2 = 4;
						numberRoutes3 = 2;
						break;
				}

				routesDuringTime.Clear ();
				int numberIntervals = (int)(season.totalTime / routeInterval);
				int numberFishes = ConfigManager.configFish.records.Count;

				Debug.LogWarning ("GenerateRoutesDuringTime:\tnumberIntervals " + season.totalTime);

				int second = -1;
				int fishID, routeID;

				for (int i = 0; i < numberIntervals; i++) {
						for (int j = 0; j < numberRoutes1; j++) {
								second++;

								routesDuringTime [second] = new List<int> ();

								fishID = GROUP1 [randomGenerator.Next (GROUP1.Length)];
								routeID = fishRoutes [season.name] [fishID] [randomGenerator.Next (fishRoutes [season.name] [fishID].Count)];

								routesDuringTime [second].Add (routeID);
						}

						second++;
						routesDuringTime [second] = new List<int> ();

						for (int j = 0; j < numberRoutes2; j++) {
								second++;

								routesDuringTime [second] = new List<int> ();

								fishID = GROUP2 [randomGenerator.Next (GROUP2.Length)];
								routeID = fishRoutes [season.name] [fishID] [randomGenerator.Next (fishRoutes [season.name] [fishID].Count)];

								routesDuringTime [second].Add (routeID);
						}

						second++;
						routesDuringTime [second] = new List<int> ();

						for (int j = 0; j < numberRoutes3; j++) {
								second++;

								routesDuringTime [second] = new List<int> ();

								fishID = GROUP3 [randomGenerator.Next (GROUP3.Length)];
								routeID = fishRoutes [season.name] [fishID] [randomGenerator.Next (fishRoutes [season.name] [fishID].Count)];

								routesDuringTime [second].Add (routeID);
						}

						second++;
						routesDuringTime [second] = new List<int> ();
				}

				// Golden turtle
				numberNormalSeasons++;   
				if (numberNormalSeasons == GOLDEN_TURTLE_CYCLE) {
						numberNormalSeasons = 0;

						second = randomGenerator.Next (numberIntervals * routeInterval);

						fishID = GOLDEN_TURTLE_ID;
						routeID = fishRoutes [season.name] [GOLDEN_TURTLE_ID] [randomGenerator.Next (fishRoutes [season.name] [fishID].Count)];
						routesDuringTime [second].Add (routeID);
				}

#if UNITY_EDITOR
				ExportToFile (routesDuringTime);
#endif
		}

		public void GetFishRoutesDuringTime (ConfigSeasonRecord season, ref Dictionary<int, List<int>> routesDuringTime)
		{
				GetFishRoutesDuringTime (season.configRouteDesign, ref routesDuringTime);
		}

		public void GetFishRoutesDuringTime (string routeDesign, ref Dictionary<int, List<int>> routesDuringTime)
		{
				ConfigManager.instance.LoadRouteDesignConfig (routeDesign);

				routesDuringTime.Clear ();

				List<ConfigRouteDesignRecord> records = ConfigManager.configRouteDesign.records;

				for (int i = 0; i < records.Count; i++)
						routesDuringTime [(int)records [i].time] = records [i].GetFishRoutes ();
		}

		void ExportToFile (Dictionary<int, List<int>> routesDuringTime)
		{
				StreamWriter sw = new StreamWriter ("ConfigRouteDesign.txt");

				sw.WriteLine ("time\trouteID");

				string line;
				foreach (KeyValuePair<int, List<int>> pair in routesDuringTime) {
						line = pair.Key.ToString () + "\t";
            
						for (int j = 0; j < pair.Value.Count - 1; j++)
								line += pair.Value [j].ToString () + ";";

						if (pair.Value.Count > 0)
								line += pair.Value [pair.Value.Count - 1].ToString ();

						sw.WriteLine (line);
				}

				sw.Flush ();
				sw.Close ();
		}
    #endregion
}