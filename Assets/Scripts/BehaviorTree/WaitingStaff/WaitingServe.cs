using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.WaitingStaff
{
    [TaskCategory("Basic/WaitingStaff")]
    public class WaitingServe : Action
    {
        [RequiredField]
        public SharedWaitingStaff waitingStaff = null;

        private Animator animator = null;

        public override void OnStart()
        {
            if (animator == null)
                animator = waitingStaff.Value.GetComponent<Animator>();
        }

        public override TaskStatus OnUpdate()
        {
            var  orderState    = waitingStaff.Value.OrderSheet.CurrentOrderState;
            bool isServingable = !(orderState == OerderState.Served || orderState == OerderState.Cancel);

            if (!isServingable &&
                animator.GetBool("isServing"))
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }
    }
}
