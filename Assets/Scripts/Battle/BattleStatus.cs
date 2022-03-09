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
    public int round { get; set; }

    public BattleStatus(Fighter player, Fighter enemy, int round = 1, bool isOver = false, Commands playerAction = Commands.START, Commands enemyAction = Commands.START, bool playerHit = false, bool enemyHit = false) {
        values = new Dictionary<Commands, int>();
        values[Commands.ZERO] = 0;

        values[Commands.PLAYER_ACTUAL_HEALTH] = player.GetLifePoints();
        values[Commands.PLAYER_ACTUAL_HEALTH_HALF] = player.GetLifePoints() / 2;
        values[Commands.PLAYER_ACTUAL_HEALTH_DOUBLE] = player.GetLifePoints() * 2;
        values[Commands.PLAYER_MAX_HEALTH] = player.getMaxLifePoints();
        values[Commands.PLAYER_MAX_HEALTH_HALF] = player.getMaxLifePoints() / 2;
        values[Commands.PLAYER_MAX_HEALTH_DOUBLE] = player.getMaxLifePoints() * 2;
        values[Commands.PLAYER_ACTUAL_SHIELD] = player.GetDefensePoints();
        values[Commands.PLAYER_ACTUAL_SHIELD_HALF] = player.GetDefensePoints() / 2;
        values[Commands.PLAYER_ACTUAL_SHIELD_DOUBLE] = player.GetDefensePoints() * 2;
        values[Commands.PLAYER_ACTUAL_CHARGE] = player.GetChargePoints();
        values[Commands.PLAYER_ACTUAL_CHARGE_HALF] = player.GetChargePoints() / 2;
        values[Commands.PLAYER_ACTUAL_CHARGE_DOUBLE] = player.GetChargePoints() * 2;

        values[Commands.ENEMY_ACTUAL_HEALTH] = enemy.GetLifePoints();
        values[Commands.ENEMY_ACTUAL_HEALTH_HALF] = enemy.GetLifePoints() / 2;
        values[Commands.ENEMY_ACTUAL_HEALTH_DOUBLE] = enemy.GetLifePoints() * 2;
        values[Commands.ENEMY_MAX_HEALTH] = enemy.getMaxLifePoints();
        values[Commands.ENEMY_MAX_HEALTH_HALF] = enemy.getMaxLifePoints() / 2;
        values[Commands.ENEMY_MAX_HEALTH_DOUBLE] = enemy.getMaxLifePoints() * 2;
        values[Commands.ENEMY_ACTUAL_SHIELD] = enemy.GetDefensePoints();
        values[Commands.ENEMY_ACTUAL_SHIELD_HALF] = enemy.GetDefensePoints() / 2;
        values[Commands.ENEMY_ACTUAL_SHIELD_DOUBLE] = enemy.GetDefensePoints() * 2;
        values[Commands.ENEMY_ACTUAL_CHARGE] = enemy.GetChargePoints();
        values[Commands.ENEMY_ACTUAL_CHARGE_HALF] = enemy.GetChargePoints() / 2;
        values[Commands.ENEMY_ACTUAL_CHARGE_DOUBLE] = enemy.GetChargePoints() * 2;

        this.round = round;
        this.isOver = isOver;
        this.playerAction = playerAction;
        this.enemyAction = enemyAction;
        this.playerHit = playerHit;
        this.enemyHit = enemyHit;
    }
}
