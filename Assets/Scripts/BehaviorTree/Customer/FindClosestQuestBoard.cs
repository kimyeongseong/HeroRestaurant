using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Customer
{
    [TaskCategory("Basic/Customer")]
    public class FindClosestQuestBoard : Action
    {
        public SharedCustomer   customer;
        public SharedGameObject questBoard;

        public override TaskStatus OnUpdate()
        {
            questBoard.Value = customer.Value.UsingFloor.QuestBoard.gameObject;
            return TaskStatus.Success;
        }
    }
}
