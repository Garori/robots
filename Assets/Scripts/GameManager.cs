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
        BattleStatus status = battleManager.RunBattle();
        try {
            do {
                Commands[] actions = new Commands[2];
                actions[0] = playerCompiler.Run(status);
                actions[1] = enemyCompiler.Run(status);
                status = battleManager.PlayRound(actions);
            } while (!status.isOver);
        } catch (ActionTookTooLongException) {
            Debug.Log("ERRO NA BATALHA: O jogador demorou muito para escolher uma ação");
        } catch (MaxNumberOfRoundsException) {
            Debug.Log("ERRO NA BATALHA: A batalha está demorando muito para acabar");
        }
        // Passa para o animador o que aconteceu
    }
}
