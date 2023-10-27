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
        EventManager.onVariableExit(this);
    }

    protected override void OnEndDragAction()
    {
        EventManager.onVariableEnter(this, colliding.GetComponent<BlockSlotController>());
    }

    protected override bool OnValidTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("VariableCollider")) return true;
        if (other.CompareTag("ForCondition") && canUseInFor) return true;
        return false;
    }

    protected override bool OnValidTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("VariableCollider")) return true;
        if (other.CompareTag("ForCondition") && canUseInFor) return true;
        return false;
    }
}
