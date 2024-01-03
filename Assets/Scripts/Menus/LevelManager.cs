using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
	[SerializeField] private ButtonController levelButtonPrefab;
	[SerializeField] private ButtonController customLevelButtonPrefab;
	private void Start()
	{
		for (int i = 0; i < Memories.memoriesLength; i++)
		{
			ButtonController button = Instantiate(levelButtonPrefab, transform);
			button.Init(i, LoadLevel, Memories.GetMemory(i).medal);
		}

		for (int i = 0; i < Memories.customMemoriesLength; i++)
		{
			ButtonController button = Instantiate(customLevelButtonPrefab, transform);
			button.Init(i + Memories.memoriesLength, LoadLevel, Memories.GetMemory(i + Memories.memoriesLength).medal);
		}
	}

	public void LoadLevel(int level)
	{
		BattleData.selectedLevel = level;
		SceneManager.LoadScene("Battle");
	}

	public void BackButton()
	{
		SceneManager.LoadScene("Menu");
	}
}
