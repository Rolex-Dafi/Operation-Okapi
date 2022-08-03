using Pathfinding;
using UnityEngine;

public class WalkToTarget : TaskBase
{
    private Vector3? target;
    private string targetName;
    private Path currentPath;
    private float nextWaypointSqrtDistance = 0.05f;
    private int nextWaypoint = 0;
    private bool currentlyMoving = false;

    private bool targetStatic;

    private float lastRecalculateTime;
    private float recalculateDelta = .2f;

    // for targets known at the time this character's bt is initialized
    public WalkToTarget(CharacterTreeBase characterBT, Vector3 target, string debugName = "") : base(characterBT, debugName) 
    {
        this.target = target;
        targetStatic = true;
    }

    // for targets set at runtime by other tasks in this character's bt
    public WalkToTarget(CharacterTreeBase characterBT, string targetName, string debugName = "") : base(characterBT, debugName)
    {
        this.targetName = targetName;
        targetStatic = false;

        // assumes runtime targets have a nonzero radius - like the player
        // hacky constant which should apply to player:
        nextWaypointSqrtDistance = 1f;
    }

    protected override void OnBegin()
    {
        RecalculatePath();
    }

    private void RecalculatePath()
    {
        // if target not set at the beginning (i.e. static), try to get it from shared data
        if (!targetStatic)
        {
            target = bt.GetItem(targetName) as Vector3?;
        }
        // if it's not in shared data either -> report failure
        if (target == null)
        {
            OnEnd(false);
            return;
        }

        // reset
        currentlyMoving = false;
        currentPath = null;
        nextWaypoint = 0;

        // try to find the path
        bt.Seeker.StartPath(bt.transform.position, target.GetValueOrDefault(), OnPathComplete);

        lastRecalculateTime = Time.time;
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

        //Debug.Log("path found, path ends at: " + currentPath.vectorPath[currentPath.vectorPath.Count-1]);
    }

    protected override void OnContinue()
    {
        // ensures that we only move after a path has been found and set
        if (currentlyMoving) WalkPath();
    }

    private void WalkPath()
    {
        // check if we want to recalculate path
        if (!targetStatic && Time.time - lastRecalculateTime > recalculateDelta)
        {
            //Debug.Log("Recalculating path");
            RecalculatePath();
            return;
        }

        // check if we want to switch to next waypoint
        float distToWaypoint;
        while (true)
        {
            // TODO stop walk when walking into a collider - ex. the player
            distToWaypoint = (bt.transform.position - currentPath.vectorPath[nextWaypoint]).sqrMagnitude;
            if (distToWaypoint < nextWaypointSqrtDistance)
            {
                if (nextWaypoint + 1 < currentPath.vectorPath.Count)
                {
                    ++nextWaypoint;
                }
                else
                {
                    Debug.LogWarning("ending walk");
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
        bt.Character.ForceUpdateSpeed(Vector2.zero);
        // report success
        OnEnd(true);
    }

    public override string ToString()
    {
        return "WalkTo task, target: " + target;
    }
}
