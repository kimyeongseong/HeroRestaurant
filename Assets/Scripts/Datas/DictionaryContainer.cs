using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class DictionaryContainer<T, V> : SerializedScriptableObject {
    [OdinSerialize]
    private Dictionary<T, V> container = null;

    public V this[T tileMarkType]
    {
        get
        {
            return container[tileMarkType];
        }
    }
}
