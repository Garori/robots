using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Header("Managers")]
	public PanelManager panelManager;
	public BattleManager battleManager;
	public AnimationManager animationManager;

	[Header("Game Objects")]
	public TMP_Text compilePopupText;
	public GameObject battlePanel;
	public GameObject roundPanel;
	public GameObject roundError;
	public RectTransform roundContent;
	public Scrollbar roundScrollbar;
	public Transform blocksArea;

	[Header("Fighters Scripts")]
	[SerializeField] private Compiler playerCompiler;
	[SerializeField] private Compiler enemyCompiler;
	private bool playerCompiled;
	private CellsContainer memory;

	private void Start()
	{
		memory = Memories.GetMemory(BattleData.selectedLevel);
		SetEnemyMemory(memory);
		SetMedalsText(int.MaxValue, int.MaxValue);
		EnableBlocks();

		foreach (Transform child in roundContent.transform)
		{
			Destroy(child.gameObject);
		}
		roundScrollbar.value = 1;
		playerCompiled = false;
	}

	private void EnableBlocks()
	{
		foreach (Transform child in blocksArea)
		{
			// Get child block controller commandname
			BlockController blockController = child.GetComponent<BlockController>();
			Commands commandName = blockController.commandName;

			child.gameObject.SetActive(memory.isBlockEnabled(commandName));
		}
	}

	public void RunBattle()
	{
		string compileResult = "";
		List<BlockController> blocks = panelManager.blocks;
		playerCompiled = playerCompiler.Compile(blocks, ref compileResult);
		if (!playerCompiled)
		{
			compilePopupText.transform.parent.gameObject.SetActive(true);
			compilePopupText.SetText(compileResult);
			return;
		}
		enemyCompiler.ResetAttributes();
		Debug.Log("Starting Battle");
		List<BattleStatus> battleStatuses = new List<BattleStatus>();
		BattleStatus status = battleManager.InitBattleAttributes(memory.playerFighterAttributes, memory.enemyFighterAttributes);
		foreach (Transform child in roundContent.transform)
		{
			Destroy(child.gameObject);
		}
		GameObject actualRoundPanel;
		RectTransform actualRoundPanelTransform;
		try
		{
			do
			{
				// Turno a turno da batalha
				Commands[] actions = new Commands[2];
				actions[0] = playerCompiler.Run(status);
				actions[1] = enemyCompiler.Run(status);
				status = battleManager.PlayRound(actions);
				// Imprime os textos dos rounds
				actualRoundPanel = Instantiate(roundPanel, roundContent);
				actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
				actualRoundPanelTransform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText($"Round {status.values[Commands.ROUND] - 1}");
				actualRoundPanelTransform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PlayerStatus(status));
				actualRoundPanelTransform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().SetText(EnemyStatus(status));
				battleStatuses.Add(status);
			} while (status.isOver == 0);
			actualRoundPanel = Instantiate(roundPanel, roundContent);
			actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
			actualRoundPanelTransform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText($"END OF BATTLE");
			actualRoundPanelTransform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(status.isOver == 1 ? "WINNER" : "LOSER");
			actualRoundPanelTransform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().SetText(status.isOver == -1 ? "WINNER" : "LOSER");

			// Atualiza as medalhas
			SetMedalsText(status.values[Commands.ROUND] - 1, blocks.Count);
		}
		catch (ActionTookTooLongException)
		{
			actualRoundPanel = Instantiate(roundError, roundContent);
			actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
			actualRoundPanelTransform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText($"ERROR\nTHE PLAYER TOOK TOO LONG TO CHOOSE AN ACTION");
		}
		catch (MaxNumberOfRoundsException)
		{
			actualRoundPanel = Instantiate(roundError, roundContent);
			actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
			actualRoundPanelTransform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText($"ERROR\nTHE BATTLE IS TAKING TOO LONG TO FINISH");
		}
		catch (PlayerOutOfActionsException)
		{
			actualRoundPanel = Instantiate(roundError, roundContent);
			actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
			actualRoundPanelTransform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText($"ERROR\nTHE CODE FINISHED BEFORE THE BATTLE ENDED");
		}
		playerCompiled = false;

		animationManager.StartAnimation(battleStatuses);
	}

	public void SetEnemyMemory(CellsContainer memory)
	{
		enemyCompiler.Compile(memory.memory);
	}

	private string PlayerStatus(BattleStatus battleStatus)
	{
		return $"ACTION:\n{battleStatus.playerAction}\nLIFE: {battleStatus.values[Commands.PLAYER_ACTUAL_HEALTH]}\nDEFENSE: {battleStatus.values[Commands.PLAYER_ACTUAL_SHIELD]}\nCHARGE: {battleStatus.values[Commands.PLAYER_ACTUAL_CHARGE]}\n";
	}

	private string EnemyStatus(BattleStatus battleStatus)
	{
		return $"ACTION:\n{battleStatus.enemyAction}\nLIFE: {battleStatus.values[Commands.ENEMY_ACTUAL_HEALTH]}\nDEFENSE: {battleStatus.values[Commands.ENEMY_ACTUAL_SHIELD]}\nCHARGE: {battleStatus.values[Commands.ENEMY_ACTUAL_CHARGE]}\n";
	}

	public void QuitGame()
	{
		panelManager.KillEvents();
		SceneManager.LoadScene("LevelSelect");
	}

	public void ClearBlocks()
	{
		panelManager.Clear();
	}

	public void SkipAnimation()
	{
		animationManager.SkipAnimation();
	}

	public void ShowDebug()
	{
		battlePanel.SetActive(!battlePanel.activeSelf);
		panelManager.gameObject.SetActive(!panelManager.gameObject.activeSelf);
	}

	//TODO: move away from GameManager
	private void SetMedalsText(int rounds, int size)
	{
		memory.SetMedals(rounds, size);

		GameObject roundMedal = GameObject.FindGameObjectWithTag("TurnMedal");
		GameObject sizeMedal = GameObject.FindGameObjectWithTag("BlocksMedal");

		bool isRoundMedalWon = memory.medal.roundsMedal;
		bool isSizeMedalWon = memory.medal.sizeMedal;

		if (roundMedal)
		{
			roundMedal.GetComponent<TooltipTrigger>().tooltipText = $"Round Medal: {(memory.medal.bestRounds == int.MaxValue ? 0 : memory.medal.bestRounds)}/{memory.medal.maxRounds}";
			if (isRoundMedalWon)
			{
				roundMedal.GetComponent<Image>().color = Color.white;
			}
			else
			{
				roundMedal.GetComponent<Image>().color = Color.black;
			}
		}

		if (sizeMedal)
		{
			sizeMedal.GetComponent<TooltipTrigger>().tooltipText = $"Size Medal: {(memory.medal.bestSize == int.MaxValue ? 0 : memory.medal.bestSize)}/{memory.medal.maxSize}";
			if (isSizeMedalWon)
			{
				sizeMedal.GetComponent<Image>().color = Color.white;
			}
			else
			{
				sizeMedal.GetComponent<Image>().color = Color.black;
			}
		}
	}
}
