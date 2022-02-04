using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public int blockId;

    private RectTransform rectTransform;
    private Canvas canvas;
    private int glueBlock;

    public GameObject topBlock;
    public GameObject bottomBlock;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        topBlock = null;
        bottomBlock = null;
    }

    private void Start() {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData) {

    }

    public void OnBeginDrag(PointerEventData eventData) {
        topBlock = null;
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        adjustBlock();
    }

    public void adjustBlock() {
        Debug.Log("Adjusting block");
        if (topBlock != null) {
            rectTransform.anchoredPosition = topBlock.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, -32f);
        }
        if (bottomBlock == null) return;
        Debug.Log(bottomBlock.GetComponent<RectTransform>().anchoredPosition);
        bottomBlock.GetComponent<BlockController>().adjustBlock();
    }

    public void OnChildCollisionEnter(GameObject child, Collision2D other) {
        if (!child.CompareTag(other.gameObject.tag)) {
            BlockController otherScript = other.gameObject.GetComponent<BlockController>();
            if (child.CompareTag("TopCollider")) {
                if (otherScript.bottomBlock != null) return;
                otherScript.bottomBlock = gameObject;
                topBlock = other.gameObject;
            }
            if (child.CompareTag("BottomCollider")) {
                if (otherScript.topBlock != null) return;
                otherScript.topBlock = gameObject;
                bottomBlock = other.gameObject;
            }
        }
    }

    public void OnChildCollisionExit(GameObject child, Collision2D other) {
        if (child.CompareTag("TopCollider")) {
            topBlock = null;
        }
        if (child.CompareTag("BottomCollider")) {
            bottomBlock = null;
        }
    }
}
