using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Pathfinding
{
    [TaskCategory("Basic/Pathfinding")]
    public class SetDestination : Action
    {
        public SharedGameObject targetGameObject = null;
        public SharedVector3    destination      = Vector3.zero;

        private AILerp     astarAI        = null;
        private GameObject prevGameObject = null;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject)
            {
                astarAI        = currentGameObject.GetComponent<AILerp>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (astarAI == null)
            {
                Debug.LogWarning("NavMeshAgent is null");
                return TaskStatus.Failure;
            }

            astarAI.enabled = true;
            astarAI.destination = destination.Value;
            astarAI.SearchPath();

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            destination      = Vector3.zero;
        }
    }
}