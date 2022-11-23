using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum Level
{
    Office,
    Street,
    Mall,
    Roof,
    End
}

public enum RoomType 
{
    Normal,
    Merchant,
    Boss
}

/// <summary>
/// Manager for one game level (each level should have its own LevelManager).
/// Last level (the mall roof) probably won't be able to make use of this -> special prefab instead.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("PCG")]
    private MapGenerator roomGenerator; 
    [SerializeField] private AIGenerator aiGenerator;

    [Header("Spawners")] 
    [SerializeField] private EnemySpawner enemySpawner;

    [HideInInspector] public UnityEvent onLevelComplete;

    private GameManager gameManager;
    private LevelSO data;

    private GameObject previousRoom;
    private GameObject previousRoomWrapper;  // for any objects that should be destroyed when exiting the room
    
    private int currentRoomIndex;
    private int merchantRoomIndex;

    private bool roomBeaten;
    
    public static Transform CurrentRoomTransform { get; private set; }

    public void Init(GameManager gameManager, LevelSO data)
    {
        this.gameManager = gameManager;
        this.data = data;

        if (data == null)
        {
            Debug.LogError("Level not found");
            return;
        }
        
        // Instantiate generator
        if (data.roomGeneratorPrefab != null) roomGenerator = Instantiate(data.roomGeneratorPrefab, transform);

        // init enemy spawner
        enemySpawner.Init(gameManager, gameManager.PlayerCharacterInstance);
        
        onLevelComplete = new UnityEvent();
        
        // merchant always in first room
        merchantRoomIndex = 0; 
        currentRoomIndex = 0;
    }

    /// <summary>
    /// All entities in the level which are normally updated stop being updated. Should be called when pausing the game.
    /// </summary>
    /// <param name="freeze">Should the update stop or start</param>
    public void FreezeLevel(bool freeze)
    {
        enemySpawner.FreezeEnemies(freeze);
    }
    
    /// <summary>
    /// Loads a new room - generates it depending on current level, adds a navmesh, spawns the enemies and
    /// places the player. For special room types, no pcg performed - just instantiates a prefab.
    /// </summary>
    /// <param name="player">the current player character</param>
    /// <param name="roomType">the type of room to spawn</param>
    public void LoadRoom(PlayerCharacter player, RoomType roomType = RoomType.Normal)
    {
        // clean up previous room
        Destroy(previousRoomWrapper);
        // if previous room was pcg and this one isn't
        roomGenerator.DestroyCurrentRoom();
        // if previous room was merchant or boss
        if (previousRoom != null)
        {
            Destroy(previousRoom);
            previousRoom = null;
        }
        
        aiGenerator.ClearGraph();
        
        // setup next room
        previousRoomWrapper = new GameObject
        {
            transform =
            {
                parent = transform
            }
        };
        CurrentRoomTransform = previousRoomWrapper.transform;
        Interactable exitTrigger = default;
        roomBeaten = false;
        
        // the rest depends on room type
        switch (roomType)
        {
            case RoomType.Normal:
                // first generate the room
                roomGenerator.Generate();
        
                // then generate the navmesh
                aiGenerator.Init(roomGenerator);
                aiGenerator.GenerateGraph();
                var numEnemies = 2;
                var enemySpawnPoints = aiGenerator.GenerateEnemySpawnPoints(numEnemies); // TODO debug, change this to higher number
        
                // spawn enemies
                var enemiesToSpawn = data.GetEnemiesToSpawn(currentRoomIndex, numEnemies);
                for (int i = 0; i < numEnemies; i++)
                {
                    enemySpawner.SpawnEnemy(enemySpawnPoints[i], enemySpawnPoints, enemiesToSpawn[i]);
                }
                
                // place the player - persistent from previous room/level
                PlayerSpawner.PlacePlayer(player, roomGenerator.GetEntranceMiddlePoint());
                
                // exit
                exitTrigger = roomGenerator.GetExitTrigger();
                exitTrigger.Init("Beat all enemies first");
                
                enemySpawner.onAllEnemiesDefeated.AddListener(() => SetRoomBeaten(exitTrigger));
                
                break;
            case RoomType.Merchant:
                // get from prefab
                var merchantRoom = Instantiate(data.merchantRoomPrefab);
                // no enemies -> no navmesh required
                // room beaten automatically
                roomBeaten = true;
                
                // place the player - persistent from previous room/level
                PlayerSpawner.PlacePlayer(player, merchantRoom.Entrance.position);
                
                // exit
                exitTrigger = merchantRoom.ExitTrigger;
                exitTrigger.Init("Press " + EButtonDown.Interact.GetButtonName() + " to exit");

                // save to delete later
                previousRoom = merchantRoom.gameObject;
                
                break;
            case RoomType.Boss:
                // get rid of the current astar first - we can do this because this is the last room (the navmesh
                // in the boss prefab is pre-generated and having two in the scene will cause errors)
                //aiGenerator.RemoveSelf();
                
                // get from prefab (navmesh should be pre-generated in the prefab)
                var bossRoom = Instantiate(data.bossRoomPrefab);
                // load the navmesh - TODO should be only after spawning the boss, so it takes into account traps - when i fix the navmesh gen
                //aiGenerator.LoadGraph(data.bossNavGraphData);
                //aiGenerator.RemoveSelf(); // TODO call this instead
                
                // spawn the boss
                enemySpawner.SpawnEnemy(bossRoom.EnemySpawn, bossRoom.EnemyPatrolPoints, data.boss);

                // place the player - persistent from previous room/level
                PlayerSpawner.PlacePlayer(player, bossRoom.Entrance.position);
                
                // exit
                exitTrigger = bossRoom.ExitTrigger;
                exitTrigger.Init("Beat the boss first");
                
                // only allow exit when boss beaten
                enemySpawner.onAllEnemiesDefeated.AddListener(() => SetRoomBeaten(exitTrigger));

                // save to delete later
                previousRoom = bossRoom.gameObject;
                
                break;
        }
        
        // initialize exit trigger
        if (exitTrigger != null)
        {            
            // init on exit dialogue
            var exitDialogue = exitTrigger.GetComponent<DialogueTrigger>();
            if (exitDialogue != null)
            {
                var levelDialogue = Utility.gayVersion ? data.gayLevelDialogue : data.straightLevelDialogue;
                exitDialogue.Init(gameManager.GetDialogueUI(), 
                    levelDialogue.rooms[currentRoomIndex].GetOutroPassage());
            }
            
            exitTrigger.onInteractPressed.AddListener(() => StartExitDialogue(exitDialogue));
        }
    }

    /// <summary>
    /// Spawns the final room. Should only be used in the final level.
    /// </summary>
    /// <param name="player">the current player character</param>
    public void LoadFinalRoom(PlayerCharacter player)
    {
        if (data.level != Level.Roof)
        {
            Debug.LogWarning("Function LoadFinalRoom() should only be used in the last level");
            return;
        }
        
        // remove the astar in the level manager prefab first
        aiGenerator.CleanUp(() => {
            // setup
            previousRoomWrapper = new GameObject
            {
                transform =
                {
                    parent = transform
                }
            };
            CurrentRoomTransform = previousRoomWrapper.transform;
            roomBeaten = false;
            
            var bossRoom = Instantiate(data.bossRoomPrefab);
            
            // spawn the boss - register callback to win the game after defeating the boss
            enemySpawner.SpawnEnemy(
                bossRoom.EnemySpawn, 
                bossRoom.EnemyPatrolPoints, 
                data.boss, 
                StartGameEndDialogue
            );
            
            // spawn the enemies
            /*var enemiesToSpawn = data.GetEnemiesToSpawn();
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                enemySpawner.SpawnEnemy(bossRoom.EnemyPatrolPoints[i], bossRoom.EnemyPatrolPoints, enemiesToSpawn[i]);
            }*/

            // place the player - persistent from previous room/level
            PlayerSpawner.PlacePlayer(player, bossRoom.Entrance.position);
        });
    }

    private void SetRoomBeaten(Interactable exitTrigger)
    {
        exitTrigger.SetTooltip("Press " + EButtonDown.Interact.GetButtonName() + " to exit");
        roomBeaten = true;
    }

    private void StartExitDialogue(DialogueTrigger exitDialogue)
    {
        // activate exit dialogue
        if (exitDialogue != null)
        {
            if (data.level == Level.Mall && currentRoomIndex == 0)
            {
                exitDialogue.StartDialogue(() =>
                {
                    gameManager.PlayerCharacterInstance.ReceiveFancyTie(CompleteRoom);
                });
            }
            else
            {
                exitDialogue.StartDialogue(CompleteRoom);
            }
        }
        else
        {
            Debug.LogWarning("No exit dialogue found, completing the room instead");
            CompleteRoom();
        }
    }

    private void StartGameEndDialogue()
    {
        var exitDialogue = FindObjectOfType<DialogueTrigger>();
        if (exitDialogue != null)
        { 
            var levelDialogue = Utility.gayVersion ? data.gayLevelDialogue : data.straightLevelDialogue;
            
            // initialize exit dialogue
            exitDialogue.Init(gameManager.GetDialogueUI(), 
                levelDialogue.rooms[0].GetOutroPassage());
            
            // activate exit dialogue
            exitDialogue.StartDialogue(()=>gameManager.GameEnd(true));
        }
        else
        {
            Debug.LogWarning("No game end dialogue found, ending game");
            gameManager.GameEnd(true);
        }
    }
    
    /// <summary>
    /// Should be called after the player has killed all enemies, is standing in the exit trigger
    /// and presses the exit (interact) button.
    /// </summary>
    private void CompleteRoom()
    {
        // if enemies still remaining - level can't complete
        if (!roomBeaten) return;
        
        // TODO save the game - call game man
        
        // increment to next room
        ++currentRoomIndex;

        // spawn next room or end the level depending on next room index
        if (currentRoomIndex == merchantRoomIndex)           // merchant
        {
            LoadRoom(gameManager.PlayerCharacterInstance, RoomType.Merchant);
        }
        else if (currentRoomIndex == data.numberOfRooms - 1) // last room - boss
        {
            if (data.boss != null) // there's a boss
            {
                // clean up the astar path - only complete load the boss room (bc it has its own astar path)
                aiGenerator.CleanUp(() => LoadRoom(gameManager.PlayerCharacterInstance, RoomType.Boss));
            }
            else // no boss in this level, load a normal room
            {
                LoadRoom(gameManager.PlayerCharacterInstance);
            }
        }
        else if (currentRoomIndex == data.numberOfRooms)     // level end
        {
            EndLevel();
        }
        else                                                 // normal room
        {
            LoadRoom(gameManager.PlayerCharacterInstance);
        }
    }

    private void EndLevel()
    {
        // clean up last room
        Destroy(previousRoomWrapper);
        // if previous room was pcg and this one isn't
        roomGenerator.DestroyCurrentRoom();
        
        if (previousRoom != null)
        {
            Destroy(previousRoom);
            previousRoom = null;
        }
        
        // clean up the astar path - only complete the level after cleanup
        aiGenerator.CleanUp(() => 
        {
            // notify all listeners - should be at least game manager and HUD
            onLevelComplete.Invoke();
        
            // destroy self
            Destroy(gameObject);
        });

    }
}
