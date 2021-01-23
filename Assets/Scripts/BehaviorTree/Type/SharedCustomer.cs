using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    public class SharedCustomer : SharedVariable<Customer>
    {
        public static implicit operator SharedCustomer(Customer value) { return new SharedCustomer { mValue = value }; }
    }
}
