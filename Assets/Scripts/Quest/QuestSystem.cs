using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class QuestSystemEvent : UnityEvent<Quest>
{
}

[System.Serializable]
public class QuestListUpdateEvent : UnityEvent<Quest[]>
{
}

public class QuestSystem : MonoBehaviour, ISavable {
    private List<Quest> startedQuests           = new List<Quest>();
    private List<Quest> completedQuests         = new List<Quest>();
    private List<Quest> removeReservationQuests = new List<Quest>();

    public QuestSystemEvent     onQuestCompleted  = new QuestSystemEvent();
    public QuestListUpdateEvent onQuestListUpdate = new QuestListUpdateEvent();

    public Quest[] StartedQuests { get { return startedQuests.ToArray(); } }

    private void Update()
    {
        foreach (var quest in startedQuests)
        {
            quest.Update();
        }

        RemoveCompletedQuests();
    }

    private void RemoveCompletedQuests()
    {
        foreach (var quest in removeReservationQuests)
        {
            RemoveQuest(quest);
            onQuestCompleted.Invoke(quest);
        }

        removeReservationQuests.Clear();
    }

    private void OnQuestCompleted(Quest completedQuest)
    {
        completedQuests.Add(completedQuest);
        removeReservationQuests.Add(completedQuest);
    }

    public void AddQuest(Quest quest)
    {
        quest.onCompleted.AddListener(OnQuestCompleted);

        startedQuests.Add(quest);

        onQuestListUpdate.Invoke(startedQuests.ToArray());
    }

    public void RemoveQuest(Quest quest)
    {
        quest.onCompleted.RemoveListener(OnQuestCompleted);

        startedQuests.Remove(quest);

        onQuestListUpdate.Invoke(startedQuests.ToArray());
    }
 
    public Quest[] GetCompletedQuestsOfType(QuestType questType)
    {
        return completedQuests.Where(quest => quest.QuestData.questType == questType)
                              .ToArray();
        
    }

    public JSONObject SaveToJson()
    {
        var root = new JSONObject(JSONObject.Type.OBJECT);

        root.SetField("startedQuests", new JSONObject(JSONObject.Type.ARRAY));
        foreach (var startedQuest in startedQuests)
            root["startedQuests"].Add(startedQuest.SaveToJson());

        root.SetField("completedQuests", new JSONObject(JSONObject.Type.ARRAY));
        foreach (var completedQuest in completedQuests)
            root["completedQuests"].Add(completedQuest.SaveToJson());

        return root;
    }

    public void LoadFromJson(JSONObject root)
    {
        for (int i = 0; i < root["startedQuests"].Count; i++)
            startedQuests.Add(new Quest(root["startedQuests"][i]));

        for (int i = 0; i < root["completedQuests"].Count; i++)
            completedQuests.Add(new Quest(root["completedQuests"][i]));
    }
}
