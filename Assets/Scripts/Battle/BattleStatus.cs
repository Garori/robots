using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStatus {
    public bool isOver { get; set; } = false;
    public Dictionary<Commands, int> values { get; set; }
    public Commands playerAction { get; set; }
    public Commands enemyAction { get; set; }
    public bool playerHit { get; set; }
    public bool enemyHit { get; set; }

    public BattleStatus(Fighter player, Fighter enemy, bool isOver = false, Commands playerAction = Commands.START, Commands enemyAction = Commands.START, bool playerHit = false, bool enemyHit = false) {
        values = new Dictionary<Commands, int>();

        values[Commands.PLAYER_ACTUAL_HEALTH] = player.lifePoints;
        values[Commands.PLAYER_ACTUAL_HEALTH_HALF] = player.lifePoints / 2;
        values[Commands.PLAYER_ACTUAL_HEALTH_DOUBLE] = player.lifePoints * 2;
        values[Commands.PLAYER_MAX_HEALTH] = player.getMaxLifePoints();
        values[Commands.PLAYER_MAX_HEALTH_HALF] = player.getMaxLifePoints() / 2;
        values[Commands.PLAYER_MAX_HEALTH_DOUBLE] = player.getMaxLifePoints() * 2;
        values[Commands.PLAYER_ACTUAL_SHIELD] = player.lifePoints;
        values[Commands.PLAYER_ACTUAL_SHIELD_HALF] = player.lifePoints / 2;
        values[Commands.PLAYER_ACTUAL_SHIELD_DOUBLE] = player.lifePoints * 2;

        values[Commands.ENEMY_ACTUAL_HEALTH] = enemy.lifePoints;
        values[Commands.ENEMY_ACTUAL_HEALTH_HALF] = enemy.lifePoints / 2;
        values[Commands.ENEMY_ACTUAL_HEALTH_DOUBLE] = enemy.lifePoints * 2;
        values[Commands.ENEMY_MAX_HEALTH] = enemy.getMaxLifePoints();
        values[Commands.ENEMY_MAX_HEALTH_HALF] = enemy.getMaxLifePoints() / 2;
        values[Commands.ENEMY_MAX_HEALTH_DOUBLE] = enemy.getMaxLifePoints() * 2;
        values[Commands.ENEMY_ACTUAL_SHIELD] = enemy.lifePoints;
        values[Commands.ENEMY_ACTUAL_SHIELD_HALF] = enemy.lifePoints / 2;
        values[Commands.ENEMY_ACTUAL_SHIELD_DOUBLE] = enemy.lifePoints * 2;

        this.isOver = isOver;
        this.playerAction = playerAction;
        this.enemyAction = enemyAction;
        this.playerHit = playerHit;
        this.enemyHit = enemyHit;
    }
}
