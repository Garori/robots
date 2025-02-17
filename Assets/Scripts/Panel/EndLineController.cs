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
        // print("chamou o pai");
        if (eventData.pointerDrag != null)
        {
            Debug.Log("ondrop endline "+eventData.pointerDrag.tag);
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
                case "BreakBlock":
                    EventManager.onBlockEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject);
                    break;
                // case "ComparatorBlock":
                //     // EventManager.onComparatorEnter(eventData.pointerDrag.GetComponent<ComparatorController>(), this.gameObject.GetComponent<BlockSlotController>());
                //     break;
                // case "VariableBlock":
                //     // EventManager.onVariableEnter(eventData.pointerDrag.GetComponent<VariableController>(), this.gameObject.GetComponent<BlockSlotController>());
                //     break;
                case "CodeBlock":
                    EventManager.onCodeEnter(eventData.pointerDrag.GetComponent<BlockController>(), this.gameObject,codeWithin: eventData.pointerDrag.GetComponent<CodeController>().CodeWithin);
                    Debug.Log("Código");
                    break;
                case "CodeInputBlock":
                    EventManager.onCodeEnter(eventData.pointerDrag.gameObject.transform.GetChild(0).gameObject.GetComponent<BlockController>(), this.gameObject,codeWithin: eventData.pointerDrag.GetComponent<CodeInputController>().CodeWithin);
                    Debug.Log("Código");
                    break;
                default:
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
