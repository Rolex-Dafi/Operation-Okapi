using System.Collections.Generic;
using UnityEngine;

public class TargetInLineOfSight : CheckBase
{
    private Transform target;
    private float range;
    private string targetTag;

    public TargetInLineOfSight(CharacterTreeBase characterBT, Transform target, float range = Mathf.Infinity, string targetTag = Utility.playerTag) : base(characterBT)
    {
        this.target = target;
        this.range = range;
        this.targetTag = targetTag;
    }

    protected override bool Check()
    {
        // takes all hits along the ray -> ignore the first collider (bc it's this)
        // we only need to check 2 points - 1st one will always be this collider, the second either null (nothing in range),
        // an obstacle (can't see the target) or the target (target unobstructed and in range)
        RaycastHit2D[] hits = new RaycastHit2D[2];
        int numHits = Physics2D.Raycast(bt.transform.position, target.position - bt.transform.position, new ContactFilter2D().NoFilter(), hits, range);

        // we hit something other than this collider
        if (numHits > 1)
        {
            // ignore the first collider (bc it's this one)
            // check whether the second collider belongs to the target
            if (hits[1].collider.CompareTag(targetTag)) return true;
        }

        return false;
    }
}
