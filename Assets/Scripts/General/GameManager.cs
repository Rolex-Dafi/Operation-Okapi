using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIInput))]
public class GameManager : MonoBehaviour
{
    [Header("Exposed Fields")]
    public Canvas worldSpaceCanvas;  // for things like enemy health bars
    
    [Header("Data")] 
    public TooltipUI tooltipUIPrefab;
    public ColorPaletteSO ColorPalette;
    [SerializeField] private LevelSO[] levelData;

    [Header("Managers and Components")] // These should already be added inside the scene
    public AudioManager audioManager;
    public UIInput uiInput;
    public PlayerSpawner playerSpawner;
    
    [Header("Prefabs")]  // These will be instantiated when needed
    [SerializeField] private PlayerCharacter playerCharacterPrefab;
    [SerializeField] private LevelManager levelManagerPrefab; 

    [Header("UI Prefabs")] 
    [SerializeField] private UIOverlayManager mainMenuPrefab;
    [SerializeField] private UIOverlayManager pauseMenuPrefab;
    [SerializeField] private UIOverlayManager gameEndPrefab;
    [SerializeField] private HUDManager hudPrefab;

    // player
    public PlayerCharacter PlayerCharacterInstance { get; private set; }

    // UI
    private UIOverlayManager menuInstance;
    private HUDManager hudInstance;

    // game specific
    private LevelManager currentLevelInstance;
    private bool gameInProgress = false;
    private bool gamePaused = false;

    private void Start()
    {
        uiInput.Init();
        uiInput.buttonEvents[EUIButton.Escape].AddListener((_) => OpenClosePauseMenu());

        // load the first scene
        // for now the main menu
        OpenMainMenu();
    }

    private void OpenMainMenu()
    {
        Debug.Log("opening menu");
        
        audioManager.StartAmbience(Utility.mainMenuIndex);
        menuInstance = Instantiate(mainMenuPrefab);
        menuInstance.Init(this);
        audioManager.Refresh(); // new buttons added -> find them and add sounds to them
    }

    private void OpenPauseMenu()
    {
        Debug.Log("opening pause menu");
        
        menuInstance = Instantiate(pauseMenuPrefab);
        menuInstance.Init(this);
        audioManager.Refresh(); // new buttons added -> find them and add sounds to them
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        Debug.Log("starting game");
        
        // spawn player after scene loaded
        audioManager.StartAmbience(Utility.firstLevelIndex);
        //sceneLoader.LoadScene(Utility.firstLevelIndex, FinishGameLoad);
        
        // only instantiate the player once, then carry over the instance to the next room/level 
        PlayerCharacterInstance = playerSpawner.SpawnPlayerAndInit(playerCharacterPrefab);
        
        // instantiate and init HUD
        hudInstance = Instantiate(hudPrefab);
        hudInstance.Init(this, PlayerCharacterInstance);

        // start the first level
        StartLevel(Level.Office);
        
        PlayerCharacterInstance.onDeath.AddListener(() => GameEnd(false));
        
        // set game in progress
        gameInProgress = true;
    }

    private void StartLevel(Level level)
    {
        switch (level)
        {
            // End the game
            case Level.End:
                GameEnd(true);
                return;
            // special level - has slightly different logic than the rest
            case Level.Roof:
                // TODO implement special logic here - probably just spawn a prefab
            
                // for now:
                GameEnd(true);
                return;
        }

        // instantiate the level
        currentLevelInstance = Instantiate(levelManagerPrefab);  
        currentLevelInstance.Init(this, levelData.First(x => x.level == level));
        // load the first room
        currentLevelInstance.LoadRoom(PlayerCharacterInstance);
        
        currentLevelInstance.onLevelComplete.AddListener(() => StartLevel(level+1));
    }

    private void OpenClosePauseMenu()
    {
        if (!gameInProgress) return;
        
        if (!gamePaused)
        {
            PauseGame(true);
            // instantiate and initiate the menu
            OpenPauseMenu();
        }
        else
        {
            // don't try closing the menu if no menu is open
            if (menuInstance == null) return;
            
            PauseGame(false);
            menuInstance.Close();
            menuInstance = null;
        }
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    /// <param name="pause"></param>
    public void PauseGame(bool pause)
    {
        Debug.Log("setting game paused to " + pause);
        
        gamePaused = pause;
        currentLevelInstance.FreezeLevel(pause);     // stop updating all entities in the level
        PlayerCharacterInstance.ReadInput = !pause;  // stop reading player input
    }

    private void GameEnd(bool won)
    {
        Debug.Log("ending game, won = " + won);
        
        menuInstance = Instantiate(gameEndPrefab);
        menuInstance.Init(this);
        menuInstance.ChangeTitle(won ? "You win!" : "Game Over");
        audioManager.Refresh(); // new buttons added -> find them and add sounds to them
    }

    /// <summary>
    /// Returns to main menu.
    /// </summary>
    public void BackToMain()
    {
        // unpause the game first
        PauseGame(false);
        
        // simply reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
