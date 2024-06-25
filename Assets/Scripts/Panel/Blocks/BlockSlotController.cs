using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSlotController : MonoBehaviour, IDropHandler
{
    public BlockController parentBlock;
    public BlockController childBlock;

    public bool isInPanel { get { return parentBlock.isInPanel; } }

    public bool isOccupied()
    {
        return childBlock != null;
    }

    public void setChildBlock(BlockController childBlock)
    {
        this.childBlock = childBlock;
    }

    public void removeChildBlock()
    {
        childBlock = null;
    }
    public void OnDrop(PointerEventData eventData)
    {   
        Debug.Log("ondrop do slot controller");
        if (eventData.pointerDrag != null)
        {
            // Debug.Log(eventData.pointerDrag.tag);
            switch (eventData.pointerDrag.tag)
            {
                case "ActionBlock":
                    // EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
                    break;
                case "StructureBlock":
                    // EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
                    break;
                case "ComparatorBlock":
                    // Debug.Log("entrou no comparator enter");
                    BlockSlotController slot = gameObject.GetComponent<BlockSlotController>();
                    Debug.Log("" + slot.gameObject.name);
                    EventManager.onComparatorEnter(eventData.pointerDrag.GetComponent<ComparatorController>(), slot);
                    break;
                case "VariableBlock":
                    EventManager.onVariableEnter(eventData.pointerDrag.GetComponent<VariableController>(),  gameObject.GetComponent<BlockSlotController>());
                    break;
                case "CodeBlock":
                    // EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
                    Debug.Log("Código");
                    break;
                    // case "ActionBlock":
                    //     EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
                    //     break;

            }
            // Debug.Log(eventData.pointerDrag.GetComponent<BlockController>());
            // eventData.pointerDrag.GetComponent<RectTransform>().parent = this.transform.parent.transform;
        }
    }
}
