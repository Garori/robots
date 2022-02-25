class WhileCell : Cell {
    public int condId { get; set; }

    public WhileCell(int condId) : base() {
        this.condId = condId;
    }

    public override bool getCond() {
        //return Utils.getCond(condId);
        return true;
    }
}
