using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RelayActivatorEvent : UnityEvent
{

}

public class RelayActivator : MonoBehaviour {
    [SerializeField]
    private int          currentActivateIndex = -1;
    [SerializeField]
    private GameObject[] objects              = null;

    public RelayActivatorEvent onActivateAllCompleted = new RelayActivatorEvent();

    public void NextObjectActivate()
    {
        if (currentActivateIndex < (objects.Length - 1))
        {
            if (currentActivateIndex >= 0)
                objects[currentActivateIndex].SetActive(false);

            objects[++currentActivateIndex].SetActive(true);

            if (currentActivateIndex == (objects.Length - 1))
                onActivateAllCompleted.Invoke();
        }
    }
}
