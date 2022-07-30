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

    public TargetInRange(CharacterTreeBase characterBT, string targetName, float range) : base(characterBT)
    {
        this.targetName = targetName;
        this.range = range;
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
            Debug.Log("target in range ? " + (Vector3.Distance((Vector3)target, bt.transform.position) <= range) + ", range = " + range);
            Debug.Log("I'm at " + bt.transform.position + ", target is at " + target);

            return Vector3.Distance((Vector3)target, bt.transform.position) <= range;
        }
    }
}
