using System;
using UnityEngine;
using UnityEngine.Events;

public enum EAttackCommand { Begin, End, NDEF }

public enum EAttackEffect { Click, Spray, Aim } // equivalent to button down, hold, up - for player

/// <summary>
/// Base wrapper class for all possible attacks.
/// </summary>
public class Attack
{
    protected AggressiveCharacter character;

    private int attackID;
    private int damage;    
    private float movementSpeedFactor = 0;
    private EAttackEffect attackEffect;

    public int AttackID { get => attackID; private set => attackID = value; }
    public int Damage { get => damage; set => damage = value; }    
    public float MovementSpeedFactor { get => movementSpeedFactor; set => movementSpeedFactor = value; } // does the character slow down during the attack + how much
    public EAttackEffect AttackEffect { get => attackEffect; set => attackEffect = value; }

    public Attack(int attackID, AggressiveCharacter character)
    {
        this.character = character;
        AttackID = attackID;
    }

    public virtual void OnBegin()
    {
        // start playing anim
        character.animator.SetTrigger(EAnimationParameter.attack.ToString());
        character.animator.SetInteger(EAnimationParameter.attackID.ToString(), AttackID);
        // slow down character
        character.SetMovementSpeed(MovementSpeedFactor);
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
