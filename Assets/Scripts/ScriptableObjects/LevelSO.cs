using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level")]
public class LevelSO : ScriptableObject
{
    public Level level;
    
    [Range(3,10)] public int numberOfRooms;

    // sorted from most to least common (rare)
    // only most common should be spawned in the first few rooms of the level
    // then later either common + rare or just rare
    public EnemyCharacterSO[] enemies;

    [Header("PCG")] 
    public MapGenerator roomGeneratorPrefab;
    
    [Header("Special rooms")]
    public SpecialRoom merchantRoomPrefab;
    public SpecialRoom bossRoomPrefab;
}
