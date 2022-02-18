using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour {
    [Header("Game Objects")]
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform linesContent;

    public GameObject lineObject;
    public GameObject endLineObject;

    [Header("Padding")]
    [SerializeField] private int minPadding;
    [SerializeField] private int tabPadding;

    private int activeLines;
    private List<GameObject> lines;
    private List<GameObject> blocks;

    private RectOffset linePadding;
    private List<HorizontalLayoutGroup> linesLayout;

    private void Awake() {
        activeLines = 0;

        blocks = new List<GameObject>();
        lines = new List<GameObject>();
        linesLayout = new List<HorizontalLayoutGroup>();
    }

    private void Start() {
        RectOffset lineObjectPadding = lineObject.GetComponent<HorizontalLayoutGroup>().padding;
        linePadding = new RectOffset(
            lineObjectPadding.left,
            lineObjectPadding.right,
            lineObjectPadding.top,
            lineObjectPadding.bottom
        );

        EventManager.BlockEnter += InsertBlock;
        EventManager.BlockExit += RemoveBlock;
    }

    private void InsertBlock(GameObject block, GameObject line) {
        int index = line.Equals(endLineObject) ? activeLines : lines.IndexOf(line);
        Debug.Log(index);

        blocks.Insert(index, block);

        GameObject newLine = Instantiate(lineObject, linesContent);
        lines.Insert(index, newLine);
        linesLayout.Insert(index, newLine.GetComponent<HorizontalLayoutGroup>());
        newLine.transform.SetSiblingIndex(index);

        block.GetComponent<RectTransform>().SetParent(newLine.GetComponent<RectTransform>());

        activeLines++;

        OrganizeBlocks();
    }

    private void RemoveBlock(GameObject block) {
        int index = blocks.IndexOf(block);
        if (index == -1) return;
        Debug.Log($"Removendo indice {index}");

        blocks.RemoveAt(index);
        block.GetComponent<RectTransform>().SetParent(canvas);

        GameObject line = lines[index];
        lines.RemoveAt(index);
        linesLayout.RemoveAt(index);

        Destroy(line);
        activeLines--;

        OrganizeBlocks();
    }

    private void OrganizeBlocks() {
        int leftPadding = minPadding;
        for (int i = 0; i < lines.Count; i++) {
            HorizontalLayoutGroup line = linesLayout[i];

            if (blocks[i].CompareTag("ElseBlock") | blocks[i].CompareTag("EndBlock") && leftPadding > minPadding) leftPadding -= tabPadding;

            RectOffset padding = new RectOffset(
                leftPadding,
                linePadding.right,
                linePadding.top,
                linePadding.bottom
            );
            line.padding = padding;

            if (blocks[i].CompareTag("ElseBlock") | blocks[i].CompareTag("StructureBlock")) leftPadding += tabPadding;
        }
    }
}
