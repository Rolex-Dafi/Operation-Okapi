using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoom : MonoBehaviour
{
    [SerializeField] private Transform entrance;
    [SerializeField] private Interactable exitTrigger;

    [SerializeField] private Transform enemySpawn;
    [SerializeField] private Transform[] enemyPatrolPoints;
    
    public Transform Entrance => entrance;
    public Interactable ExitTrigger => exitTrigger;
    
    public Vector3 EnemySpawn => enemySpawn.position;
    public Vector3[] EnemyPatrolPoints
    {
        get
        {
            var patrolPositions = new Vector3[enemyPatrolPoints.Length];
            for (int i = 0; i < enemyPatrolPoints.Length; i++)
            {
                patrolPositions[i] = enemyPatrolPoints[i].position;
            }

            return patrolPositions;
        }
    }
}
