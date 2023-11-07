using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class CellsContainer
{
    public int totalCells;
    public Cell[] memory { get; set; }
    public Dictionary<string, int> variables { get; set; }

    public CellsContainer(Compiler compiler, Dictionary<string, int> variables)
    {
        this.totalCells = compiler.TotalCells;
        this.memory = compiler.Memory;
        this.variables = variables;
    }

    public void Serialize(string fileName)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream("CustomCodes/" + fileName, FileMode.Create, FileAccess.Write))
        {
            formatter.Serialize(stream, this);
        }

        CellsContainer cellsContainer = Deserialize("CustomCodes/" + fileName);
        // print every variable
        foreach (KeyValuePair<string, int> variable in cellsContainer.variables)
        {
            Debug.Log(variable.Key + ": " + variable.Value);
        }
    }

    public static CellsContainer Deserialize(string fileName)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            return (CellsContainer)formatter.Deserialize(stream);
        }
    }
}
