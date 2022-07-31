using UnityEngine;

public class FindTargetInFOVRange : CheckBase
{
    private string targetName;
    private float range;
    private string targetTag;
    private string myTag;

    public FindTargetInFOVRange(CharacterTreeBase characterBT, string targetName, float range, string targetTag = Utility.playerTagAndLayer, string debugName = "") : base(characterBT, debugName)
    {
        this.range = range;
        this.targetName = targetName;
        this.targetTag = targetTag;
        myTag = bt.gameObject.tag;
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
                // TODO make this more modular ?
                LayerMask layerMask = LayerMask.NameToLayer(Utility.obstacleLayer);
                RaycastHit2D hit = Physics2D.Raycast(bt.transform.position, collider.transform.position - bt.transform.position, layerMask);

                // if there is no obstacle
                if (hit.collider == null)
                {
                    // save the found target to shared data
                    bt.AddItem(targetName, collider.transform.position);

                    Debug.LogWarning("found target at " + collider.transform.position);

                    // report success
                    return true;
                } 

            }
        }

        return false;
    }

}
