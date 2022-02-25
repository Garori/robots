class IfCell : Cell {
    public int condId { get; set; }

    public IfCell(int condId) : base() {
        this.condId = condId;
    }

    public override bool getCond() {
        // return Utils.getCond(condId);
        return true;
    }
}
