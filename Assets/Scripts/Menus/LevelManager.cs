using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	private void Start() {
		foreach(Medal medal in UserData.Instance.medals) {
			Debug.Log(medal.levelName + " " + medal.roundsMedal + " " + medal.sizeMedal);
		}
	}
	public void LoadLevel(int level)
	{
		UserData.Instance.Level = level;
		SceneManager.LoadScene("Battle");
	}
}
