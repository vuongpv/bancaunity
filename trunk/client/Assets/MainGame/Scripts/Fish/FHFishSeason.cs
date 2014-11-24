using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

public class FHFishSeason : MonoBehaviour
{
		private const float SEASON_UPDATE_INTERVAL = 0.2f;
		string LOG = "FHFishSeason  ";
		public FHFishSeasonManager manager;
		public ConfigSeasonRecord config;
		public bool finished;
		Dictionary<int, List<int>> fishRoutesDuringTime = new Dictionary<int, List<int>> ();
		float totalTime, elapsedTime, activeTime;
		private int lastSecondTime;
		GameObject routeRoot;
	
		public void Setup (FHFishSeasonManager _manager, ConfigSeasonRecord _config)
		{
				manager = _manager;
				config = _config;
		}
	
		public virtual void OnSeasonStart ()
		{
				Debug.LogWarning (LOG + "OnSeasonStart: " + config.name);
		
				routeRoot = GetRouteRoot ();
				FHRouteManager.instance.SpawnRoutes (routeRoot.transform, config);
		
//				if (Application.loadedLevelName == FHScenes.Online) {
//						FHRouteManager.instance.GetFishRoutesDuringTime (FHFishSeasonManager.instance.SeasonOnlineName, ref fishRoutesDuringTime); 
//						//						if (FHFishSeasonManager.instance.SeasonOnlineName.Length > 0) {
////								FHRouteManager.instance.GetFishRoutesDuringTime (FHFishSeasonManager.instance.SeasonOnlineName, ref fishRoutesDuringTime); 
////						} else {
////								FHRouteManager.instance.GetFishRoutesDuringTime (config, ref fishRoutesDuringTime);
////						}
//				} else {
				Debug.LogWarning (LOG + "randomRoutes:" + config.randomRoutes);
				FHRouteManager.instance.GetFishRoutesDuringTime (config, ref fishRoutesDuringTime);
//						if (config.randomRoutes == 0)
//								FHRouteManager.instance.GetFishRoutesDuringTime (config, ref fishRoutesDuringTime);
//						else
//								FHRouteManager.instance.GenerateRoutesDuringTime (config, ref fishRoutesDuringTime);
//				}
		
				totalTime = config.totalTime;
				elapsedTime = -1.0f;
				activeTime = -1;
				lastSecondTime = -1;
		
				finished = false;
		
				StartCoroutine (OnSeasonInterval ());
		}
	
		public virtual void OnSeasonUpdate ()
		{
		}
	
		public virtual void OnSeasonEnd ()
		{
				FHRouteManager.instance.DespawnRoutes (routeRoot.transform);
		}
	
		public void SyncTimeSpawn (float _time)
		{
				if (Mathf.Abs (elapsedTime - _time) > 0.4f)
						elapsedTime = _time;
		}
	
		public IEnumerator OnSeasonInterval ()
		{
				//		Debug.LogWarning ("******step 1*************OnSeasonInterval");
				yield return new WaitForSeconds (SEASON_UPDATE_INTERVAL);
		
				elapsedTime += SEASON_UPDATE_INTERVAL;
		
		
				if (Mathf.Abs (elapsedTime) - activeTime > 1) {
						activeTime = (int)elapsedTime;
						int current = (int)elapsedTime;
			
						for (int i = lastSecondTime+1; i <= current; i++) {
								if (fishRoutesDuringTime.ContainsKey (i)) {
										SpawnFisheRoutes (fishRoutesDuringTime [i]);
								}
				
						}
			
						lastSecondTime = current;
				}
		
				if (FHFishManager.instance.GetActiveFishes ().Count == 0 && elapsedTime >= totalTime)
						finished = true;
		
				if (!finished)
						StartCoroutine (OnSeasonInterval ());
				else {
						fishRoutesDuringTime.Clear ();
						yield break;
				}
		}
	
		GameObject GetRouteRoot ()
		{
				GameObject root = new GameObject ("routeRoot");
		
				root.transform.localPosition = Vector3.zero;
				root.transform.localScale = Vector3.one;
				root.transform.localEulerAngles = Vector3.zero;
		
				root.transform.parent = gameObject.transform;
		
				return root;
		}
	
	#region Spawn fish routes
		void SpawnFisheRoutes (List<int> fishRoutes)
		{
				for (int i = 0; i < fishRoutes.Count; i++) {
						FHRoute route = FHRouteManager.instance.GetRoute (fishRoutes [i]);
						if (route == null) {
								continue;
						}
						FHFishData fishData = route.gameObject.GetComponent<FHFishData> ();

						if (fishData.isGroup) {
								SpawnFishGroup (fishData, route);
						} else {
								SpawnFish (fishData, route);
						}
					
				}
		}
		/**
	 * create fish alone
	 */
		void SpawnFish (FHFishData fishData, FHRoute route)
		{
				Fish fish = FHFishManager.instance.SpawnFish (fishData.fishID);
				ConfigFishRecord configFish = ConfigManager.configFish.GetFishByID (fishData.fishID);
		
				if (fish == null || route == null || configFish == null)
						return;
		
				fish.Setup (configFish, this, route);
		}
	

	

		/***
	 * create group fish
	 */
		void SpawnFishGroup (FHFishData fishData, FHRoute route)
		{
				Transform fishGroup = FHFishGroupManager.instance.SpawnFishGroup (fishData.fishGroupID);
				FGCustomInfo info = fishGroup.gameObject.GetComponent<FGCustomInfo> ();
		
				if (info == null)
						return;
		
		
		
				int numberFishes = info.GetNodes ().Length;
		
				if (info.fgType == FGType.GROUP_NORMAL || info.fgType == FGType.GROUP_ACTION) {
						for (int i = 0; i < numberFishes; i++)
								StartCoroutine (SpawnFishInGroup (fishData.fishGroupID, info, fishData.fishID, route, i, info.GetNodes () [i].timeAppear));
				}
		}
	
		IEnumerator SpawnFishInGroup (int groupID, FGCustomInfo info, int fishID, FHRoute route, int index, float timeAppear)
		{
				if (info == null)
						yield break;
		
				yield return new WaitForSeconds (timeAppear);
		
		
				Fish fish = FHFishManager.instance.SpawnFish (fishID);
				ConfigFishRecord configFish = ConfigManager.configFish.GetFishByID (fishID);
		
				if (fish == null || route == null || configFish == null)
						yield break;
		
				fish.SetupInGroup (configFish, this, route, groupID, info, index);
		}
	#endregion
}