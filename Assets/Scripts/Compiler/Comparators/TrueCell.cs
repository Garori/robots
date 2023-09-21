[System.Serializable]
public class TrueCell : ComparatorCell
{
    public override bool Evaluate(BattleStatus battleStatus)
    {
        return true;
    }
}
