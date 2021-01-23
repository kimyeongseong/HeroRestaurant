using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ColorFadeAnimation : MonoBehaviour {
    [SerializeField, Required]
    private SpriteRenderer spriteRenderer = null;
    [SerializeField]
    private bool  isAutoStart = false;
    [SerializeField]
    private float startDelay = 0f;
    [SerializeField]
    private float fadeTime = 0f;
    [SerializeField]
    private Color startColor = Color.white;
    [SerializeField]
    private Color arriveColor = Color.white;

    public ColorFadeAnimtionEvent onCompleted = new ColorFadeAnimtionEvent();

    private void Start()
    {
        if (isAutoStart)
            StartAnimation();
    }

    private void OnEnable()
    {
        if (isAutoStart)
            StartAnimation();
    }

    private IEnumerator Animate()
    {
        float currentTime = 0f;
        float timePoint = 0f;

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        while (timePoint < 1f)
        {
            currentTime += Time.smoothDeltaTime;
            timePoint = currentTime / fadeTime;
            spriteRenderer.color = Color.Lerp(startColor, arriveColor, timePoint);
            yield return null;
        }

        onCompleted.Invoke();
    }

    public void StartAnimation()
    {
        StopCoroutine("Animate");
        StartCoroutine("Animate");
    }

    public void StopAnimation()
    {
        StopCoroutine("Animate");
    }
}
