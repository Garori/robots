using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
class IfCell : Cell, IConditionCell
{
    public ComparatorCell comparatorCell { get; set; } = null;
    //todo lista de comparator cells
    public List<ConditionalCell> conditionalList { get; set; }

    public IfCell(ComparatorCell comparatorCell, int jmp) : base(jmp)
    {
        this.comparatorCell = comparatorCell;
    }

    public IfCell(ComparatorCell comparatorCell) : base()
    {
        this.comparatorCell = comparatorCell;
    }
    
    public IfCell(List<ConditionalCell> conditionalList) : base()
    {
        this.conditionalList = conditionalList;
    }
    public IfCell(List<ConditionalCell> conditionalList, int jmp) : base(jmp)
    {
        this.conditionalList = conditionalList;
    }

    public bool Evaluate(BattleStatus battleStatus)
    {

        if (comparatorCell != null)
        {
            return comparatorCell.Evaluate(battleStatus);
        }
        // string teste = "";
        // foreach (ConditionalCell conditionalCell in this.conditionalList)
        // {
        //     teste += conditionalCell.tipo.ToString()+", ";
        // }
        // Debug.Log("teste: " + teste);
        //todo completar
        // List<ConditionalCell> toSolve = estruturaCellList.Select(a => a.Clone()).ToList();
        List<ConditionalCell> toSolve = new List<ConditionalCell>();
        // {
        //     new ConditionalCell(typeof(OpenParenthesisCell))
        // };
        List<Type> types = new List<Type>{typeof(EqualsCell), typeof(GreaterCell), typeof(GreaterEqualsCell), typeof(EvenCell), typeof(NotEqualsCell), typeof(LesserCell), typeof(LesserEqualsCell) };
        for(int i = 0; i < this.conditionalList.Count; i++)
        {   
            if(types.Contains(this.conditionalList[i].tipo))
            {
                ConditionalCell aux = this.conditionalList[i].Clone();
                aux.resultado = aux.comparator.Evaluate(battleStatus);
                aux.tipo = typeof(bool);
                toSolve.Add(aux);
            }
            else
            {
                toSolve.Add(this.conditionalList[i].Clone());
            }

        }
        // toSolve.Add(new ConditionalCell(typeof(CloseParenthesisCell)));
        bool resultado = Solve(toSolve,0)[0].resultado;
        
        return resultado;
    }

    private List<ConditionalCell> Solve(List<ConditionalCell> toSolve, int k)
    {
        Stack<int> stackIndexes = new Stack<int>();
        int cont = k;
        int index = -1;
        while (toSolve.Count > 1)
        {
            Debug.Log($"I am at {cont} - {toSolve.Count} restantes - tipo: " + toSolve[cont].tipo.ToString());
            if (toSolve[cont].tipo == typeof(AndCell))
            {
                Debug.Log(toSolve[cont - 1].tipo);
                Debug.Log(toSolve[cont + 1].tipo);
                if (toSolve[cont + 1].tipo != typeof(bool))
                {
                    cont++;
                    continue;
                }
                toSolve[cont - 1].resultado = AndCell.Evaluate(toSolve[cont - 1].resultado, toSolve[cont + 1].resultado);
                toSolve[cont - 1].tipo = typeof(bool);
                toSolve.RemoveRange(cont,2);
                continue;
            }
            else if (toSolve[cont].tipo == typeof(OrCell))
            {
                Debug.Log(toSolve[cont - 1].tipo);
                Debug.Log(toSolve[cont + 1].tipo);
                if (toSolve[cont + 1].tipo != typeof(bool))
                {
                    cont++;
                    continue;
                }
                toSolve[cont - 1].resultado = OrCell.Evaluate(toSolve[cont - 1].resultado, toSolve[cont + 1].resultado);
                toSolve[cont - 1].tipo = typeof(bool);
                toSolve.RemoveRange(cont, 2);
                continue;
            }
            else if (toSolve[cont].tipo == typeof(OpenParenthesisCell))
            {
                stackIndexes.Push(cont);
                cont++;
                continue;
            }
            else if (toSolve[cont].tipo == typeof(CloseParenthesisCell))
            {
                index = stackIndexes.Pop();
                Debug.Log($"index: {index} - tipo: {toSolve[index].tipo}");
                toSolve[index].resultado = toSolve[index+1].resultado;
                toSolve[index].tipo = typeof(bool);
                toSolve.RemoveRange(index+1, 2);
                cont = index;
                if (index > 0 && toSolve[index-1].tipo == typeof(NotCell))
                {
                    toSolve[index].resultado = !toSolve[index].resultado;
                    toSolve.RemoveRange(index-1, 1);
                    cont--;
                }
                if(cont-1>-1){
                    Debug.Log("entrei");
                    cont--;
                    continue;
                }
            }
            else if (cont>=1 && toSolve[cont - 1].tipo == typeof(NotCell))
            {
                toSolve[cont-1].resultado = !toSolve[cont].resultado;
                toSolve[cont-1].tipo = typeof(bool);
                toSolve.RemoveAt(cont);
                cont--;
            }
            cont++;
        }
        return toSolve;
    }
    public override Commands GetCommand()
    {
        return Commands.IF;
    }
}
