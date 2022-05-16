using UnityEngine;
using UnityEngine.Events;
public enum EAttackCommand { Begin, End, NDEF }
public enum EAttackType { Melee, Ranged, Special, NDEF }
public enum EAttackEffect { Click, Spray, Aim } // equivalent to button down, hold, up - for player

/// <summary>
/// Base wrapper class for all possible attacks.
/// </summary>
public class Attack
{
    // for animator parameter
    public int attackNumber;

    // does the character slow down during the attack + how much
    [Range(0,1)] public float movementSpeedFactor = 0;

    public EAttackType attackType;
    public EAttackEffect attackEffect;

    protected AggressiveCharacter character;

    public Attack(AggressiveCharacter character, int attackNumber)
    {
        this.character = character;
        this.attackNumber = attackNumber;
    }

    public void OnBegin()
    {
        // start playing anim
        character.animator.SetTrigger(EAnimationParameter.attack.ToString());
        character.animator.SetInteger(EAnimationParameter.attackNumber.ToString(), attackNumber);
        // slow down character
        character.SetMovementSpeed(movementSpeedFactor);


    }

    public void OnContinue()
    {

    }

    public void OnEnd()
    {
        // do nothing by default - so click attack works as intended on button down only
    }
}

public static class AttackExtensions
{



}
