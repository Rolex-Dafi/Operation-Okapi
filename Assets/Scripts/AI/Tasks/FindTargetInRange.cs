using UnityEngine;

public class FindTargetInRange : CheckBase
{
    private string targetName;
    private float range;
    private string targetTag;

    public FindTargetInRange(CharacterTreeBase characterBT, string targetName, float range, string targetTag = Utility.playerTagAndLayer, string debugName = "") : base(characterBT, debugName)
    {
        this.range = range;
        this.targetName = targetName;
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
                // save the found target to shared data
                bt.AddItem(targetName, collider.transform.position);

                //Debug.LogWarning("found target at " + collider.transform.position);

                // report success
                return true;
            }
        }

        return false;
    }

}
