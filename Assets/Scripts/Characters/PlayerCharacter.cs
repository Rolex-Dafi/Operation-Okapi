using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles player movement and combat - both animation and physics.
/// </summary>
public class PlayerCharacter : CombatCharacter, IPushable
{
    private Respect respect;

    private Dictionary<EAttackButton, Attack> currentAttacks;

    private PlayerController playerController;

    [SerializeField] private GameObject aimingGFX;

    private PlayerInventory inventory;

    public Respect Respect { get => respect; private set => respect = value; }

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
        inventory.Init(Data);

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

            // not enought money to perform the attack
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

    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        // change inventory if applicable
    }

    private void OnDestroy()
    {
        respect.CleanUp();
    }

    public void Push(Vector2 direction, float distance, float speed)
    {
        // interupt movement
        canMove = false;
        StartCoroutine(rb.AddForceCustom(direction, distance, speed, () => canMove = true));
    }

}
