using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private GameManager gameManager;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// Begin game - only call from main menu.
    /// </summary>
    public void StartGame()
    {
        gameManager.StartGame();
        Close();
    }

    /// <summary>
    /// Load last save - only from main menu.
    /// </summary>
    public void LoadGame()
    {
        //TODO implement loading
    }

    /// <summary>
    /// After pausing game - only call from pause menu.
    /// </summary>
    public void ResumeGame()
    {
        gameManager.PauseGame(false);
        Close();
    }

    /// <summary>
    /// Return to main menu.
    /// </summary>
    public void BackToMain()
    {
        gameManager.BackToMain();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void Close()
    {
        // TODO tween on menu close
        Destroy(gameObject);
    }
}
