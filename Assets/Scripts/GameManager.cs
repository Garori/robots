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
	public GameObject roundPanel;
	public GameObject roundError;
	public RectTransform roundContent;
	public Scrollbar roundScrollbar;

	[Header("Fighters Scripts")]
	[SerializeField] private Compiler playerCompiler;
	[SerializeField] private Compiler enemyCompiler;
	private bool playerCompiled;
	private Cell[] memory;

	private void Start()
	{
		memory = Memories.GetMemory(UserData.Instance.Level);
		SetEnemyMemory(memory);

		foreach (Transform child in roundContent.transform)
		{
			Destroy(child.gameObject);
		}
		roundScrollbar.value = 1;
		playerCompiled = false;
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
		BattleStatus status = battleManager.RunBattle();
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
				Debug.Log(status);
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
			UserData.Instance.SetMedals(status.values[Commands.ROUND], blocks.Count);
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

	public void SetEnemyMemory(Cell[] memory)
	{
		enemyCompiler.Compile(memory);
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
}
