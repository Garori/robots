using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour, IFighter {
  [SerializeField] private float maxLifePoints;
  private float lifePoints;
  public float attackPoints { get; set; }
  public float defensePoints { get; set; }
  private bool isDefending;
  private bool isDodging;
  private bool dodgedLastRound;
  private bool healedLastRound;
  private bool chargedLastRound;

  private void Start() {
    attackPoints = 1;
    defensePoints = 3;
    lifePoints = maxLifePoints;
    healedLastRound = chargedLastRound = isDefending = isDodging = dodgedLastRound = false;
  }

  public void attack(Fighter enemy) {
    if (!enemy.isDefending || !enemy.isDodging || !this.dodgedLastRound) {
      enemy.lifePoints -= Mathf.Max(this.attackPoints, 1);
      this.attackPoints = 1;
    }
  }

  public void defend() {
    if (defensePoints > 0) {
      defensePoints -= 1;
      isDefending = true;
    }
  }

  public void charge() {
    attackPoints += chargedLastRound ? 2 : 1;
  }
  public void dodge() {
    isDodging = true;
  }

  public void heal() {
    lifePoints += healedLastRound ? 2 : 1;
    lifePoints = Mathf.Min(lifePoints, maxLifePoints);
  }

  public void executeAction(Commands action, Fighter enemy) {
    switch (action) {
      case Commands.ATTACK:
        attack(enemy);
        dodgedLastRound = healedLastRound = chargedLastRound = false;
        break;
      case Commands.DEFEND:
        defend();
        dodgedLastRound = healedLastRound = chargedLastRound = false;
        break;
      case Commands.CHARGE:
        charge();
        dodgedLastRound = healedLastRound = false;
        chargedLastRound = !chargedLastRound;
        break;
      case Commands.DODGE:
        dodge();
        healedLastRound = chargedLastRound = false;
        dodgedLastRound = true;
        break;
      case Commands.HEAL:
        heal();
        dodgedLastRound = chargedLastRound = false;
        healedLastRound = !healedLastRound;
        break;
    }

  }

  public bool isDead() {
    return this.lifePoints == 0;
  }

  public float GetLife() {
    return this.lifePoints;
  }

}
