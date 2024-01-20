using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StructureController : BlockController
{
    public BlockSlotController comparatorSlot;
    protected override void OnBeginDragAction()
    {
        EventManager.onBlockExit(this);
    }

    protected override void OnEndDragAction()
    {
        EventManager.onBlockEnter(this, colliding);
    }

    protected override bool OnValidTriggerEnter2D(Collider2D other)
    {
        return other.CompareTag("Line");
    }

    protected override bool OnValidTriggerExit2D(Collider2D other)
    {
        return other.CompareTag("Line");
    }

    public Commands GetComparatorCommand()
    {
        if (comparatorSlot.childBlock == null) return Commands.NONE;
        return comparatorSlot.childBlock.commandName;
    }

    public Commands GetVariable1Command()
    {
        ComparatorController comparator = comparatorSlot.childBlock as ComparatorController;
        if (comparator == null) return Commands.NONE;
        return comparator.GetVariable1Command();
    }

    public Commands GetVariable2Command()
    {
        ComparatorController comparator = comparatorSlot.childBlock as ComparatorController;
        if (comparator == null) return Commands.NONE;
        return comparator.GetVariable2Command();
    }
}
