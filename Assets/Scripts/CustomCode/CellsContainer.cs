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

    public void Serialize()
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream("CellsContainer.bin", FileMode.Create, FileAccess.Write))
        {
            formatter.Serialize(stream, this);
        }
    }

    public static CellsContainer Deserialize()
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream("CellsContainer.bin", FileMode.Open, FileAccess.Read))
        {
            return (CellsContainer)formatter.Deserialize(stream);
        }
    }
}
