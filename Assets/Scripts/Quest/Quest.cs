using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SimpleDatabase;

[System.Serializable]
public class QuestEvent : UnityEvent<Quest>
{
}

public class Quest {
    private QuestData questData;

    private float currentTime = 0f;

    public QuestEvent onCompleted = new QuestEvent();

    public QuestData QuestData
    {
        get
        {
            var readonlyData = questData;
            return readonlyData;
        }
    }
    public bool IsCompleted { get; private set; }

    public Quest(string title)
    {
        questData = Database.Instance.Select<QuestData>("QuestDataTable").Select(title);
    }

    public Quest(QuestData questData)
    {
        this.questData = questData;
    }

    public Quest(JSONObject root)
    {
        questData   = Database.Instance.Select<QuestData>("QuestDataTable").Select(root["title"].str);
        currentTime = root["currentTime"].f;
        IsCompleted = root["IsCompleted"].b;
    }

    private void OnCompleted()
    {
        IsCompleted = true;
        Inventory.Instance.IncreaseMoney(questData.rewardMoney);       
        onCompleted.Invoke(this);
    }

    public void Update()
    {
        if (!IsCompleted)
        {
            currentTime += Time.smoothDeltaTime;
            if (currentTime >= questData.completeDelayTime)
                OnCompleted();
        }
    }

    public JSONObject SaveToJson()
    {
        var root = new JSONObject(JSONObject.Type.OBJECT);
        root.SetField("title", questData.title);
        root.SetField("currentTime", currentTime);
        root.SetField("IsCompleted", IsCompleted);

        return root;
    }
}
