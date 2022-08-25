using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles player movement and combat - both animation and physics.
/// </summary>
public class PlayerCharacter : CombatCharacter, IPushable
{
    private Dictionary<EAttackButton, Attack> currentAttacks;

    private PlayerController playerController;

    [SerializeField] private GameObject aimingGFX;

    public PlayerInventory Inventory { get; private set; }

    public Respect Respect { get; private set; }

    public new PlayerCharacterSO Data {  
        get {
            if (data.GetType() == typeof(PlayerCharacterSO)) return data as PlayerCharacterSO;
            else
            {
                Debug.LogError("Player data not of correct type. Expected type: PlayerCharacterSO.");
                return null; 
            }
        } 
    }

    public override void Init()
    {
        base.Init();
        Respect = new Respect(Data.respect);

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

        // inventory
        Inventory = new PlayerInventory(Data);

        playerController = GetComponent<PlayerController>();
    }

    public void StartAiming()
    {
        // inform player controller
        playerController.Aiming = true;
        // show aiming gfx
        aimingGFX.SetActive(true);
    }

    public void RotateAimingGFX()
    {
        // rotate in the direction the player is facing
        aimingGFX.transform.rotation = Quaternion.FromToRotation(transform.up, Facing);

        // make it follow isometry rules
        Vector3 eulerAngles = Quaternion.FromToRotation(transform.up, Facing).eulerAngles;
        aimingGFX.transform.rotation = Quaternion.Euler(
            Utility.isometricAngle,
            eulerAngles.y,
            eulerAngles.z
        );
    }

    public void StopAiming()
    {
        // inform player controller
        playerController.Aiming = false;
        // hide aiming gfx
        aimingGFX.SetActive(false);
    }

    public void Attack(EAttackButton attackButton, EAttackCommand command)
    {
        if (currentAttacks[attackButton] != null)
        {
            int attackCost = currentAttacks[attackButton].Data.cost;

            // not enough money to perform the attack
            if (money.GetCurrent() < attackCost) return;

            // only decrease money when ending the attack
            if (command == EAttackCommand.End)
            {
                money.ChangeCurrent(-attackCost);
            }

            Attack(currentAttacks[attackButton], command);
        }
    }

    public void CollectMoney(int amount)
    {
        money.ChangeCurrent(amount);
    }

    public void CollectItem(ItemSO item)
    {
        Inventory.AddItem(item);
    }

    public override void TakeDamage(int amount)
    {
        Animator.SetTrigger(EAnimationParameter.hit.ToString());
        
        // redirect dmg to inventory (equipped items), get back player health
        var current = Inventory.ReceiveDamage(amount);
        if (current == 0) Die();
        else
        {
            RuntimeManager.PlayOneShot(data.onHitSound.Guid);
        }
    }

    private void OnDestroy()
    {
        Respect.CleanUp();
    }

    public void Push(Vector2 direction, float distance, float speed)
    {
        // interrupt movement
        canMove = false;
        StartCoroutine(rb.AddForceCustom(direction, distance, speed, () => canMove = true));
    }

}
