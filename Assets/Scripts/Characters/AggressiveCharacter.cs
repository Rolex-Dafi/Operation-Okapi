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
public class AggressiveCharacter : Character, IDamagable
{
    // scriptable objects
    [SerializeField] private AttackScriptableObject[] attackScriptableObjects;
    [SerializeField] private DashScriptableObject dashScriptableObject;

    // movement
    [SerializeField] protected float movementSpeed;
    protected float currentSpeed;
    protected Vector2 facing;

    // combat
    protected Health health;
    protected bool canMove;
    protected List<Attack> attacks;
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
    public Vector2 Facing { get => facing; protected set => facing = value; }
    public float ColliderRadius { get => col.radius; }
    public Rigidbody2D RB { get => rb; protected set => rb = value; }

    public void Init(int startingHealth, int startingMoney)
    {
        Init(startingMoney);
        currentSpeed = movementSpeed;
        Facing = Vector2.down;

        health = new Health(startingHealth);
        attacks = new List<Attack>();
        foreach (AttackScriptableObject scriptableObject in attackScriptableObjects)
        {
            Attack attack = scriptableObject.GetAttack(this);
            if (attack != null) attacks.Add(attack);
        }
        dash = dashScriptableObject.GetDash(this);

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

        if (!canMove || dash.CurrentlyDashing) return;

        // moving in isometric coordinates !
        RB.MovePosition(RB.position + move.CartesianToIsometric().normalized * Time.fixedDeltaTime * currentSpeed);

        // animator in cartesian
        Animator.SetFloat(EAnimationParameter.speed.ToString(), move.sqrMagnitude);
    }

    public void Dash()
    {
        if (!canMove) return;

        dash.OnBegin();
    }


    public void Attack(Attack attack, EAttackCommand attackCommand)
    {
        switch (attackCommand)
        {
            case EAttackCommand.Begin:
                attack.OnBegin();
                break;
            case EAttackCommand.End:
                attack.OnEnd();
                break;
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

    public void TakeDamage(int amount)
    {
        Animator.SetTrigger(EAnimationParameter.hit.ToString());
        int current = health.ChangeCurrent(-amount);
        if (current == 0) Die();
    }

    public void Die()
    {
        Animator.SetTrigger(EAnimationParameter.death.ToString());
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void SetMovementSpeed(float movementSpeed)
    {
        canMove = movementSpeed == 0 ? false : true;
        currentSpeed = movementSpeed * movementSpeed;
    }

    public void ResetMovementSpeed() 
    {
        currentSpeed = movementSpeed;
        canMove = true; 
    }

    private void OnDestroy()
    {
        onDeath.RemoveAllListeners();
    }
}
