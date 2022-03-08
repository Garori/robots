using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    [Header("Managers")]
    public Compiler compiler;
    public PanelManager panelManager;
    public BattleManager battleManager;
    public AnimationManager animationManager;

    [Header("Game Objects")]
    public TMP_Text compilePopupText;

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
            new IfCell (new GreaterCell(Commands.ENEMY_MAX_HEALTH_HALF, Commands.ENEMY_ACTUAL_HEALTH), 1), 
            new ActionCell(Commands.CHARGE), new EndCell(), 
            new ElseCell (new GreaterCell(Commands.ENEMY_MAX_HEALTH_HALF, Commands.ENEMY_ACTUAL_HEALTH), 1), 
            new ActionCell(Commands.ATTACK), 
            new EndCell()
        };

        Cell[] memory3 = new Cell[] 
        {		
            new IfCell (new NotEqualsCell(Commands.ENEMY_ACTUAL_SHIELD, 0), 5),
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
            new ActionCell(Commands.ATTACK),
            new EndCell()
        };

        memories = new Cell[3][];
        memories[0] = memory1;
        memories[1] = memory2;
        memories[2] = memory3;
    }

    public void Compile(List<GameObject> blocks) {
        compilePopupText.SetText("Compilando...");
        compilePopupText.SetText(compiler.Compile(blocks));
    }

    public void RunBattle() {
        Debug.Log("Começou batalha");
        List<BattleStatus> battleStatuses = new List<BattleStatus>();
        BattleStatus status = battleManager.RunBattle();

        List<Commands[]> actions = new List<Commands[]>();
        actions.Add(new Commands[] { Commands.ATTACK, Commands.DEFEND });
        actions.Add(new Commands[] { Commands.ATTACK, Commands.HEAL });
        actions.Add(new Commands[] { Commands.DODGE, Commands.ATTACK });
        int i = 0;
        try {
            do {
                //Commands[] actions = new Commands[2];
                //actions[0] = playerCompiler.Run(status);
                //actions[1] = enemyCompiler.Run(status);
                status = battleManager.PlayRound(actions[i++]);
                battleStatuses.Add(status);
                if (i > 2) break;
            } while (!status.isOver);
        } catch (ActionTookTooLongException) {
            Debug.Log("ERRO NA BATALHA: O jogador demorou muito para escolher uma ação");
        } catch (MaxNumberOfRoundsException) {
            Debug.Log("ERRO NA BATALHA: A batalha está demorando muito para acabar");
        }
        Debug.Log("Acabou batalha");
        animationManager.StartAnimation(battleStatuses);
        Debug.Log("Acabou batalha2");
    }

    public void SetEnemyMemory(int num) {
        enemyCompiler.memory = memories[num];
    }
}
