using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigRouteDesignRecord
{
    public float time;
    public string routes;

    public List<int> GetFishRoutes()
    {
        string[] splits = routes.Split(';');
        int numberRoutes = splits.Length;

        List<int> fishRoutes = new List<int>();

        for (int i = 0; i < numberRoutes; i++)
        {
            int routeID;
            if (int.TryParse(splits[i], out routeID))
                fishRoutes.Add(routeID);
        }

        return fishRoutes;
    }
}

public class ConfigRouteDesign : GConfigDataTable<ConfigRouteDesignRecord>
{
    public ConfigRouteDesign()
        : base("ConfigRouteDesign")
    {
    }

    protected override void OnDataLoaded()
    {
    }
}
