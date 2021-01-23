using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleDatabase;

[RequireComponent(typeof(TypingAnimation))]
public class TextDisplay : Singleton<TextDisplay> {
    private TypingAnimation typingAnimation = null;

    private void Awake()
    {
        typingAnimation = GetComponent<TypingAnimation>();
    }

    public void Show(string text)
    {
        typingAnimation.StopAnimation();
        typingAnimation.StartAnimation(text);
    }

    public void ShowTextFromTable(string key)
    {
        var text= Database.Instance.Select<TextData>("TextDataTable").Select(key);
        Show(text.text);
    }
}
