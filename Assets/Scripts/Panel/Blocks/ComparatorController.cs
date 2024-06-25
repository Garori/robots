using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComparatorController : BlockController
{
    public BlockSlotController structureSlot;
    public BlockSlotController variableSlot1;
    public BlockSlotController variableSlot2;
    protected override void OnBeginDragAction()
    {
        EventManager.onComparatorExit(this);
    }

    protected override void OnEndDragAction()
    {
        EventManager.onComparatorEnter(this, colliding.GetComponent<BlockSlotController>());
    }

    protected override bool OnValidTriggerEnter2D(Collider2D other)
    {
        return other.CompareTag("ComparatorCollider");
    }

    protected override bool OnValidTriggerExit2D(Collider2D other)
    {
        return other.CompareTag("ComparatorCollider");
    }

    public Commands GetVariable1Command()
    {
        if (variableSlot1.childBlock == null) return Commands.NONE;
        return variableSlot1.childBlock.commandName;
    }

    public Commands GetVariable2Command()
    {
        if (variableSlot2.childBlock == null) return Commands.NONE;
        return variableSlot2.childBlock.commandName;
    }


}
