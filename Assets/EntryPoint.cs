using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPoint : MonoBehaviour
{
    private void Awake()
    {
        Utility.secondSciat = false;
        
        Debug.Log("setting second sciat to " + Utility.secondSciat);
        SceneManager.LoadScene("SC-IAT");
    }
}
