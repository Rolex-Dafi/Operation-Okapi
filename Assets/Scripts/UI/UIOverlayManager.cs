using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for any UI overlay with buttons relating to high-level game logic - ex. menus, game over screen.
/// </summary>
public class UIOverlayManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    
    private GameManager gameManager;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        
        // if gamepad is connected, try to select the first button you can find
        UIInput.TrySelectFirstButton();
    }

    /// <summary>
    /// Changes the title text of the overlay, if present.
    /// </summary>
    /// <param name="text"></param>
    public void ChangeTitle(string text)
    {
        if (title != null) title.text = text;
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
    
    /// <summary>
    /// Opens a debug window.
    /// </summary>
    public void OpenDebug() 
    {
    
    }
    
}
