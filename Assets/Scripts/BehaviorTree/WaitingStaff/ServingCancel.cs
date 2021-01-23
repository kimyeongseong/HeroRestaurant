using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.WaitingStaff
{
    [TaskCategory("Basic/WaitingStaff")]
    public class ServingCancel : Action
    {
        [RequiredField]
        public SharedWaitingStaff waitingStaff = null;

        public override TaskStatus OnUpdate()
        {
            waitingStaff.Value.ServingCancel();
            return TaskStatus.Success;
        }
    }
}