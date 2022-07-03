using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [HideInInspector] public UnityEvent sceneLoaded = new UnityEvent();

    // 0 is the index of the scene base - i.e. the one containint this, the game manager, etc.
    private int currentScene = 0;

    public void LoadNextScene()
    {
        LoadScene(currentScene + 1);
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadAsyncScene(index));
    }

    private IEnumerator LoadAsyncScene(int index)
    {
        // unload the current scene if there is one to unload
        AsyncOperation asyncUnload = null;
        if (currentScene > 0) asyncUnload = SceneManager.UnloadSceneAsync(currentScene);

        // add the desired scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        // Wait until unloading and loading is finished
        bool loading = true;
        while (loading)
        {
            loading = !asyncLoad.isDone;
            if (currentScene > 0) loading |= !asyncUnload.isDone;

            yield return null;
        }

        // set current scene index to added scene
        currentScene = index;
        sceneLoaded.Invoke();
    }

    private void OnDestroy()
    {
        sceneLoaded.RemoveAllListeners();
    }
}
