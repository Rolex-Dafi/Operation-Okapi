using UnityEngine;

/// <summary>
/// A task which activates a trap
/// </summary>
public class ActivateTrapTask : TaskBase
{
    private Trap trap;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterBT">The behavioral tree of this character</param>
    /// <param name="trap">The trap to activate</param>
    /// <param name="debugName"></param>
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
