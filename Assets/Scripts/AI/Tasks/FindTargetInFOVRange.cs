using UnityEngine;

public class FindTargetInFOVRange : CheckBase
{
    private string targetName;
    private float range;
    private string targetTag;
    private bool checkObstacles;

    public FindTargetInFOVRange(CharacterTreeBase characterBT, string targetName, float range, 
        string targetTag = Utility.playerTagAndLayer, bool checkObstacles = true, string debugName = "") : base(characterBT, debugName)
    {
        this.range = range;
        this.targetName = targetName;
        this.targetTag = targetTag;
        this.checkObstacles = checkObstacles;
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
            Debug.Log("checking collider " + collider.name);

            if (collider.CompareTag(targetTag))
            {
                if (checkObstacles)
                {
                    // is there an obstacle between this and the target ?
                    LayerMask layerMask = LayerMask.NameToLayer(Utility.obstacleLayer);
                    RaycastHit2D hit = Physics2D.Linecast(bt.transform.position, collider.transform.position, layerMask);

                    // if there is no obstacle
                    if (hit.collider == null)
                    {
                        // save the found target to shared data
                        bt.AddItem(targetName, collider.transform.position);

                        // report success
                        return true;
                    }
                }
                else
                {
                    // save the found target to shared data
                    bt.AddItem(targetName, collider.transform.position);

                    // report success
                    return true;
                }
            }
        }

        return false;
    }

}
