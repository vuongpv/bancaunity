using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigFishGroupRecord
{
	public int id;
	public string name;
    public int fishID;
}

public class ConfigFishGroup : GConfigDataTable<ConfigFishGroupRecord>
{
    public ConfigFishGroup()
        : base("ConfigFishGroup")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<int>("id");
        RebuildIndexField<int>("fishID");
        RebuildIndexField<string>("name");
    }

    public ConfigFishGroupRecord GetFishGroupByID(int ID)
	{
		return FindRecordByIndex<int>("id", ID);
	}

    public List<ConfigFishGroupRecord> GetFishGroupsByFishID(int fishID)
    {
        return FindRecordsByIndex<int>("fishID", fishID);
    }
}
