using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks whether target is within specified range. (Doesn't check whether it's unobstructed.)
/// </summary>
public class TargetInRange : CheckBase
{
    private string targetName;
    private float range;
    private EObstacleFilter obstacleFilter;

    public TargetInRange(CharacterTreeBase characterBT, string targetName, float range, 
        EObstacleFilter obstacleFilter = EObstacleFilter.None, string debugName = "") : base(characterBT, debugName)
    {
        this.targetName = targetName;
        this.range = range;
        this.obstacleFilter = obstacleFilter;
    }

    protected override bool Check()
    {
        // try to get the taget from shared data
        Vector3? target = bt.GetItem(targetName) as Vector3?;
        // if it's not in shared data -> report failure
        if (target == null)
        {
            return false;
        }
        // else return if it's in range
        else
        {
            // target out of range
            if (Vector3.Distance((Vector3)target, bt.transform.position) > range) return false;

            // should we take into account any obstacles ?
            if (obstacleFilter.ShouldFilter())
            {
                // is there an obstacle between this and the target ?
                RaycastHit2D hit = Physics2D.Linecast(bt.transform.position, (Vector3)target, obstacleFilter.ToLayerMask());

                if (hit.collider != null)
                    Debug.LogWarning("in should filter, hit collider: " + hit.collider.name);

                // if no collider found -> no obstacles -> target in FOV range
                return hit.collider == null;
            }

            // target in range
            return true;
        }
    }
}
