using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigGunRecord
{
	// ID	Season	Fish	Spawn Rate
	public int id;
	public string name;
	public string bulletName;
	public string impactEffectName;
	public float bulletSpeed;
	public float bulletAoe;
    public int bulletPrice;
	public float cooldown;
	public int maxHits;
    public float hitRateMultiplier;
}

public class ConfigGun : GConfigDataTable<ConfigGunRecord>
{
	public ConfigGun()
		: base("ConfigGun")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<int>("id");
		RebuildIndexField<string>("name");
	}

	public ConfigGunRecord GetGunByID(int ID)
	{
		return FindRecordByIndex<int>("id", ID);
	}

	public ConfigGunRecord GetGunByName(string name)
	{
		return FindRecordByIndex<string>("name", name);
	}
}
