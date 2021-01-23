using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WellClock : MonoBehaviour {
    [SerializeField]
    private Transform hourHandTransform = null;

    private Animator animator = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameMode.Instance.onBusinessModeStarted.AddListener(StartTickTock);
        GameMode.Instance.onEditorModeStarted.AddListener(StopTickTock);
    }

    public void StartTickTock()
    {
        animator.SetBool("isTickTock", true);
        StartCoroutine("TickTock");
    }

    public void StopTickTock()
    {
        animator.SetBool("isTickTock", false);
        StopCoroutine("TickTock");
    }

    private IEnumerator TickTock()
    {
        while (true)
        {
            float timePoint = 1 - GameMode.Instance.RemainedBusinessTime / GameMode.Instance.BusinessTime;
            hourHandTransform.transform.eulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0f, 0, -360f), timePoint);
            yield return null;
        }
    }
}
