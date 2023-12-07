using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Managers")]
    public GameManager manager;

    [Header("Robots")]
    [SerializeField] private Fighter player;
    [SerializeField] private Fighter enemy;

    [Header("Battle Parameters")]
    [SerializeField] private int maxRounds;
    private int round;
    private int isOver;

    private Commands playerAction;
    private Commands enemyAction;

    public BattleStatus InitBattleAttributes(FighterAttributes playerAttributes, FighterAttributes enemyAttributes)
    {
        isOver = 0;
        round = 1;
        player.Init(playerAttributes);
        enemy.Init(enemyAttributes);
        return new BattleStatus(player, enemy);
    }

    public BattleStatus PlayRound(Commands[] actions)
    {
        round++;
        if (round > maxRounds) throw new MaxNumberOfRoundsException();

        playerAction = actions[0];
        enemyAction = actions[1];
        bool[] attacks = execute();
        player.PassTurn();
        enemy.PassTurn();
        checkWin();

        return new BattleStatus(player, enemy, round, isOver, playerAction, enemyAction, attacks[0], attacks[1]);
    }

    private bool[] execute()
    {
        bool playerSuccess, enemySuccess;
        if ((int)playerAction <= (int)enemyAction)
        {
            playerSuccess = player.executeAction(playerAction, enemy);
            enemySuccess = enemy.executeAction(enemyAction, player);
        }
        else
        {
            enemySuccess = enemy.executeAction(enemyAction, player);
            playerSuccess = player.executeAction(playerAction, enemy);
        }

        bool[] attacks = new bool[2];
        attacks[1] = (playerAction == Commands.ATTACK) && playerSuccess;
        attacks[0] = (enemyAction == Commands.ATTACK) && enemySuccess;
        return attacks;
    }

    private void checkWin()
    {
        if (!player.isDead() && !enemy.isDead()) return;

        if (player.isDead()) isOver = -1;
        else isOver = 1;
    }
}
