using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using UnityEngine;
[System.Serializable]
class IfCell : Cell, IConditionCell
{
    public ComparatorCell comparatorCell { get; set; } = null;
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
                //resolve comparações e EVEN e adiciona o resultado booleano na lista de coisas a resolver
                ConditionalCell aux = this.conditionalList[i].Clone();
                aux.resultado = aux.comparator.Evaluate(battleStatus);
                aux.tipo = typeof(bool);
                toSolve.Add(aux);
            }
            else
            {   
                // adiciona os parentesis e partículas lógicas a resolver
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
        int index = k;
        int index_close_parenthesis = -1;
        while (toSolve.Count > 1)
        {
            Debug.Log($"I am at {index} - {toSolve.Count} restantes - tipo: " + toSolve[index].tipo.ToString());
            if (toSolve[index].tipo == typeof(AndCell))
            {
                //se for um and
                Debug.Log(toSolve[index - 1].tipo);
                Debug.Log(toSolve[index + 1].tipo);
                if (toSolve[index + 1].tipo != typeof(bool))
                {
                    //se o token logo aós o and não for booleano (vai ser um parentesis)
                    //ele adiciona no index e volta para o inicio do while
                    index++;
                    continue;
                }
                //aqui ele calcula o resultado do and e joga no resultado do token anterior
                toSolve[index - 1].resultado = AndCell.Evaluate(toSolve[index - 1].resultado, toSolve[index + 1].resultado);
                //o token anterior entao passa a ser um booleano
                toSolve[index - 1].tipo = typeof(bool);
                //os tokens and e seu posterior são removidos da lista do que fazer
                toSolve.RemoveRange(index,2);
                continue;
            }
            else if (toSolve[index].tipo == typeof(OrCell))
            {
                // se for um or
                Debug.Log(toSolve[index - 1].tipo);
                Debug.Log(toSolve[index + 1].tipo);
                if (toSolve[index + 1].tipo != typeof(bool))
                {
                    //se o token logo aós o and não for booleano (vai ser um parentesis)
                    //ele adiciona no index e volta para o inicio do while
                    index++;
                    continue;
                }
                //aqui ele calcula o resultado do and e joga no resultado do token anterior
                toSolve[index - 1].resultado = OrCell.Evaluate(toSolve[index - 1].resultado, toSolve[index + 1].resultado);
                //o token anterior entao passa a ser um booleano
                toSolve[index - 1].tipo = typeof(bool);
                //os tokens and e seu posterior são removidos da lista do que fazer
                toSolve.RemoveRange(index, 2);
                continue;
            }
            else if (toSolve[index].tipo == typeof(OpenParenthesisCell))
            {
                //se for uma abertura de parentesis

                //ele empilha o index na pilha de parentesis para guardar a posição dele
                stackIndexes.Push(index);
                //e depois aumenta ele
                // index++;
                // continue;
            }
            else if (toSolve[index].tipo == typeof(CloseParenthesisCell))
            {
                //se for um apretesis fechando

                //ele retira um item da pilha de parentesis
                //agora o index interno do parentesis aponta para o parentesis correspondente
                index_close_parenthesis = stackIndexes.Pop();
                Debug.Log($"index: {index_close_parenthesis} - tipo: {toSolve[index_close_parenthesis].tipo}");
                //o resultado desse parentesis é o resultado interno entre ele e o fechamento dele
                toSolve[index_close_parenthesis].resultado = toSolve[index_close_parenthesis+1].resultado;
                // ele, como agora carrega um booleano de resultado é agora um booleano
                toSolve[index_close_parenthesis].tipo = typeof(bool);
                //é removido então o resultado e o fecha parentesis
                toSolve.RemoveRange(index_close_parenthesis+1, 2);
                //o index principal agora é igual a o interno 
                index = index_close_parenthesis;
                if (index_close_parenthesis > 0 && toSolve[index_close_parenthesis-1].tipo == typeof(NotCell))
                {
                    //se a célula anterior for um not o resultado deste é invertido
                    toSolve[index_close_parenthesis].resultado = !toSolve[index_close_parenthesis].resultado;
                    //e então o note é removido da lista
                    toSolve.RemoveRange(index_close_parenthesis-1, 1);
                    //volta um para checar ele mesmo mais uma vez para ves se é nescessário fazer operações de not ou -
                    index--;
                }
                if(index-1>-1){
                    //não lembro pq fiz isso
                    // Debug.Log("entrei");
                    index--;
                    continue;
                }
            }
            else if (index>=1 && toSolve[index - 1].tipo == typeof(NotCell))
            {
                //se o index atual for maior ou igual a 1 e o token anterior for um not
                //o resultado é invertido
                toSolve[index-1].resultado = !toSolve[index].resultado;
                //e este vira um booleano
                toSolve[index-1].tipo = typeof(bool);
                //o not é retirado da lista
                toSolve.RemoveAt(index);
                
                index--;
            }
            else if (index >= 1 && toSolve[index - 1].tipo == typeof(NegativeCell))
            {
                //se o index atual for maior ou igual a 1 e o token anterior for um -
                //o resultado é invertido
                toSolve[index - 1].numericalValue = -1 * toSolve[index].numericalValue;
                //e este vira um float
                toSolve[index - 1].tipo = typeof(float);
                //o not é retirado da lista
                toSolve.RemoveAt(index);
                
                index--;
            }
            index++;
        }
        return toSolve;
    }
    public override Commands GetCommand()
    {
        return Commands.IF;
    }
}
