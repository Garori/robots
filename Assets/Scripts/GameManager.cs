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
  [SerializeField] private Fighter playerScript;
  [SerializeField] private Fighter enemyScript;

  public void compile(List<GameObject> blocks) {
    compilePopupText.SetText("Compilando...");
    compilePopupText.SetText(compiler.Compile(blocks));
  }



  public Commands[] playerActions = new Commands[]{
      Commands.ATTACK,
      Commands.ATTACK,
      Commands.ATTACK,
      Commands.ATTACK,
      Commands.ATTACK,
      Commands.ATTACK
    };
  public Commands[] enemyActions = new Commands[]{
      Commands.HEAL,
      Commands.CHARGE,
      Commands.HEAL,
      Commands.HEAL,
      Commands.CHARGE,
      Commands.ATTACK,
    };




  public Commands getPlayerAction() {
    return playerActions[battleManager.round - 1];
  }

  public Commands getEnemyAction() {
    return enemyActions[battleManager.round - 1];
  }


}
