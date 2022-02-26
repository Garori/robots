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
    private BoxCollider2D boxCollider2D;

    private float scaledWidth;

    protected GameObject colliding;
    private bool newBlock;

    [Header("Enum")]
    public Commands commandName;

    private void Start() {
        newBlock = true;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        scaledWidth = rectTransform.sizeDelta.x * rectTransform.localScale.x / 2f;
    }

    public void OnPointerDown(PointerEventData eventData) {

    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (newBlock) {
            GameObject createdBlock = Instantiate(gameObject, gameObject.transform.parent);
            createdBlock.name = gameObject.name;
            createdBlock.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex());
            //gameObject.transform.SetSiblingIndex(gameObject.transform.parent.childCount - 1);
            newBlock = false;
        }
        canvasGroup.alpha = .8f;
        OnBeginDragAction();
    }

    protected virtual void OnBeginDragAction() {

    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        boxCollider2D.enabled = (rectTransform.anchoredPosition.x + scaledWidth) >= panelManager.panelX;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.alpha = 1f;
        if (colliding == null) {
            Destroy(gameObject);
            return;
        }
        OnEndDragAction();
    }

    protected virtual void OnEndDragAction() {

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!OnValidTriggerEnter2D(other)) return;
        colliding = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!OnValidTriggerExit2D(other)) return;
        colliding = null;
    }

    protected virtual bool OnValidTriggerEnter2D(Collider2D other) {
        return true;
    }

    protected virtual bool OnValidTriggerExit2D(Collider2D other) {
        return true;
    }
}
