using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Useable
{
    [TaskCategory("Basic/Useable")]
    public class UnuseTarget : Action
    {
        public SharedGameObject usableObject = null;

        public override TaskStatus OnUpdate()
        {
            var usable = GetDefaultGameObject(usableObject.Value).GetComponent<IUsable>();
            if (usable == null)
                return TaskStatus.Failure;
            else
            {
                usable.Unuse(gameObject);
                return TaskStatus.Success;
            }
        }
    }
}
