using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using SimpleDatabase;

public class FurnitureScrollView : MonoBehaviour {
    [SerializeField]
    private GameObject    furnitureItemPrefab = null;
    [SerializeField]
    private RectTransform content             = null;

    public void Awake()
    {
        var atlas = Resources.Load<SpriteAtlas>("Atlas/FurnitureAtlas");

        var furnitureDatas = Database.Instance.Select<FurnitureData>("FurnitureDataTable").Rows;
        foreach (var furnitureData in furnitureDatas)
        {
            var furniturePrefab = Resources.Load<GameObject>("Furniture/" + furnitureData.name);
            var previewSprite   = atlas.GetSprite(furnitureData.spritePaths[0]);
            var foodItem        = Instantiate(furnitureItemPrefab, content);
            foodItem.GetComponent<FurnitureItem>().Setup(furniturePrefab, previewSprite, furnitureData);
        }

        Resources.UnloadAsset(atlas);
    }
}
