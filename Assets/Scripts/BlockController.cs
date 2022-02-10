using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public PanelManager panelManager;

    public GameObject hover;
    public GameObject lastHover;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    [SerializeField] private Canvas canvas;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData) {

    }

    public void OnBeginDrag(PointerEventData eventData) {
        canvasGroup.alpha = .8f;
        canvasGroup.blocksRaycasts = false;

        panelManager.onBlockExit(gameObject);
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        lastHover = hover;
        hover = eventData.pointerEnter;
        if (hover == lastHover) return;
        if (hover != null && hover.CompareTag("Block")) {
            Debug.Log($"Primeiro frame no bloco {hover.name}");
            panelManager.onBlockDragOverBlock(gameObject, hover);
            return;
        }
        if ((lastHover == null || !lastHover.CompareTag("Line")) && hover != null && hover.CompareTag("Line")) panelManager.onBlockDragEnter(gameObject, hover.GetComponent<LineAttach>().lineNumber);
        if (lastHover != null && lastHover.CompareTag("Line") && (hover == null || !hover.CompareTag("Line"))) panelManager.onBlockDragExit(gameObject, lastHover.GetComponent<LineAttach>().lineNumber);
        if (lastHover == null || hover == null) return;
        if (lastHover.CompareTag("Line") && hover.CompareTag("Line")) {
            panelManager.onBlockDragExit(gameObject, lastHover.GetComponent<LineAttach>().lineNumber);
            panelManager.onBlockDragEnter(gameObject, hover.GetComponent<LineAttach>().lineNumber);
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (hover == null || !hover.CompareTag("Line")) {
            //Debug.Log("Soltou no nada");
            return;
        }
        //Debug.Log($"Soltou na linha {hover.GetComponent<LineAttach>().lineNumber}");
    }

}
