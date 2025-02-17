using System;

[System.Serializable]
class CodeCell : Cell
{
    public string CodeWithin;
    public CodeCell(int jmp, string CodeWithin = "") : base(jmp)
    {
        this.CodeWithin = CodeWithin;
    }

    public CodeCell(string CodeWithin = "") : base(0)
    {
        this.CodeWithin = CodeWithin;
    }
    public override Commands GetCommand()
    {
        return Commands.CODE;
    }
}
