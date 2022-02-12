using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineAttach : MonoBehaviour, IDropHandler {
    private RectTransform rectTransform;

    public int lineNumber;
    public PanelManager panelManager;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(int lineNumber, PanelManager panelManager) {
        this.lineNumber = lineNumber;
        this.panelManager = panelManager;
    }

    public void OnDrop(PointerEventData eventData) {
        return;
        if (eventData.pointerDrag == null) return;

        panelManager.onBlockEnter(eventData.pointerDrag.gameObject, lineNumber);
    }
}
