using UnityEngine;

/// <summary>
/// A task which tries to find a target (typically the player) and save it.
/// </summary>
public class FindTargetInRange : CheckBase
{
    private string targetName;
    private float range;
    private string targetTag;
    private EObstacleFilter obstacleFilter;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterBT">The behavioral tree of this character</param>
    /// <param name="targetName">The name of the target - this task will save any found target to shared memory.</param>
    /// <param name="range"></param>
    /// <param name="obstacleFilter"></param>
    /// <param name="targetTag">The tag of the target we're trying to find</param>
    /// <param name="debugName"></param>
    public FindTargetInRange(CharacterTreeBase characterBT, string targetName, float range, EObstacleFilter obstacleFilter = EObstacleFilter.None, 
        string targetTag = Utility.playerTagAndLayer, string debugName = "") : base(characterBT, debugName)
    {
        this.range = range;
        this.targetName = targetName;
        this.obstacleFilter = obstacleFilter;
        this.targetTag = targetTag;
    }

    protected override bool Check()
    {
        // clear target
        bt.RemoveItem(targetName);

        // check all colliders within range
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(bt.transform.position, range);

        // see if any of them match the target's tag
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag(targetTag))
            {
                if (obstacleFilter.ShouldFilter())
                {
                    // is there an obstacle between this and the target ?
                    RaycastHit2D hit = Physics2D.Linecast(bt.transform.position, collider.transform.position, obstacleFilter.ToLayerMask());

                    // if there is no obstacle
                    if (hit.collider == null)
                    {
                        // save the found target to shared data
                        bt.AddItem(targetName, collider.transform.position);

                        //Debug.LogWarning("found unobstructed target at " + collider.transform.position);

                        // report success
                        return true;
                    }
                }
                else
                {
                    // save the found target to shared data
                    bt.AddItem(targetName, collider.transform.position);

                    //Debug.LogWarning("found target at " + collider.transform.position);

                    // report success
                    return true;
                }
            }
        }

        return false;
    }

}
