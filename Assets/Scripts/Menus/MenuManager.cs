using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    
    public void PlayGame() {
        SceneManager.LoadScene("LevelSelect");
    }

    public void CreateMode() {
        SceneManager.LoadScene("CreatorMode");
    }

    public void QuitGame() {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
