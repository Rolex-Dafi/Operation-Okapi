using FMODUnity;
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
    private CircleCollider2D col;

    // vars exposed to other classes
    public Transform ProjectileSpawnerTransform { get => projectileSpawnerTransform; }
    // character rotation in Cartesian coordinates
    public Vector2 Facing { get => facing; private set => facing = value; }
    public float ColliderRadius { get => col.radius; }
    public Rigidbody2D RB { get => rb; private set => rb = value; }
    public Health Health { get => health; private set => health = value; }  // only enemies use this, the player has items instead of hp

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
            if (attack != null)
            {
                attacks.Add(attack);
            }
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

    /// <summary>
    /// Moves this character in the given direction.
    /// </summary>
    /// <param name="move">Direction to move in</param>
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

    /// <summary>
    /// Forces the character into an idle animation.
    /// </summary>
    public void ForceIdle()
    {
        Animator.SetFloat(EAnimationParameter.speed.ToString(), 0);
    }

    /// <summary>
    /// Performs a dash.
    /// </summary>
    public void Dash()
    {
        if (!canMove) return;

        dash.OnBegin();
    }

    /// <summary>
    /// Tries to begin or end an attack.
    /// </summary>
    /// <param name="attack">The attack to perform</param>
    /// <param name="attackCommand">The attack command (i.e. should the attack begin or end)</param>
    /// <returns></returns>
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
    /// Tries to begin or end an attack on the specified target.
    /// </summary>
    /// <param name="attack">The attack to perform</param>
    /// <param name="target">The target to attack</param>
    /// <param name="attackCommand"></param>
    /// <returns></returns>
    public bool AttackTarget(Attack attack, Vector3 target, EAttackCommand attackCommand = EAttackCommand.Begin)
    {
        if (attack == null) return false;

        attack.Target = target;
        
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
    /// Activates the given trap. If no trap specified, activates the first trap in the characters trap list, if present.
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
    /// Deals damage to this character.
    /// </summary>
    /// <param name="amount">The amount of damage to deal</param>
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

    /// <summary>
    /// Kills this character.
    /// </summary>
    public virtual void Die()
    {
        Debug.Log("in Die", gameObject);
        
        Animator.SetTrigger(EAnimationParameter.death.ToString());
        RuntimeManager.PlayOneShot(data.onDeathSound.Guid);
        canMove = false;
        isDead = true;
    }

    /// <summary>
    /// Handles clean up of the character after death.
    /// </summary>
    public void CleanUp()
    {
        Debug.Log("in Cleanup", gameObject);
        
        onDeath.Invoke();
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets the movement speed of this character to the given value.
    /// </summary>
    /// <param name="movementSpeed"></param>
    public void SetMovementSpeed(float movementSpeed)
    {
        canMove = movementSpeed == 0 ? false : true;
        currentSpeed = movementSpeed * movementSpeed;
    }

    /// <summary>
    /// Ends all attacks of the character.
    /// </summary>
    public void ResetAttacks()
    {
        foreach (var attack in attacks)
        {
            attack.InUse = false;
        }
    }

    /// <summary>
    /// Resets movement speed of the character.
    /// </summary>
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
