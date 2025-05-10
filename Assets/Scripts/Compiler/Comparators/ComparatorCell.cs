using System.Collections.Generic;

[System.Serializable]
public abstract class ComparatorCell
{
    public ComparatorCell()
    {

    }

    public abstract bool Evaluate(BattleStatus battleStatus);
    public virtual List<Commands> GetVariables()
    {
        return null;
    }
    public virtual List<float> GetVariablesNumbers()
    {
        return null;
    }

    public abstract Commands GetCommand();
}
