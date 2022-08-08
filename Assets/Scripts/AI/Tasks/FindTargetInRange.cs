using UnityEngine;

public class FindTargetInRange : CheckBase
{
    private string targetName;
    private float range;
    private string targetTag;
    private EObstacleFilter obstacleFilter;

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
