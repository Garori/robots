public class EvenCell : ComparatorCell {
    private Commands variable;

    public EvenCell(Commands variable) : base() {
        this.variable = variable;
    }

    public override bool getCond() {
        return CompilerUtils.GetVariableValue(variable) % 2 == 0;
    }
}
