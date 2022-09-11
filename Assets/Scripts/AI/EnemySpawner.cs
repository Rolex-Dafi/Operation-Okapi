using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Responsible for spawning and managing enemy characters in the scene.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyCharacter[] enemyPrefabs;

    // UI
    [SerializeField] private ResourceUI healthBarPrefab;

    private int numEnemiesAlive;

    private PlayerCharacter playerCharacter;
    private GameManager gameManager;

    private List<EnemyCharacter> currentEnemies;
    
    [HideInInspector] public UnityEvent onAllEnemiesDefeated;
    
    /// <summary>
    /// Initializes the enemy spawner.
    /// </summary>
    /// <param name="gameManager">the current game manager</param>
    /// <param name="playerCharacter">the current player character instance</param>
    public void Init(GameManager gameManager, PlayerCharacter playerCharacter)
    {
        this.gameManager = gameManager;
        this.playerCharacter = playerCharacter;
        //canSpawn = true;
        numEnemiesAlive = 0;

        currentEnemies = new List<EnemyCharacter>();
        
        onAllEnemiesDefeated = new UnityEvent();
    }

    /// <summary>
    /// Spawns a new enemy in the scene and initializes it.
    /// </summary>
    /// <param name="spawnPoint">The position to spawn the enemy at.</param>
    /// <param name="patrolPoints">The positions between which the enemy will patrol when not engaged in combat</param>
    /// <param name="enemy">The enemy initial data</param>
    /// <param name="onEnemyDeath">Action to perform after the enemy dies</param>
    public void SpawnEnemy(Vector3 spawnPoint, Vector3[] patrolPoints, EnemyCharacterSO enemy, UnityAction onEnemyDeath = null)
    {
        var enemyCharacter = enemyPrefabs.First(x => x.Data == enemy);

        if (enemyCharacter == null)
        {
            Debug.LogError("Prefab for enemy " + enemy.name + " not found");
            return;
        }
        
        var enemyInstance = Instantiate(enemyCharacter, spawnPoint, Quaternion.identity, transform);
        enemyInstance.Init(playerCharacter);

        // UI
        var healthBarInstance = Instantiate(healthBarPrefab, gameManager.worldSpaceCanvas.transform);
        healthBarInstance.Init(enemyInstance.Health);
        healthBarInstance.GetComponent<FollowTarget>().Init(enemyInstance.transform);
        
        enemyInstance.onDeath.AddListener(() =>
        {
            Debug.Log("enemy dying " + gameObject.name);
            onEnemyDeath?.Invoke();
            CleanUpEnemy(healthBarInstance, enemyInstance);
        });

        // Navigation
        enemyInstance.GetComponent<CharacterTreeBase>().patrollPoints = patrolPoints;

        currentEnemies.Add(enemyInstance);
        
        ++numEnemiesAlive;
    }
    
    /// <summary>
    /// All enemies currently spawned stop/start being updated.
    /// </summary>
    /// <param name="freeze"></param>
    public void FreezeEnemies(bool freeze)
    {
        foreach (var currentEnemy in currentEnemies)
        {
            currentEnemy.Freeze(freeze);
        }
    }
    
    private void CleanUpEnemy(Component healthBarInstance, EnemyCharacter enemyInstance)
    {
        Destroy(healthBarInstance.gameObject);

        currentEnemies.Remove(enemyInstance);
        
        --numEnemiesAlive;
        
        if (numEnemiesAlive == 0) onAllEnemiesDefeated.Invoke();
    }

    private void OnDestroy()
    {
        onAllEnemiesDefeated.RemoveAllListeners();
    }
}
