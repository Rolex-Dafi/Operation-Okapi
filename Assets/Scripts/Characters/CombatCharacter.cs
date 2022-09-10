using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for all aggressive characters - i.e. characters that can engage in combat.
/// Handles character movement and combat - both animation and physics.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class CombatCharacter : Character, IDamagable
{
    // movement
    protected float movementSpeed;
    protected float currentSpeed;
    protected Vector2 facing;

    // combat
    private bool isDead;
    protected Health health;
    protected bool canMove;
    protected List<Attack> attacks;
    protected List<Trap> traps;
    protected Dash dash;
    [SerializeField] private Transform projectileSpawnerTransform;

    // events
    [HideInInspector] public UnityEvent onDeath = new UnityEvent();

    // components
    protected Rigidbody2D rb;
    protected CircleCollider2D col;

    // vars exposed to other classes
    public Transform ProjectileSpawnerTransform { get => projectileSpawnerTransform; }
    // character rotation in Cartesian coordinates
    public Vector2 Facing { get => facing; private set => facing = value; }
    public float ColliderRadius { get => col.radius; }
    public Rigidbody2D RB { get => rb; private set => rb = value; }
    public Health Health { get => health; private set => health = value; }

    public Dash GetDash() => dash; 
    public Attack GetAttackByID(int id) => attacks.Find(x => x.Data.id == id);

    public Trap GetMainTrap() => traps.Count > 0 ? traps[0] : null;

    public override void Init()
    {
        base.Init();
        movementSpeed = data.speed;
        currentSpeed = movementSpeed;
        Facing = Vector2.down;

        Health = new Health(data.health);
        attacks = new List<Attack>();
        foreach (AttackSO attackSO in data.attacks)
        {
            Attack attack = attackSO.GetAttack(this);
            if (attack != null) attacks.Add(attack);
        }

        traps = new List<Trap>();
        foreach (TrapSO trapSO in data.traps)
        {
            Trap trap = trapSO.GetTrap(this);
            if (trap != null) traps.Add(trap);
        }
        
        if (data.dash != null)
        {
            dash = data.dash.GetDash(this);
        }

        RB = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();

        canMove = true;
    }

    private void Update()
    {
        Rotate(Facing);
    }

    /// <summary>
    /// Sets character to face in the given direction.
    /// </summary>
    /// <param name="direction">The direction to look in. Expects a normalized vector.</param>
    public void Rotate(Vector2 direction)
    {
        Facing = direction;
        Animator.SetFloat(EAnimationParameter.directionX.ToString(), direction.x);
        Animator.SetFloat(EAnimationParameter.directionY.ToString(), direction.y);
    }

    public void Move(Vector2 move)
    {
        if (move != Vector2.zero) Facing = move;

        if (!canMove || (dash != null && dash.InUse)) return;

        // moving in isometric coordinates !
        RB.MovePosition(RB.position + move.CartesianToIsometric().normalized * Time.fixedDeltaTime * currentSpeed);

        // animator in cartesian
        float speed = move.magnitude * currentSpeed;
        speed = speed < .1f ? 0 : speed;
        Animator.SetFloat(EAnimationParameter.speed.ToString(), speed);
    }

    public void ForceUpdateSpeed(Vector2 move)
    {
        Animator.SetFloat(EAnimationParameter.speed.ToString(), move.sqrMagnitude * currentSpeed);
    }

    public void Dash()
    {
        if (!canMove) return;

        dash.OnBegin();
    }


    public bool Attack(Attack attack, EAttackCommand attackCommand = EAttackCommand.Begin)
    {
        if (attack == null) return false;

        switch (attackCommand)
        {
            case EAttackCommand.Begin:
                attack.OnBegin();
                return true;
            case EAttackCommand.End:
                attack.OnEnd();
                return true;
        }
        return false;
    }

    /// <summary>
    /// Activates the given trap. If no trap specified, activates the first trap in the characters trap list if present.
    /// </summary>
    /// <param name="trap">The trap to activate</param>
    public void ActivateTrap(Trap trap = null)
    {
        Debug.Log("trying to activate trap", gameObject);
        if (trap == null)
        {
            if (traps.Count > 0)
            {
                traps[0].OnBegin();
            }
        }
        else
        {
            trap.OnBegin();
        }
    }

    /// <summary>
    /// Tries to find a melee attack and performs the first one it finds.
    /// </summary>
    /// <param name="attackCommand"></param>
    /// <returns>Whether an attack was performed.</returns>
    public bool MeleeAttack(EAttackCommand attackCommand = EAttackCommand.Begin)
    {
        MeleeAttack attack = attacks.OfType<MeleeAttack>().ToArray()[0];

        if (attack != null)
        {
            Attack(attack, attackCommand);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to find a ranged attack and performs the first one it finds.
    /// </summary>
    /// <param name="attackCommand"></param>
    /// <returns>Whether an attack was performed.</returns>
    public bool RangedAttack(EAttackCommand attackCommand = EAttackCommand.Begin)
    {
        RangedAttack attack = attacks.OfType<RangedAttack>().ToArray()[0];

        if (attack != null)
        {
            Attack(attack, attackCommand);
            return true;
        }

        return false;
    }

    public virtual void TakeDamage(int amount)
    {
        Animator.SetTrigger(EAnimationParameter.hit.ToString());
        int current = Health.ChangeCurrent(-amount);
        if (current == 0 && !isDead) Die();
        else
        {
            RuntimeManager.PlayOneShot(data.onHitSound.Guid);
        }
    }

    public virtual void Die()
    {
        Animator.SetTrigger(EAnimationParameter.death.ToString());
        RuntimeManager.PlayOneShot(data.onDeathSound.Guid);
        canMove = false;
        isDead = true;
    }

    public void CleanUp()
    {
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void SetMovementSpeed(float movementSpeed)
    {
        canMove = movementSpeed == 0 ? false : true;
        currentSpeed = movementSpeed * movementSpeed;
    }

    public void ResetAttacks()
    {
        foreach (var attack in attacks)
        {
            attack.InUse = false;
        }
    }

    public void ResetMovementSpeed() 
    {
        currentSpeed = movementSpeed;
        canMove = true; 
    }

    private void OnDestroy()
    {
        health.CleanUp();
        onDeath.RemoveAllListeners();
    }
}
