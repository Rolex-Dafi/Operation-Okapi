using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyCharacter[] enemyPrefabs;

    // UI
    [SerializeField] private ResourceUI healthBarPrefab;

    // Navigation
    //[SerializeField] private Transform spawnPoint;
    //[SerializeField] private Transform[] patrollPoints;

    //private bool canSpawn = false;
    //private bool enemyAlive = false;

    private int numEnemiesAlive;
    
    //private int lastEnemyIdx = 0;

    private PlayerCharacter playerCharacter;
    private GameManager gameManager;

    private List<EnemyCharacter> currentEnemies;
    
    [HideInInspector] public UnityEvent onAllEnemiesDefeated;
    
    public void Init(GameManager gameManager, PlayerCharacter playerCharacter)
    {
        this.gameManager = gameManager;
        this.playerCharacter = playerCharacter;
        //canSpawn = true;
        numEnemiesAlive = 0;

        currentEnemies = new List<EnemyCharacter>();
        
        onAllEnemiesDefeated = new UnityEvent();
    }

    void Update()
    {
        //if (canSpawn && !enemyAlive) SpawnEnemy(lastEnemyIdx);
    }

    /// <summary>
    /// Spawns a new enemy in the scene and initializes it.
    /// </summary>
    /// <param name="spawnPoint">The position to spawn the enemy at.</param>
    /// <param name="patrolPoints">The positions between which the enemy will patrol when not engaged in combat</param>
    /// <param name="enemy">The enemy initial data</param>
    public void SpawnEnemy(Vector3 spawnPoint, Vector3[] patrolPoints, EnemyCharacterSO enemy)
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
        
        enemyInstance.onDeath.AddListener(() => CleanUpEnemy(healthBarInstance, enemyInstance));

        // Navigation
        enemyInstance.GetComponent<CharacterTreeBase>().patrollPoints = patrolPoints;

        currentEnemies.Add(enemyInstance);
        
        ++numEnemiesAlive;
    }
    
    /*private void SpawnEnemy(int i)
    {
        EnemyCharacter enemyInstance = Instantiate(enemyPrefabs[i], spawnPoint.position, Quaternion.identity, transform);
        enemyInstance.Init(playerCharacter);
        enemyInstance.onDeath.AddListener(CleanUpEnemy);

        // UI
        healthBarInstance = Instantiate(healthBarPrefab, gameManager.worldSpaceCanvas.transform);
        healthBarInstance.Init(enemyInstance.Health);
        healthBarInstance.GetComponent<FollowTarget>().Init(enemyInstance.transform);

        // Navigation
        enemyInstance.GetComponent<CharacterTreeBase>().patrollPoints = patrollPoints;

        enemyAlive = true;
        ++numEnemiesAlive;

        ++lastEnemyIdx;
        lastEnemyIdx = lastEnemyIdx > enemyPrefabs.Length - 1 ? 0 : lastEnemyIdx;
    }*/

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
        
        //enemyAlive = false;
        --numEnemiesAlive;
        
        if (numEnemiesAlive == 0) onAllEnemiesDefeated.Invoke();
    }

    private void OnDestroy()
    {
        onAllEnemiesDefeated.RemoveAllListeners();
    }
}
