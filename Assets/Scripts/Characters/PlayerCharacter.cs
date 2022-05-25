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

    private PlayerController playerController;

    [SerializeField] private GameObject aimingGFX;

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

        playerController = GetComponent<PlayerController>();
    }

    public void StartAiming()
    {
        // inform player controller
        playerController.RotatingWithMouse = true;
        // show aiming gfx
        aimingGFX.SetActive(true);
    }

    public void RotateAimingGFX()
    {
        // TODO make it follow isometry rules
        /*Vector3 eulerAngles = Quaternion.FromToRotation(transform.up, Facing).eulerAngles;
        aimingGFX.transform.rotation = Quaternion.Euler(
            Utility.isometricAngle * (1 - (eulerAngles.z % 180) / 180), 
            eulerAngles.y, 
            eulerAngles.z
        );*/

        // rotate in the direction the player is facing
        aimingGFX.transform.rotation = Quaternion.FromToRotation(transform.up, Facing);
    }

    public void StopAiming()
    {
        // inform player controller
        playerController.RotatingWithMouse = false;
        // hide aiming gfx
        aimingGFX.SetActive(false);
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


}
