using UnityEngine;

/// <summary>
/// Level data class.
/// </summary>
[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level")]
public class LevelSO : ScriptableObject
{
    public Level level;
    
    [Range(3,10)] public int numberOfRooms; // only relevant for the first 3 levels

    // sorted from most to least common (rare)
    // only most common should be spawned in the first few rooms of the level
    // then later either common + rare or just rare
    public EnemyCharacterSO[] enemies;
    public EnemyCharacterSO[] enemyOne;
    public EnemyCharacterSO[] enemyTwo;

    public EnemyCharacterSO boss;

    [Header("PCG")]  // only relevant for the first 3 levels
    public MapGenerator roomGeneratorPrefab;
    
    [Header("Special rooms")] // only relevant for the first 3 levels
    public SpecialRoom merchantRoomPrefab;
    public SpecialRoom bossRoomPrefab;

    public LevelDialogue gayLevelDialogue;
    public LevelDialogue straightLevelDialogue;

    /// <summary>
    /// Semi-randomly generates what enemies to spawn depending on the room number.
    /// </summary>
    /// <param name="roomNumber">The room number for which to spawn enemies</param>
    /// <param name="enemyCount">The amount of enemies to spawn</param>
    /// <param name="roomsCompleted"></param>
    /// <returns>The data of the enemies to spawn</returns>
    public EnemyCharacterSO[] GetEnemiesToSpawn(int roomNumber, int enemyCount, int roomsCompleted = 0)
    {
        if (roomNumber > numberOfRooms) return null;

        // TODO make this depend on how many rooms they've passed
        var enemiesToSpawn = new EnemyCharacterSO[enemyCount];
        if (roomNumber == 1)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                enemiesToSpawn[i] = enemyOne[i % (enemyOne.Length)];
            } 
        }
        else if (roomNumber == 2)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                enemiesToSpawn[i] = enemyTwo[i % (enemyTwo.Length)];
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
