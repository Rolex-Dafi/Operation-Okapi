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
    
    private int currentRoomIndex;
    private int merchantRoomIndex;

    private bool roomBeaten;

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
        roomGenerator = Instantiate(data.roomGeneratorPrefab, transform);

        // init enemy spawner
        enemySpawner.Init(gameManager, gameManager.PlayerCharacterInstance);
        
        onLevelComplete = new UnityEvent();
        
        merchantRoomIndex = Random.Range(1, data.numberOfRooms - 2); // merchant can't be in the first room or the last room
        //merchantRoomIndex = 0; // debug - remove this after merchant tested
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
        // if previous room was pcg and this one isn't
        roomGenerator.DestroyCurrentRoom();
        // if previous room was merchant or boss
        if (previousRoom != null)
        {
            Destroy(previousRoom);
            previousRoom = null;
        }
        
        aiGenerator.ClearNavMesh();
        
        Interactable exitTrigger = default;

        roomBeaten = false;
        
        switch (roomType)
        {
            case RoomType.Normal:
                // first generate the room
                roomGenerator.Generate();
        
                // then generate the navmesh
                aiGenerator.Init(roomGenerator);
                aiGenerator.GenerateNavMesh();
                var enemySpawnPoints = aiGenerator.GenerateEnemySpawnPoints(2); // TODO debug, change this to higher number
        
                // spawn enemies
                // parameters TODO spawn more types of enemy in some rooms
                //var numEnemyTypes = currentRoomIndex > 2 ? Random.Range(1, 2) : 1; // chance for 2 types if far enough
                var enemyIndex = currentRoomIndex < 1 ? 0 : Random.Range(0, data.enemies.Length - 1);
                foreach (var spawnPoint in enemySpawnPoints)
                {
                    enemySpawner.SpawnEnemy(spawnPoint, enemySpawnPoints, data.enemies[enemyIndex]); // this passes enemy spawn points as patrol points for each enemy TODO make better
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
                exitTrigger.Init("Press E to exit");

                // save to delete later
                previousRoom = merchantRoom.gameObject;
                
                break;
            case RoomType.Boss:
                // get rid of the current astar first - we can do this because this is the last room (the navmesh
                // in the boss prefab is pre-generated and having two in the scene will cause errors)
                //aiGenerator.RemoveSelf();
                
                // get from prefab (navmesh should be pre-generated in the prefab)
                var bossRoom = Instantiate(data.bossRoomPrefab);
                // load the navmesh
                aiGenerator.LoadGraph(data.bossNavGraphData);
                
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
            exitTrigger.onInteractPressed.AddListener(CompleteRoom);
        }
    }

    private void SetRoomBeaten(Interactable exitTrigger)
    {
        exitTrigger.SetTooltip("Press E to exit");
        roomBeaten = true;
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
            LoadRoom(gameManager.PlayerCharacterInstance, RoomType.Boss);
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
        // clean up last room - should always be the boss
        if (previousRoom != null)
        {
            Destroy(previousRoom);
            previousRoom = null;
        }
        
        // notify all listeners - should be at least game manager and HUD
        onLevelComplete.Invoke();
        
        // destroy self
        Destroy(gameObject);
    }
}
