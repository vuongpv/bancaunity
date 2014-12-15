using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigFishRecord
{
	public int id;
	public string name;
	public float rate;
	public int price;
    public string powerupRates;
}

public class ConfigFish : GConfigDataTable<ConfigFishRecord>
{
    Dictionary<int, Dictionary<int, float>> powerupRates = new Dictionary<int, Dictionary<int, float>>();

	public ConfigFish()
		: base("ConfigFish")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<int>("id");
		RebuildIndexField<string>("name");

        foreach (ConfigFishRecord record in records)
        {
            powerupRates[record.id] = new Dictionary<int, float>();

            string[] splits = record.powerupRates.Split(';');
            powerupRates[record.id][FHGameConstant.POWERUP_GUN] = float.Parse(splits[0]);
            powerupRates[record.id][FHGameConstant.LIGHTNING_GUN] = float.Parse(splits[1]);
            powerupRates[record.id][FHGameConstant.NUKE_GUN] = float.Parse(splits[2]);
        }
	}

	public ConfigFishRecord GetFishByID(int ID)
	{
		return FindRecordByIndex<int>("id", ID);
	}

	public ConfigFishRecord GetFishByName(string name)
	{
		return FindRecordByIndex<string>("name", name);
	}

    public Dictionary<int, float> GetPowerupRates(int fishID)
    {
        if (powerupRates.ContainsKey(fishID))
            return powerupRates[fishID];
        else
            return null;
    }
}
