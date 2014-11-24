using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;
using FileHelpers;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigQuestRecord
{
    public int id;
    public int type;
    public int param1;
    public int param2;
    
    [FieldNullValue(typeof(float), "0")]
    public float time;

    public int award;

    public int titleID;

    public int descriptionID;
}

public class ConfigQuest : GConfigDataTable<ConfigQuestRecord>
{
    public ConfigQuest()
        : base("ConfigQuest")
    {
    }

    protected override void OnDataLoaded()
    {
        RebuildIndexField<int>("id");
    }

    public ConfigQuestRecord GetQuestByID(int ID)
    {
        return FindRecordByIndex<int>("id", ID);
    }
}
