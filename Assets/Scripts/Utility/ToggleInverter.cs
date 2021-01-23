using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class ReverseToggleEvent : UnityEvent<bool>
{
}

[RequireComponent(typeof(Toggle))]
public class ToggleInverter : MonoBehaviour {
    public ReverseToggleEvent onValueChanged = new ReverseToggleEvent();

    void Awake() {
        GetComponent<Toggle>().onValueChanged.AddListener(isOn => onValueChanged.Invoke(!isOn));
    }	
}
