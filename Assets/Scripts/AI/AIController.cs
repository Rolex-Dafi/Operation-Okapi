using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// TODO change this into Pathfinder script only, and write a new AI controller
public class AIController : MonoBehaviour
{
    // for debug purposes
    private Vector3 targetPosition;

    private AggressiveCharacter enemyCharacter;
    private Seeker seeker;

    private bool shouldMove = false;
    private Path currentPath;
    private int nextWaypoint = 0;
    private float nextWaypointSqrtDistance = 0.01f;

    [HideInInspector] public UnityEvent onDestinationReached = new UnityEvent();

    void Start()
    {
        enemyCharacter = GetComponent<AggressiveCharacter>();
        enemyCharacter.Init(5, 0);

        seeker = GetComponent<Seeker>();
    }

    private void Update()
    {
        /*if (GetTarget())
        {
            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }*/

        if (shouldMove)
        {
            Move();
        }
    }

    public void MoveTo(Transform target)
    {
        if (shouldMove) return;

        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    private void OnPathComplete(Path path)
    {
        if (path.error) return;

        shouldMove = true;
        currentPath = path;
        nextWaypoint = 0;
    }

    private void Move()
    {
        // check if we want to switch to next waypoint
        float distToWaypoint = 0;
        while(true)
        {
            distToWaypoint = (transform.position - currentPath.vectorPath[nextWaypoint]).sqrMagnitude;
            if (distToWaypoint < nextWaypointSqrtDistance)
            {
                if (nextWaypoint + 1 < currentPath.vectorPath.Count)
                {
                    ++nextWaypoint;
                }
                else
                {
                    shouldMove = false;
                    enemyCharacter.Move(Vector2.zero);
                    onDestinationReached.Invoke();
                    Debug.Log("End of path reached");
                    return;
                }
            }
            else
            {
                break;
            }
        }

        Vector2 movementDir = (currentPath.vectorPath[nextWaypoint] - transform.position).normalized;
        enemyCharacter.Move(movementDir);
    }


    private void OnDestroy()
    {
        onDestinationReached.RemoveAllListeners();
    }

    // for debug purposes
    private bool GetTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            Debug.Log("mouse world: " + targetPosition);
            return true;
        }
        return false;
    }
}
