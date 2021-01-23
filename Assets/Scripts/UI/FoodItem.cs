using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;

public class FoodItem : MonoBehaviour {
    [SerializeField]
    private Image           foodPreviewImage  = null;
    [SerializeField]
    private Image           previewBlindImage = null;
    [SerializeField]
    private TextMeshProUGUI foodAmountText    = null;

    private float disableTime = 0f;
    private float currentTime = 0f;

    private bool     isCookStarted = false;
    private FoodData foodData;

    private void Awake()
    {
        previewBlindImage.enabled = false;
    }

    private void Start()
    {
        Inventory.Instance.onFoodAmountUpdated.AddListener(OnFoodViewUpdate);
    }

    private void OnEnable()
    {
        if (isCookStarted)
        {
            float betweenTime = Time.time - disableTime;
            currentTime += betweenTime;

            if ((currentTime / foodData.cookDelay) >= 1f)
            {
                previewBlindImage.enabled = false;
                isCookStarted = false;
                currentTime = 0f;
            }
            else
                StartCoroutine("Cooking");
        }
    }

    private void OnDisable()
    {
        disableTime = Time.time;
    }

    private void OnFoodViewUpdate(Food food)
    {
        if (food.FoodData.name == foodData.name)
            foodAmountText.text = food.Amount.ToString();
    }

    public void Setup(FoodData foodData, Sprite previewSprite)
    {
        this.foodData = foodData;
        foodPreviewImage.sprite = previewSprite;
    }

    public void OnClicked()
    {
        if (isCookStarted)
            return;

        Inventory playerInventory = Inventory.Instance;
        if (playerInventory.CurrentMoney >= foodData.buyPrice)
        {
            playerInventory.IncreaseMoney(-foodData.buyPrice);
            Inventory.Instance.IncreaseFoodAmount(foodData.name);
            StartCoroutine("Cooking");
        }
    }

    private IEnumerator Cooking()
    {
        float timePoint   = 0f;

        previewBlindImage.enabled = true;
        isCookStarted = true;

        while (timePoint < 1)
        {
            currentTime += Time.smoothDeltaTime;
            timePoint = (currentTime / foodData.cookDelay);
            previewBlindImage.fillAmount = 1f - timePoint;

            yield return null;
        }

        previewBlindImage.enabled = false;
        isCookStarted = false;
        currentTime   = 0f;
    }
}
