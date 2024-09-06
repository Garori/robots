using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndLineController : MonoBehaviour, IDropHandler
{
    // Start is called before the first frame update
    
    
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {   
        if (eventData.pointerDrag != null)
        {
            // Debug.Log(eventData.pointerDrag.tag);
            switch (eventData.pointerDrag.tag)   
            {
                case "ActionBlock":
                    EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
                    break;
                case "ElseBlock":
                    EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
                    break;
                case "StructureBlock":
                    EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
                    break;
                case "EndBlock":
                    EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
                    break;
                case "ComparatorBlock":
                    // EventManager.onComparatorEnter(eventData.pointerDrag.GetComponent<ComparatorController>(), this.gameObject.GetComponent<BlockSlotController>());
                    break;
                case "VariableBlock":
                    // EventManager.onVariableEnter(eventData.pointerDrag.GetComponent<VariableController>(), this.gameObject.GetComponent<BlockSlotController>());
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
