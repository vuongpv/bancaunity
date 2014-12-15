using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FHRouteAsset
{
    public int id;

    public bool isGroup;

    public int fishID;

    public int fishGroupID;
    
    public Vector3 splinePos;
    
    public Vector3[] splineNodes;
}

public class FHRoutesAsset : ScriptableObject
{
    public FHRouteAsset[] routes;
}