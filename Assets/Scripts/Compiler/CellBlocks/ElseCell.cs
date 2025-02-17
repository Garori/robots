[System.Serializable]
class ElseCell : Cell
{
    public ElseCell(int jmp) : base(jmp)
    {

    }

    public ElseCell() : base(0)
    {

    }

    public override Commands GetCommand()
    {
        return Commands.ELSE;
    }
}
