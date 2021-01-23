using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class DraggableObjectEvent : UnityEvent<GameObject>
{
}

public class DraggableObject : MonoBehaviour
{
    [SerializeField]
    private bool isAutoResetPosition = false;

    public DraggableObjectEvent onDragStarted = new DraggableObjectEvent();
    public DraggableObjectEvent onDragging = new DraggableObjectEvent();
    public DraggableObjectEvent onDragEnded = new DraggableObjectEvent();

    private int     interactiveFingerID = -1;
    private Vector3 originPosition      = Vector3.zero;
    private Vector3 dragOffset          = Vector3.zero;

    private void OnMouseDown()
    {
        if (!IsPointerOverGameObject() && enabled)
        {
            interactiveFingerID = GetLastTouch().fingerId;
            originPosition = transform.position;
            dragOffset = originPosition - GetCurrentTouchPoint();

            onDragStarted.Invoke(gameObject);
        }
    }

    private void OnMouseDrag()
    {
        if (!IsPointerOverGameObject() &&
            IsSameInteractiveFingerAndLastTouch() &&
            enabled)
        {
            transform.position = GetCurrentTouchPoint() + dragOffset;

            onDragging.Invoke(gameObject);
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!IsPointerOverGameObject() &&
            IsSameInteractiveFingerAndLastTouch() &&
            enabled)
        {
            interactiveFingerID = -1;

            if (isAutoResetPosition)
                transform.position = originPosition;

            onDragEnded.Invoke(gameObject);
        }
    }

    private bool IsPointerOverGameObject()
    {
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

    private Vector3 GetCurrentTouchPoint()
    {
#if UNITY_EDITOR || UNITY_STANDARD
        Vector3 touchPosition = Input.mousePosition;
#else
        Vector3 touchPosition = GetLastTouch().position;
#endif
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(touchPosition);
        screenToWorld.z = transform.position.z;

        return screenToWorld;
    }
}
