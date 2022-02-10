using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelManager : MonoBehaviour {
    public int totalLines;
    public GameObject lineObject;

    public int activeLines;

    private Vector2 lineSize;
    public List<GameObject> blocks;
    private List<GameObject> lines;

    private void Start() {
        activeLines = 1;

        blocks = new List<GameObject>();
        // blocks.Add(END);

        float h = lineObject.GetComponent<RectTransform>().sizeDelta.y;
        lineSize = new Vector2(0, -h);

        lines = new List<GameObject>();
        for (int i = 0; i < totalLines; i++) {
            GameObject newLine = Instantiate(lineObject, gameObject.transform);
            lines.Add(newLine);
            newLine.transform.SetSiblingIndex(i);
            newLine.GetComponent<RectTransform>().anchoredPosition += lineSize * i;
            newLine.GetComponent<LineAttach>().Init(i + 1, this);
            newLine.SetActive(false);
        }
        lines[0].SetActive(true);
    }

    public void onBlockEnter(GameObject block, int lineNumber) {
        blocks.Insert(lineNumber - 1, block);

        block.GetComponent<RectTransform>().anchoredPosition = lines[lineNumber - 1].GetComponent<RectTransform>().anchoredPosition;

        lines[activeLines++].SetActive(true);
    }

    public void onBlockExit(GameObject block) {
        int lineNumber = blocks.IndexOf(block);
        if (lineNumber == -1) return;
        blocks.RemoveAt(lineNumber);
        for (int i = lineNumber; i < blocks.Count; i++) {
            blocks[i].GetComponent<RectTransform>().anchoredPosition -= lineSize;
        }
        lines[--activeLines].SetActive(false);
    }

    public void onBlockDragEnter(GameObject block, int lineNumber) {
        Debug.Log($"Bloco {block.name} entrou na linha {lineNumber}");
        for (int i = lineNumber - 1; i < blocks.Count; i++) {
            blocks[i].GetComponent<RectTransform>().anchoredPosition += lineSize;
        }
    }

    public void onBlockDragExit(GameObject block, int lineNumber) {
        Debug.Log($"Bloco {block.name} saiu da linha {lineNumber}");
        for (int i = lineNumber - 1; i < blocks.Count; i++) {
            blocks[i].GetComponent<RectTransform>().anchoredPosition -= lineSize;
        }
    }

    public void onBlockDragOverBlock(GameObject draggingBlock, GameObject bottomBlock) {
        int lineNumber = blocks.IndexOf(bottomBlock);
        if (lineNumber == -1) return;
        onBlockDragEnter(draggingBlock, lineNumber);
    }
}
