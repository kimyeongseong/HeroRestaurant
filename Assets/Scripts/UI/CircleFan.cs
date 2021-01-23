using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class CircleFan : MonoBehaviour {
    [SerializeField]
    private float angleBetweenChildren = 0f;
    [SerializeField]
    private float faningCompleteTime   = 0f;
    [SerializeField]
    private float radius               = 0f;

    private Transform[] children          = null;
    private Vector3[]   arrivePositions   = null;      

    private void Awake()
    {
        children        = transform.Cast<Transform>().ToArray(); ;
        arrivePositions = new Vector3[children.Length];

        float childHalfCount = children.Length * 0.5f;
        for (int i = 0; i < children.Length; i++)
        {
            float factor = childHalfCount - i;
            float angle = (factor * angleBetweenChildren) - (angleBetweenChildren * 0.5f);
            arrivePositions[i] = AngleToDirZ(-angle) * radius;
        }
    }

    private void OnEnable()
    {
        StartCoroutine("ChildrenMoveToArrivePosition");
    }

    private Vector3 AngleToDirZ(float angleInDegree)
    {
        float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian));
    }

    private IEnumerator ChildrenMoveToArrivePosition()
    {
        float currentTime = 0f;
        float timePoint   = 0f;

        while (timePoint < 1f)
        {
            currentTime += Time.smoothDeltaTime;
            timePoint = currentTime / faningCompleteTime;

            for (int i = 0; i < children.Length; i++)
            {
                children[i].localPosition = Vector3.Lerp(Vector3.zero, arrivePositions[i], timePoint);
            }

            yield return null;
        }
    }
}
