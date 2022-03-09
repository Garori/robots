using UnityEngine;

public class GreaterCell : ComparatorCell {
    private Commands variable1;
    private Commands variable2;

    public GreaterCell(Commands variable1, Commands variable2) : base() {
        this.variable1 = variable1;
        this.variable2 = variable2;
    }

    public override bool Evaluate(BattleStatus battleStatus) {
        Debug.Log($"Evaluating:\n{variable1}:{battleStatus.values[variable1]}\n{variable2}:{battleStatus.values[variable2]}");
        return battleStatus.values[variable1] > battleStatus.values[variable2];
    }
}
