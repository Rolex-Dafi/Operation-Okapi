using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyCharacter[] enemyPrefabs;

    // UI
    [SerializeField] private Canvas worldSpaceCanvas;
    [SerializeField] private ResourceUI healthBarPrefab;
    private ResourceUI healthBarInstance;

    // Navigation
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] patrollPoints;

    private bool canSpawn = false;
    private bool enemyAlive = false;

    private int lastEnemyIdx = 0;

    private PlayerCharacter playerCharacter;
    
    public void Init(PlayerCharacter playerCharacter)
    {
        this.playerCharacter = playerCharacter;
        canSpawn = true;
    }

    void Update()
    {
        if (canSpawn && !enemyAlive) SpawnEnemy(lastEnemyIdx);
    }

    private void SpawnEnemy(int i)
    {
        EnemyCharacter enemyInstance = Instantiate(enemyPrefabs[i], spawnPoint.position, Quaternion.identity, transform);
        enemyInstance.Init(playerCharacter);
        enemyInstance.onDeath.AddListener(CleanUpEnemy);

        // UI
        healthBarInstance = Instantiate(healthBarPrefab, worldSpaceCanvas.transform);
        healthBarInstance.Init(enemyInstance.Health);
        healthBarInstance.GetComponent<FollowTarget>().Init(enemyInstance.transform);

        // Navigation
        enemyInstance.GetComponent<CharacterTreeBase>().patrollPoints = patrollPoints;

        enemyAlive = true;

        ++lastEnemyIdx;
        lastEnemyIdx = lastEnemyIdx > enemyPrefabs.Length - 1 ? 0 : lastEnemyIdx;
    }

    private void CleanUpEnemy()
    {
        Destroy(healthBarInstance.gameObject);

        enemyAlive = false;

    }
}
