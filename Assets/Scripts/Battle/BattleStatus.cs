using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStatus {
    public bool isOver { get; set; } = false;
    public Dictionary<Commands, int> values { get; set; }
    public Commands playerAction { get; set; }
    public Commands enemyAction { get; set; }

    public void setBattleStatus(Fighter player, Fighter enemy, bool isOver) {
        values[Commands.PLAYER_ACTUAL_HEALTH] = enemy.lifePoints;
        values[Commands.PLAYER_ACTUAL_HEALTH_HALF] = enemy.lifePoints / 2;
        values[Commands.PLAYER_ACTUAL_HEALTH_DOUBLE] = enemy.lifePoints * 2;
        values[Commands.PLAYER_ACTUAL_SHIELD] = enemy.lifePoints;
        values[Commands.PLAYER_ACTUAL_SHIELD_HALF] = enemy.lifePoints / 2;
        values[Commands.PLAYER_ACTUAL_SHIELD_DOUBLE] = enemy.lifePoints * 2;

        values[Commands.ENEMY_ACTUAL_HEALTH] = enemy.lifePoints;
        values[Commands.ENEMY_ACTUAL_HEALTH_HALF] = enemy.lifePoints / 2;
        values[Commands.ENEMY_ACTUAL_HEALTH_DOUBLE] = enemy.lifePoints * 2;
        values[Commands.ENEMY_ACTUAL_SHIELD] = enemy.lifePoints;
        values[Commands.ENEMY_ACTUAL_SHIELD_HALF] = enemy.lifePoints / 2;
        values[Commands.ENEMY_ACTUAL_SHIELD_DOUBLE] = enemy.lifePoints * 2;

        this.isOver = isOver;
    }
}
