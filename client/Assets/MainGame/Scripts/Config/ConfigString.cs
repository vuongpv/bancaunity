using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
[FileHelpers.IgnoreEmptyLines(true)]
public class ConfigStringItem
{
	public int id;
	public string alias;
	public string en;
	public string vn;
	public string chn;
	public string info;
}

public class ConfigString : GConfigDataTable<ConfigStringItem>
{
	public ConfigString()
		: base("ConfigString")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<int>("id");
		RebuildIndexField<string>("alias");
	}

	public ConfigStringItem GetStringItem(int ID)
	{
		return FindRecordByIndex<int>("id", ID);
	}
}
