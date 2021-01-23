using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour, IUsable {
    [SerializeField]
    private GameObject textPrefab = null;
    [SerializeField]
    private Vector2    textShowingPositionOffset;

    private Canvas mainCanvas = null;

    private void Awake()
    {
        mainCanvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
    }

    public void Use(GameObject user)
    {
        var customer = user.GetComponent<Customer>();

        Debug.Assert(customer != null, $"Counter::Use({user.name}) - Customer component not exist in user");

        var orderSheet    = customer.OrderSheet;
        var orderableFood = orderSheet.OrderedFood;
        orderableFood.IncreaseSatisfaction();

        int   randomSeed        = Random.Range(0, 100);
        int   salePrice         = orderableFood.FoodData.salePrice;
        float priceScaleFaction = orderableFood.FoodData.priceScaleFactor;

        salePrice = randomSeed < orderableFood.Satisfaction ? Mathf.CeilToInt(salePrice * priceScaleFaction) :
                                                               salePrice;

        var textObj = Instantiate(textPrefab, mainCanvas.transform);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        textObj.transform.position = screenPosition + textShowingPositionOffset;

        textObj.GetComponent<TextMeshProUGUI>().text = salePrice.ToString();

        Inventory.Instance.IncreaseMoney(salePrice);
    }

    public void Unuse(GameObject user)
    {

    }
}
