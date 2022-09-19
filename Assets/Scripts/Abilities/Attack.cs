using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base wrapper class for all possible attacks.
/// </summary>
public class Attack : Ability
{
    public Vector3? Target { get; set; }
    
    public AttackSO Data => (AttackSO)data;

    protected Attack(CombatCharacter character, AttackSO data, EAbilityType type) : base(character, data, type)
    {
        this.data = data;
    }

    public override void OnBegin()
    {
        base.OnBegin();

        // start playing anim
        character.Animator.SetTrigger(EAnimationParameter.attack.ToString());
        character.Animator.SetInteger(EAnimationParameter.attackID.ToString(), Data.id);
        // slow down character
        character.SetMovementSpeed(Data.movementSpeedFactor);
    }

}
