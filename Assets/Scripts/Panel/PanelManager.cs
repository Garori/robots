using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour {
    [Header("Game Objects")]
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform linesContent;
    public GameObject lineObjectPrefab;
    public GameObject endLineObject;

    // Line info
    private float minLineWidth;
    private float lineHeight;
    private float colliderHeight;
    private Vector2 endLineSize;
    private RectOffset linePadding;
    private List<HorizontalLayoutGroup> linesLayout;

    [Header("Padding")]
    [SerializeField] private int minPadding;
    [SerializeField] private int tabPadding;

    // Blocks and lines arrays
    private int activeLines;
    private List<GameObject> lines;
    public List<GameObject> blocks { get; set; }

    // Panel size
    public float panelX { get; set; }

    private void Awake() {
        activeLines = 0;

        blocks = new List<GameObject>();
        lines = new List<GameObject>();
        linesLayout = new List<HorizontalLayoutGroup>();
    }

    private void Start() {
        RectTransform lineObjectPrefabTransform = lineObjectPrefab.GetComponent<RectTransform>();
        minLineWidth = lineObjectPrefabTransform.sizeDelta.x;
        lineHeight = lineObjectPrefabTransform.sizeDelta.y;
        colliderHeight = lineObjectPrefab.GetComponent<BoxCollider2D>().size.y;

        panelX = canvas.sizeDelta.x - GetComponent<RectTransform>().sizeDelta.x;

        endLineSize = endLineObject.GetComponent<RectTransform>().sizeDelta;

        RectOffset lineObjectPrefabPadding = lineObjectPrefab.GetComponent<HorizontalLayoutGroup>().padding;
        linePadding = new RectOffset(
            lineObjectPrefabPadding.left,
            lineObjectPrefabPadding.right,
            lineObjectPrefabPadding.top,
            lineObjectPrefabPadding.bottom
        );

        EventManager.BlockEnter += InsertBlock;
        EventManager.BlockExit += RemoveBlock;

        EventManager.ComparatorEnter += InsertComparator;
        EventManager.ComparatorExit += RemoveComparator;

        EventManager.VariableEnter += InsertVariable;
        EventManager.VariableExit += RemoveVariable;
    }

    private void InsertBlock(GameObject block, GameObject line) {
        int index = line.Equals(endLineObject) ? activeLines : lines.IndexOf(line);

        blocks.Insert(index, block);

        GameObject newLine = Instantiate(lineObjectPrefab, linesContent);
        lines.Insert(index, newLine);
        linesLayout.Insert(index, newLine.GetComponent<HorizontalLayoutGroup>());
        newLine.transform.SetSiblingIndex(index);

        RectTransform blockTransform = block.GetComponent<RectTransform>();
        blockTransform.SetParent(newLine.GetComponent<RectTransform>());

        activeLines++;

        OrganizeBlocks();
    }

    private void RemoveBlock(GameObject block) {
        int index = blocks.IndexOf(block);
        if (index == -1) return;

        blocks.RemoveAt(index);
        block.GetComponent<RectTransform>().SetParent(canvas); // Volta o bloco como filho de canvas para poder arrastar

        GameObject line = lines[index];
        lines.RemoveAt(index);
        linesLayout.RemoveAt(index);

        Destroy(line);
        activeLines--;

        OrganizeBlocks();
    }

    private void InsertComparator(GameObject comparator, GameObject blockCondition) {
        Transform blockConditionTransform = blockCondition.GetComponent<RectTransform>();
        if (blockConditionTransform.parent.parent.CompareTag("Canvas") || // verifica se o bloco comparador está fora do painel (parece desnecessário)
            blockConditionTransform.childCount != 0) {
            Destroy(comparator);
            return;
        }

        GameObject block = blockConditionTransform.parent.gameObject;
        if (!blocks.Contains(block)) {
            Destroy(comparator);
            return;
        }

        comparator.GetComponent<RectTransform>().SetParent(blockConditionTransform);
    }

    private void RemoveComparator(GameObject comparator) {
        Transform comparatorTransform = comparator.GetComponent<RectTransform>();
        if (comparatorTransform.parent.CompareTag("Canvas")) return;

        GameObject block = comparatorTransform.parent.parent.gameObject;
        if (!blocks.Contains(block)) return;

        comparatorTransform.SetParent(canvas);
    }

    private void InsertVariable(GameObject variable, GameObject conditionVariable) {
        Transform conditionVariableTransform = conditionVariable.GetComponent<RectTransform>();
        if (conditionVariableTransform.parent.parent.CompareTag("Canvas") ||
            conditionVariableTransform.parent.parent.parent.parent.CompareTag("Canvas") ||
            conditionVariableTransform.childCount != 0) {
            Destroy(variable);
            return;
        }

        GameObject block = conditionVariableTransform.parent.parent.parent.gameObject;
        if (!blocks.Contains(block)) {
            Destroy(variable);
            return;
        }

        variable.GetComponent<RectTransform>().SetParent(conditionVariableTransform);
    }

    private void RemoveVariable(GameObject variable) {
        Transform variableTransform = variable.GetComponent<RectTransform>();
        if (variableTransform.parent.CompareTag("Canvas")) return;

        GameObject block = variableTransform.parent.parent.parent.parent.gameObject;
        if (!blocks.Contains(block)) return;

        variableTransform.SetParent(canvas);
    }

    private void OrganizeBlocks() {
        string blocksPrint = "BLOCKS ARRAY\n";
        int leftPadding = minPadding;
        float maxWidth = minLineWidth;
        for (int i = 0; i < lines.Count; i++) {
            HorizontalLayoutGroup line = linesLayout[i];
            blocksPrint += $"{blocks[i].name}\n";

            if (blocks[i].CompareTag("ElseBlock") || blocks[i].CompareTag("EndBlock")) leftPadding -= tabPadding;
            leftPadding = Mathf.Max(leftPadding, minPadding);

            RectOffset padding = new RectOffset(
                leftPadding,
                linePadding.right,
                linePadding.top,
                linePadding.bottom
            );
            line.padding = padding;

            RectTransform blockTransform = blocks[i].GetComponent<RectTransform>();
            maxWidth = Mathf.Max(maxWidth, leftPadding + blockTransform.sizeDelta.x * blockTransform.localScale.x);

            if (blocks[i].CompareTag("ElseBlock") || blocks[i].CompareTag("StructureBlock") || blocks[i].CompareTag("ForBlock")) leftPadding += tabPadding;
        }

        foreach (GameObject line in lines) {
            Vector2 transformSize = new Vector2(maxWidth, lineHeight);
            line.GetComponent<RectTransform>().sizeDelta = transformSize;

            Vector2 colliderSize = new Vector2(maxWidth, colliderHeight);
            line.GetComponent<BoxCollider2D>().size = colliderSize;
        }
        endLineSize.x = maxWidth;
        endLineObject.GetComponent<RectTransform>().sizeDelta = endLineSize;
        endLineObject.GetComponent<BoxCollider2D>().size = endLineSize;

        Debug.Log(blocksPrint);
    }

    public void Clear() {
        foreach (GameObject line in lines) {
            Destroy(line);
        }
        activeLines = 0;
        lines.Clear();
        blocks.Clear();
        OrganizeBlocks();
    }
}
