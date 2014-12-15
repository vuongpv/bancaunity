using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigPayPortRecord
{
	public int id;
	public string name;
    public string logoName;
}

public class ConfigPayPort : GConfigDataTable<ConfigPayPortRecord>
{
    public ConfigPayPort()
        : base("ConfigPayPort")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<int>("id");
	}

    public ConfigPayPortRecord GetItemByID(int ID)
	{
		return FindRecordByIndex<int>("id", ID);
	}
}