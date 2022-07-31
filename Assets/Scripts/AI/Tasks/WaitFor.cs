using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitFor : TaskBase
{
    private float waitFor;

    private float timePassed;

    public WaitFor(CharacterTreeBase characterBT, float waitFor, string debugName = "") : base(characterBT, debugName)
    {
        this.waitFor = waitFor;
    }

    protected override void OnBegin()
    {
        timePassed = 0;
        // ensure we play idle animation, not movement
        bt.Character.ForceUpdateSpeed(Vector2.zero);
    }

    protected override void OnContinue()
    {
        if (timePassed > waitFor)
        {
            OnEnd(true);
        }
        else
        {
            timePassed += Time.deltaTime;
        }
    }
}
