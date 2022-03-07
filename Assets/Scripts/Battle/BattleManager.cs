using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    [Header("Managers")]
    public GameManager manager;

    [Header("Robots")]
    [SerializeField] private Fighter player;
    [SerializeField] private Fighter enemy;

    [Header("Battle Parameters")]
    [SerializeField] private int maxRounds;
    private int round;
    private bool isOver;

    private Commands playerAction;
    private Commands enemyAction;

    public BattleStatus RunBattle() {
        isOver = false;
        round = 1;
        return new BattleStatus(player, enemy, isOver, Commands.START, Commands.START);
    }

    public BattleStatus PlayRound(Commands[] actions) {
        round++;
        if (round > maxRounds) throw new MaxNumberOfRoundsException();

        playerAction = actions[0];
        enemyAction = actions[1];
        //execute();
        //checkWin();

        return new BattleStatus(player, enemy, isOver, playerAction, enemyAction);
    }

    private void execute() {
        bool playerSuccess, enemySuccess;
        if ((int)playerAction <= (int)enemyAction) {
            playerSuccess = player.executeAction(playerAction, enemy);
            enemySuccess = enemy.executeAction(enemyAction, player);
            Debug.Log("PLAYER USED " + playerAction);
            Debug.Log("ENEMY USED " + enemyAction);
        } else {
            enemySuccess = enemy.executeAction(enemyAction, player);
            playerSuccess = player.executeAction(playerAction, enemy);
            Debug.Log("ENEMY USED " + enemyAction);
            Debug.Log("PLAYER USED " + playerAction);
        }
    }

    private void checkWin() {
        if (!player.isDead() && !enemy.isDead()) return;

        //printBattle();

        isOver = true;
        if (enemy.isDead()) Debug.Log("YOU WIN!!! =)");
        else Debug.Log("YOU LOSE. =(");
    }

    private void printBattle() {
        Debug.Log("ROUND " + round);
        Debug.Log("");
        Debug.Log("PLAYER ");
        Debug.Log("LIFE: " + player.lifePoints);
        Debug.Log("ATTACK: " + player.attackPoints);
        Debug.Log("DEFENSE: " + player.defensePoints);
        Debug.Log("");
        Debug.Log("ENEMY ");
        Debug.Log("LIFE: " + enemy.lifePoints);
        Debug.Log("ATTACK: " + enemy.attackPoints);
        Debug.Log("DEFENSE: " + enemy.defensePoints);
        Debug.Log("");
    }

}
