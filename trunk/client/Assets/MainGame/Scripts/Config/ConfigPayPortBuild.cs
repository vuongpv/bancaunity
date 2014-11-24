using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigPayPortBuildRecord
{
	public int id;
	public string name;
    public string payPortsGold;
    public string payPortsDiamond;
}

public class ConfigPayPortBuild : GConfigDataTable<ConfigPayPortBuildRecord>
{
    public ConfigPayPortBuild()
        : base("ConfigPayPortBuild")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<int>("id");
	}

    public ConfigPayPortBuildRecord GetItemByID(int ID)
	{
		return FindRecordByIndex<int>("id", ID);
	}
}