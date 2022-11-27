using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPoint : MonoBehaviour
{
    private void Awake()
    {
        Utility.secondSciat = false;
        
        SceneManager.LoadScene("SC-IAT");
    }
}
