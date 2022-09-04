using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIInput))]
public class GameManager : MonoBehaviour
{
    [Header("Exposed Fields")]
    public Canvas worldSpaceCanvas;
    
    [Header("Data")] 
    public TooltipUI tooltipUIPrefab;
    public ColorPaletteSO ColorPalette;
    [SerializeField] private LevelSO[] levelData;

    [Header("Managers and Components")] // These should already be added inside the scene
    public AudioManager audioManager;
    [SerializeField] private UIInput uiInput;
    public PlayerSpawner playerSpawner;
    
    [Header("Prefabs")]  // These will be instantiated when needed
    [SerializeField] private PlayerCharacter playerCharacterPrefab;
    [SerializeField] private LevelManager levelManagerPrefab; 

    [Header("UI Prefabs")] 
    [SerializeField] private MenuManager mainMenuPrefab;
    [SerializeField] private HUDManager hudPrefab;

    // player
    public PlayerCharacter playerCharacterInstance { get; private set; }

    // UI
    private MenuManager menuInstance;
    private HUDManager hudInstance;

    // game specific
    private LevelManager currentLevelInstance;
    private bool gameInProgress = false;
    private bool gamePaused = false;

    private void Start()
    {
        uiInput.Init();
        uiInput.buttonEvents[EUIButton.Escape].AddListener(OpenClosePauseMenu);

        // load the first scene
        // for now the main menu
        OpenMenu();
    }

    private void OpenMenu()
    {
        Debug.Log("opening menu");
        
        audioManager.StartAmbience(Utility.mainMenuIndex);
        menuInstance = Instantiate(mainMenuPrefab);
        menuInstance.Init(this);
        audioManager.Refresh(); // new buttons added -> find them and add sounds to them
    }

    public void StartGame()
    {
        Debug.Log("starting game");
        
        // spawn player after scene loaded
        audioManager.StartAmbience(Utility.firstLevelIndex);
        //sceneLoader.LoadScene(Utility.firstLevelIndex, FinishGameLoad);
        
        // only instantiate the player once, then carry over the instance to the next room/level 
        playerCharacterInstance = playerSpawner.SpawnPlayerAndInit(playerCharacterPrefab);
        
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

    private void StartLevel(Level level)
    {
        // instantiate the level
        currentLevelInstance = Instantiate(levelManagerPrefab);
        currentLevelInstance.Init(this, levelData.First(x => x.level == level));
        // load the first room
        currentLevelInstance.LoadRoom(playerCharacterInstance);
        
        currentLevelInstance.onLevelComplete.AddListener(() => StartLevel(level+1));
    }

    private void OpenClosePauseMenu<T>(T interaction)
    {
        if (!gameInProgress) return;

        if (!gamePaused)
        {
            PauseGame(true);
            // instantiate and initiate the menu
            OpenMenu();
        }
        else
        {
            menuInstance.Close();
        }
    }

    // TODO implement pause mechanic better
    public void PauseGame(bool pause)
    {
        gamePaused = pause;
        Time.timeScale = pause ? 0 : 1;
    }

    private void GameOver()
    {
        // TODO display game over sign + return to menu button instead
        
        // TODO clean everything up - probably just reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        // restart game
        StartGame();
    }

}
