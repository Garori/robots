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
    public float numericalValue;

    public ConditionalCell(Type tipo, ComparatorCell comparator = null, string str = null, Cell celula = null, bool resultado = false, float numericalValue = 0)
    {
        this.tipo = tipo;
        this.comparator = comparator;
        this.str = str;
        this.celula = celula;
        this.resultado = resultado;
        this.numericalValue = numericalValue;
    }
    public ConditionalCell Clone()
    {
        return new ConditionalCell(tipo,comparator,str,celula,resultado);
    }
}
