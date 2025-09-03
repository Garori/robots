using System.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class CellsContainer
{
    public string fileName;
    public bool isCustom;
    public int totalCells;
    public Cell[] memory { get; set; }
    public FighterAttributes playerFighterAttributes { get; set; }
    public FighterAttributes enemyFighterAttributes { get; set; }
    public List<FighterAttributes> testesPlayer { get; set; }
    public List<FighterAttributes> testesEnemy { get; set; }

    public Medal medal { get; set; }
    public string hint { get; set; }
    public bool[] enabledBlocks { get; set; }
    public bool isLevelCleared { get; set; }
    public Cell[] lastUserCode { get; set; }

    public CellsContainer(
        Compiler compiler,
        FighterAttributes playerFighterAttributes,
        FighterAttributes enemyFighterAttributes,
        Medal medal,
        bool[] enabledBlocks,
        string hint,
        List<FighterAttributes> testesPlayer= null,
        List<FighterAttributes> testesEnemy = null,
        bool isCustom = true
    )
    {
        this.isCustom = isCustom;
        this.totalCells = compiler.TotalCells;
        this.memory = compiler.Memory;
        this.playerFighterAttributes = playerFighterAttributes;
        this.enemyFighterAttributes = enemyFighterAttributes;
        this.testesPlayer = testesPlayer ?? new List<FighterAttributes>();
        this.testesEnemy = testesEnemy ?? new List<FighterAttributes>();
        this.medal = medal;
        this.hint = hint;
        this.enabledBlocks = enabledBlocks;
    }

    public void Serialize(string fileName)
    {
        IFormatter formatter = new BinaryFormatter();
#if UNITY_WEBGL
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, this);
                string save_string64 = System.Convert.ToBase64String(ms.ToArray());
                Debug.Log(fileName);
                PlayerPrefs.SetString(fileName, save_string64);
                PlayerPrefs.Save();
            }
#else
        using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, this);
            }
#endif
    }

    public static CellsContainer Deserialize(string fileName)
    {
        IFormatter formatter = new BinaryFormatter();
#if UNITY_WEBGL
        try
        {
            string save_string64 = PlayerPrefs.GetString(fileName); ;
            byte[] save_bytes = System.Convert.FromBase64String(save_string64);
            using (MemoryStream ms = new MemoryStream(save_bytes))
            {
                ms.Position = 0;
                CellsContainer memory = (CellsContainer)formatter.Deserialize(ms);
                return memory;
            }
        }
        catch (Exception ex)
        {
            return null;    
        }
            
#else
        using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                CellsContainer memory = (CellsContainer)formatter.Deserialize(stream);
                memory.fileName = fileName;

                return memory;
            }
#endif

    }

    public void UpdateFile()
    {
        Serialize(fileName);
    }

    public void SetMedals(int rounds, int size)
    {
        medal.CheckMedals(rounds, size);
    }

    public void SetWin(bool won)
    {
        isLevelCleared = isLevelCleared || won;
    }

    public void SetLastCode(Cell[] lastUserCode)
    {
        this.lastUserCode = lastUserCode;
    }

    public bool isBlockEnabled(Commands command)
    {
        return enabledBlocks[(int)command];
    }
}
