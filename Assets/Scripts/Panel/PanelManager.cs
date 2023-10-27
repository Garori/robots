using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class PanelManager : MonoBehaviour
{
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
	public List<BlockController> blocks { get; set; }

	// Panel size
	public float panelX { get; set; }

	private void Awake()
	{
		activeLines = 0;

		blocks = new List<BlockController>();
		lines = new List<GameObject>();
		linesLayout = new List<HorizontalLayoutGroup>();
	}

	private void Start()
	{
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

	private void InsertBlock(BlockController block, GameObject line)
	{
		// Pega o index da linha e adiciona o bloco na lista
		int index = line.Equals(endLineObject) ? activeLines : lines.IndexOf(line);
		blocks.Insert(index, block);

		// Cria a linha para o bloco
		GameObject newLine = Instantiate(lineObjectPrefab, linesContent);
		lines.Insert(index, newLine);
		activeLines++;
		linesLayout.Insert(index, newLine.GetComponent<HorizontalLayoutGroup>());
		newLine.transform.SetSiblingIndex(index);

		// Adiciona o bloco na linha
		block.SetParent(newLine.GetComponent<RectTransform>());
		block.isInPanel = true;

		OrganizeBlocks();
	}

	private void RemoveBlock(BlockController block)
	{
		// Verifica se o bloco está no painel
		if (!block.isInPanel) return;
		block.isInPanel = false;

		// Remove da lista de blocos
		int index = blocks.IndexOf(block);
		blocks.RemoveAt(index);

		// Remove a linha correspondente do painel 
		GameObject line = lines[index];
		lines.RemoveAt(index);
		linesLayout.RemoveAt(index);
		Destroy(line);
		activeLines--;

		OrganizeBlocks();
	}

	private void InsertComparator(ComparatorController comparator, BlockSlotController blockSlot)
	{
		// Verifica se o espaco esta ocupado
		if (blockSlot.isOccupied())
		{
			Destroy(comparator.gameObject);
			return;
		}

		// Verifica se o bloco esta no painel
		if (!blockSlot.isInPanel)
		{
			Destroy(comparator.gameObject);
			return;
		}

		// Define parent de comparador
		comparator.isInPanel = true;
		Transform blockSlotTransform = blockSlot.GetComponent<RectTransform>();
		comparator.SetParent(blockSlotTransform);

		// Define espaco parent de comparador como blockSlot
		comparator.structureSlot = blockSlot;
		// E espaco child de blockSlot como comparador
		blockSlot.setChildBlock(comparator);
	}

	private void RemoveComparator(ComparatorController comparator)
	{
		if (!comparator.isInPanel) return;
		comparator.isInPanel = false;

		comparator.structureSlot.removeChildBlock();
	}

	private void InsertVariable(VariableController variable, BlockSlotController variableSlot)
	{
		if (variableSlot.isOccupied())
		{
			Destroy(variable.gameObject);
			return;
		}

		if (!variableSlot.isInPanel)
		{
			Destroy(variable.gameObject);
			return;
		}

		// Define parent da variavel
		variable.isInPanel = true;
		Transform variableSlotTransform = variableSlot.GetComponent<RectTransform>();
		variable.SetParent(variableSlotTransform);

		variable.blockSlot = variableSlot;
		variableSlot.setChildBlock(variable);
	}

	private void RemoveVariable(VariableController variable)
	{
		// Verifica se o bloco esta no painel
		if (!variable.isInPanel) return;
		variable.isInPanel = false;

		variable.blockSlot.removeChildBlock();
	}

	private void OrganizeBlocks()
	{
		string blocksPrint = "BLOCKS ARRAY\n";
		int leftPadding = minPadding;
		float maxWidth = minLineWidth;
		for (int i = 0; i < lines.Count; i++)
		{
			Debug.Log(blocks[i].GetType());
			GameObject blockGameObject = blocks[i].gameObject;
			HorizontalLayoutGroup line = linesLayout[i];
			blocksPrint += $"{blockGameObject.name}\n";

			if (blockGameObject.CompareTag("ElseBlock") || blockGameObject.CompareTag("EndBlock")) leftPadding -= tabPadding;
			leftPadding = Mathf.Max(leftPadding, minPadding);

			RectOffset padding = new RectOffset(
				leftPadding,
				linePadding.right,
				linePadding.top,
				linePadding.bottom
			);
			line.padding = padding;

			RectTransform blockTransform = blockGameObject.GetComponent<RectTransform>();
			maxWidth = Mathf.Max(maxWidth, leftPadding + blockTransform.sizeDelta.x * blockTransform.localScale.x);

			if (blockGameObject.CompareTag("ElseBlock") || blockGameObject.CompareTag("StructureBlock") || blockGameObject.CompareTag("ForBlock")) leftPadding += tabPadding;
		}

		foreach (GameObject line in lines)
		{
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

	public void Clear()
	{
		foreach (GameObject line in lines)
		{
			Destroy(line);
		}
		activeLines = 0;
		lines.Clear();
		blocks.Clear();
		OrganizeBlocks();
	}

	public void KillEvents()
	{
		EventManager.BlockEnter -= InsertBlock;
		EventManager.BlockExit -= RemoveBlock;

		EventManager.ComparatorEnter -= InsertComparator;
		EventManager.ComparatorExit -= RemoveComparator;

		EventManager.VariableEnter -= InsertVariable;
		EventManager.VariableExit -= RemoveVariable;
	}
}
