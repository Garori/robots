[System.Serializable]
class EndCell : Cell
{
    public EndCell(int jmp) : base(jmp)
    {

    }

    public EndCell() : base(0)
    {

    }
    public override Commands GetCommand()
    {
        return Commands.END;
    }
}
