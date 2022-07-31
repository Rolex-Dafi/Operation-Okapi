using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAtRange : CheckBase
{
    private string targetName;
    private float range;
    private float leeway;

    public TargetAtRange(CharacterTreeBase characterBT, string targetName, float range, float leeway = .01f, string debugName = "") : base(characterBT, debugName)
    {
        this.targetName = targetName;
        this.range = range;
        this.leeway = leeway;
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
        // else return if it's at range with the specified leeway
        else
        {
            return Mathf.Abs(Vector3.Distance((Vector3)target, bt.transform.position) - range) < leeway;
        }
    }
}
