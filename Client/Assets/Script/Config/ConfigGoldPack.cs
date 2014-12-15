using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GFramework;
using FileHelpers;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigGoldPackRecord
{
    [FieldNullValue(typeof(string), "")]
    public string id;
    public int packType;
    public int payPortID;
	public int goldValue;
	public int goldBonus;
    public float cashValue;
	public string cashType;

    [FieldNullValue(typeof(string), "")]
    public string cardType;

    [FieldNullValue(typeof(string), "")]
    public string tag;
}

public class ConfigGoldPack : GConfigDataTable<ConfigGoldPackRecord>
{
    public ConfigGoldPack()
		: base("ConfigGoldPack")
	{
	}

	protected override void OnDataLoaded()
	{
        RebuildIndexField<string>("id");
        RebuildIndexField<int>("payPortID");
	}

    public ConfigGoldPackRecord GetPackByID(string ID)
	{
		return FindRecordByIndex<string>("id", ID);
	}

    public List<ConfigGoldPackRecord> GetPacksByPayPortID(int payPortID, FHShopPackType packType)
    {
        List<ConfigGoldPackRecord> result = new List<ConfigGoldPackRecord>();

        foreach (ConfigGoldPackRecord record in records)
            if (record.payPortID == payPortID && record.packType == (int)packType)
                result.Add(record);

        return result;
    }

    public ConfigGoldPackRecord GetPackByCashValue(int cashValue)
    {
        foreach (ConfigGoldPackRecord record in records)
        {
            FHPayPortIndex payPortID = (FHPayPortIndex)record.payPortID;
            if (!(payPortID == FHPayPortIndex.ZingCard || payPortID == FHPayPortIndex.VinaCard || payPortID == FHPayPortIndex.MobiCard || payPortID == FHPayPortIndex.ViettelCard))
                continue;

            if (cashValue == record.cashValue)
                return record;
        }

        ConfigGoldPackRecord pack = new ConfigGoldPackRecord();
        pack.cashValue = cashValue;
        pack.goldValue = cashValue / 10;
        pack.goldBonus = 0;

        if (cashValue < FHGameConstant.CASH_VALUE[0])
            return pack;

        int index = FHGameConstant.CASH_VALUE.Length - 1;

        for (int i = 1; i < FHGameConstant.CASH_VALUE.Length; i++)
            if (cashValue <= FHGameConstant.CASH_VALUE[i])
            {
                index = i - 1;
                break;
            }

        pack.goldBonus = pack.goldValue * FHGameConstant.GOLD_BONUS[index] / 100;

        return pack;
    }
}