using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum DirectionLimit
{
    Left,
    Right,
    Up,
    Down,
    All,
    Max
}

[System.Serializable]
public class DragInteractionEvent : UnityEvent<GameObject>
{
}

public class DragInteraction : MonoBehaviour
{
    [SerializeField]
    private DirectionLimit directionLimit = DirectionLimit.All;
    [SerializeField]
    private float dragRecognitionDistance = 0f;
    [SerializeField]
    private bool isInteractiveOverUI;
    [SerializeField]
    private bool isOnlyOnce = false;

    public DragInteractionEvent onDragStarted = new DragInteractionEvent();
    public DragInteractionEvent onDragging = new DragInteractionEvent();
    public DragInteractionEvent onDragEnded = new DragInteractionEvent();

    private int interactiveFingerID = -1;
    private bool isTouched     = false;
    private bool isDragStarted = false;
    private Vector3 originPosition = Vector3.zero;

    private void OnMouseDown()
    {
        isTouched = true;
    }

    private void OnMouseDrag()
    {
        if (!isDragStarted && isTouched)
            DragStart();

        if (!IsPointerOverGameObject() &&
            IsSameInteractiveFingerAndLastTouch() &&
            isDragStarted &&
            enabled)
        {
            switch (directionLimit)
            {
                case DirectionLimit.Left:
                    if (!(originPosition.x - GetCurrentTouchPoint().x < -dragRecognitionDistance))
                        return;
                    break;

                case DirectionLimit.Up:
                    if (!(originPosition.y - GetCurrentTouchPoint().y < -dragRecognitionDistance))
                        return;
                    break;

                case DirectionLimit.Right:
                    if (!(originPosition.x - GetCurrentTouchPoint().x > dragRecognitionDistance))
                        return;
                    break;

                case DirectionLimit.Down:
                    if (!(originPosition.y - GetCurrentTouchPoint().y > dragRecognitionDistance))
                        return;
                    break;

                case DirectionLimit.All:
                    if (!(Vector3.SqrMagnitude(GetCurrentTouchPoint() - originPosition) >= (dragRecognitionDistance * dragRecognitionDistance)))
                        return;
                    break;
            }

            onDragging.Invoke(gameObject);

            if (isOnlyOnce)
                DragEnd();
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!IsPointerOverGameObject() &&
            IsSameInteractiveFingerAndLastTouch() &&
            isDragStarted &&
            enabled)
        {
            DragEnd();
            
        }
    }

    private void DragStart()
    {
        if (!IsPointerOverGameObject() &&
            enabled)
        {
            interactiveFingerID = GetLastTouch().fingerId;
            originPosition = GetCurrentTouchPoint();
            isDragStarted = true;

            onDragStarted.Invoke(gameObject);
        }
    }

    private void DragEnd()
    {
        interactiveFingerID = -1;
        isDragStarted = false;
        isTouched     = false;

        onDragEnded.Invoke(gameObject);
    }

    private bool IsPointerOverGameObject()
    {
        if (isInteractiveOverUI)
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

