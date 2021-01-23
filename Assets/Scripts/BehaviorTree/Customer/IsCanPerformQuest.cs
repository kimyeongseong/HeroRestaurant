using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Customer
{
    [TaskCategory("Basic/Customer")]
    public class IsCanPerformQuest : Action
    {
        public SharedCustomer customer = null;

        public override TaskStatus OnUpdate()
        {
            float probability = Random.value;
            float questObtainOrderProbability = customer.Value.CustomerData.questObtainOrderProbability;

            if (probability < questObtainOrderProbability)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
