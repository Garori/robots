using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LesserEqualsCell : ComparatorCell
{
    private Commands variable1;
    private Commands variable2;
    private int variable1INT;
    private int variable2INT;

    public LesserEqualsCell(Commands variable1, Commands variable2, int variable1INT = -1, int variable2INT = -1) : base()
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
        Debug.Log($"dentro do lesser equals aux1 = {aux1} e aux2 = {aux2} -> {aux1 <= aux2}");
        return aux1 <= aux2;
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
        return Commands.LESSER_EQUALS;
    }
}

