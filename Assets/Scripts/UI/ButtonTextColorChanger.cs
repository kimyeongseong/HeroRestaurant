using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ButtonTextColorChanger : MonoBehaviour {
    [SerializeField]
    private Color normalColor = Color.white;
    [SerializeField]
    private Color disableColor = Color.white;

    private Button          parentButton = null;
    private TextMeshProUGUI text         = null;

    private bool prevButtonInteractable = false;

    private void Awake()
    {
        text              = GetComponent<TextMeshProUGUI>();
        parentButton      = GetComponentInParent<Button>();

        ColorChange(parentButton.interactable);
    }

    public void Update()
    {
        if (parentButton.interactable != prevButtonInteractable)
        {
            ColorChange(parentButton.interactable);
        }
    }

    private void ColorChange(bool isEnable)
    {
        if (isEnable)
            text.color = normalColor;
        else
            text.color = disableColor;

        prevButtonInteractable = isEnable;
    }
}
