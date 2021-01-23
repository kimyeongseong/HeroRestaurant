using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleDatabase;
using Sirenix.OdinInspector;
using SimpleDatabase;

[RequireComponent(typeof(TouchInteraction))]
public class Kitchen : Furniture {
    [ShowInInspector, TabGroup("Kitchen")]
    private KitchenData kitchenData;

    private int chefCount = 0;

    private void Awake()
    {
        kitchenData = Database.Instance.Select<KitchenData>("KitchenDataTable").Rows[0];

        GetComponent<TouchInteraction>().onClicked.AddListener(OnClicked);

        GameMode.Instance.onBusinessModeStarted.AddListener(StartCook);
        GameMode.Instance.onBusinessTimeOvered.AddListener(StopCook);
    }

    private void OnDestroy()
    {
        var gameMode = GameMode.Instance;
        if (gameMode)
        {
            gameMode.onBusinessModeStarted.RemoveListener(StartCook);
            gameMode.onEditorModeStarted.RemoveListener(StopCook);
        }
    }

    private void OnClicked(GameObject target)
    {
        if (InteriorSystem.Instance.IsRebuildMode)
            return;

        if (CurrentState == FurnitureState.Bought &&
            Inventory.Instance.CurrentMoney >= kitchenData.chefPrice &&
            chefCount < kitchenData.maximumEmployable)
        {
            ChefBuy();
        }
    }

    private IEnumerator Cooking()
    {
        var waitForcookingDelay = new WaitForSeconds(kitchenData.cookDelay);
        var foodDatas           = Database.Instance.Select<FoodData>("FoodDataTable").Rows;

        while (true)
        {
            if (chefCount == 0)
                yield return null;
            else
                yield return waitForcookingDelay;

            for (int i = 0; i < chefCount; i++)
            {
                int randomIndex = Random.Range(0, foodDatas.Length);
                var foodData = foodDatas[randomIndex];

                Inventory.Instance.IncreaseFoodAmount(foodData.name);
            }
        }
    }

    public void ChefBuy()
    {
        Inventory.Instance.IncreaseMoney(-kitchenData.chefPrice);
        chefCount += 1;
    }

    public void StartCook()
    {
        StartCoroutine("Cooking");
    }

    public void StopCook()
    {
        StopCoroutine("Cooking");
    }

    public override JSONObject SaveToJson()
    {
        var root = base.SaveToJson();
        root.AddField("chefCount", chefCount);

        return root;
    }

    public override void LoadFromJson(JSONObject root)
    {
        base.LoadFromJson(root);

        chefCount = (int)root["chefCount"].i;
    }
}
