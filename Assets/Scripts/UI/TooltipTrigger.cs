using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour
{
    [SerializeField] private string tooltipText;
    public bool isTooltipEnabled { get; set; }

    private void Start()
    {
        isTooltipEnabled = true;
    }	

    // calls tooltip when mouse hovers over object
    public void OnMouseEnter()
    {
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
