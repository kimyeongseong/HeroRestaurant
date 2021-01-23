using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable]
public class SaveSlotoViewListEvent : UnityEvent<SaveSlotView>
{
}

[RequireComponent(typeof(ToggleGroup))]
public class SaveSlotViewList : Singleton<SaveSlotViewList>
{
    [SerializeField, Required]
    private GameObject saveSlotViewPrefab = null;

    public SaveSlotoViewListEvent onChangeSelectedSlotView = new SaveSlotoViewListEvent();

    public SaveSlotView CurrentSelectedSlotView { get; private set; }

	void Start () {
        var toggleGroup = GetComponent<ToggleGroup>();

        for (int i = 0; i < SaveSlotSystem.Instance.MaxNumOfSlots; i++)
        {
            var saveSlotViewObj = Instantiate(saveSlotViewPrefab, transform);
            var saveSlotView = saveSlotViewObj.GetComponent<SaveSlotView>();

            Debug.Assert(saveSlotView != null, "SaveSlotViewList::Start - SaveSlotView Component Not Exist");

            saveSlotView.Setup(i);
            saveSlotView.onSelected.AddListener(OnChangeSelectedSlotView);

            saveSlotViewObj.GetComponent<Toggle>().group = toggleGroup;
        }
    }

    private void OnChangeSelectedSlotView(SaveSlotView saveSlotView)
    {
        CurrentSelectedSlotView = saveSlotView;
        onChangeSelectedSlotView.Invoke(saveSlotView);
    }

    public void SelectedSaveSlotViewUpdate()
    {
        CurrentSelectedSlotView.ViewUpdate();
    }
}
