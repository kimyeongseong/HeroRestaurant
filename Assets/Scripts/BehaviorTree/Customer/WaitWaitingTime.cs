using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Customer
{
    [TaskCategory("Basic/Customer")]
    public class WaitWaitingTime : Action
    {
        public SharedCustomer customer = null;

        private float waitDuration;
        private float startTime;
        private float pauseTime;

        public override void OnStart()
        {
            startTime    = Time.time;
            waitDuration = customer.Value.CustomerData.waitingTime;
        }

        public override TaskStatus OnUpdate()
        {
            if (startTime + waitDuration < Time.time)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        public override void OnPause(bool paused)
        {
            if (paused)
            {
                pauseTime = Time.time;
            }
            else
            {
                startTime += (Time.time - pauseTime);
            }
        }
    }
}