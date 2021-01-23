using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Customer
{
    [TaskCategory("Basic/Customer")]
    public class FindClosestCounter : Action
    {
        public SharedCustomer customer;
        public SharedGameObject questBoard;

        public override TaskStatus OnUpdate()
        {
            questBoard.Value = customer.Value.UsingFloor.Counter.gameObject;
            return TaskStatus.Success;
        }
    }
}
