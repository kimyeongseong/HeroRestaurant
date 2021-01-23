using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RelativeMoveToEvent : UnityEvent
{

}

public class RelativeMoveTo : MonoBehaviour {
    [SerializeField]
    private bool isAutoPlay  = false;
    [SerializeField]
    private Vector3 moveTo   = Vector3.zero;
    [SerializeField]
    private float   moveTime = 0f;

    public RelativeMoveToEvent onRelativeMoveToEvent = new RelativeMoveToEvent();

    private void Start()
    {
        if (isAutoPlay)
            StartMoving();
    }

    private IEnumerator Moving()
    {
        float currentTime = 0f;
        float timePoint = 0f;

        Vector3 origionPosition = transform.position;
        Vector3 arrivePosition = transform.position + moveTo;

        while (timePoint < 1f)
        {
            currentTime += Time.smoothDeltaTime;
            timePoint = currentTime / moveTime;

            transform.position = Vector3.Lerp(origionPosition, arrivePosition, timePoint);
            yield return null;
        }

        onRelativeMoveToEvent.Invoke();
    }

    public void StartMoving()
    {
        StartCoroutine("Moving");
    }
}
