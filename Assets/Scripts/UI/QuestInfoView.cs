using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class QuestInfoView : MonoBehaviour {
    [SerializeField, Required]
    private TextMeshProUGUI questTitleText = null;
    [SerializeField, Required]
    private GameObject[]    starts         = null;

	public void Setup(QuestData questData) {
        questTitleText.text = questData.title;
        for (int i = 0; i < starts.Length; i++)
        {
            if (i < questData.difficulty)
                starts[i].SetActive(true);
            else
                starts[i].SetActive(false);
        }
	}
}
