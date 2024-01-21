using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[Serializable]
public struct ActionsPrefabs
{
	public GameObject attackPrefab;
	public GameObject defendPrefab;
	public GameObject chargePrefab;
	public GameObject healPrefab;
}

[Serializable]
public struct StructuresPrefabs
{
	public GameObject ifPrefab;
	public GameObject elsePrefab;
	public GameObject whilePrefab;
	public GameObject forPrefab;
	public GameObject endPrefab;
}

[Serializable]
public struct ComparatorsPrefabs
{
	public GameObject truePrefab;
	public GameObject equalsPrefab;
	public GameObject notEqualsPrefab;
	public GameObject greaterPrefab;
	public GameObject evenPrefab;

}

[Serializable]
public struct FighterVariableModifiersPrefabs
{
	public GameObject currentPrefab;
	public GameObject halfPrefab;
	public GameObject doublePrefab;
}

[Serializable]
public struct FighterPrefabs
{
	public FighterVariableModifiersPrefabs health;
	public FighterVariableModifiersPrefabs maxHealth;
	public FighterVariableModifiersPrefabs defense;
	public FighterVariableModifiersPrefabs charge;
}

[Serializable]
public struct VariablesPrefabs
{
	public FighterPrefabs player;
	public FighterPrefabs enemy;
	public GameObject roundPrefab;
	public GameObject[] numbersPrefabs;
}

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

	[Header("Prefabs")]
	[SerializeField] private ActionsPrefabs actionsPrefabs;
	[SerializeField] private StructuresPrefabs structuresPrefabs;
	[SerializeField] private ComparatorsPrefabs comparatorsPrefabs;
	[SerializeField] private VariablesPrefabs variablesPrefabs;

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
	}

	private void Start()
	{

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
	}

	public void LoadCommands(List<List<Commands>> commands)
	{
		Clear();
		foreach (List<Commands> lineCommands in commands)
		{
			Commands mainCommand = lineCommands[0];
			switch (mainCommand)
			{
				case Commands.ELSE:
					GameObject elseBlock = InstantiateStructure(mainCommand);
					InsertBlock(elseBlock.GetComponent<ElseController>(), endLineObject);
					break;
				case Commands.END:
					GameObject endBlock = InstantiateStructure(mainCommand);
					InsertBlock(endBlock.GetComponent<EndController>(), endLineObject);
					break;
				case Commands.FOR:
					GameObject forBlock = InstantiateStructure(mainCommand);
					InsertBlock(forBlock.GetComponent<ForController>(), endLineObject);

					GameObject forVariable = InstantiateVariable(lineCommands[1]);
					InsertVariable(forVariable.GetComponent<VariableController>(), forBlock.GetComponent<ForController>().variableSlot);
					break;
				case Commands.IF:
				case Commands.WHILE:
					GameObject structureBlock = InstantiateStructure(mainCommand);
					InsertBlock(structureBlock.GetComponent<BlockController>(), endLineObject);

					GameObject comparator = InstantiateComparator(lineCommands[1]);
					InsertComparator(comparator.GetComponent<ComparatorController>(), structureBlock.GetComponent<StructureController>().comparatorSlot);

					switch (lineCommands[1])
					{
						case Commands.TRUE:
							break;
						case Commands.EVEN:
							GameObject variable = InstantiateVariable(lineCommands[2]);
							InsertVariable(variable.GetComponent<VariableController>(), structureBlock.GetComponent<ComparatorController>().variableSlot1);
							break;
						default:
							GameObject variable1 = InstantiateVariable(lineCommands[2]);
							InsertVariable(variable1.GetComponent<VariableController>(), structureBlock.GetComponent<ComparatorController>().variableSlot1);

							GameObject variable2 = InstantiateVariable(lineCommands[3]);
							InsertVariable(variable2.GetComponent<VariableController>(), structureBlock.GetComponent<ComparatorController>().variableSlot2);
							break;
					}
					break;
				default:
					GameObject actionBlock = InstantiateAction(mainCommand);
					InsertBlock(actionBlock.GetComponent<BlockController>(), endLineObject);
					break;
			}
		}
		OrganizeBlocks();
	}

	public GameObject InstantiateAction(Commands command)
	{
		switch (command)
		{
			case Commands.ATTACK:
				return Instantiate(actionsPrefabs.attackPrefab);
			case Commands.DEFEND:
				return Instantiate(actionsPrefabs.defendPrefab);
			case Commands.CHARGE:
				return Instantiate(actionsPrefabs.chargePrefab);
			case Commands.HEAL:
				return Instantiate(actionsPrefabs.healPrefab);
			default:
				return null;
		}
	}

	public GameObject InstantiateStructure(Commands command)
	{
		switch (command)
		{
			case Commands.IF:
				return Instantiate(structuresPrefabs.ifPrefab);
			case Commands.ELSE:
				return Instantiate(structuresPrefabs.elsePrefab);
			case Commands.WHILE:
				return Instantiate(structuresPrefabs.whilePrefab);
			case Commands.FOR:
				return Instantiate(structuresPrefabs.forPrefab);
			case Commands.END:
				return Instantiate(structuresPrefabs.endPrefab);
			default:
				return null;
		}
	}

	public GameObject InstantiateComparator(Commands command)
	{
		switch (command)
		{
			case Commands.TRUE:
				return Instantiate(comparatorsPrefabs.truePrefab);
			case Commands.EQUALS:
				return Instantiate(comparatorsPrefabs.equalsPrefab);
			case Commands.NOT_EQUALS:
				return Instantiate(comparatorsPrefabs.notEqualsPrefab);
			case Commands.GREATER:
				return Instantiate(comparatorsPrefabs.greaterPrefab);
			case Commands.EVEN:
				return Instantiate(comparatorsPrefabs.evenPrefab);
			default:
				return null;
		}
	}

	public GameObject InstantiateVariable(Commands command)
	{
		switch (command)
		{
			case Commands.PLAYER_ACTUAL_HEALTH:
				return Instantiate(variablesPrefabs.player.health.currentPrefab);
			case Commands.PLAYER_ACTUAL_HEALTH_HALF:
				return Instantiate(variablesPrefabs.player.health.halfPrefab);
			case Commands.PLAYER_ACTUAL_HEALTH_DOUBLE:
				return Instantiate(variablesPrefabs.player.health.doublePrefab);
			case Commands.PLAYER_MAX_HEALTH:
				return Instantiate(variablesPrefabs.player.maxHealth.currentPrefab);
			case Commands.PLAYER_MAX_HEALTH_HALF:
				return Instantiate(variablesPrefabs.player.maxHealth.halfPrefab);
			case Commands.PLAYER_MAX_HEALTH_DOUBLE:
				return Instantiate(variablesPrefabs.player.maxHealth.doublePrefab);
			case Commands.PLAYER_ACTUAL_SHIELD:
				return Instantiate(variablesPrefabs.player.defense.currentPrefab);
			case Commands.PLAYER_ACTUAL_SHIELD_HALF:
				return Instantiate(variablesPrefabs.player.defense.halfPrefab);
			case Commands.PLAYER_ACTUAL_SHIELD_DOUBLE:
				return Instantiate(variablesPrefabs.player.defense.doublePrefab);
			case Commands.PLAYER_ACTUAL_CHARGE:
				return Instantiate(variablesPrefabs.player.charge.currentPrefab);
			case Commands.PLAYER_ACTUAL_CHARGE_HALF:
				return Instantiate(variablesPrefabs.player.charge.halfPrefab);
			case Commands.PLAYER_ACTUAL_CHARGE_DOUBLE:
				return Instantiate(variablesPrefabs.player.charge.doublePrefab);
			case Commands.ENEMY_ACTUAL_HEALTH:
				return Instantiate(variablesPrefabs.enemy.health.currentPrefab);
			case Commands.ENEMY_ACTUAL_HEALTH_HALF:
				return Instantiate(variablesPrefabs.enemy.health.halfPrefab);
			case Commands.ENEMY_ACTUAL_HEALTH_DOUBLE:
				return Instantiate(variablesPrefabs.enemy.health.doublePrefab);
			case Commands.ENEMY_MAX_HEALTH:
				return Instantiate(variablesPrefabs.enemy.maxHealth.currentPrefab);
			case Commands.ENEMY_MAX_HEALTH_HALF:
				return Instantiate(variablesPrefabs.enemy.maxHealth.halfPrefab);
			case Commands.ENEMY_MAX_HEALTH_DOUBLE:
				return Instantiate(variablesPrefabs.enemy.maxHealth.doublePrefab);
			case Commands.ENEMY_ACTUAL_SHIELD:
				return Instantiate(variablesPrefabs.enemy.defense.currentPrefab);
			case Commands.ENEMY_ACTUAL_SHIELD_HALF:
				return Instantiate(variablesPrefabs.enemy.defense.halfPrefab);
			case Commands.ENEMY_ACTUAL_SHIELD_DOUBLE:
				return Instantiate(variablesPrefabs.enemy.defense.doublePrefab);
			case Commands.ENEMY_ACTUAL_CHARGE:
				return Instantiate(variablesPrefabs.enemy.charge.currentPrefab);
			case Commands.ENEMY_ACTUAL_CHARGE_HALF:
				return Instantiate(variablesPrefabs.enemy.charge.halfPrefab);
			case Commands.ENEMY_ACTUAL_CHARGE_DOUBLE:
				return Instantiate(variablesPrefabs.enemy.charge.doublePrefab);
			case Commands.ROUND:
				return Instantiate(variablesPrefabs.roundPrefab);
			case Commands.ZERO:
				return Instantiate(variablesPrefabs.numbersPrefabs[0]);
			case Commands.ONE:
				return Instantiate(variablesPrefabs.numbersPrefabs[1]);
			case Commands.TWO:
				return Instantiate(variablesPrefabs.numbersPrefabs[2]);
			case Commands.THREE:
				return Instantiate(variablesPrefabs.numbersPrefabs[3]);
			case Commands.FOUR:
				return Instantiate(variablesPrefabs.numbersPrefabs[4]);
			case Commands.FIVE:
				return Instantiate(variablesPrefabs.numbersPrefabs[5]);
			case Commands.SIX:
				return Instantiate(variablesPrefabs.numbersPrefabs[6]);
			case Commands.SEVEN:
				return Instantiate(variablesPrefabs.numbersPrefabs[7]);
			case Commands.EIGHT:
				return Instantiate(variablesPrefabs.numbersPrefabs[8]);
			case Commands.NINE:
				return Instantiate(variablesPrefabs.numbersPrefabs[9]);
			default:
				return null;
		}
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
