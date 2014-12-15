using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHFishManager : SingletonMono<FHFishManager>
{
		private SpawnPool fishPool;

		string LOG = "FHFishManager: ";

		private Dictionary<int, GameObject> fishPrefabs = new Dictionary<int, GameObject> ();

		private HashSet<FHFish> activeFishes = new HashSet<FHFish> ();

		void Start ()
		{
//		Debug.Log (LOG+"Start");
				List<ConfigFishRecord> configFishes = ConfigManager.configFish.records;
				foreach (var record in configFishes) {
						if (!fishPrefabs.ContainsKey (record.id)) {
								Debug.Log (LOG + "recordName: " + record.name);
								GameObject fishPrefab = (GameObject)Resources.Load ("Prefabs/Fish/" + record.name, typeof(GameObject));
								fishPrefab.name = record.name;
								fishPrefabs.Add (record.id, fishPrefab);
						}
				}

				fishPool = PoolManager.Pools ["fishes"];
		}

		public void CollectFish (FHFish fish)
		{
//		Debug.Log (LOG+"CollectFish");
				if (fish.gameObject.active) {
						fishPool.Despawn (fish.transform);
						activeFishes.Remove (fish);
				}
		}

		public FHFish SpawnFish (int fishID)
		{
//		Debug.Log (LOG+"SpawnFish");
				Transform obj = fishPool.Spawn (fishPrefabs [fishID].transform);
				FHFish fish = obj.GetComponent<FHFish> ();
				fish.SetManager (this);

				if (fish.viewType != FHFishViewType.None)
						activeFishes.Add (fish);

				return fish;
		}

		public HashSet<FHFish> GetActiveFishes ()
		{
//		Debug.Log (LOG+"GetActiveFishes");
				return activeFishes;
		}
}