using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	[SerializeField] private string levelSelectSceneName = "LevelSelect";
	[SerializeField] private string creatorModeSceneName = "CreatorMode";

	[Header("Tutorial")]
	[SerializeField] private Image tutorialImage;
	[SerializeField] private Sprite[] tutorialImages;
	private int currentTutorialImage = 0;

	public void PlayGame()
	{
		SceneManager.LoadScene(levelSelectSceneName);
	}

	public void CreateMode()
	{
		SceneManager.LoadScene(creatorModeSceneName);
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif

		Application.Quit();
	}

	public void NextTutorialImage()
	{
		currentTutorialImage++;
		if (currentTutorialImage >= tutorialImages.Length)
		{
			currentTutorialImage = 0;
		}

		tutorialImage.sprite = tutorialImages[currentTutorialImage];
	}

	public void PreviousTutorialImage()
	{
		currentTutorialImage--;
		if (currentTutorialImage < 0)
		{
			currentTutorialImage = tutorialImages.Length - 1;
		}

		tutorialImage.sprite = tutorialImages[currentTutorialImage];
	}
}
