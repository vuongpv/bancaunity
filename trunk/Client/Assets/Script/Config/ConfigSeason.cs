using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigSeasonRecord
{
    public int id;
    public string name;
    public float totalTime;
    public int randomRoutes;
    public string configRouteDesign;
    public string scenes;
}

public class ConfigSeason : GConfigDataTable<ConfigSeasonRecord>
{
    public ConfigSeason()
        : base("ConfigSeason")
    {
    }

    protected override void OnDataLoaded()
    {
        RebuildIndexField<int>("id");
    }

    public ConfigSeasonRecord GetFishByID(int ID)
    {
        return FindRecordByIndex<int>("id", ID);
    }

    public List<ConfigSeasonRecord> GetAllSeason()
    {
        return records;
    }
}
