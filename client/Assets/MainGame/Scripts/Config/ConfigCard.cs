using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GFramework;
using FileHelpers;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigCardRecord
{
    public string id;
    public int cardType;
	public int diamondValue;
	public int cardValue;
}

public class ConfigCard : GConfigDataTable<ConfigCardRecord>
{
    public ConfigCard()
        : base("ConfigCard")
	{
	}

	protected override void OnDataLoaded()
	{
        RebuildIndexField<int>("cardType");
	}

    public List<ConfigCardRecord> GetCardByType(int cardType)
    {
        List<ConfigCardRecord> result = new List<ConfigCardRecord>();

        foreach (ConfigCardRecord record in records)
            if (record.cardType == cardType)
                result.Add(record);

        return result;
    }
}