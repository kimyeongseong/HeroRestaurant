using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BehaviorDesigner.Runtime;
using SimpleDatabase;
using Pathfinding;

public enum WaitingStaffState
{
    WaitingOrder,
    TakenOrder,
    Moving,
}

[System.Serializable]
public class WaitingStaffEvent : UnityEvent<WaitingStaff>
{
}

[RequireComponent(typeof(BehaviorTree))]
public class WaitingStaff : MonoBehaviour {
    [SerializeField]
    private WaitingStaffData waitingStaffData;

    private Vector3      watingPoint  = Vector3.zero;
    private BehaviorTree behaviorTree = null;
    private Animator     animator     = null;
    private IAstarAI     astarAI      = null;

    public WaitingStaffData  WaitingStaffData
    {
        get
        {
            var readonlyData = waitingStaffData;
            return readonlyData;
        }
    }
    public Vector3 WaitingPoint
    {
        get
        {
            return watingPoint;
        }
        set
        {
            watingPoint = value;
            behaviorTree.SetVariableValue("waitingPoint", watingPoint);
        }
}
    public OrderSheet        OrderSheet   { get; private set; }
    public Floor             PlacedFloor  { get; set; }
    public WaitingStaffState CurrentState { get; set; } = WaitingStaffState.WaitingOrder;

    private void Awake()
    {
        string objectID  = gameObject.name.Replace("(Clone)", "");
        waitingStaffData = Database.Instance.Select<WaitingStaffData>("WaitingStaffDataTable").Select(objectID);
        behaviorTree     = GetComponent<BehaviorTree>();
        animator         = GetComponent<Animator>();
        astarAI          = GetComponent<IAstarAI>();
    }

    private void Update()
    {
        Vector3 velocityNormalize = astarAI.velocity;
        animator.SetFloat("horizontal", velocityNormalize.x);
        animator.SetFloat("vertical", velocityNormalize.y);
    }

    public void TakeOrder(OrderSheet orderSheet)
    {
        OrderSheet   = orderSheet;
        CurrentState = WaitingStaffState.TakenOrder;
        GetComponent<BehaviorTree>().SetVariableValue("customerObj", orderSheet.Customer.gameObject);

        Inventory.Instance.IncreaseFoodAmount(OrderSheet.OrderedFood.FoodData.name, -1);

        animator.SetBool("isServing", true);
    }

    public void ServingCancel()
    {
        Inventory.Instance.IncreaseFoodAmount(OrderSheet.OrderedFood.FoodData.name);
    }

    public void Serve()
    {
        OrderSheet.Customer.TakeFood();
        OrderSheet.ChangeState(OerderState.Served);

        GetComponent<BehaviorTree>().SetVariableValue("customerObj", null);

        animator.SetBool("isServing", false);
    }
}
