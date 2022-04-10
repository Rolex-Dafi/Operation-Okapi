using UnityEngine;

/// <summary>
/// Handles player movement and combat - both animation and physics.
/// </summary>
public class PlayerCharacter : AggressiveCharacter
{
    [SerializeField] private float playerMovementSpeed;

    public void Init()
    {
        Init(3, 0, playerMovementSpeed);
    }

    public void MeleeAttack()
    {
        // TODO later: figure out what attack to use depending on the current weapon equipped
        // - this means attacks will be stored on the weapons
        Attack(new MeleeAttack());
    }

    public void RangedAttack()
    {
        // TODO later: figure out what attack to use depending on the current weapon equipped
        // - this means attacks will be stored on the weapons
        Attack(new RangedAttack());
    }

    public void SpecialAttack()
    {
        // TODO later: figure out what attack to use depending on the current weapon equipped
        // - this means attacks will be stored on the weapons
        Attack(new SpecialAttack());
    }
}
