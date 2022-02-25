class ActionCell : Cell {
    private Commands action;

    public ActionCell(Commands action) : base() {
        this.action = action;
    }

    public override string ToString() {
        return action.ToString();
    }
}
