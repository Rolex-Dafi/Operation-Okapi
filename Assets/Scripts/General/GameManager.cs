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
        sceneLoader.sceneLoaded.AddListener(FinishLevelLoad);

        sceneLoader.LoadScene(Utility.firstLevelIndex);
    }

    private void FinishLevelLoad()
    {
        // try to spawn player after scene loaded
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
        if (spawner != null)
        {
            playerCharacterCurrent = spawner.SpawnPlayer(playerCharacterPrefab);

            // set up HUD
            HUDManager hud = FindObjectOfType<HUDManager>();
            if (hud != null) hud.Init(playerCharacterCurrent);

            // let the enemy spawner (if present) know it can spawn enemies
            // can only happen after the player is present
            EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
            if (enemySpawner != null) enemySpawner.Init();

            // handle player death event
            playerCharacterCurrent.onDeath.AddListener(RestartLevel);
        }

        // remove itself after loaded
        sceneLoader.sceneLoaded.RemoveListener(FinishLevelLoad);
    }

    private void RestartLevel()
    {
        StartGame();
    }

}
