using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class LobbyWorkViewController : MonoBehaviour {
    [SerializeField, Required]
    private Button startButton  = null;
    [SerializeField, Required]
    private Button createButton = null;
    [SerializeField, Required]
    private Button removeButton = null;

    private void Start () {
        SaveSlotViewList.Instance.onChangeSelectedSlotView.AddListener(OnLobbyWorkViewUpdate);

        startButton.interactable  = false;
        createButton.interactable = false;
        removeButton.interactable = false;
    }

    private void OnLobbyWorkViewUpdate(SaveSlotView saveSlotView)
    {
        bool isDataExisted = saveSlotView.IsDataExisted;
        startButton.interactable = isDataExisted;
        createButton.interactable = !isDataExisted;
        removeButton.interactable = isDataExisted;
    }

    public void ViewUpdate()
    {
        OnLobbyWorkViewUpdate(SaveSlotViewList.Instance.CurrentSelectedSlotView);
    }
}
