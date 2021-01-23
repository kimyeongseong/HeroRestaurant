using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class FloorView : MonoBehaviour {
    [SerializeField]
    private GameObject  defaultUI        = null;
    [SerializeField]
    private GameObject  backgroundObject = null;
    [SerializeField, Required]
    private GameObject  buyButtonObj     = null;
    [SerializeField, Required]
    private GameObject  lockImageObj     = null;

    private FloorSystem floorSystem = null;

    public bool IsShowingBuyUI
    {
        set
        {
            backgroundObject.SetActive(value);
            buyButtonObj.SetActive(value);
            lockImageObj.SetActive(value);

            if (GameMode.Instance.RemainedBusinessTime == 0f)
                defaultUI.SetActive(!value);
        }
    }
    public bool IsBuyable
    {
        set
        {
            buyButtonObj.SetActive(value);
            lockImageObj.SetActive(!value);
        }
    }

    public void Awake()
    {
        IsShowingBuyUI = false;

        floorSystem = FloorSystem.Instance;
        floorSystem.onShowingDifferentFloor.AddListener(Show);
        floorSystem.onBuyFloor.AddListener(Show);
    }

    public void Show(Floor floor)
    {
        if (floor.CurrentState == FloorState.Bought)
        {
            IsShowingBuyUI = false;
            return;
        }
        else
        {
            IsShowingBuyUI = true;

            if (floorSystem.IsCanBuyNextFloor && floor == floorSystem.NextBuyableFloor)
                IsBuyable = true;
            else
                IsBuyable = false;
        }   
    }
}
