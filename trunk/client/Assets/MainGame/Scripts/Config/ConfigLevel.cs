using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;
using FileHelpers;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]

public class ConfigLevelRecord
{
    // Level	XPPerLevel	AccXP	Day	PayoutRate	MaxBet
    [FieldNullValue(typeof(int), "0")]
    public int level;

    [FieldNullValue(typeof(int), "0")]
	public int xpPerLevel;

    [FieldNullValue(typeof(int), "0")]
	public int accountXP;

    [FieldNullValue(typeof(int), "0")]
	public int day;

    [FieldNullValue(typeof(int), "0")]
    public int payoutRate;

    [FieldNullValue(typeof(int), "0")]
    public int maxBet;

    [FieldNullValue(typeof(int), "0")]
    public int gold;
}

public class ConfigLevel : GConfigDataTable<ConfigLevelRecord>
{
    public ConfigLevel()
        : base("ConfigLevel")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<int>("level");
	}

    public ConfigLevelRecord GetLevel(int level)
	{
		return FindRecordByIndex<int>("level", level);
	}
}
