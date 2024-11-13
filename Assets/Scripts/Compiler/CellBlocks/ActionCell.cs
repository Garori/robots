[System.Serializable]
class ActionCell : Cell
{
    public Commands action { get; set; }

    public ActionCell(Commands action) : base()
    {
        this.action = action;
    }

    public override string ToString()
    {
        return action.ToString();
    }
    public override Commands GetCommand()
    {
        return Commands.NONE;
    }
}
