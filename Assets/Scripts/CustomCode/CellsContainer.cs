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

    public CellsContainer(Compiler compiler)
    {
        totalCells = compiler.TotalCells;
        memory = compiler.Memory;
    }

    public void Serialize(string fileName)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream("CustomCodes/" + fileName, FileMode.Create, FileAccess.Write))
        {
            formatter.Serialize(stream, this);
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
