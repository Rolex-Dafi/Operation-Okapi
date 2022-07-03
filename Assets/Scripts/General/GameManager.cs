using UnityEngine;

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacterPrefab;

    private PlayerCharacter playerCharacterCurrent;

    // components
    private SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();

        // load the first scene
        // for now the main menu
        sceneLoader.LoadScene(Utility.mainMenuIndex);
    }

    public void StartGame()
    {
        // spawn player after scene loaded
        sceneLoader.sceneLoaded.AddListener(SpawnPlayer);

        sceneLoader.LoadScene(Utility.firstLevelIndex);
    }

    private void SpawnPlayer()
    {
        // spawn player after scene loaded

        // remove itself after loaded
        sceneLoader.sceneLoaded.RemoveListener(SpawnPlayer);
    }

}
