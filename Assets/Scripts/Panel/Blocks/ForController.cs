using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForController : BlockController
{
    public BlockSlotController variableSlot;
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

    public Commands GetVariableCommand()
    {
        if (variableSlot.childBlock == null) return Commands.NONE;
        return variableSlot.childBlock.commandName;
    }
}
