[System.Serializable]
public class EvenCell : ComparatorCell
{
    private Commands variable;
    private int variableINT;

    public EvenCell(Commands variable, int variableINT = -1) : base()
    {
        this.variable = variable;
        this.variableINT = variableINT;
    }

    public override bool Evaluate(BattleStatus battleStatus)
    {
        int aux = variable == Commands.NUMBER ? variableINT : battleStatus.values[variable];
        return aux % 2 == 0;
    }
    public Commands GetVariable()
    {
        return variable;
    }

    public int GetVariableInt()
    {
        return variableINT;
    }

    public override Commands GetCommand()
    {
        return Commands.EVEN;
    }
}
