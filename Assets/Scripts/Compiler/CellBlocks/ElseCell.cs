class ElseCell : Cell {
    private int condId;

    public ElseCell(int condId) : base() {
        this.condId = condId;
    }

    public override bool getCond() {
        //return !Utils.getCond(condId);
        return false;
    }
}
