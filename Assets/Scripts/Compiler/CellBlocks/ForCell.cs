using System.Collections.Generic;

[System.Serializable]
class ForCell : Cell, IConditionCell
{
    private Commands variable;
    private int maxCount;
    private int count;
    private int number;
    public List<ConditionalCell> conditionalList { get; set; }
    public ComparatorCell comparatorCell { get; set; } = null;


    public ForCell(Commands variable, int number = -1) : base()
    {
        this.variable = variable;
        this.number = number;
        maxCount = -1;
        count = -1;
    }

    public bool Evaluate(BattleStatus battleStatus)
    {
        if (maxCount == -1)
        {   
            if (variable == Commands.NUMBER){
                maxCount = number;
            }
            else{
                maxCount = battleStatus.values[variable];
            }
            count = maxCount;
        }
        count--;
        bool mustContinue = count >= 0;
        if (count < 0) count = maxCount;
        return mustContinue;
    }

    public override void ResetCell()
    {
        count = maxCount;
    }

    public Commands GetVariable()
    {
        return variable;
    }

    public override Commands GetCommand()
    {
        return Commands.FOR;
    }
}
