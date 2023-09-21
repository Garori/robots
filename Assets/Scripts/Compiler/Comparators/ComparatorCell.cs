[System.Serializable]
public abstract class ComparatorCell
{
    public ComparatorCell()
    {

    }

    public abstract bool Evaluate(BattleStatus battleStatus);
}
