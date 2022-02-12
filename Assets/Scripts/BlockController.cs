using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public PanelManager panelManager;

    public GameObject hover;
    //public GameObject lastHover;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    [SerializeField] private Canvas canvas;

    private int touchingLine;

    private void Start() {
        touchingLine = -1;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData) {

    }

    public void OnBeginDrag(PointerEventData eventData) {
        canvasGroup.alpha = .8f;
        //canvasGroup.blocksRaycasts = false;

        panelManager.onBlockExit(gameObject);
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //lastHover = hover;
        hover = eventData.pointerEnter;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.alpha = 1f;
        if (touchingLine == -1) {
            Debug.Log("Soltou no nada");
            return;
        }
        Debug.Log($"Soltou na linha {touchingLine}");
        /*//canvasGroup.blocksRaycasts = true;
        if (hover == null || !hover.CompareTag("Line")) {
            Debug.Log("Soltou no nada");
            return;
        }
        // panelmanager.blocoentrounalinha(hover);
        Debug.Log($"Soltou na linha {hover.GetComponent<LineAttach>().lineNumber}");*/
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"Entrou na linha {other.name}");
        touchingLine = other.GetComponent<LineAttach>().lineNumber;
    }

    private void OnTriggerExit2D(Collider2D other) {
        Debug.Log($"Saiu da linha {other.name}");
        touchingLine = -1;
    }
}
