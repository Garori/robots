[System.Serializable]
public abstract class Cell
{
    public int jmp { get; set; }

    public Cell()
    {

    }

    public Cell(int jmp)
    {
        this.jmp = jmp;
    }

    public virtual bool getCond()
    {
        return false;
    }

    public virtual void ResetCell()
    {
        
    }
    public abstract Commands GetCommand();
}
