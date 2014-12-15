using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHFishGroupManager : SingletonMono<FHFishGroupManager>
{
	private SpawnPool fishGroupPool;

	private Dictionary<int, GameObject> fishGroupPrefabs = new Dictionary<int, GameObject>();

	void Start()
	{
		List<ConfigFishGroupRecord> configFishGroups = ConfigManager.configFishGroup.records;

        foreach (var record in configFishGroups)
		{
            if (!fishGroupPrefabs.ContainsKey(record.id))
			{
				GameObject fishGroupPrefab = (GameObject)Resources.Load("Prefabs/FishGroup/" + record.name, typeof(GameObject));
                fishGroupPrefab.name = record.name;
                fishGroupPrefabs.Add(record.id, fishGroupPrefab);
			}
		}

        fishGroupPool = PoolManager.Pools["fishgroups"];
	}

	public void CollectFishGroup(Transform fishGroup)
	{
        if (fishGroup.gameObject.active)
            fishGroupPool.Despawn(fishGroup);
	}

    public Transform SpawnFishGroup(int groupID)
    {
        return fishGroupPool.Spawn(fishGroupPrefabs[groupID].transform);
    }
}