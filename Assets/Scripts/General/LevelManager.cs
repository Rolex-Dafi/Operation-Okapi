using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum Level
{
    Office,
    Street,
    Mall,
    Roof
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
    [SerializeField] private MapGenerator roomGenerator; 
    [SerializeField] private NavMeshGraphGenerator navMeshGenerator;

    [Header("Spawners")] 
    [SerializeField] private EnemySpawner enemySpawner;

    [HideInInspector] public UnityEvent onLevelComplete;

    private GameManager gameManager;
    private LevelSO data;
    
    private int currentRoomIndex;
    private int merchantRoomIndex;

    private bool levelBeaten;

    public void Init(GameManager gameManager, LevelSO data)
    {
        this.gameManager = gameManager;
        this.data = data;

        if (data == null)
        {
            Debug.LogError("Level not found");
            return;
        }

        onLevelComplete = new UnityEvent();
        
        //merchantRoomIndex = Random.Range(1, data.numberOfRooms - 2); // merchant can't be in the first room or the last room
        merchantRoomIndex = 1; // TODO debug - remove this after merchant tested
        currentRoomIndex = 0;

        // TODO debug - change this to only true when all enemies beaten
        levelBeaten = true;
    }

    /// <summary>
    /// Loads a new room - generates it depending on current level, adds a navmesh, spawns the enemies and
    /// places the player. For special room types, no pcg performed - just instantiates a prefab.
    /// </summary>
    /// <param name="player">the current player character</param>
    /// <param name="roomType">the type of room to spawn</param>
    public void LoadRoom(PlayerCharacter player, RoomType roomType = RoomType.Normal)
    {
        // clean up previous room - important when the new room is not a pcg one but a prefab
        roomGenerator.DestroyCurrentRoom();
        
        navMeshGenerator.ClearNavMesh();
        
        Vector3 playerSpawn = default;
        Interactable exitTrigger = default;
        
        switch (roomType)
        {
            case RoomType.Normal:
                // first generate the room
                roomGenerator.Generate();
        
                // then generate the navmesh
                navMeshGenerator.GenerateNavMesh(roomGenerator);
        
                // TODO spawn the enemies
                //enemySpawner.Init(gameManager, gameManager.playerCharacterInstance);
                
                // player
                playerSpawn = roomGenerator.GetEntranceCollider().position;
                
                // exit
                exitTrigger = roomGenerator.GetExitTrigger();
                
                break;
            case RoomType.Merchant:
                // get from prefab - or custom generation
                var merchantRoom = Instantiate(data.merchantRoomPrefab);
                // no enemies -> no navmesh required
                
                // player
                playerSpawn = merchantRoom.Entrance.position;
                
                // exit
                exitTrigger = merchantRoom.ExitTrigger;
                
                break;
            case RoomType.Boss:
                // get from prefab
                var bossRoom = Instantiate(data.bossRoomPrefab);
                
                // ! generate navmesh too ! - or have the navmesh pre-generated in the prefab
                
                // spawn the boss - or have them pre-placed in the prefab
                
                // player
                playerSpawn = bossRoom.Entrance.position;
                
                // exit
                exitTrigger = bossRoom.ExitTrigger;
                
                break;
        }
        
        // TODO entrance and exit in non-generated rooms
        
        // place the player - persistent from previous room/level
        PlayerSpawner.PlacePlayer(player, playerSpawn);
        
        // initialize exit trigger
        if (exitTrigger != null)
        {
            exitTrigger.Init("Press E to exit");
            exitTrigger.onInteractPressed.AddListener(CompleteRoom);
        }
    }

    /// <summary>
    /// Should be called after the player has killed all enemies, is standing in the exit trigger
    /// and presses the exit (interact) button.
    /// </summary>
    private void CompleteRoom()
    {
        // if enemies still remaining - level can't complete
        if (!levelBeaten) return;
        
        // TODO save the game
        
        // increment to next room
        ++currentRoomIndex;

        // spawn next room or end the level depending on next room index
        if (currentRoomIndex == merchantRoomIndex)           // merchant
        {
            LoadRoom(gameManager.playerCharacterInstance, RoomType.Merchant);
        }
        else if (currentRoomIndex == data.numberOfRooms - 1) // last room - boss
        {
            LoadRoom(gameManager.playerCharacterInstance, RoomType.Boss);
        }
        else if (currentRoomIndex == data.numberOfRooms)     // level end
        {
            EndLevel();
        }
        else                                                 // normal room
        {
            LoadRoom(gameManager.playerCharacterInstance);
        }
    }

    private void EndLevel()
    {
        // notify all listeners - should be at least game manager and HUD
        onLevelComplete.Invoke();
        
        // destroy self
        Destroy(gameObject);
    }
}
