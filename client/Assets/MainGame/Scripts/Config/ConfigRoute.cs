using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]

public class ConfigRouteRecord
{
	public int id;
	public string prefabName;
    public string nodes;
}

public class ConfigRoute : GConfigDataTable<ConfigRouteRecord>
{
    public Dictionary<int, string> routeIDToPrefabName = new Dictionary<int, string>();

    public ConfigRoute()
        : base("ConfigRoute")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<int>("id");

        foreach (ConfigRouteRecord record in records)
            routeIDToPrefabName[record.id] = record.prefabName;
	}

	public ConfigRouteRecord GetRouteByID(int ID)
	{
		return FindRecordByIndex<int>("id", ID);
	}
}
