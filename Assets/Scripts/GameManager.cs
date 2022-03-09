using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    [Header("Managers")]
    public PanelManager panelManager;
    public BattleManager battleManager;
    public AnimationManager animationManager;

    [Header("Game Objects")]
    public TMP_Text compilePopupText;
    public GameObject roundText;
    public RectTransform roundContent;

    [Header("Fighters Scripts")]
    [SerializeField] private Compiler playerCompiler;
    [SerializeField] private Compiler enemyCompiler;
    private Cell[][] memories;

    private void Start() {
        Cell[] memory1 = new Cell[]
        {
            new ActionCell(Commands.DEFEND)
        };

        Cell[] memory2 = new Cell[]
        {
            new IfCell (new GreaterCell(Commands.ENEMY_ACTUAL_HEALTH, Commands.ENEMY_MAX_HEALTH_HALF), 1),
            new ActionCell(Commands.CHARGE),
            new EndCell(),
            new ElseCell (new GreaterCell(Commands.ENEMY_ACTUAL_HEALTH, Commands.ENEMY_MAX_HEALTH_HALF), 1),
            new ActionCell(Commands.ATTACK),
            new EndCell()
        };

        Cell[] memory3 = new Cell[]
        {
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
    }

    public void Compile(List<GameObject> blocks) {
        compilePopupText.SetText("Compilando...");
        compilePopupText.SetText(playerCompiler.Compile(blocks));
    }

    public void RunBattle() {
        Debug.Log("Começou batalha");
        //List<BattleStatus> battleStatuses = new List<BattleStatus>();
        BattleStatus status = battleManager.RunBattle();
        try {
            do {
                Commands[] actions = new Commands[2];
                actions[0] = playerCompiler.Run(status);
                actions[1] = enemyCompiler.Run(status);
                status = battleManager.PlayRound(actions);
                GameObject actualRoundText = Instantiate(roundText.gameObject, roundContent);
                actualRoundText.GetComponent<TMPro.TextMeshProUGUI>().SetText(RoundStatus(status));
                //battleStatuses.Add(status);
            } while (!status.isOver);
        } catch (ActionTookTooLongException) {
            Debug.Log("ERRO NA BATALHA: O jogador demorou muito para escolher uma ação");
        } catch (MaxNumberOfRoundsException) {
            Debug.Log("ERRO NA BATALHA: A batalha está demorando muito para acabar");
        }
        /*
        Debug.Log("Acabou batalha");
        animationManager.StartAnimation(battleStatuses);
        Debug.Log("Acabou batalha2");
        */
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

    private string RoundStatus(BattleStatus battleStatus) {
        string roundStatus = $"ROUND {battleStatus.round}\n\n";
        roundStatus += $"PLAYER\n";
        roundStatus += $"LIFE: {battleStatus.values[Commands.PLAYER_ACTUAL_HEALTH]}\n";
        roundStatus += $"DEFENSE: {battleStatus.values[Commands.PLAYER_ACTUAL_SHIELD]}\n";
        roundStatus += $"CHARGE: {battleStatus.values[Commands.PLAYER_ACTUAL_CHARGE]}\n\n";
        roundStatus += $"ENEMY\n";
        roundStatus += $"LIFE: {battleStatus.values[Commands.ENEMY_ACTUAL_HEALTH]}\n";
        roundStatus += $"DEFENSE: {battleStatus.values[Commands.ENEMY_ACTUAL_SHIELD]}\n";
        roundStatus += $"CHARGE: {battleStatus.values[Commands.ENEMY_ACTUAL_CHARGE]}\n\n";

        return roundStatus;
    }
}
