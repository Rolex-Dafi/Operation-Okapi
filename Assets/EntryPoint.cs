using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The entry point for the application. Sets which version of the game (experimental or control) is active.
/// </summary>
public class EntryPoint : MonoBehaviour
{
    private void Awake()
    {
        // change here for str8 version + switch Bridget animator manually
        Utility.gayVersion = false;
        
        Utility.secondSciat = false;
        Utility.gameStartTime = -1;
        Utility.gameWon = false;
        
        SceneManager.LoadScene("SC-IAT");
    }
}
