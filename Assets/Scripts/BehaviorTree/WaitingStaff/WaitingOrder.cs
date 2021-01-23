using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.WaitingStaff
{
    [TaskCategory("Basic/WaitingStaff")]
    public class WaitingOrder : Action
    {
        [RequiredField]
        public SharedWaitingStaff waitingStaff = null;

        private Animator animator = null;

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if (waitingStaff.Value.CurrentState == WaitingStaffState.TakenOrder)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }
    }
}
