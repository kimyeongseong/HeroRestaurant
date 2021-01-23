using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using SimpleDatabase;
using Pathfinding;

public enum CustomerState
{
    Moving,
    Waiting,
    Eating,
    FinishedEating,
}

[RequireComponent(typeof(AILerp), typeof(BehaviorTree))]
public class Customer : MonoBehaviour {
    [SerializeField]
    private CustomerData customerData;
    [SerializeField]
    private OrderSheet orderSheet = null;

    private Animator animator    = null;
    private IUsable  usingObject = null;
    private IAstarAI astarAI     = null;

    public CustomerData CustomerData
    {
        get
        {
            var readonlyData = customerData;
            return readonlyData;
        }
    }
    public CustomerState CurrentState { get; set; } = CustomerState.Moving;

    public Floor      UsingFloor { get; private set; }
    public OrderSheet OrderSheet { get { return orderSheet; } }

    public void Setup(Floor usingFloor, Chair usingChair)
    {
        customerData = Database.Instance.Select<CustomerData>("CustomerDataTable").Rows[0];

        astarAI = GetComponent<IAstarAI>();
        astarAI.maxSpeed = customerData.moveSpeed;

        animator = GetComponent<Animator>();

        UsingFloor = usingFloor;
        usingObject = usingChair;

        var behaviorTree = GetComponent<BehaviorTree>();
        behaviorTree.SetVariableValue("targetObj", usingChair.gameObject);
    }

    private void Update()
    {
        if (astarAI.remainingDistance != 0f && astarAI.remainingDistance != float.PositiveInfinity)
        {
            Vector3 velocityNormalize = astarAI.velocity.normalized;
            animator.SetFloat("horizontal", velocityNormalize.x);
            animator.SetFloat("vertical", velocityNormalize.y);
        }
    }

    public void OrderFood()
    {
        var orderableFood = UsingFloor.GetComponent<BusinessSystem>().FoodMenu.GetOrderableFoodByRandom();

        orderSheet.Order(this, UsingFloor, orderableFood);
        CurrentState = CustomerState.Waiting;
    }

    public void OrderCancel()
    {
        orderSheet.Cancel();
    }

    public void TakeFood()
    {
        animator.SetBool("isEating", true);

        CurrentState = CustomerState.Eating;
        StartCoroutine("Eating");
    }

    private IEnumerator Eating()
    {
        float eatingTime = Random.Range(customerData.minimumEatingTime, customerData.maximumEatingTime);

        yield return new WaitForSeconds(eatingTime);

        CurrentState = CustomerState.FinishedEating;
        usingObject.Unuse(gameObject);
        usingObject = null;

        animator.SetBool("isEating", false);
    }
}
