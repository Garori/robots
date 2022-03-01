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

    private BattleStatus status;

    private void Start() {
        status = new BattleStatus();
    }

    public BattleStatus RunBattle() {
        isOver = false;
        round = 1;
        status.setBattleStatus(player, enemy, isOver);
        return status;
    }

    public BattleStatus PlayRound(Commands[] actions) {
        if (isOver) return status;

        playerAction = actions[0];
        enemyAction = actions[1];
        execute();
        checkWin();
        round++;
        status.setBattleStatus(player, enemy, isOver);
        if (round > maxRounds) throw new MaxNumberOfRoundsException();
        return status;
    }

    private void execute() {
        if ((int) playerAction <= (int) enemyAction) {
            player.executeAction(playerAction, enemy);
            enemy.executeAction(enemyAction, player);
            Debug.Log("PLAYER USED " + playerAction);
            Debug.Log("ENEMY USED " + enemyAction);
        } else {
            enemy.executeAction(enemyAction, player);
            player.executeAction(playerAction, enemy);
            Debug.Log("ENEMY USED " + enemyAction);
            Debug.Log("PLAYER USED " + playerAction);
        }      
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
