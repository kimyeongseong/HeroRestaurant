using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Customer
{
    [TaskCategory("Basic/Customer")]
    public class OrderCacel : Action
    {
        public SharedCustomer customer = null;

        public override TaskStatus OnUpdate()
        {
            customer.Value.OrderCancel();
            return TaskStatus.Success;
        }
    }
}
