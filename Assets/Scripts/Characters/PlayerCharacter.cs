using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player movement and combat - both animation and physics.
/// </summary>
public class PlayerCharacter : AggressiveCharacter
{
    [SerializeField] private float playerMovementSpeed;

    private Dictionary<EAttackButton, Attack> currentAttacks;

    public void Init()
    {
        Init(3, 0, playerMovementSpeed);

        // TODO - init attacks from scriptable objects
        MeleeAttack melee = new MeleeAttack(this);
        RangedAttack ranged = new RangedAttack(this, null);

        attacks.Add(melee);
        attacks.Add(ranged);

        currentAttacks = new Dictionary<EAttackButton, Attack>
        {
            { EAttackButton.Melee, melee },
            { EAttackButton.Ranged, ranged },
        };
    }

    public void Attack(EAttackButton attackButton, EAttackCommand command)
    {
        switch (command)
        {
            case EAttackCommand.Begin:
                currentAttacks[attackButton]?.OnBegin();
                break;
            case EAttackCommand.End:
                currentAttacks[attackButton]?.OnEnd();
                break;
        }
    }

    public void RotateWithMouse()
    {
        // set facing acc to mouse position
    }


}
