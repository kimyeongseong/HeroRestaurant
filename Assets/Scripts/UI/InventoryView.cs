using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SimpleDatabase;

public class InventoryView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI moneyText = null;

    private string moneyOriginText = null;

	void Awake () {

        moneyOriginText = Database.Instance.Select<TextData>("TextDataTable").Select("MONEY").text;

        Inventory.Instance.onMoneyAmountUpdated.AddListener(MoneyViewUpdate);
    }

    private void Start()
    {
        MoneyViewUpdate(Inventory.Instance.CurrentMoney);
    }

    private void MoneyViewUpdate(int money)
    {
        moneyText.text = moneyOriginText.Replace("${money}", money.ToString());
    }
}
