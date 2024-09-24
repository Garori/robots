using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public PanelManager panelManager;
    public BattleManager battleManager;
    public AnimationManager animationManager;

    [Header("Game Objects")]
    public TMP_Text compilePopupText;
    public GameObject battlePanel;
    public GameObject statsPanel;
    public GameObject roundPanel;
    public GameObject roundError;
    public RectTransform roundContent;
    public Scrollbar roundScrollbar;
    public Transform blocksArea;
    public HintPanelMove hintPanel;

    [Header("Fighters Scripts")]
    [SerializeField]
    private Compiler playerCompiler;

    [SerializeField]
    private Compiler enemyCompiler;
    private bool playerCompiled;
    private CellsContainer memory;

    [Header("Events")]
    public UnityEvent SetDebugColor;

    private void Start()
    {
        if (BattleData.isTest)
        {
            memory = BattleData.levelMemory;
            SetTestMedalsText(int.MaxValue, int.MaxValue);
        }
        else
        {
            memory = Memories.GetMemory(BattleData.selectedLevel);
            SetMedalsText(int.MaxValue, int.MaxValue);
        }

        SetStatusText(memory);
        SetEnemyMemory(memory);
        SetHintText(memory);
        LoadLastCode(memory);
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
        Debug.Log(playerCompiled);
        enemyCompiler.ResetAttributes();
        Debug.Log("Starting Battle");
        List<BattleStatus> battleStatuses = new List<BattleStatus>();
        BattleStatus lastStatus = battleManager.InitBattleAttributes(
            memory.playerFighterAttributes,
            memory.enemyFighterAttributes
        );
        BattleStatus newStatus;
        foreach (Transform child in roundContent.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject actualRoundPanel;
        RectTransform actualRoundPanelTransform;
        bool FirstTime = true;
        int i = 0;
        try
        {
            do
            {
                // Turno a turno da batalha
                i++;
                Commands[] actions = new Commands[2];
                Debug.Log("player");
                actions[0] = playerCompiler.Run(lastStatus,battleManager);
                Debug.Log("inimigo");
                actions[1] = enemyCompiler.Run(lastStatus);
                newStatus = battleManager.PlayRound(actions);
                if(newStatus.isOver == 0 && !battleManager.checkWin())
                {
                    PrintStatus(lastStatus, newStatus);
                    battleStatuses.Add(newStatus);
                    lastStatus = newStatus;
                }
                else if(battleManager.checkWin() && FirstTime)
                {
                    FirstTime = false;
                    PrintStatus(lastStatus, newStatus);
                    battleStatuses.Add(newStatus);
                    lastStatus = newStatus; 
                }
                else if(newStatus.isOver != 0)
                {
                    FirstTime = false;
                    PrintStatus(lastStatus, newStatus);
                    battleStatuses.Add(newStatus);
                    lastStatus = newStatus;
                }
            } while (newStatus.isOver == 0);
            // PrintStatus(lastStatus, newStatus);
            actualRoundPanel = Instantiate(roundPanel, roundContent);
            actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
            actualRoundPanelTransform
                .GetChild(0)
                .GetComponent<TMPro.TextMeshProUGUI>()
                .SetText($"FIM DA BATALHA");
            actualRoundPanelTransform
                .GetChild(1)
                .GetChild(0)
                .GetComponent<TMPro.TextMeshProUGUI>()
                .SetText(newStatus.isOver == 1 ? "VENCEDOR" : "PERDEDOR");
            actualRoundPanelTransform
                .GetChild(1)
                .GetChild(1)
                .GetComponent<TMPro.TextMeshProUGUI>()
                .SetText(newStatus.isOver == -1 ? "VENCEDOR" : "PERDEDOR");

            if (newStatus.isOver == 1)
            {
                if (BattleData.isTest)
                {
                    SetTestMedalsText(newStatus.values[Commands.ROUND] - 1, blocks.Count);
                }
                else
                {
                    SetMedalsText(newStatus.values[Commands.ROUND] - 1, blocks.Count);
                    memory.SetWin(true);
                    List<List<Commands>> commands = playerCompiler.GetCommands(blocks);
                    SetLastCode(commands);
                }
            }
        }
        catch (ActionTookTooLongException)
        {
            actualRoundPanel = Instantiate(roundError, roundContent);
            actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
            actualRoundPanelTransform
                .GetChild(0)
                .GetComponent<TMPro.TextMeshProUGUI>()
                .SetText($"ERRO:\nJOGADOR DEMOROU MUITO PARA ESCOLHER UMA AÇÃO");
        }
        catch (MaxNumberOfRoundsException)
        {
            string msg;
            if (playerCompiler.BattleManager.currentlyWhileLoop)
            {
                msg = $"ERRO:\nA BATALHA FICOU PRESA NO WHILE";
            }
            else
            {
                msg = $"ERRO:\nA BATALHA DEMOROU MUITO PARA ACABAR";
            }
            actualRoundPanel = Instantiate(roundError, roundContent);
            actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
            actualRoundPanelTransform
                .GetChild(0)
                .GetComponent<TMPro.TextMeshProUGUI>()
                .SetText(msg);
        }
        catch (PlayerOutOfActionsException)
        {
            actualRoundPanel = Instantiate(roundError, roundContent);
            actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
            actualRoundPanelTransform
                .GetChild(0)
                .GetComponent<TMPro.TextMeshProUGUI>()
                .SetText($"ERRO:\nO CÓDIGO ACABOU ANTES DO FIM DA BATALHA");
        }
        if (!BattleData.isTest)
        {
            memory.UpdateFile();
        }
        ShowDebug();
        playerCompiled = false;
        animationManager.StartAnimation(battleStatuses);
    }

    private void PrintStatus(BattleStatus lastStatus, BattleStatus newStatus)
    {
        // Imprime os textos dos rounds
        if (lastStatus.values[Commands.PLAYER_ACTUAL_HEALTH] == 0 || lastStatus.values[Commands.ENEMY_ACTUAL_HEALTH] == 0)
        {
            return;
        }
        GameObject actualRoundPanel = Instantiate(roundPanel, roundContent);
        RectTransform actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
        actualRoundPanelTransform
            .GetChild(0)
            .GetComponent<TMPro.TextMeshProUGUI>()
            .SetText($"Round {newStatus.values[Commands.ROUND] - 1}");
        actualRoundPanelTransform
            .GetChild(1)
            .GetChild(0)
            .GetComponent<TMPro.TextMeshProUGUI>()
            .SetText(PlayerStatus(lastStatus, newStatus));
        actualRoundPanelTransform
            .GetChild(1)
            .GetChild(1)
            .GetComponent<TMPro.TextMeshProUGUI>()
            .SetText(EnemyStatus(lastStatus, newStatus));
    }

    public void SetEnemyMemory(CellsContainer memory)
    {
        enemyCompiler.Compile(memory.memory);
    }

    private string PlayerStatus(BattleStatus lastStatus, BattleStatus newStatus)
    {
        return $"COMANDO:\n{newStatus.playerAction}\n"
            + $"VIDA: {lastStatus.values[Commands.PLAYER_ACTUAL_HEALTH]} -> {newStatus.values[Commands.PLAYER_ACTUAL_HEALTH]}\n"
            + $"ESCUDOS: {lastStatus.values[Commands.PLAYER_ACTUAL_SHIELD]} -> {newStatus.values[Commands.PLAYER_ACTUAL_SHIELD]}\n"
            + $"CARGAS: {lastStatus.values[Commands.PLAYER_ACTUAL_CHARGE]} -> {newStatus.values[Commands.PLAYER_ACTUAL_CHARGE]}\n";
    }

    private string EnemyStatus(BattleStatus lastStatus, BattleStatus newStatus)
    {
        return $"COMANDO:\n{newStatus.enemyAction}\n"
            + $"VIDA: {lastStatus.values[Commands.ENEMY_ACTUAL_HEALTH]} -> {newStatus.values[Commands.ENEMY_ACTUAL_HEALTH]}\n"
            + $"ESCUDOS: {lastStatus.values[Commands.ENEMY_ACTUAL_SHIELD]} -> {newStatus.values[Commands.ENEMY_ACTUAL_SHIELD]}\n"
            + $"CARGAS: {lastStatus.values[Commands.ENEMY_ACTUAL_CHARGE]} -> {newStatus.values[Commands.ENEMY_ACTUAL_CHARGE]}\n";
    }

    public void QuitGame()
    {
        panelManager.KillEvents();
        if (BattleData.isTest)
        {
            SceneManager.LoadScene("CustomCode");
        }
        else
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    public void ClearBlocks()
    {
        panelManager.Clear();
    }

    public void SkipAnimation()
    {
        animationManager.SkipAnimation();
    }

    public void ToggleDebug()
    {
        battlePanel.SetActive(!battlePanel.activeSelf);
        panelManager.gameObject.SetActive(!panelManager.gameObject.activeSelf);
    }

    public void ShowDebug()
    {
        battlePanel.SetActive(true);
        panelManager.gameObject.SetActive(false);
        SetDebugColor.Invoke();
    }

    private void SetMedalsText(int rounds, int size)
    {
        memory.SetMedals(rounds, size);

        GameObject roundMedal = GameObject.FindGameObjectWithTag("TurnMedal");
        GameObject sizeMedal = GameObject.FindGameObjectWithTag("BlocksMedal");

        bool isRoundMedalWon = memory.medal.roundsMedal;
        bool isSizeMedalWon = memory.medal.sizeMedal;

        if (roundMedal)
        {
            roundMedal.GetComponent<TooltipTrigger>().tooltipText =
                $"Medalha Turnos: {(memory.medal.bestRounds == int.MaxValue ? 0 : memory.medal.bestRounds)}/{memory.medal.maxRounds}";
            ColorMedal(isRoundMedalWon, roundMedal);
        }

        if (sizeMedal)
        {
            sizeMedal.GetComponent<TooltipTrigger>().tooltipText =
                $"Medalha Blocos: {(memory.medal.bestSize == int.MaxValue ? 0 : memory.medal.bestSize)}/{memory.medal.maxSize}";
            ColorMedal(isSizeMedalWon, sizeMedal);
        }
    }

    private void SetTestMedalsText(int round, int size)
    {
        GameObject roundMedal = GameObject.FindGameObjectWithTag("TurnMedal");
        GameObject sizeMedal = GameObject.FindGameObjectWithTag("BlocksMedal");

        bool isRoundMedalWon = round <= memory.medal.maxRounds;
        bool isSizeMedalWon = size <= memory.medal.maxSize;

        if (roundMedal)
        {
            roundMedal.GetComponent<TooltipTrigger>().tooltipText =
                $"Medalha Turnos: {(round == int.MaxValue ? 0 : round)}/{memory.medal.maxRounds}";
            ColorMedal(isRoundMedalWon, roundMedal);
        }

        if (sizeMedal)
        {
            sizeMedal.GetComponent<TooltipTrigger>().tooltipText =
                $"Medalha Blocos: {(size == int.MaxValue ? 0 : size)}/{memory.medal.maxSize}";
            ColorMedal(isSizeMedalWon, sizeMedal);
        }
    }

    private void SetStatusText(CellsContainer memory)
    {
        GameObject playerStatsUI = statsPanel.transform.Find("PlayerStatsUI").gameObject;
        GameObject enemyStatsUI = statsPanel.transform.Find("EnemyStatsUI").gameObject;

        playerStatsUI
            .transform.Find("HealthText")
            .GetComponent<TMP_Text>()
            .SetText("Vida: " + memory.playerFighterAttributes.maxLifePoints);
        playerStatsUI
            .transform.Find("DefenseText")
            .GetComponent<TMP_Text>()
            .SetText("Defesa: " + memory.playerFighterAttributes.maxDefensePoints);
        playerStatsUI
            .transform.Find("ChargeText")
            .GetComponent<TMP_Text>()
            .SetText("Carga: " + memory.playerFighterAttributes.maxChargePoints);
        playerStatsUI
            .transform.Find("DamageText")
            .GetComponent<TMP_Text>()
            .SetText("Dano: " + memory.playerFighterAttributes.minAttackPoints);

        enemyStatsUI
            .transform.Find("HealthText")
            .GetComponent<TMP_Text>()
            .SetText("Vida: " + memory.enemyFighterAttributes.maxLifePoints);
        enemyStatsUI
            .transform.Find("DefenseText")
            .GetComponent<TMP_Text>()
            .SetText("Defesa: " + memory.enemyFighterAttributes.maxDefensePoints);
        enemyStatsUI
            .transform.Find("ChargeText")
            .GetComponent<TMP_Text>()
            .SetText("Carga: " + memory.enemyFighterAttributes.maxChargePoints);
        enemyStatsUI
            .transform.Find("DamageText")
            .GetComponent<TMP_Text>()
            .SetText("Dano: " + memory.enemyFighterAttributes.minAttackPoints);
    }

    private void ColorMedal(bool medalWon, GameObject medal)
    {
        if (medalWon)
        {
            medal.GetComponent<Image>().color = Color.white;
        }
        else
        {
            medal.GetComponent<Image>().color = Color.black;
        }
    }

    private void SetHintText(CellsContainer memory)
    {
        hintPanel.SetText(memory.hint);
    }

    private void LoadLastCode(CellsContainer memory)
    {
        panelManager.LoadCommands(memory.lastUserCode);
    }

    private void SetLastCode(List<List<Commands>> commands)
    {
        memory.lastUserCode = commands;
    }
}
