[System.Serializable]
class WhileCell : Cell, IConditionCell
{
    public ComparatorCell comparatorCell { get; set; }

    public WhileCell(ComparatorCell comparatorCell, int jmp) : base(jmp)
    {
        this.comparatorCell = comparatorCell;
    }

    public WhileCell(ComparatorCell comparatorCell) : base()
    {
        this.comparatorCell = comparatorCell;
    }

    public bool Evaluate(BattleStatus battleStatus)
    {
        return comparatorCell.Evaluate(battleStatus);
    }

    public override Commands GetCommand()
    {
        return Commands.WHILE;
    }
}
