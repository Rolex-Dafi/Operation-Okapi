using BehaviourTree;
using Pathfinding;
using UnityEngine;

public class WalkToTarget : TaskBase
{
    private Vector3 target;
    private Path currentPath;
    private float nextWaypointSqrtDistance = 0.01f;
    private int nextWaypoint = 0;
    private bool currentlyMoving = false;

    public WalkToTarget(CharacterTreeBase characterBT, Vector3 target) : base(characterBT) 
    {
        this.target = target;
    }

    protected override void OnBegin()
    {
        bt.Seeker.StartPath(bt.transform.position, target, OnPathComplete);
    }

    private void OnPathComplete(Path path)
    {
        if (path.error)
        {
            // end task with failure
            OnEnd(false);
            return;
        }

        Debug.Log("starting path to position " + target);
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
        Debug.Log("End of path reached");
        // end task with success
        OnEnd(true);
    }

    public override string ToString()
    {
        return "WalkTo task, target: " + target;
    }
}
