using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FighterAttributes
{
    public int maxLifePoints { get; set; }
    public int minAttackPoints { get; set; }
    public int maxDefensePoints { get; set; }
    public int maxChargePoints { get; set; }

    public FighterAttributes(int maxLifePoints, int minAttackPoints, int maxDefensePoints, int maxChargePoints)
    {
        this.maxLifePoints = maxLifePoints;
        this.minAttackPoints = minAttackPoints;
        this.maxDefensePoints = maxDefensePoints;
        this.maxChargePoints = maxChargePoints;
    }

    public override string ToString()
    {
        return "Max Life Points: " + maxLifePoints + "\nMin Attack Points: " + minAttackPoints + "\nMax Defense Points: " + maxDefensePoints + "\nMax Charge Points: " + maxChargePoints;
    }
}
