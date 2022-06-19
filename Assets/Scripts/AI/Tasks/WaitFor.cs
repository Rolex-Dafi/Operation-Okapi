using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitFor : BTTaskBase
{
    private float waitFor;

    private float timePassed;

    public WaitFor(BTTreeBase characterBT, float waitFor) : base(characterBT)
    {
        this.waitFor = waitFor;
    }

    protected override void OnBegin()
    {
        timePassed = 0;
        // play idle animation
        bt.Character.Move(Vector2.zero);
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
