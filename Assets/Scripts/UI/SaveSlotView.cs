using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using SimpleDatabase;

[System.Serializable]
public class SaveSlotViewEvent : UnityEvent<SaveSlotView>
{
}

[RequireComponent(typeof(Toggle))]
public class SaveSlotView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI slotNumberText = null;
    [SerializeField]
    private TextMeshProUGUI slotInfoText   = null;

    public SaveSlotViewEvent onSelected = new SaveSlotViewEvent();

    public int  SlotNumber    { get; private set; }
    public bool IsDataExisted { get; private set; }

    private void Awake()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(OnSelected);
    }

    public void Setup(int slotNumber)
    {
        SlotNumber = slotNumber;
        slotNumberText.text = slotNumber.ToString();

        ViewUpdate();
    }

    public void ViewUpdate()
    {
        var textDataTable = Database.Instance.Select<TextData>("TextDataTable");

        if (!SaveSlotSystem.Instance.IsExistSavedSlotData(SlotNumber))
        {
            slotInfoText.text = textDataTable.Select("SLOT_NOT_EXIST_MESSAGE").text;
            IsDataExisted = false;
        }
        else
        {
            var root = new JSONObject(SaveSlotSystem.Instance.SavedSlotDataToJson(SlotNumber));

            string day   = root.keys.Count > 0 ? root["Game Mode"][0]["day"].i.ToString() : "1";
            string money = root.keys.Count > 0 ? root["Inventory"][0]["money"].i.ToString() : "20000";

            string showingMessage = textDataTable.Select("SLOT_EXIST_MESSAGE").text;
            showingMessage = showingMessage.Replace("${day}", day);
            showingMessage = showingMessage.Replace("${money}", money);

            slotInfoText.text = showingMessage;
            IsDataExisted = true;
        }
    }

    private void OnSelected(bool isOn)
    {
        var slotSelectedMessage = Database.Instance.Select<TextData>("TextDataTable")
                                                   .Select("SLOT_SELECT_MESSAGE")
                                                   .text;
        slotSelectedMessage = slotSelectedMessage.Replace("${slotNumber}", SlotNumber.ToString());

        if (isOn)
        {
            onSelected.Invoke(this);
            TextDisplay.Instance.Show(slotSelectedMessage);
            SaveSlotSystem.Instance.SelectSlot(SlotNumber);
        }
    }
}
