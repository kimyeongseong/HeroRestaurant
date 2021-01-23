using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FurnitureData
{
    public string     name;
    public int        buyPrice;
    public Vector2Int tileSize;
    public float      buildCompleteTime;
    public float      demolitionCompensationPercent;
    public string[]   spritePaths;
}
