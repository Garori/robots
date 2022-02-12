using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelManager : MonoBehaviour {
    public int totalLines;
    public GameObject lineObject;
    private List<GameObject> lines;

    public int activeLines;
    private List<GameObject> blocks;

    private Vector2 lineColliderSize;
    private Vector2 lineColliderOffset;

    private Vector2 lineSizeY;

    [Header("Small Collider Size")]
    [SerializeField] private float smallColliderOffsetY;
    [SerializeField] private float smallColliderSizeY;
    private Vector2 smallColliderOffset;
    private Vector2 smallColliderSize;

    [Header("Big Collider Size")]
    [SerializeField] private float bigColliderOffsetY;
    [SerializeField] private float bigColliderSizeY;
    private Vector2 bigColliderOffset;
    private Vector2 bigColliderSize;

    private void Start() {
        activeLines = 1;

        BoxCollider2D lineObjectCollider = lineObject.GetComponent<BoxCollider2D>();
        lineColliderSize = lineObjectCollider.size;
        lineColliderOffset = lineObjectCollider.offset;

        blocks = new List<GameObject>();
        // blocks.Add(END);

        float lineHeight = lineObject.GetComponent<RectTransform>().sizeDelta.y;
        lineSizeY = new Vector2(0, -lineHeight);

        lines = new List<GameObject>();
        for (int i = 0; i < totalLines; i++) {
            GameObject newLine = Instantiate(lineObject, gameObject.transform);
            lines.Add(newLine);
            newLine.transform.SetSiblingIndex(i);
            newLine.GetComponent<RectTransform>().anchoredPosition += lineSizeY * i;
            newLine.GetComponent<LineAttach>().Init(i + 1, this);
            newLine.SetActive(false);
        }
        lines[0].SetActive(true);

        smallColliderOffset = new Vector2(lineColliderOffset.x, smallColliderOffsetY);
        smallColliderSize = new Vector2(lineColliderSize.x, smallColliderSizeY);
        bigColliderOffset = new Vector2(lineColliderOffset.x, bigColliderOffsetY);
        bigColliderSize = new Vector2(lineColliderSize.x, bigColliderSizeY);


        BoxCollider2D lineCollider = lines[0].GetComponent<BoxCollider2D>();
        setColliderBig(ref lineCollider);
    }

    public void onBlockEnter(GameObject block, int lineNumber) {
        blocks.Insert(lineNumber - 1, block);

        block.GetComponent<RectTransform>().anchoredPosition = lines[lineNumber - 1].GetComponent<RectTransform>().anchoredPosition;

        GameObject newLine = lines[activeLines];
        GameObject oldLine = lines[activeLines - 1];
        activeLines++;

        newLine.SetActive(true);
        BoxCollider2D newLineCollider = newLine.GetComponent<BoxCollider2D>();
        setColliderBig(ref newLineCollider);

        BoxCollider2D oldLineCollider = oldLine.GetComponent<BoxCollider2D>();
        setColliderSmall(ref oldLineCollider);
    }

    public void onBlockExit(GameObject block) {
        int lineId = blocks.IndexOf(block);
        if (lineId == -1) return;
        blocks.RemoveAt(lineId);
        onBlockDragExit(block, lineId + 1);

        activeLines--;
        GameObject newLine = lines[activeLines];
        newLine.SetActive(false);

        GameObject oldLine = lines[activeLines - 1];
        BoxCollider2D oldLineCollider = oldLine.GetComponent<BoxCollider2D>();
        setColliderBig(ref oldLineCollider);
    }

    public void onBlockDragEnter(GameObject block, int lineNumber) {
        Debug.Log($"Bloco {block.name} entrou na linha {lineNumber}");
        for (int i = lineNumber - 1; i < blocks.Count; i++) {
            blocks[i].GetComponent<RectTransform>().anchoredPosition += lineSizeY;
        }
    }

    public void onBlockDragExit(GameObject block, int lineNumber) {
        Debug.Log($"Bloco {block.name} saiu da linha {lineNumber}");
        for (int i = lineNumber - 1; i < blocks.Count; i++) {
            blocks[i].GetComponent<RectTransform>().anchoredPosition -= lineSizeY;
        }
    }

    private void setColliderSmall(ref BoxCollider2D collider2D) {
        collider2D.size = smallColliderSize;
        collider2D.offset = smallColliderOffset;
    }

    private void setColliderBig(ref BoxCollider2D collider2D) {
        collider2D.size = bigColliderSize;
        collider2D.offset = bigColliderOffset;
    }
}
