using UnityEngine;
using UnityEngine.SceneManagement;

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
