using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    // 0 is the index of the scene base - i.e. the one containint this, the game manager, etc.
    private int currentScene = 0;

    public void Init()
    {
        loadingScreen.SetActive(false);
    }

    public void LoadNextScene(UnityAction onEnd = null)
    {
        LoadScene(currentScene + 1, onEnd);
    }

    public void LoadScene(int index, UnityAction onEnd = null)
    {
        StartCoroutine(LoadAsyncScene(index, onEnd));
    }

    // load a scene on top of current one(s)
    public void AddScene(int index, UnityAction onEnd = null)
    {
        // check if scene to add is already loaded
        if (SceneLoaded(index)) return;

        // if not, add it
        StartCoroutine(AddAsyncScene(index, onEnd));
    }

    // try to remove a scene - fails if scene with given index is not currently loaded or if it's the only loaded scene.
    public void TryRemoveScene(int index, UnityAction onEnd = null)
    {
        // if there's only one scene loaded -> abort
        if (SceneManager.sceneCount < 2) return;

        // check if scene to remove is loaded
        if (!SceneLoaded(index)) return;

        StartCoroutine(RemoveAsyncScene(index, onEnd));
    }

    private bool SceneLoaded(int index)
    {
        bool sceneLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            sceneLoaded |= SceneManager.GetSceneAt(i).buildIndex == index;
        }
        return sceneLoaded;
    }

    private IEnumerator LoadAsyncScene(int index, UnityAction onEnd)
    {
        loadingScreen.SetActive(true);

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

        loadingScreen.SetActive(false);

        onEnd?.Invoke();
    }

    private IEnumerator AddAsyncScene(int index, UnityAction onEnd)
    {
        // add the desired scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        // Wait until loading is finished
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        onEnd?.Invoke();
    }

    private IEnumerator RemoveAsyncScene(int index, UnityAction onEnd)
    {
        // add the desired scene
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(index);

        // Wait until loading is finished
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        onEnd?.Invoke();
    }

}
