class ForCell : Cell {
    private Commands variable;
    private int count;
    private bool firstTime;

    public ForCell(Commands variable) : base() {
        this.variable = variable;
        count = -1;
        firstTime = true;
    }

    public override bool getCond() {
        if (firstTime) {
            count = (int)CompilerUtils.GetVariableValue(variable);
        }
        count--;
        return count >= 0;
    }
}
