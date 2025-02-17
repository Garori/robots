using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VariableController : BlockController
{
    public BlockSlotController blockSlot;
    public bool canUseInFor;
    protected override void OnBeginDragAction()
    {
        Debug.Log("OnBeginDragAction");
        TooltipTrigger TooltipTrigger = GetComponent<TooltipTrigger>();
        TooltipTrigger.isTooltipEnabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        EventManager.onVariableExit(this);
    }

    protected override void OnEndDragAction()
    {
        Debug.Log("OnEndDragAction");
        TooltipTrigger TooltipTrigger = GetComponent<TooltipTrigger>();
        TooltipTrigger.isTooltipEnabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        // EventManager.onVariableEnter(this, colliding.GetComponent<BlockSlotController>());
    }

    protected override bool OnValidTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnValidTriggerEnter2D");
        if (other.CompareTag("VariableCollider")) return true;
        if (other.CompareTag("ForCondition") && canUseInFor) return true;
        return false;
    }

    protected override bool OnValidTriggerExit2D(Collider2D other)
    {
        Debug.Log("OnValidTriggerExit2D");
        if (other.CompareTag("VariableCollider")) return true;
        if (other.CompareTag("ForCondition") && canUseInFor) return true;
        return false;
    }
}
