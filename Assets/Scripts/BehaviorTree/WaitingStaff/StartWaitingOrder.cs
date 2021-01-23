using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.WaitingStaff
{
    [TaskCategory("Basic/WaitingStaff")]
    public class StartWaitingOrder : Action
    {
        [RequiredField]
        public SharedWaitingStaff waitingStaff = null;

        public override TaskStatus OnUpdate()
        {
            waitingStaff.Value.CurrentState = WaitingStaffState.WaitingOrder;
            return TaskStatus.Success;
        }
    }
}
