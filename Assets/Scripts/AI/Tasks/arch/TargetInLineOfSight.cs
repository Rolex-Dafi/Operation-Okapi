using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if the given entity is in line of sight given the specified range and saves the entity's position
/// to the shared data under the given name.
/// </summary>
public class TargetInLineOfSight : CheckBase
{
    private Vector3 target;
    private float range;
    private string targetName;
    private string entityTag;

    public TargetInLineOfSight(CharacterTreeBase characterBT, Vector3 target, string targetName, float range = Mathf.Infinity, string entityTag = Utility.playerTagAndLayer) 
        : base(characterBT)
    {
        this.target = target;
        this.targetName = targetName;
        this.range = range;
        this.entityTag = entityTag;
    }

    protected override bool Check()
    {
        // takes all hits along the ray -> ignore the first collider (bc it's this)
        // we only need to check 2 points - 1st one will always be this collider, the second either null (nothing in range),
        // an obstacle (can't see the target) or the target (target unobstructed and in range)
        RaycastHit2D[] hits = new RaycastHit2D[2];
        int numHits = Physics2D.Raycast(bt.transform.position, target - bt.transform.position, new ContactFilter2D().NoFilter(), hits, range);

        // we hit something other than this collider
        if (numHits > 1)
        {
            // ignore the first collider (bc it's this one)
            // check whether the second collider belongs to the target
            if (hits[1].collider.CompareTag(entityTag))
            {
                // save the found target to shared data
                bt.AddItem(targetName, target);

                Debug.Log("entity in LOS range, range = " + range);
                Debug.Log("I'm at " + bt.transform.position + ", target is at " + target);

                // report success
                return true;
            }
        }

        return false;
    }
}
