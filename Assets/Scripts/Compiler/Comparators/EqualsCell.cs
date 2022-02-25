public class EqualsCell : ComparatorCell {
    private Commands variable1;
    private Commands variable2;

    public EqualsCell(Commands variable1, Commands variable2) : base() {
        this.variable1 = variable1;
        this.variable2 = variable2;
    }

    public override bool getCond() {
        return CompilerUtils.GetVariableValue(variable1) == CompilerUtils.GetVariableValue(variable2);
    }
}
