using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour
{
    [TextArea] public string tooltipText;
    public bool isTooltipEnabled { get; set; }

    private void Start()
    {
        // Debug.Log("start tooltiptrigger");
        isTooltipEnabled = true;
    }

    private void Awake()
    {
        // Debug.Log("awake tooltiptrigger");
        isTooltipEnabled = true;
    }

    // calls tooltip when mouse hovers over object
    public void OnMouseEnter()
    {
        // Debug.Log("entrou mouse");
        if (!isTooltipEnabled) return;
        ShowTooltip();
    }

    public void OnMouseExit()
    {
        HideTooltip();
    }

    public void ShowTooltip()
    {
        Tooltip.instance.ShowTooltip(tooltipText);
    }

    public void HideTooltip()
    {
        Tooltip.instance.HideTooltip();
    }
}
