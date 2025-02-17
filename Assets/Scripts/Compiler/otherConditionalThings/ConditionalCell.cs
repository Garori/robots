using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ConditionalCell
{
    public Type tipo;
    public ComparatorCell comparator;
    public string str;
    public Cell celula;
    public bool resultado;

    public ConditionalCell(Type tipo, ComparatorCell comparator = null, string str = null, Cell celula = null, bool resultado = false)
    {
        this.tipo = tipo;
        this.comparator = comparator;
        this.str = str;
        this.celula = celula;
        this.resultado = resultado;
    }
    public ConditionalCell Clone()
    {
        return new ConditionalCell(tipo,comparator,str,celula,resultado);
    }
}
