using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum QuestType
{
    Normal,
    Hidden
}

[System.Serializable]
public struct QuestData
{
    public string title;
    public QuestType questType;
    public int rewardMoney;
    public float generationDelayTime;
    public int difficulty;
    public float selectProbability;
    public float completeDelayTime;
}
