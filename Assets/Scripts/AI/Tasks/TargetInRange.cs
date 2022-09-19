using UnityEngine;

/// <summary>
/// Checks whether target is within specified range. 
/// </summary>
public class TargetInRange : CheckBase
{
    private string targetName;
    private float range;
    private EObstacleFilter obstacleFilter;

    /// <summary>
    /// Creates a task instance.
    /// </summary>
    /// <param name="characterBT">The behavioral tree of this character</param>
    /// <param name="targetName">The name of the target - this task will try to retrieve the target from shared memory.</param>
    /// <param name="range">How far can the character see</param>
    /// <param name="obstacleFilter">Specifies which objects in the scene should obstruct this character's line of sight</param>
    /// <param name="debugName">Used in ToString() for debug purposes</param>
    public TargetInRange(CharacterTreeBase characterBT, string targetName, float range, 
        EObstacleFilter obstacleFilter = EObstacleFilter.None, string debugName = "") : base(characterBT, debugName)
    {
        this.targetName = targetName;
        this.range = range;
        this.obstacleFilter = obstacleFilter;
    }

    protected override bool Check()
    {
        // try to get the target from shared data
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

                // if no collider found -> no obstacles -> target in FOV range
                return hit.collider == null;
            }

            // target in range
            return true;
        }
    }
}
