using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Pathfinding
{
    [TaskCategory("Basic/Pathfinding")]
    public class WaitArrive : Action
    {
        public SharedGameObject targetGameObject = null;

        private IAstarAI     astartAI       = null;
        private GameObject   prevGameObject = null;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject)
            {
                astartAI       = currentGameObject.GetComponent<IAstarAI>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (!astartAI.pathPending && astartAI.remainingDistance == 0)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}
