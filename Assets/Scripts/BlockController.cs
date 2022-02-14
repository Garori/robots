using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("Game Objects")]
    [SerializeField] private PanelManager panelManager;
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

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
        panelManager.onBlockExit(gameObject);
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.alpha = 1f;
        if (touchingLine == -1) {
            Debug.Log("Soltou no nada");
            return;
        }
        Debug.Log($"Soltou na linha {touchingLine}");
        panelManager.onBlockEnter(gameObject, touchingLine);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"Entrou na linha {other.name}");
        if (!other.CompareTag("BetweenLine")) return;
        touchingLine = other.GetComponent<RectTransform>().GetSiblingIndex();
    }

    private void OnTriggerExit2D(Collider2D other) {
        Debug.Log($"Saiu da linha {other.name}");
        touchingLine = -1;
    }
}
