using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private AggressiveCharacter enemyPrefab;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] patrollPoints;

    private bool enemyAlive = false;


    void Start()
    {
        SpawnEnemy();
    }

    void Update()
    {
        if (!enemyAlive) SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        enemyAlive = true;
        AggressiveCharacter enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        // TODO change this to initialization from a scriptable object
        enemyInstance.Init(5, 1);
        enemyInstance.onDeath.AddListener(CleanUpEnemy);
        enemyInstance.GetComponent<PatrollBT>().patrollPoints = patrollPoints;
        Debug.Log("enemy spawning finished");
    }

    private void CleanUpEnemy()
    {
        enemyAlive = false;
        Debug.Log("enemy cleanup finished");

    }
}
