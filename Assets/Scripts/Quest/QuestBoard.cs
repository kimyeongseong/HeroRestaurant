using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using SimpleDatabase;
using Sirenix.OdinInspector;

[RequireComponent(typeof(QuestSystem), typeof(TouchInteraction))]
public class QuestBoard : MonoBehaviour, IUsable
{
    public GameObject a;
    public Sprite b;
    public Sprite c;
    private class QuestGenerationHandler
    {
        private QuestData generationTargetData;
        private float currentTime = 0f;
        public QuestEvent onCompleted = new QuestEvent();
        public bool IsCompleted { get; private set; }

        public QuestGenerationHandler(QuestData generationTargetData)
        {
            this.generationTargetData = generationTargetData;
        }

        private void OnCompleted()
        {
            IsCompleted = true;

            var generationQuest = new Quest(generationTargetData);
            onCompleted.Invoke(generationQuest);
        }

        public void Update()
        {
            if (!IsCompleted)
            {
                currentTime += Time.smoothDeltaTime;
                if (currentTime >= generationTargetData.generationDelayTime)
                    OnCompleted();
            }
        }

        public void Reset()
        {
            currentTime = 0f;
            IsCompleted = false;
        }
    }

    private Dictionary<string, QuestGenerationHandler> questGenerationHandlerDic = null;
    private List<Quest> generatedQuests = null;

    private QuestSystem quesySystem = null;

    private void Awake()
    {
        quesySystem = GetComponent<QuestSystem>();
    }

    private void Start()
    {
        QuestData[] questDatas = Database.Instance.Select<QuestData>("QuestDataTable").Rows;

        questGenerationHandlerDic = new Dictionary<string, QuestGenerationHandler>(questDatas.Length);
        generatedQuests = new List<Quest>(questDatas.Length);

        foreach (var questData in questDatas)
        {
            var questGenerationHandler = new QuestGenerationHandler(questData);
            questGenerationHandler.onCompleted.AddListener(OnQuestGenerated);

            questGenerationHandlerDic.Add(questData.title, questGenerationHandler);
        }

        GameMode.Instance.onBusinessModeStarted.AddListener(StartHandlerUpdate);
        GameMode.Instance.onEditorModeStarted.AddListener(StopHandlerUpdate);
    }

    private void OnQuestGenerated(Quest quest)
    {
        generatedQuests.Add(quest);
    }

    private void StartHandlerUpdate()
    {
        StartCoroutine("HandlerUpdate");
    }

    private void StopHandlerUpdate()
    {
        StopCoroutine("HandlerUpdate");
        foreach (var handler in questGenerationHandlerDic)
        {
            handler.Value.Reset();
        }
    }

    private IEnumerator HandlerUpdate()
    {
        while (true)
        {
            foreach (var handler in questGenerationHandlerDic)
            {
                handler.Value.Update();
            }

            yield return null;
        }
    }

    private void StartQuestByRandom()
    {
        generatedQuests.OrderBy(item => item.QuestData.selectProbability);

        float totalProbability = generatedQuests.Sum(item => item.QuestData.selectProbability);
        float currentProbability = 0f;
        float probability = Random.Range(0f, 1f);

        Quest selectedQuest = null;

        foreach (var quest in generatedQuests)
        {
            currentProbability += quest.QuestData.selectProbability;
            if ((currentProbability / totalProbability) >= probability)
            {
                selectedQuest = quest;
                break;
            }
        }

        quesySystem.AddQuest(selectedQuest);
        string questTitle = selectedQuest.QuestData.title;
        questGenerationHandlerDic[questTitle].Reset();
    }
    public void ani()
    {
    }
    public void Use(GameObject user)
    {
        StartQuestByRandom();
    }

    public void Unuse(GameObject user)
    {

    }
}
