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

        // TODO - init attacks from scriptable object
        attacks.Add(new MeleeAttack(this));
        attacks.Add(new RangedAttack(this));

    }



}
