using System.Collections.Generic;

public interface IConditionCell
{
    public List<ConditionalCell> conditionalList { get; set; }
    public ComparatorCell comparatorCell { get; set; }
    public bool Evaluate(BattleStatus battleStatus);
}
