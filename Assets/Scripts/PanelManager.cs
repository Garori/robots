using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelManager : MonoBehaviour {
    [Header("Game Objects")]
    private RectTransform rectTransform;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform linesContent;
    public GameObject lineObject;
    public GameObject colliderObject;

    private int activeLines;
    private List<GameObject> lines;
    private List<GameObject> blocks;

    private void Start() {
        activeLines = 0;

        rectTransform = GetComponent<RectTransform>();

        blocks = new List<GameObject>();
        lines = new List<GameObject>();
        // blocks.Add(END);
    }

    public void onBlockEnter(GameObject block, int siblingIndex) {
        int index = siblingIndex / 2;
        blocks.Insert(index, block);

        GameObject newCollider = Instantiate(colliderObject, linesContent);
        newCollider.transform.SetSiblingIndex(siblingIndex);

        GameObject newLine = Instantiate(lineObject, linesContent);
        lines.Insert(index, newLine);
        newLine.transform.SetSiblingIndex(siblingIndex + 1);

        block.GetComponent<RectTransform>().SetParent(newLine.GetComponent<RectTransform>());

        activeLines++;
    }

    public void onBlockExit(GameObject block) {
        int index = blocks.IndexOf(block);
        if (index == -1) return;
        blocks.RemoveAt(index);
        Debug.Log($"Removendo indice {index}");

        block.GetComponent<RectTransform>().SetParent(canvas);

        Destroy(linesContent.GetChild(2 * index + 1).gameObject);
        Destroy(linesContent.GetChild(2 * index).gameObject);
    }
}
