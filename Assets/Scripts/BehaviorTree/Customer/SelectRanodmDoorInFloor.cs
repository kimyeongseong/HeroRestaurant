using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Customer
{
    [TaskCategory("Basic/Customer")]
    public class SelectRandomDoorInFloor : Action
    {
        public SharedCustomer   customer = null;
        [RequiredField]
        public SharedGameObject doorObj  = null;

        public override TaskStatus OnUpdate()
        {
            var doors = customer.Value.UsingFloor.GetFurnitures<Door>();
            if (doors == null)
                return TaskStatus.Failure;
            else
            {
                int randomIndex = Random.Range(0, doors.Length);
                doorObj.Value = doors[randomIndex].gameObject;

                return TaskStatus.Success;
            }
        }
    }
}
