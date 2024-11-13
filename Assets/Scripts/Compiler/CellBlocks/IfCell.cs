[System.Serializable]
class IfCell : Cell, IConditionCell
{
    public ComparatorCell comparatorCell { get; set; }
    //todo lista de comparator cells

    public IfCell(ComparatorCell comparatorCell, int jmp) : base(jmp)
    {
        this.comparatorCell = comparatorCell;
    }

    public IfCell(ComparatorCell comparatorCell) : base()
    {
        this.comparatorCell = comparatorCell;
    }

    public bool Evaluate(BattleStatus battleStatus)
    //todo for each comparator cell in list comparator cells
    //todo fazer um for(int i == 1, i< lineCommands.Count, i++) que leia de 1 em um bloquinho
    //todo sendo '>','<','=='... lê os próximos dois 
    //todo sendo '&&','||' levanta flag de calcular quando o próximo estiver pronto
    //todo sendo '(' segura os resultados até achar um ')'
    {
        return comparatorCell.Evaluate(battleStatus);
    }
    public override Commands GetCommand()
    {
        return Commands.IF;
    }
}
