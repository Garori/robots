[System.Serializable]
class ForCell : Cell, IConditionCell
{
    private Commands variable;
    private int maxCount;
    private int count;

    public ForCell(Commands varible) : base()
    {
        this.variable = varible;
        maxCount = -1;
        count = -1;
    }

    public bool Evaluate(BattleStatus battleStatus)
    {
        if (maxCount == -1)
        {
            maxCount = battleStatus.values[variable];
            count = maxCount;
        }
        count--;
        bool mustContinue = count >= 0;
        if (count < 0) count = maxCount;
        return mustContinue;
    }
}
