using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Customer
{
    [TaskCategory("Basic/Customer")]
    public class IsSameState : Action
    {
        public SharedCustomer customer = null;
        public CustomerState state;

        public override TaskStatus OnUpdate()
        {
            if (customer.Value.CurrentState == state)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
