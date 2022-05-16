using UnityEngine;
using UnityEngine.Events;

public enum EAttackCommand { Begin, End, NDEF }

public enum EAttackEffect { Click, Spray, Aim } // equivalent to button down, hold, up - for player

/// <summary>
/// Base wrapper class for all possible attacks.
/// </summary>
public class Attack
{
    // for animator parameter
    public int attackID;

    public int damage;

    // does the character slow down during the attack + how much
    public float movementSpeedFactor = 0;

    public EAttackEffect attackEffect;

    protected AggressiveCharacter character;

    public Attack(AggressiveCharacter character, int attackID)
    {
        this.character = character;
        this.attackID = attackID;
    }

    public virtual void OnBegin()
    {
        // start playing anim
        character.animator.SetTrigger(EAnimationParameter.attack.ToString());
        character.animator.SetInteger(EAnimationParameter.attackID.ToString(), attackID);
        // slow down character
        character.SetMovementSpeed(movementSpeedFactor);
    }

    public virtual void OnContinue()
    {

    }

    public virtual void OnEnd()
    {
        // do nothing by default - so click attack works as intended on button down only
    }
}

public static class AttackExtensions
{



}
