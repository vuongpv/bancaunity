using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
[FileHelpers.IgnoreEmptyLines(true)]
public class ConfigFontItem
{
	public string alias;
	public string en;
	public string vn;
	public string chn;
}

public class ConfigFont : GConfigDataTable<ConfigFontItem>
{
	public ConfigFont()
		: base("ConfigFont")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<string>("alias");
	}

	public ConfigFontItem GetFontItem(string font)
	{
		return FindRecordByIndex<string>("alias", font);
	}
}
