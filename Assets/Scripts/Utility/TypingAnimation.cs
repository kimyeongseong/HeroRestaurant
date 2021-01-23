using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class TypingAnimation : MonoBehaviour {
    [SerializeField, Required]
    private TextMeshProUGUI text = null;
    [SerializeField]
    private float delayBetweenChars = 0;

    private int maxVisibleCharacters = 0;

    public bool IsAnimating { get; private set; }

    private void Awake()
    {
        maxVisibleCharacters = text.maxVisibleCharacters;
    }

    private IEnumerator Animation()
    {
        var waitForDelay = new WaitForSeconds(delayBetweenChars);

        for (int i = 0; i <= text.text.Length; i++)
        {
            text.maxVisibleCharacters = i;
            yield return waitForDelay;
        }

        text.maxVisibleCharacters = maxVisibleCharacters;

        IsAnimating = false;
    }

    public void StartAnimation(string text)
    {
        IsAnimating = true;
        this.text.text = text;
        this.text.maxVisibleCharacters = 0;

        StartCoroutine("Animation");
    }

    public void StartAnimation()
    {
        StartAnimation(text.text);
    }

    public void StopAnimation()
    {
        StopCoroutine("Animation");
        IsAnimating = false;
    }

    public void CompleteImmediate()
    {
        StopAnimation();

        text.maxVisibleCharacters = maxVisibleCharacters;
        IsAnimating = false;
    }
}
