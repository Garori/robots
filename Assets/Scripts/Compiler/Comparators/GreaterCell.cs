using System.Collections.Generic;

[System.Serializable]
public class GreaterCell : ComparatorCell
{
    private Commands variable1;
    private Commands variable2;
    private float variable1NUMBER;
    private float variable2NUMBER;

    public GreaterCell(Commands variable1, Commands variable2, float variable1NUMBER = -1, float variable2NUMBER = -1) : base()
    {
        this.variable1 = variable1;
        this.variable2 = variable2;
        this.variable1NUMBER = variable1NUMBER;
        this.variable2NUMBER = variable2NUMBER;
    }

    public override bool Evaluate(BattleStatus battleStatus)
    {
        float aux1 = variable1 != Commands.NUMBER ? battleStatus.values[variable1] : variable1NUMBER;
        float aux2 = variable2 != Commands.NUMBER ? battleStatus.values[variable2] : variable2NUMBER;
        return aux1 > aux2;
    }

    public override List<Commands> GetVariables()
    {
        List<Commands> variables = new List<Commands>();
        variables.Add(variable1);
        variables.Add(variable2);

        return variables;
    }

    public override List<float> GetVariablesNumbers()
    {
        List<float> variables = new List<float>();
        variables.Add(variable1NUMBER);
        variables.Add(variable2NUMBER);

        return variables;
    }

    public override Commands GetCommand()
    {
        return Commands.GREATER;
    }
}

