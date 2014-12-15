using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;
using FileHelpers;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigPowerupRecord
{
    public int multiplier;
    public int gold;
}

public class ConfigPowerup : GConfigDataTable<ConfigPowerupRecord>
{
    Dictionary<int, int> powerupTable = new Dictionary<int, int>();

    public ConfigPowerup()
        : base("ConfigPowerup")
    {
    }

    protected override void OnDataLoaded()
    {
        foreach (ConfigPowerupRecord record in records)
            powerupTable[record.multiplier] = record.gold;
    }

    public int GetGoldLimitForMultiplier(int multiplier)
    {
        return powerupTable[multiplier];
    }
}