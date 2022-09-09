using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTrapTask : TaskBase
{
    public ActivateTrapTask(CharacterTreeBase characterBT, string debugName = "") : base(characterBT, debugName)
    {
    }
    
    protected override void OnBegin()
    {
        bt.Character.ActivateTrap();
    }

    protected override void OnContinue()
    {
        
    }
}
