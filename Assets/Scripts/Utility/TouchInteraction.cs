using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class TouchInteractionEvent : UnityEvent<GameObject>
{
}

public class TouchInteraction : MonoBehaviour {
    public TouchInteractionEvent onEntered = new TouchInteractionEvent();
    public TouchInteractionEvent onPressed = new TouchInteractionEvent();
    public TouchInteractionEvent onHolding = new TouchInteractionEvent();
    public TouchInteractionEvent onStaying = new TouchInteractionEvent();
    public TouchInteractionEvent onClicked = new TouchInteractionEvent();
    public TouchInteractionEvent onRelesed = new TouchInteractionEvent();
    public TouchInteractionEvent onExited  = new TouchInteractionEvent();

    private int interactiveFingerID = -1;

    private void Start()
    {
    }

    private void OnMouseEnter()
    {
        if (!IsPointerOverGameObject() && enabled)
        {
            onEntered.Invoke(gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (!IsPointerOverGameObject() && enabled)
        {
            interactiveFingerID = GetLastTouch().fingerId;

            onPressed.Invoke(gameObject);
        }
    }

    private void OnMouseOver()
    {
        if (!IsPointerOverGameObject() &&
            IsSameInteractiveFingerAndLastTouch() &&
            enabled)
        {
            onStaying.Invoke(gameObject);
        }
    }

    private void OnMouseDrag()
    {
        if (!IsPointerOverGameObject() &&
            IsSameInteractiveFingerAndLastTouch() &&
            enabled)
        {
            onHolding.Invoke(gameObject);
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!IsPointerOverGameObject() &&
            IsSameInteractiveFingerAndLastTouch() &&
            enabled)
        {
            interactiveFingerID = -1;

            onClicked.Invoke(gameObject);
        }
    }

    private void OnMouseExit()
    {
        if (!IsPointerOverGameObject() &&
            IsSameInteractiveFingerAndLastTouch() &&
            enabled)
        {
            onRelesed.Invoke(gameObject);
            interactiveFingerID = -1;
        }
        else if(IsPointerOverGameObject())
        {
            onExited.Invoke(gameObject);
        }
    }

    private bool IsPointerOverGameObject()
    {
        if (EventSystem.current == null)
            return false;
        else
            return EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(1);
    }

    private bool IsSameInteractiveFingerAndLastTouch()
    {
#if UNITY_EDITOR || UNITY_STANDARD
        return true;
#else
        return interactiveFingerID == GetLastTouch().fingerId;
#endif
    }

    private Touch GetLastTouch()
    {
        if (Input.touches.Length > 0)
        {
            Touch[] touches = Input.touches;
            touches.OrderByDescending(a => a.fingerId);

            return touches[0];
        }
        else
        {
            Touch emptyTouch = new Touch
            {
                fingerId = -1
            };
            return emptyTouch;
        }
    }
}
