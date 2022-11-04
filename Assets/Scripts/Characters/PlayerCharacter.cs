using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
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

    private new PlayerCharacterSO Data {  
        get {
            if (data.GetType() == typeof(PlayerCharacterSO)) return data as PlayerCharacterSO;
            else
            {
                Debug.LogError("Player data not of correct type. Expected type: PlayerCharacterSO.");
                return null; 
            }
        } 
    }

    /// <summary>
    /// Should the player character read input?
    /// </summary>
    public bool ReadInput
    {
        set => playerController.ReadInput = value;
    }
   
    /// <summary>
    /// Initializes the player character.
    /// </summary>
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

    /// <summary>
    /// Starts aiming. Should be called when performing an aimed ranged attack.
    /// </summary>
    public void StartAiming()
    {
        // inform player controller
        playerController.Aiming = true;
        // show aiming gfx
        aimingGFX.SetActive(true);
    }

    /// <summary>
    /// Rotates the aiming graphic of an aimed ranged attack.
    /// </summary>
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

    /// <summary>
    /// Releases aiming of a ranged attack.
    /// </summary>
    public void StopAiming()
    {
        // inform player controller
        playerController.Aiming = false;
        // hide aiming gfx
        aimingGFX.SetActive(false);
    }
    
    /// <summary>
    /// Tries to begin or end an attack.
    /// </summary>
    /// <param name="attackButton">What button the player pressed</param>
    /// <param name="command">What command to perform</param>
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

            // set target for attack according to where the player is pointing their cursor
            if (!GeneralInput.GamepadConnected)
            {
                currentAttacks[attackButton].Target = playerController.GetMouseWorld();
            }
            
            Attack(currentAttacks[attackButton], command);
        }
    }

    /// <summary>
    /// Add money to player character.
    /// </summary>
    /// <param name="amount">The amount to collect</param>
    public void CollectMoney(int amount)
    {
        money.ChangeCurrent(amount);
    }

    /// <summary>
    /// Try to add an item to player inventory.
    /// </summary>
    /// <param name="item">Data of the item to add</param>
    /// <param name="onItemAdded">Called if the item was added successfully</param>
    public void CollectItem(ItemSO item, UnityAction onItemAdded = null)
    {
        if (Inventory.AddItem(item))
        {
            onItemAdded?.Invoke();
        }
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

    /// <summary>
    /// Pushes the character in the given direction.
    /// </summary>
    /// <param name="direction">The direction to push in</param>
    /// <param name="distance">The distance of the push</param>
    /// <param name="speed">The speed of the push</param>
    public void Push(Vector2 direction, float distance, float speed)
    {
        // interrupt movement
        canMove = false;
        StartCoroutine(rb.AddForceCustom(direction, distance, speed, () => canMove = true));
    }

}
