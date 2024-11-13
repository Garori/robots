using System.IO.MemoryMappedFiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private ButtonController levelButtonPrefab;

    [SerializeField]
    private ButtonController levelClearedButtonPrefab;

    [SerializeField]
    private ButtonController customLevelButtonPrefab;

    [SerializeField]
    private ButtonController customLevelClearedButtonPrefab;
    
    [SerializeField]
    private GameObject createLevelButton;

    private void Start()
    {
        if(!Memories.getToEdit())
        {
            createLevelButton.SetActive(false);
        }
        Memories.UpdateMemories();

        for (int i = 0; i < Memories.memoriesLength; i++)
        {
            ButtonController prefab;
            CellsContainer memory = Memories.GetMemory(i);
            if (memory.isLevelCleared)
            {
                prefab = levelClearedButtonPrefab;
            }
            else
            {
                prefab = levelButtonPrefab;
            }
            ButtonController button = Instantiate(prefab, transform);
            button.Init(i, LoadLevel, memory.medal);
        }

        for (int i = 0; i < Memories.customMemoriesLength; i++)
        {
            ButtonController prefab;
            CellsContainer memory = Memories.GetMemory(i + Memories.memoriesLength);
            if (memory.isLevelCleared)
            {
                prefab = customLevelClearedButtonPrefab;
            }
            else
            {
                prefab = customLevelButtonPrefab;
            }
            ButtonController button = Instantiate(prefab, transform);
            button.Init(i + Memories.memoriesLength, LoadLevel, memory.medal);
        }

    }

    public void LoadLevel(int level)
    {
        if (Memories.getToEdit()){
            BattleData.selectedLevel = level;
            BattleData.isTest = true;
            Memories.setNewLevel(false);
            Memories.setToEdit(false);
            SceneManager.LoadScene("CustomCode");
        }
        else{
            BattleData.selectedLevel = level;
            BattleData.isTest = false;
            SceneManager.LoadScene("Battle");
        }
    }

    public void CreateNewLevel()
    {
        BattleData.selectedLevel = -1;
        BattleData.isTest = false;
        Memories.setNewLevel(true);
        SceneManager.LoadScene("CustomCode");
    }

    public void BackButton()
    {
        SceneManager.LoadScene("Menu");
    }
}
