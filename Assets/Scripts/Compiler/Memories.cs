using System;
using System.Linq;
using UnityEditor.Experimental;
using UnityEngine;

public static class Memories
{
    private static CellsContainer[] memories;
    private static CellsContainer[] customMemories;
    private static bool toEditLevelSelect { get; set; }

    private static bool isNewLevel { get; set; }
    public static int memoriesLength
    {
        get { return memories.Length; }
    }
    public static int customMemoriesLength
    {
        get { return customMemories.Length; }
    }

    static Memories()
    {
        UpdateMemories();
    }

    public static CellsContainer[] GetFiles(string folderName)
    {
#if UNITY_WEBGL
        int level_number = 0;
        string[] fileNames = new string[0];
        while (true)
        {
            level_number++;

            if (PlayerPrefs.HasKey($"{folderName}/{level_number}"))
            {
                Debug.Log("has key = " + level_number);
                fileNames = fileNames.Append($"{folderName}/{level_number}").ToArray();
            }
            else
            {
                Debug.Log("era para dar break");
                break;
            }
        }
        Debug.Log("filenames length = " + fileNames.Length);
#else
        if (!System.IO.Directory.Exists(folderName))
            return new CellsContainer[0];
        string[] fileNames = System.IO.Directory.GetFiles(folderName);
#endif


        CellsContainer[] memories = new CellsContainer[fileNames.Length];
        for (int i = 0; i < fileNames.Length; i++)
        {
            memories[i] = CellsContainer.Deserialize(fileNames[i]);
            int k = 0;

            foreach (bool a in memories[i].enabledBlocks)
            {
                // Debug.Log($"{i} {a} {k}");
                k++;
            }
        }
        return memories;
    }


    public static CellsContainer GetMemory(int level)
    {
        if (level < memoriesLength)
        {
            return memories[level];
        }
        else
        {
            return customMemories[level - memoriesLength];
        }
    }

    public static void UpdateMemories()
    {
        memories = GetFiles("Memories");
        customMemories = GetFiles("CustomMemories");
    }

    public static void setToEdit(bool editValue)
    {
        toEditLevelSelect = editValue;
    }

    public static bool getToEdit()
    {
        return toEditLevelSelect;
    }

    public static void setNewLevel(bool editValue)
    {
        isNewLevel = editValue;
    }

    public static bool getNewLevel()
    {
        return isNewLevel;
    }

}
