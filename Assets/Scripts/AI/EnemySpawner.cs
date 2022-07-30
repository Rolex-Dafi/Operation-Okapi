using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyCharacter enemyPrefab;

    // UI
    [SerializeField] private Canvas worldSpaceCanvas;
    [SerializeField] private ResourceUI healthBarPrefab;
    private ResourceUI healthBarInstance;

    // Navigation
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] patrollPoints;

    private bool canSpawn = false;
    private bool enemyAlive = false;

    public void Init()
    {
        canSpawn = true;
    }

    void Update()
    {
        if (canSpawn && !enemyAlive) SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        EnemyCharacter enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, transform);
        enemyInstance.Init();
        enemyInstance.onDeath.AddListener(CleanUpEnemy);

        // UI
        healthBarInstance = Instantiate(healthBarPrefab, worldSpaceCanvas.transform);
        healthBarInstance.Init(enemyInstance.Health);
        healthBarInstance.GetComponent<FollowTarget>().Init(enemyInstance.transform);

        // Navigation
        enemyInstance.GetComponent<CharacterTreeBase>().patrollPoints = patrollPoints;

        enemyAlive = true;
    }

    private void CleanUpEnemy()
    {
        Destroy(healthBarInstance.gameObject);

        enemyAlive = false;

    }
}
