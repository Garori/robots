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
        Debug.Log(comparatorSlot);
        if (comparatorSlot.childBlock == null) return Commands.NONE;
        return comparatorSlot.childBlock.commandName;
    }
    public void OnDrop(PointerEventData eventData)
    {
        // if (eventData.pointerDrag != null)
        // {
        //     // Debug.Log(eventData.pointerDrag.tag);
        //     switch (eventData.pointerDrag.tag)
        //     {
        //         case "ActionBlock":
        //             // EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
        //             break;
        //         case "StructureBlock":
        //             // EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
        //             break;
        //         case "ComparatorBlock":
        //         Debug.Log("entrou no comparator enter");
        //             EventManager.onComparatorEnter(eventData.pointerDrag.GetComponent<ComparatorController>(), comparatorSlot);
        //             break;
        //         case "VariableBlock":
        //             EventManager.onVariableEnter(eventData.pointerDrag.GetComponent<VariableController>(), comparatorSlot);
        //             break;
        //         case "CodeBlock":
        //             // EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
        //             Debug.Log("Código");
        //             break;
        //             // case "ActionBlock":
        //             //     EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
        //             //     break;

        //     }
        //     // Debug.Log(eventData.pointerDrag.GetComponent<BlockController>());
        //     // eventData.pointerDrag.GetComponent<RectTransform>().parent = this.transform.parent.transform;
        // }
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
