using System;

[System.Serializable]
class BreakCell : Cell
{
    public BreakCell(int jmp) : base(jmp)
    {

    }

    public BreakCell() : base(0)
    {

    }
    public override Commands GetCommand()
    {
        return Commands.BREAK;
    }
}
