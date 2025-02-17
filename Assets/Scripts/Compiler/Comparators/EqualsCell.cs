using System.Collections.Generic;
using System.Runtime.InteropServices;

[System.Serializable]
public class EqualsCell : ComparatorCell
{
    private Commands variable1 = Commands.NUMBER;
    private Commands variable2 = Commands.NUMBER;

    private int variable1INT;
    private int variable2INT;

    public EqualsCell(Commands variable1, Commands variable2, int variable1INT = -1, int variable2INT = -1) : base()
    {
        this.variable1 = variable1;
        this.variable2 = variable2;
        this.variable1INT = variable1INT;
        this.variable2INT = variable2INT;
    }

    public override bool Evaluate(BattleStatus battleStatus)
    {
        int aux1 = variable1 != Commands.NUMBER ? battleStatus.values[variable1] : variable1INT;
        int aux2 = variable2 != Commands.NUMBER ? battleStatus.values[variable2] : variable2INT;

        
        return  aux1 == aux2;
        // return variable1INT == variable2INT;
    }

    public override List<Commands> GetVariables()
    {
        List<Commands> variables = new List<Commands>();
        variables.Add(variable1);
        variables.Add(variable2);

        return variables; 
    }
    public override List<int> GetVariablesInt()
    {
        List<int> variables = new List<int>();
        variables.Add(variable1INT);
        variables.Add(variable2INT);

        return variables;
    }

    public override Commands GetCommand()
    {
        return Commands.EQUALS;
    }
}
