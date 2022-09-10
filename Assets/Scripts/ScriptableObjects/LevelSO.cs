using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level")]
public class LevelSO : ScriptableObject
{
    public Level level;
    
    [Range(3,10)] public int numberOfRooms; // only relevant for the first 3 levels

    // sorted from most to least common (rare)
    // only most common should be spawned in the first few rooms of the level
    // then later either common + rare or just rare
    public EnemyCharacterSO[] enemies;

    public EnemyCharacterSO boss;

    [Header("PCG")]  // only relevant for the first 3 levels
    public MapGenerator roomGeneratorPrefab;
    
    [Header("Special rooms")] // only relevant for the first 3 levels
    public SpecialRoom merchantRoomPrefab;
    public SpecialRoom bossRoomPrefab;

    [Header("Roof level")] // only relevant for the last special level
    public Vector3[] enemySpawnPoints; 

    public TextAsset bossNavGraphData; // pre-computed and saved data about the boss room navigation graph

    /// <summary>
    /// Semi-randomly generates what enemies to spawn depending on the room number.
    /// </summary>
    /// <param name="roomNumber">The room number for which to spawn enemies</param>
    /// <param name="enemyCount">The amount of enemies to spawn</param>
    /// <returns>The data of the enemies to spawn</returns>
    public EnemyCharacterSO[] GetEnemiesToSpawn(int roomNumber, int enemyCount)
    {
        if (roomNumber > numberOfRooms) return null;
        
        var enemiesToSpawn = new EnemyCharacterSO[enemyCount];

        if (roomNumber < enemies.Length)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                enemiesToSpawn[i] = enemies[roomNumber];
            }
        }
        else
        {
            // spawn two different types of enemies randomly
            var idx1 = Random.Range(0, enemies.Length);
            var idx2 = Random.Range(0, enemies.Length);  // ! this will sometimes spawn only one enemy type
            for (int i = 0; i < enemyCount; i++)
            {
                var idx = i < enemyCount / 2 ? idx1 : idx2;
                enemiesToSpawn[i] = enemies[idx];
            }
        }

        return enemiesToSpawn;
    }
    
    /// <summary>
    /// Semi-randomly generates what enemies to spawn in the final room - should only be used with the final level.
    /// </summary>
    /// <returns>The data of the enemies to spawn</returns>
    public EnemyCharacterSO[] GetEnemiesToSpawn()
    {
        var enemiesToSpawn = new EnemyCharacterSO[enemies.Length];

        for (int i = 0; i < enemies.Length; i++)
        {
            enemiesToSpawn[i] = enemies[i];
        }

        return enemiesToSpawn;
    }
}
