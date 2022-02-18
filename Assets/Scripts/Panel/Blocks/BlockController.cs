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

    private GameObject colliding;
    private bool newBlock;

    private void Start() {
        newBlock = true;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData) {

    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (newBlock) {
            GameObject createdBlock = Instantiate(gameObject, gameObject.transform.parent);
            createdBlock.name = gameObject.name;
            gameObject.transform.SetSiblingIndex(gameObject.transform.parent.childCount - 1);
            newBlock = false;
        }
        canvasGroup.alpha = .8f;
        EventManager.onBlockExit(gameObject);
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.alpha = 1f;
        if (colliding == null) {
            Destroy(gameObject);
            return;
        }
        EventManager.onBlockEnter(gameObject, colliding);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Line")) return;
        colliding = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!other.CompareTag("Line")) return;
        colliding = null;
    }
}
