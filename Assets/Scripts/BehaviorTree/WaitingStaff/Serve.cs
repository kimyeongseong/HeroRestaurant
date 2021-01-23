using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Basic.WaitingStaff
{
    [TaskCategory("Basic/WaitingStaff")]
    public class Serve: Action
    {
        [RequiredField]
        public SharedWaitingStaff waitingStaff = null;

        public override TaskStatus OnUpdate()
        {
            waitingStaff.Value.Serve();
            return TaskStatus.Success;
        }
    }
}
