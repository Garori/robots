using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {
  public GameManager manager;
  public BattleStatus status;
  public int round;
  [SerializeField] private Fighter player;
  [SerializeField] private Fighter enemy;
  private Commands playerAction;
  private Commands enemyAction;
  public bool isOver;

  public void RunBattle() {
    isOver = false;
    round = 1;
    status.setBattleStatus(player, enemy, isOver);
  }
  public BattleStatus setActions(Commands[] actions) {
    if (isOver) return status;

    playerAction = actions[0];
    enemyAction = actions[1];
    execute();
    checkWin();
    round++;
    status.setBattleStatus(player, enemy, isOver);
    return status;
  }

  private void execute() {
    player.executeAction(playerAction, enemy);
    enemy.executeAction(enemyAction, player);
    Debug.Log("PLAYER USED " + playerAction);
    Debug.Log("ENEMY USED " + enemyAction);
    Debug.Log("=====================================");
    Debug.Log("");
  }

  private void checkWin() {
    if (!player.isDead() && !enemy.isDead()) return;

    printBattle();
    Debug.Log("=====================================");
    Debug.Log("");

    isOver = true;
    if (enemy.isDead()) Debug.Log("YOU WIN!!! =)");
    else Debug.Log("YOU LOSE. =(");
  }

  private void printBattle() {
    Debug.Log("ROUND " + round);
    Debug.Log("");
    Debug.Log("PLAYER ");
    Debug.Log("LIFE: " + player.GetLife());
    Debug.Log("ATTACK: " + player.attackPoints);
    Debug.Log("DEFENSE: " + player.defensePoints);
    Debug.Log("");
    Debug.Log("ENEMY ");
    Debug.Log("LIFE: " + enemy.GetLife());
    Debug.Log("ATTACK: " + enemy.attackPoints);
    Debug.Log("DEFENSE: " + enemy.defensePoints);
    Debug.Log("");
  }

}
