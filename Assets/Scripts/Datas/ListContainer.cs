using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ListContainer<T> : ScriptableObject where T : class {
    [SerializeField]
    private List<T> list = new List<T>();

    public IReadOnlyList<T> List { get { return list; } }
}
