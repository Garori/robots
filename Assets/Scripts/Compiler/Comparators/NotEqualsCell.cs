public class NotEqualsCell : ComparatorCell {
    private Commands variable1;
    private Commands variable2;

    public NotEqualsCell(Commands variable1, Commands variable2) : base() {
        this.variable1 = variable1;
        this.variable2 = variable2;
    }

    public override bool Evaluate(BattleStatus battleStatus) {
        return battleStatus.values[variable1] != battleStatus.values[variable2];
    }
}
