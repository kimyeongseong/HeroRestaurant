using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class ChangedIndexEvent : UnityEvent<GameObject, int>
{

}

public enum ScrollDirection
{
    Left,
    Up,
    Max
}

[RequireComponent(typeof(ScrollRect))]
public class InfinityScrollView : MonoBehaviour {
    [SerializeField]
    private ScrollDirection scrollDirection = ScrollDirection.Left;
    [SerializeField]
    private GameObject    scrollingPrefab   = null;
    [SerializeField]
    private RectOffset    rectOffset        = new RectOffset();
    [SerializeField]
    private float         prefabWidth       = 0f;
    [SerializeField]
    private float         prefabHight       = 0f;
    [SerializeField]
    private int           numOfItemInScreen = 0;
    [SerializeField]
    private int           maxAmount         = 0;
    [SerializeField]
    private RectTransform content           = null;
    [SerializeField]
    private bool          isAutoReset       = true;

    private int  prevIndex = 0;
    private bool isReseted = false;
    private bool isUpdate  = false;

    private ScrollRect scrollRect = null;

    public ChangedIndexEvent onChangedIndex = new ChangedIndexEvent();

    public GameObject[] Childs { get; private set; }

    public int MaxAmount
    {
        get
        {
            return maxAmount;
        }
        set
        {
            if (maxAmount < numOfItemInScreen ||
                value < numOfItemInScreen)
            {
                for (int i = 0; i < numOfItemInScreen; i++)
                {
                    bool isActive = i < value ? true : false;

                    if (!Childs[i].activeSelf && isActive)
                        onChangedIndex.Invoke(Childs[i], i);

                    Childs[i].SetActive(isActive);
                }
            }

            maxAmount = value;
        }
    }

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        Childs = new GameObject[numOfItemInScreen];
        for (int i = 0; i < numOfItemInScreen; i++)
        {
            GameObject newObject = Instantiate(scrollingPrefab);
            newObject.transform.SetParent(content, false);

            Childs[i] = newObject;

            if (i >= maxAmount)
                newObject.SetActive(false);
        }

        ResetPosition();
        RecalculateContentSize();
    }

    private void OnEnable()
    {
        if (isAutoReset)
        {
            StartCoroutine(LazyResetPosition());
            RecalculateContentSize();
        }

        scrollRect.onValueChanged.AddListener(OnValueChange);
    }

    private void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnValueChange);
    }

    private void OnValueChange(Vector2 position)
    {
        if (isReseted)
        {
            isReseted = false;
            return;
        }

        int currentIndex = (int)(-content.localPosition.x / prefabWidth);

        if (currentIndex >= 0 &&
            currentIndex < maxAmount &&
            prevIndex != currentIndex
            )
        {
            int chanedValue = currentIndex > prevIndex ? currentIndex - prevIndex : prevIndex - currentIndex;
            if (chanedValue > numOfItemInScreen)
                RecalculateIndexPositionAll();
            else
            {
                Transform firstChild = null;
                for (int i = 0; i < chanedValue; i++)
                {
                    int childCount = Childs.Length;
                    if (currentIndex > prevIndex)
                    {
                        firstChild = content.GetChild(0);
                        firstChild.localPosition = CalculateIndexPosition(firstChild, prevIndex + i);
                        firstChild.SetAsLastSibling();

                        GameObject temp = Childs[0];
                        System.Array.Copy(Childs, 1, Childs, 0, childCount - 1);
                        Childs[childCount - 1] = temp;

                        onChangedIndex.Invoke(firstChild.gameObject, (prevIndex + i));
                    }
                    else if (currentIndex < prevIndex)
                    {
                        firstChild = content.GetChild(content.childCount - 1);
                        firstChild.localPosition = CalculateIndexPosition(firstChild, prevIndex - 1 - i);
                        firstChild.SetAsFirstSibling();

                        GameObject temp = Childs[childCount - 1];
                        System.Array.Copy(Childs, 0, Childs, 1, childCount - 1);
                        Childs[0] = temp;

                        onChangedIndex.Invoke(firstChild.gameObject, (prevIndex - i - 1));
                    }
                }
            }

            prevIndex = currentIndex;
        }
    }

    private void RecalculateIndexPositionAll()
    {
        int currentIndex = 0;
        switch (scrollDirection)
        {
            case ScrollDirection.Left:
                currentIndex = (int)(-content.anchoredPosition.x / prefabWidth);
                break;

            case ScrollDirection.Up:
                currentIndex = (int)(content.anchoredPosition.y / prefabHight);
                break;
        }

        for (int i = 0; i < content.childCount; i++)
        {
            var child = content.GetChild(i);

            switch (scrollDirection)
            {
                case ScrollDirection.Left:
                    child.localPosition = new Vector2(rectOffset.left + ((currentIndex + i) * prefabWidth), -rectOffset.top + child.localPosition.y);
                    break;

                case ScrollDirection.Up:
                    child.localPosition = new Vector2(rectOffset.left + child.localPosition.x, -rectOffset.top + ((currentIndex + i) * prefabWidth));
                    break;
            }

            onChangedIndex.Invoke(child.gameObject, currentIndex + i);
        }
    }

    private Vector2 CalculateIndexPosition(Transform child, int index)
    {
        float indexPos = 0f;
        switch (scrollDirection)
        {
            case ScrollDirection.Left:
                indexPos = ((numOfItemInScreen - 1) * prefabWidth) + (index * prefabWidth);
                return new Vector2(rectOffset.left + indexPos, -rectOffset.top + child.localPosition.y);

            case ScrollDirection.Up:
                indexPos = ((numOfItemInScreen - 1) * prefabHight) + (index * prefabHight);
                return new Vector2(rectOffset.left + child.localPosition.x, -rectOffset.top + indexPos);

            default:
                return Vector2.zero;
        }
    }

    private IEnumerator LazyResetPosition()
    {
        yield return null;
        ResetPosition();
    }

    public void RecalculateContentSize()
    {
        switch (scrollDirection)
        {
            case ScrollDirection.Left:
                content.sizeDelta = new Vector2((maxAmount * prefabWidth) + rectOffset.right, content.sizeDelta.y);
                break;

            case ScrollDirection.Up:
                content.sizeDelta = new Vector2(content.sizeDelta.x, (maxAmount * prefabHight) + rectOffset.bottom);
                break;
        }
    }
    
    public void ResetPosition()
    {
        isReseted = true;

        content.localPosition = new Vector2(0, content.localPosition.y);
        for (int i = 0; i < Childs.Length; i++)
        {
            var child = Childs[i].transform;
            switch (scrollDirection)
            {
                case ScrollDirection.Left:
                    child.localPosition = new Vector2(rectOffset.left + (i * prefabWidth), -rectOffset.top + child.localPosition.y);
                    break;

                case ScrollDirection.Up:
                    child.localPosition = new Vector2(rectOffset.left + child.localPosition.x, -rectOffset.top + (i * prefabHight));
                    break;
            }
        }
    }
}
