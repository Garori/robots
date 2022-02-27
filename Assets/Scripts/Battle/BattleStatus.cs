using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStatus : MonoBehaviour {
  public bool isOver = false;
  public float playerLife { get; set; }
  public float playerDefense { get; set; }
  public float playerAttack { get; set; }
  public float enemyLife { get; set; }
  public float enemyDefense { get; set; }
  public float enemyAttack { get; set; }

  public void setBattleStatus(Fighter player, Fighter enemy, bool isOver) {
    playerLife = player.GetLife();
    playerDefense = player.defensePoints;
    playerAttack = player.attackPoints;

    enemyLife = enemy.GetLife();
    enemyDefense = enemy.defensePoints;
    enemyAttack = enemy.attackPoints;

    this.isOver = isOver;
  }
}
