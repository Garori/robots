[System.Serializable]
public class EvenCell : ComparatorCell
{
    private Commands variable;

    public EvenCell(Commands variable) : base()
    {
        this.variable = variable;
    }

    public override bool Evaluate(BattleStatus battleStatus)
    {
        return battleStatus.values[variable] % 2 == 0;
    }
    public Commands GetVariable()
    {
        return variable;
    }
    public override Commands GetCommand()
    {
        return Commands.EVEN;
    }
}
