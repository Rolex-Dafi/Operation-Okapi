using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SceneLoader), typeof(UIInput))]
public class GameManager : MonoBehaviour
{
    [Header("Data")] 
    public ColorPaletteSO ColorPalette;
    [SerializeField] private LevelSO[] levelData;

    [Header("Managers")] // These should already be added inside the scene
    public AudioManager audioManager;
    [SerializeField] private UIInput uiInput;
    
    [Header("Prefabs")]  // These will be instantiated when needed
    [SerializeField] private PlayerCharacter playerCharacterPrefab;
    [SerializeField] private LevelManager levelManagerPrefab; 

    [Header("UI Prefabs")] 
    [SerializeField] private MenuManager mainMenuPrefab;
    [SerializeField] private HUDManager hudPrefab;

    // player
    public PlayerCharacter playerCharacterInstance { get; private set; }

    // components
    //private SceneLoader sceneLoader;

    // UI
    private MenuManager menuInstance;
    private HUDManager hudInstance;

    // game specific
    private LevelManager currentLevelInstance;
    private bool gameInProgress = false;
    private bool gamePaused = false;

    private void Start()
    {
        //sceneLoader = GetComponent<SceneLoader>();
        //sceneLoader.Init();
        uiInput.Init();
        uiInput.buttonEvents[EUIButton.Escape].AddListener(OpenClosePauseMenu);

        // load the first scene
        // for now the main menu
        OpenMenu();
    }

    private void OpenMenu()
    {
        audioManager.StartAmbience(Utility.mainMenuIndex);
        //sceneLoader.LoadScene(Utility.mainMenuIndex, FinishMenuLoad);
        menuInstance = Instantiate(mainMenuPrefab);
        menuInstance.Init(this);
        audioManager.Refresh(); // new buttons added -> find them and add sounds to them
    }

    /*private void FinishMenuLoad()
    {
        menuInstance = FindObjectOfType<MenuManager>();
        menuInstance.Init(this);
        audioManager.Refresh();
    }*/

    public void StartGame()
    {
        // spawn player after scene loaded
        audioManager.StartAmbience(Utility.firstLevelIndex);
        //sceneLoader.LoadScene(Utility.firstLevelIndex, FinishGameLoad);
        
        var spawner = FindObjectOfType<PlayerSpawner>();
        // only instantiate the player once, then carry over the instance to the next room/level 
        playerCharacterInstance = spawner.SpawnPlayerAndInit(playerCharacterPrefab);
        
        // instantiate and init HUD
        hudInstance = Instantiate(hudPrefab);
        hudInstance.Init(this, playerCharacterInstance);

        // TODO load a save here
        // start the first level
        StartLevel(Level.Office);
        
        playerCharacterInstance.onDeath.AddListener(GameOver);
        
        // set game in progress
        gameInProgress = true;
    }

    public void StartLevel(Level level)
    {
        // instantiate the level
        currentLevelInstance = Instantiate(levelManagerPrefab);
        currentLevelInstance.Init(this, levelData.First(x => x.level == level));
        // load the first room
        currentLevelInstance.LoadRoom(playerCharacterInstance);
        
        currentLevelInstance.onLevelComplete.AddListener(() => StartLevel(level+1));
    }

    private void FinishGameLoad()
    {
        // try to spawn player after scene loaded
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
        if (spawner != null)
        {
            // only instantiate the player once, then carry over the instance to the next room/level (even across scenes)
            // actually - levels don't have to be in separate scenes - since the rooms are generated anyway
            playerCharacterInstance = spawner.SpawnPlayerAndInit(playerCharacterPrefab);

            // set up HUD
            HUDManager hud = FindObjectOfType<HUDManager>();
            if (hud != null) hud.Init(this, playerCharacterInstance);

            // let the enemy spawner (if present) know it can spawn enemies
            // can only happen after the player is present
            EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
            if (enemySpawner != null) enemySpawner.Init(playerCharacterInstance);

            // handle player death event
            // TODO restart game here - or rather display game over sign + return to menu button
            playerCharacterInstance.onDeath.AddListener(GameOver);
        }

        // set game in progress
        gameInProgress = true;
    }

    private void OpenClosePauseMenu<T>(T interaction)
    {
        if (!gameInProgress) return;

        if (!gamePaused)
        {
            PauseGame(true);
            // instantiate and initiate the menu
            OpenMenu();
            //sceneLoader.AddScene(Utility.mainMenuIndex, audioManager.InitScene);
        }
        else
        {
            menuInstance.Close();
            //sceneLoader.TryRemoveScene(Utility.mainMenuIndex, () => PauseGame(false));
        }
    }

    // TODO implement pause mechanic
    private void PauseGame(bool pause)
    {
        gamePaused = pause;
    }

    private void GameOver()
    {
        // TODO display game over sign + return to menu button
        
        // restart game
        StartGame();
    }

}
