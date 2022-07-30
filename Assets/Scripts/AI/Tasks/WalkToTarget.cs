using Pathfinding;
using UnityEngine;

public class WalkToTarget : TaskBase
{
    private Vector3? target;
    private string targetName;
    private Path currentPath;
    private float nextWaypointSqrtDistance = 0.01f;
    private int nextWaypoint = 0;
    private bool currentlyMoving = false;

    // for targets known at the time this character's bt is initialized
    public WalkToTarget(CharacterTreeBase characterBT, Vector3 target) : base(characterBT) 
    {
        this.target = target;
    }

    // for targets set at runtime by other tasks in this character's bt
    public WalkToTarget(CharacterTreeBase characterBT, string targetName) : base(characterBT)
    {
        this.targetName = targetName;
    }

    protected override void OnBegin()
    {
        // if target not set at the beginning, try to get it from shared data
        if (target == null)
        {
            target = bt.GetItem(targetName) as Vector3?;
            // if it's not in shared data either -> report failure
            if (target == null)
            {
                OnEnd(false);
            }
        }

        // try to find the path
        bt.Seeker.StartPath(bt.transform.position, (Vector3)target, OnPathComplete);
    }

    private void OnPathComplete(Path path)
    {
        // if path not found, report failure
        if (path.error)
        {
            OnEnd(false);
            return;
        }

        currentlyMoving = true;
        currentPath = path;
        nextWaypoint = 0;
    }

    protected override void OnContinue()
    {
        // ensures that we only move after a path has been found and set
        if (currentlyMoving) WalkPath();
    }

    private void WalkPath()
    {
        // check if we want to switch to next waypoint
        float distToWaypoint;
        while (true)
        {
            distToWaypoint = (bt.transform.position - currentPath.vectorPath[nextWaypoint]).sqrMagnitude;
            if (distToWaypoint < nextWaypointSqrtDistance)
            {
                if (nextWaypoint + 1 < currentPath.vectorPath.Count)
                {
                    ++nextWaypoint;
                }
                else
                {
                    EndWalk();
                    return;
                }
            }
            else
            {
                break;
            }
        }

        Vector2 movementDir = (currentPath.vectorPath[nextWaypoint] - bt.transform.position).normalized;
        bt.Character.Move(movementDir);
    }

    private void EndWalk()
    {
        currentlyMoving = false;
        bt.Character.Move(Vector2.zero);
        // report success
        OnEnd(true);
    }

    public override string ToString()
    {
        return "WalkTo task, target: " + target;
    }
}
