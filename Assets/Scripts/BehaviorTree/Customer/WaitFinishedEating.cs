using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Customer
{
    [TaskCategory("Basic/Customer")]
    public class WaitFinishedEating : Action
    {
        public SharedCustomer customer = null;

        public override TaskStatus OnUpdate()
        {
            if (customer.Value.CurrentState == CustomerState.FinishedEating)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }
    }
}