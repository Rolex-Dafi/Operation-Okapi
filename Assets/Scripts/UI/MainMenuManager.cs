using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void StartGame()
    {
        gameManager.StartGame();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
