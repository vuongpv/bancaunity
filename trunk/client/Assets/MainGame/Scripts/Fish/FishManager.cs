using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishManager : SingletonMono<FishManager>
{
		private SpawnPool fishPool;
	
	
		private Dictionary<int, GameObject> fishPrefabs = new Dictionary<int, GameObject> ();
	
		private HashSet<Fish> activeFishes = new HashSet<Fish> ();
	
		void Start ()
		{
				List<ConfigFishRecord> configFishes = ConfigManager.configFish.records;
				foreach (var record in configFishes) {
						if (!fishPrefabs.ContainsKey (record.id)) {
								GameObject fishPrefab = (GameObject)Resources.Load ("Prefabs/Fishs/" + record.name, typeof(GameObject));
								fishPrefab.name = record.name;
								fishPrefabs.Add (record.id, fishPrefab);
						}
				}
				fishPool = PoolManager.Pools ["fishes"];
		}
	
		public void CollectFish (Fish fish)
		{
				if (fish.gameObject.active) {
						fishPool.Despawn (fish.transform);
						activeFishes.Remove (fish);
				}
		}
	
		public Fish SpawnFish (int fishID)
		{
				Transform obj = fishPool.Spawn (fishPrefabs [fishID].transform);
				Fish fish = obj.GetComponent<Fish> ();
				fish.SetManager (this);
		
//				if (fish.viewType != FHFishViewType.None)
				activeFishes.Add (fish);
		
				return fish;
		}
	
		public HashSet<Fish> GetActiveFishes ()
		{
				return activeFishes;
		}
}