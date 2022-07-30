using UnityEngine;

[RequireComponent(typeof(SceneLoader), typeof(UIInput))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacterPrefab;

    private PlayerCharacter playerCharacterCurrent;

    // components
    private SceneLoader sceneLoader;
    private UIInput uiInput;
    public AudioManager audioManager;

    // Menu
    private MenuManager menuManager;

    // game specific
    private bool gameInProgress = false;
    private bool gamePaused = false;

    private void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();
        sceneLoader.Init();
        uiInput = GetComponent<UIInput>();
        uiInput.Init();
        uiInput.buttonEvents[EUIButton.Escape].AddListener(OpenClosePauseMenu);

        // load the first scene
        // for now the main menu
        OpenMenu();
    }

    private void OpenMenu()
    {
        audioManager.StartAmbience(Utility.mainMenuIndex);
        sceneLoader.LoadScene(Utility.mainMenuIndex, FinishMenuLoad);
    }

    private void FinishMenuLoad()
    {
        menuManager = FindObjectOfType<MenuManager>();
        menuManager.Init(this);
    }

    public void StartGame()
    {
        // spawn player after scene loaded
        audioManager.StartAmbience(Utility.firstLevelIndex);
        sceneLoader.LoadScene(Utility.firstLevelIndex, FinishLevelLoad);
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

        // set game in progress
        gameInProgress = true;
    }

    private void OpenClosePauseMenu<T>(T interaction)
    {
        if (!gameInProgress) return;

        Debug.Log("in openclosepausemenu");

        if (!gamePaused)
        {
            sceneLoader.AddScene(Utility.mainMenuIndex, () => gamePaused = true);
        }
        else
        {
            sceneLoader.TryRemoveScene(Utility.mainMenuIndex, () => gamePaused = false);
        }
    }

    private void PauseGame(bool pause)
    {

    }


    private void RestartLevel()
    {
        StartGame();
    }

}
