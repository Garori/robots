using System.Collections.Generic;

[System.Serializable]
public class GreaterCell : ComparatorCell
{
    private Commands variable1;
    private Commands variable2;

    public GreaterCell(Commands variable1, Commands variable2) : base()
    {
        this.variable1 = variable1;
        this.variable2 = variable2;
    }

    public override bool Evaluate(BattleStatus battleStatus)
    {
        return battleStatus.values[variable1] > battleStatus.values[variable2];
    }

    public override List<Commands> GetVariables()
    {
        List<Commands> variables = new List<Commands>();
        variables.Add(variable1);
        variables.Add(variable2);

        return variables;
    }

    public override Commands GetCommand()
    {
        return Commands.GREATER;
    }
}

