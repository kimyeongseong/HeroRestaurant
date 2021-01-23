using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using SimpleDatabase;

public class QuestListScrollView : MonoBehaviour {
    [SerializeField, Required]
    private QuestSystem   questSystem     = null;
    [SerializeField, Required]
    private GameObject    questViewPrefab = null;
    [SerializeField, Required]
    private RectTransform content         = null;

    private List<GameObject> items = new List<GameObject>();

    private void Awake()
    {
        var questCount = Database.Instance.Select<QuestData>("QuestDataTable").Rows.Length;
        for (int i = 0; i < questCount; i++)
        {
            var questViewObj = Instantiate(questViewPrefab, content);
            items.Add(questViewObj);

            questViewObj.SetActive(false);
        }

        questSystem.onQuestListUpdate.AddListener(ListUpdate);
        questSystem.GetComponent<TouchInteraction>().onClicked.AddListener((target) =>
        {
            gameObject.SetActive(!gameObject.activeSelf);
        });

        gameObject.SetActive(false);
    }

    public void ListUpdate(Quest[] quests)
    {
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];

            if (i < quests.Length)
            {
                item.SetActive(true);
                item.GetComponent<QuestInfoView>().Setup(quests[i].QuestData);
            }
            else
                item.SetActive(false);
        }
    }
}
