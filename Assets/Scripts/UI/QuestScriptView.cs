using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using SimpleDatabase;
using UnityEngine.Events;

[System.Serializable]
public class HiddenQuestViewEvent : UnityEvent
{
}

[RequireComponent(typeof(Button), typeof(TypingAnimation))]
public class QuestScriptView : MonoBehaviour {
    [SerializeField, Required]
    private TextMeshProUGUI nameText = null;

    private TypingAnimation typingAnimation = null;
    private ScriptData[]    scriptDatas;

    private int currentScriptIndex = 0;

    public HiddenQuestViewEvent onScriptShowingCompleted = new HiddenQuestViewEvent();

    public void Awake()
    {
        typingAnimation = GetComponent<TypingAnimation>();
        GetComponent<Button>().onClick.AddListener(ShowingScriptText);
    }

    public void Show(QuestScriptData questScriptData)
    {
        transform.parent.gameObject.SetActive(true);

        scriptDatas = questScriptData.scriptDatas;

        currentScriptIndex = 0;
        ShowingScriptText();

    }

    public void ShowingScriptText()
    {
        if (typingAnimation.IsAnimating)
        {
            typingAnimation.CompleteImmediate();
            return;
        }

        if (currentScriptIndex == scriptDatas.Length)
        {
            onScriptShowingCompleted.Invoke();
            gameObject.SetActive(false);
            return;
        }

        nameText.text = scriptDatas[currentScriptIndex].talkerName;
        typingAnimation.StartAnimation(scriptDatas[currentScriptIndex].script);
        currentScriptIndex++;
    }
}
