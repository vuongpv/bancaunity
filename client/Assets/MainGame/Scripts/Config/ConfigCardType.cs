using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GFramework;
using FileHelpers;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigCardTypeRecord
{
    public int id;
    public string name;
	public string logoName;
}

public class ConfigCardType : GConfigDataTable<ConfigCardTypeRecord>
{
    public ConfigCardType()
        : base("ConfigCardType")
	{
	}

	protected override void OnDataLoaded()
	{
        RebuildIndexField<int>("id");
	}

    public ConfigCardTypeRecord GetCardTypeByID(int ID)
	{
        return FindRecordByIndex<int>("id", ID);
	}
}