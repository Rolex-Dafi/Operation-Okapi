using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTrapTask : TaskBase
{
    private Trap trap;
    
    public ActivateTrapTask(CharacterTreeBase characterBT, Trap trap, string debugName = "") : base(characterBT, debugName)
    {
        this.trap = trap;
    }
    
    protected override void OnBegin()
    {
        bt.Character.ActivateTrap();
    }

    protected override void OnContinue()
    {
        // call on end when finished
        if (!trap.InUse)
        {
            Debug.Log("calling on end");
            OnEnd(true);
        }
    }
}
