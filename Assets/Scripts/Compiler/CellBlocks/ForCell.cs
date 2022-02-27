class ForCell : Cell {
    private Commands variable;
    private int count;
    private bool firstTime;

    public ForCell(Commands variable) : base() {
        this.variable = variable;
        count = -1;
        firstTime = true;
    }

    /*public override bool Evaluate() {
        if (firstTime) {
            count = Com.GetVariableValue(variable);
        }
        count--;
        return count >= 0;
    }*/
}
