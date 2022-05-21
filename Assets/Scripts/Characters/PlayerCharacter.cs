using System.Collections.Generic;
using System.Linq;
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

        // init attacks
        MeleeAttack melee = attacks.OfType<MeleeAttack>().ToArray()[0];
        if (melee == null) Debug.LogError("Melee attack for player not found!");

        RangedAttack ranged = attacks.OfType<RangedAttack>().ToArray()[0];
        if (ranged == null) Debug.LogError("Ranged attack for player not found!");

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
