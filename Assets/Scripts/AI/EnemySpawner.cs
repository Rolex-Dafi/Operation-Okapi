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
    private Dictionary<int, ResourceUI> healthBarInstances;

    // Navigation
    //[SerializeField] private Transform spawnPoint;
    //[SerializeField] private Transform[] patrollPoints;

    //private bool canSpawn = false;
    //private bool enemyAlive = false;

    private int numEnemiesAlive;
    
    //private int lastEnemyIdx = 0;

    private PlayerCharacter playerCharacter;
    private GameManager gameManager;

    [HideInInspector] public UnityEvent onAllEnemiesDefeated;
    
    public void Init(GameManager gameManager, PlayerCharacter playerCharacter)
    {
        this.gameManager = gameManager;
        this.playerCharacter = playerCharacter;
        //canSpawn = true;
        numEnemiesAlive = 0;

        healthBarInstances = new Dictionary<int, ResourceUI>();
        
        onAllEnemiesDefeated = new UnityEvent();
    }

    void Update()
    {
        //if (canSpawn && !enemyAlive) SpawnEnemy(lastEnemyIdx);
    }

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
        healthBarInstances.Add(numEnemiesAlive, healthBarInstance);
        
        enemyInstance.onDeath.AddListener(() => CleanUpEnemy(healthBarInstance));

        // Navigation
        enemyInstance.GetComponent<CharacterTreeBase>().patrollPoints = patrolPoints;

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

    private void CleanUpEnemy(Component healthBarInstance)
    {
        Destroy(healthBarInstance.gameObject);

        //enemyAlive = false;
        --numEnemiesAlive;
        
        if (numEnemiesAlive == 0) onAllEnemiesDefeated.Invoke();
    }

    private void OnDestroy()
    {
        onAllEnemiesDefeated.RemoveAllListeners();
    }
}
