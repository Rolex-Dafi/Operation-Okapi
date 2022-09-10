using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A task which makes the character wait for a specified time.
/// </summary>
public class WaitFor : TaskBase
{
    private float waitFor;

    private float timePassed;

    /// <summary>
    /// Creates a new wait task.
    /// </summary>
    /// <param name="characterBT">The behavioral tree of this character</param>
    /// <param name="waitFor">How long do we want to wait for</param>
    /// <param name="debugName">Used in ToString() for debug purposes</param>
    public WaitFor(CharacterTreeBase characterBT, float waitFor, string debugName = "") : base(characterBT, debugName)
    {
        this.waitFor = waitFor;
    }

    protected override void OnBegin()
    {
        timePassed = 0;
        // ensure we play idle animation, not movement
        bt.Character.ForceIdle();
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
