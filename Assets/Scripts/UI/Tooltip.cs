using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;

    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        HideTooltip();
    }

    void Update()
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f;
        tooltipRect.position = Camera.main.ScreenToWorldPoint(screenPoint) + offset;
    }

    private void SetText(string text)
    {
        tooltipText.SetText(text);
        tooltipText.ForceMeshUpdate();

        Vector2 textSize = tooltipText.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(8, 8);

        tooltipRect.sizeDelta = textSize + paddingSize;
    }

    public void ShowTooltip(string text)
    {
        this.gameObject.SetActive(true);
        SetText(text);
    }

    public void HideTooltip()
    {
        this.gameObject.SetActive(false);
    }
}
