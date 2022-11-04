using UnityEngine;

/// <summary>
/// Performs the given attack in the direction of the target set in shared data.
/// </summary>
public class AttackTarget : TaskBase
{
    private string targetName;
    private Vector3? target;
    private Attack attack;
    private bool fromSky;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterBT">The behavioral tree of this character</param>
    /// <param name="attack">The attack to perform</param>
    /// <param name="targetName">The name of the target - this task will try to retrieve the target from shared memory.</param>
    /// <param name="fromSky">Should we attack the target specifically or attack in the direction of the target</param>
    /// <param name="debugName"></param>
    public AttackTarget(CharacterTreeBase characterBT, Attack attack, string targetName, bool fromSky = false, string debugName = "") : base(characterBT, debugName)
    {
        this.targetName = targetName;
        this.attack = attack;
        this.fromSky = fromSky;
    }

    protected override void OnBegin()
    {
        // get the target from shared data
        target = bt.GetItem(targetName) as Vector3?;
        // if it's not in shared data -> report failure
        if (target == null)
        {
            OnEnd(false);
            return;
        }        

        // direct the character towards the target
        bt.Character.Rotate((target.GetValueOrDefault() - bt.Character.transform.position).normalized);

        // try to attack
        if (fromSky)
        {
            if (!bt.Character.AttackTargetFromSky(attack as RangedAttack, target.GetValueOrDefault()))
            {
                OnEnd(false);
            }
        }
        else
        {
            if (!bt.Character.Attack(attack))
            {
                OnEnd(false);
            }
        }
        
    }

    protected override void OnContinue()
    {
        // ensure we're not playing move anim while attacking
        bt.Character.ForceIdle();

        // call onEnd with success after attack finishes
        if (!attack.InUse)
        {
            OnEnd(true);
        }

    }
}
