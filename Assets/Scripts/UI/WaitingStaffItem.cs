using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class WaitingStaffItem : MonoBehaviour {
    [SerializeField, Required]
    private Image previewImage = null;

    private GameObject waitingStaffPrefab = null;

    private int waitingStaffPrice = 0;

    public void Setup(GameObject waitingStaffPrefab, Sprite previewSprite, WaitingStaffData waitingStaffData)
    {
        this.waitingStaffPrefab = waitingStaffPrefab;
        previewImage.sprite     = previewSprite;
        waitingStaffPrice       = waitingStaffData.buyPrice;
    }

    public void OnCliked()
    {
        var currentShowingFloor = FloorSystem.Instance.CurrentShowingFloor;
        var businessSystem      = currentShowingFloor.GetComponent<BusinessSystem>();

        if (currentShowingFloor.CurrentState == FloorState.Bought &&
            Inventory.Instance.CurrentMoney >= waitingStaffPrice &&
            businessSystem.IsWaitingStaffHireable)
        {
            Inventory.Instance.IncreaseMoney(-waitingStaffPrice);

            var waitingStaffObj      = Instantiate(waitingStaffPrefab);
            var waitingStaff         = waitingStaffObj.GetComponent<WaitingStaff>();
            waitingStaff.PlacedFloor = currentShowingFloor;
            businessSystem.HireWaitingStaff(waitingStaff);
        }
    }
}
