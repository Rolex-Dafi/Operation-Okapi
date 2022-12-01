using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPoint : MonoBehaviour
{
    private void Awake()
    {
        // only change here for str8 version
        Utility.gayVersion = true;
        
        Utility.secondSciat = false;
        Utility.gameStartTime = -1;
        Utility.gameWon = false;
        
        SceneManager.LoadScene("SC-IAT");
    }
}
