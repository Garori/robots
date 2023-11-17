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
    public Medal medal { get; set; }

    public CellsContainer(Compiler compiler, Dictionary<string, int> variables, Medal medal)
    {
        this.totalCells = compiler.TotalCells;
        this.memory = compiler.Memory;
        this.variables = variables;
        this.medal = medal;
    }

    public void Serialize(string fileName)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream("CustomMemories/" + fileName, FileMode.Create, FileAccess.Write))
        {
            formatter.Serialize(stream, this);
        }

        CellsContainer cellsContainer = Deserialize("CustomMemories/" + fileName);
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

    public void SetMedals(int rounds, int size)
    {
        medal.CheckMedals(rounds, size);
    }
}
