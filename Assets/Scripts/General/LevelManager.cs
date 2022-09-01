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
    [Header("Data")]
    [SerializeField] private LevelSO data;
    
    [Header("PCG")]
    [SerializeField] private MapGenerator roomGenerator; 
    [SerializeField] private NavMeshGraphGenerator navMeshGenerator;

    [Header("Spawners")] 
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerSpawner playerSpawner;

    [Header("Special Room Prefabs")] 
    [SerializeField] private GameObject merchantRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;

    [HideInInspector] public UnityEvent onLevelComplete;

    private GameManager gameManager;
    private int currentRoomIndex;
    private int merchantRoomIndex;

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
        
        merchantRoomIndex = Random.Range(1, data.numberOfRooms - 2); // merchant can't be in the first room or the last room
        currentRoomIndex = 0;
    }
    
    /// <summary>
    /// Loads a new room - generates it depending on current level, adds a navmesh, spawns the enemies and
    /// places the player.
    /// </summary>
    /// <param name="player">the current player character</param>
    public void LoadRoom(PlayerCharacter player, RoomType roomType = RoomType.Normal)
    {
        switch (roomType)
        {
            case RoomType.Normal:
                // first generate the room
                roomGenerator.Generate();
        
                // then generate the navmesh
                navMeshGenerator.GenerateNavMesh(roomGenerator);
        
                // spawn the enemies
                
                break;
            case RoomType.Merchant:
                // get from prefab - or custom generation
                Instantiate(merchantRoomPrefab);
                // no enemies -> no navmesh required
                break;
            case RoomType.Boss:
                // get from prefab
                Instantiate(bossRoomPrefab);
                
                // ! generate navmesh too ! - or have the navmesh pre-generated in the prefab
                
                // spawn the boss - or have them pre-placed in the prefab
                
                break;
        }

        // TODO initialize exit trigger

        // place the player - persistent from previous room/level
        PlayerSpawner.PlacePlayer(player, roomGenerator.entranceCollider.transform);
    }

    /// <summary>
    /// Should be called after the player has killed all enemies, is standing in the exit trigger
    /// and presses the exit (interact) button.
    /// </summary>
    public void CompleteRoom()
    {
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
