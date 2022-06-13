using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base wrapper class for all possible attacks.
/// </summary>
public class Attack
{
    protected AggressiveCharacter character;

    protected AttackScriptableObject data;

    public Attack(AggressiveCharacter character, AttackScriptableObject data)
    {
        this.character = character;
        this.data = data;
    }

    public virtual void OnBegin()
    {
        // start playing anim
        character.animator.SetTrigger(EAnimationParameter.attack.ToString());
        character.animator.SetInteger(EAnimationParameter.attackID.ToString(), data.id);
        // slow down character
        character.SetMovementSpeed(data.movementSpeedFactor);
    }

    public virtual void OnContinue()
    {
        // do nothing by default - so click attack works as intended on button down only
    }

    public virtual void OnEnd()
    {
        // do nothing by default - so click attack works as intended on button down only
    }

}

public static class AttackExtensions
{



}
