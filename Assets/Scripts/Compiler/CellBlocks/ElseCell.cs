class ElseCell : Cell, IConditionCell {
    public ComparatorCell comparatorCell { get; set; }

    public ElseCell(ComparatorCell comparatorCell) : base() {
        this.comparatorCell = comparatorCell;
    }

    public bool Evaluate(BattleStatus battleStatus) {
        return !comparatorCell.Evaluate(battleStatus);
    }
}
