using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    public class SharedWaitingStaff : SharedVariable<WaitingStaff>
    {
        public static implicit operator SharedWaitingStaff(WaitingStaff value) { return new SharedWaitingStaff { mValue = value }; }
    }
}