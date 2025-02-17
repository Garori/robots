[System.Serializable]
class AfterEndCell : Cell
{
    public AfterEndCell(int jmp) : base(jmp)
    {

    }

    public AfterEndCell() : base(0)
    {

    }
    public override Commands GetCommand()
    {
        return Commands.NONE;
    }
}
