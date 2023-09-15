using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	[SerializeField] private string levelSelectSceneName = "LevelSelect";
	[SerializeField] private string creatorModeSceneName = "CreatorMode";

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
}
