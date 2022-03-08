using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour, IFighter {
    [Header("Starting Attributes")]
    [SerializeField] private int maxLifePoints;
    [SerializeField] private int minAttackPoints;
    [SerializeField] private int maxDefensePoints;

    public int lifePoints { get; set; }
    public int attackPoints { get; set; }
    public int defensePoints { get; set; }
    private bool isDefending;
    private bool isDodging;
    private bool dodgedLastRound;
    private bool healedLastRound;
    private bool chargedLastRound;

    private void Start() {
        lifePoints = maxLifePoints;
        attackPoints = minAttackPoints;
        defensePoints = maxDefensePoints;
        healedLastRound = chargedLastRound = isDefending = isDodging = dodgedLastRound = false;
    }

    private void Reset() {
        healedLastRound = chargedLastRound = isDefending = isDodging = dodgedLastRound = false;
    }

    public int getMaxLifePoints() {
        return maxLifePoints;
    }

    public bool attack(Fighter enemy) {
        if (this.dodgedLastRound) return false;
        if (!enemy.isDefending && !enemy.isDodging) {
            enemy.lifePoints -= Mathf.Max(this.attackPoints, 1);
            this.attackPoints = 1;
        }
        Reset();
        return true;
    }

    public bool defend() {
        Reset();
        if (defensePoints > 0) {
            defensePoints -= 1;
            isDefending = true;
            return true;
        }
        return false;
    }

    public bool charge() {
        attackPoints += chargedLastRound ? 2 : 1;
        Reset();
        chargedLastRound = true;
        return true;
    }
    public bool dodge() {
        Reset();
        dodgedLastRound = true;
        return true;
    }

    public bool heal() {
        lifePoints += healedLastRound ? 2 : 1;
        lifePoints = Mathf.Min(lifePoints, maxLifePoints);
        Reset();
        healedLastRound = true;
        return true;
    }

    public bool executeAction(Commands action, Fighter enemy) {
        bool success = true;
        switch (action) {
            case Commands.ATTACK:
                success = attack(enemy);
                dodgedLastRound = healedLastRound = chargedLastRound = false;
                break;
            case Commands.DEFEND:
                success = defend();
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
        return success;
    }

    public bool isDead() {
        return this.lifePoints == 0;
    }
}
