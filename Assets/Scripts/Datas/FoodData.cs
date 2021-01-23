using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FoodData {
    public string name;
    public float  cookDelay;
    public int    buyPrice;
    public int    salePrice;
    public int    satisfactionPerSale;
    public float  priceScaleFactor;
    public string spritePath;
}
