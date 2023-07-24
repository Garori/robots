using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {
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
    private Cell[][] memories;

    private void Start() {
        Cell[] memory1 = new Cell[]{
            new ActionCell(Commands.DEFEND)
        };

        Cell[] memory2 = new Cell[]{
            new IfCell (new GreaterCell(Commands.ENEMY_ACTUAL_HEALTH, Commands.ENEMY_MAX_HEALTH_HALF), 1),
            new ActionCell(Commands.CHARGE),
            new EndCell(),
            new ElseCell (new GreaterCell(Commands.ENEMY_ACTUAL_HEALTH, Commands.ENEMY_MAX_HEALTH_HALF), 1),
            new ActionCell(Commands.ATTACK),
            new EndCell()
        };

        Cell[] memory3 = new Cell[]{
            new IfCell (new NotEqualsCell(Commands.ENEMY_ACTUAL_SHIELD, Commands.ZERO), 5),
            new ActionCell(Commands.DEFEND),
            new ActionCell(Commands.DEFEND),
            new ActionCell(Commands.CHARGE),
            new ActionCell(Commands.CHARGE),
            new ActionCell(Commands.ATTACK),
            new EndCell(),
            new IfCell (new EqualsCell(Commands.ENEMY_ACTUAL_HEALTH, Commands.ENEMY_MAX_HEALTH), 5),
            new ActionCell(Commands.DODGE),
            new ActionCell(Commands.CHARGE),
            new ActionCell(Commands.CHARGE),
            new ActionCell(Commands.ATTACK),
            new ActionCell(Commands.ATTACK),
            new EndCell(),
            new ActionCell(Commands.HEAL),
            new ActionCell(Commands.HEAL),
            new ActionCell(Commands.CHARGE),
            new ActionCell(Commands.CHARGE),
            new ActionCell(Commands.ATTACK)
        };

        memories = new Cell[3][];
        memories[0] = memory1;
        memories[1] = memory2; 
        memories[2] = memory3;
        SetEnemyMemory(0);

        playerCompiled = false;
    }

    public void RunBattle() {
        string compileResult = "";
        List<GameObject> blocks = panelManager.blocks;
        playerCompiled = playerCompiler.Compile(blocks, ref compileResult);
        if (!playerCompiled) {
            compilePopupText.transform.parent.gameObject.SetActive(true);
            compilePopupText.SetText(compileResult);
            return;
        }
        Debug.Log("Começou batalha");
        List<BattleStatus> battleStatuses = new List<BattleStatus>();
        BattleStatus status = battleManager.RunBattle();
        foreach (Transform child in roundContent.transform) {
            Destroy(child.gameObject);
        }
        GameObject actualRoundPanel;
        RectTransform actualRoundPanelTransform;
        try {
            do {
                // Turno a turno da batalha
                Commands[] actions = new Commands[2];
                actions[0] = playerCompiler.Run(status);
                actions[1] = enemyCompiler.Run(status);
                status = battleManager.PlayRound(actions);
                Debug.Log(status);
                // Imprime os textos dos rounds
                actualRoundPanel = Instantiate(roundPanel, roundContent);
                actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
                actualRoundPanelTransform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText($"Round {status.values[Commands.ROUND]}");
                actualRoundPanelTransform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PlayerStatus(status));
                actualRoundPanelTransform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().SetText(EnemyStatus(status));
                battleStatuses.Add(status);
            } while (status.isOver == 0);
            actualRoundPanel = Instantiate(roundPanel, roundContent);
            actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
            actualRoundPanelTransform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText($"END OF BATTLE");
            actualRoundPanelTransform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(status.isOver == 1 ? "WINNER" : "LOSER");
            actualRoundPanelTransform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().SetText(status.isOver == -1 ? "WINNER" : "LOSER");
        } catch (ActionTookTooLongException) {
            actualRoundPanel = Instantiate(roundError, roundContent);
            actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
            actualRoundPanelTransform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText($"ERROR\nTHE PLAYER TOOK TOO LONG TO CHOOSE AN ACTION");
        } catch (MaxNumberOfRoundsException) {
            actualRoundPanel = Instantiate(roundError, roundContent);
            actualRoundPanelTransform = actualRoundPanel.GetComponent<RectTransform>();
            actualRoundPanelTransform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText($"ERROR\nTHE BATTLE IS TAKING TOO LONG TO FINISH");
        }
        playerCompiled = false;
        
        StartCoroutine(animationManager.CO_StartAnimation(battleStatuses));
    }

    public void SetEnemyMemory(int num) {
        enemyCompiler.Compile(memories[num]);
    }

    public void OnChooseEnemy1() {
        SetEnemyMemory(0);
    }

    public void OnChooseEnemy2() {
        SetEnemyMemory(1);
    }

    public void OnChooseEnemy3() {
        SetEnemyMemory(2);
    }

    private string PlayerStatus(BattleStatus battleStatus) {
        return $"ACTION:\n{battleStatus.playerAction}\nLIFE: {battleStatus.values[Commands.PLAYER_ACTUAL_HEALTH]}\nDEFENSE: {battleStatus.values[Commands.PLAYER_ACTUAL_SHIELD]}\nCHARGE: {battleStatus.values[Commands.PLAYER_ACTUAL_CHARGE]}\n";
    }

    private string EnemyStatus(BattleStatus battleStatus) {
        return $"ACTION:\n{battleStatus.enemyAction}\nLIFE: {battleStatus.values[Commands.ENEMY_ACTUAL_HEALTH]}\nDEFENSE: {battleStatus.values[Commands.ENEMY_ACTUAL_SHIELD]}\nCHARGE: {battleStatus.values[Commands.ENEMY_ACTUAL_CHARGE]}\n";
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ClearBlocks() {
        panelManager.Clear();
    }
}
