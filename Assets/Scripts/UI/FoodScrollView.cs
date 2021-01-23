using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using SimpleDatabase;

public class FoodScrollView : MonoBehaviour {
    [SerializeField]
    private GameObject    foodItemItemPrefab = null;
    [SerializeField]
    private RectTransform content        = null;

    public void Awake()
    {
        var atlas = Resources.Load<SpriteAtlas>("Atlas/FoodAtlas");

        var foodDatas = Database.Instance.Select<FoodData>("FoodDataTable").Rows;
        foreach (var foodData in foodDatas)
        {
            var foodItem      = Instantiate(foodItemItemPrefab, content);
            var previewSprite = atlas.GetSprite(foodData.name);
            foodItem.GetComponent<FoodItem>().Setup(foodData, previewSprite);
        }

        Resources.UnloadAsset(atlas);
    }
}
