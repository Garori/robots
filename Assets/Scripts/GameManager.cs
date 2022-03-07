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
        actions.Add(new Commands[] { Commands.CHARGE, Commands.HEAL });
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
}
