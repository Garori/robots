[System.Serializable]
class IfCell : Cell, IConditionCell
{
    public ComparatorCell comparatorCell { get; set; }

    public IfCell(ComparatorCell comparatorCell, int jmp) : base(jmp)
    {
        this.comparatorCell = comparatorCell;
    }

    public IfCell(ComparatorCell comparatorCell) : base()
    {
        this.comparatorCell = comparatorCell;
    }

    public bool Evaluate(BattleStatus battleStatus)
    {
        return comparatorCell.Evaluate(battleStatus);
    }
}
