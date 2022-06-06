using System.Collections;
using System.Collections.Generic;
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

    // movement
    [SerializeField] protected float movementSpeed;
    protected float currentSpeed;
    protected Vector2 facing;

    // combat
    protected Health health;
    protected bool canMove;
    protected List<Attack> attacks;
    [SerializeField] private Transform projectileSpawnerTransform;

    // events
    [HideInInspector] public UnityEvent onDeath = new UnityEvent();

    // components
    protected Rigidbody2D rb;
    protected CircleCollider2D col;

    public Transform ProjectileSpawnerTransform { get => projectileSpawnerTransform; }

    // character rotation in Cartesian coordinates
    public Vector2 Facing { get => facing; protected set => facing = value; }
    public float ColliderRadius { get => col.radius; }

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

        rb = GetComponent<Rigidbody2D>();
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
    /// <param name="direction"></param>
    public void Rotate(Vector2 direction)
    {
        Facing = direction;
        animator.SetFloat(EAnimationParameter.directionX.ToString(), direction.x);
        animator.SetFloat(EAnimationParameter.directionY.ToString(), direction.y);
    }

    public virtual void Move(Vector2 move)
    {
        if (move != Vector2.zero) Facing = move;

        if (!canMove) return;

        // moving in isometric coordinates !
        rb.MovePosition(rb.position + move.CartesianToIsometric().normalized * Time.fixedDeltaTime * currentSpeed);

        // animator in cartesian
        animator.SetFloat(EAnimationParameter.speed.ToString(), move.sqrMagnitude);
    }

    public void Dash()
    {
        if (!canMove) return;

        animator.SetTrigger(EAnimationParameter.dash.ToString());
        // dash in isometric coordinates !
        Vector2 direction = Facing.CartesianToIsometric().normalized;
        rb.AddForce(direction * 1500);
    }

    public void Attack()
    {

    }

    public void TakeDamage(int amount)
    {
        animator.SetTrigger(EAnimationParameter.hit.ToString());
        int current = health.ChangeCurrent(-amount);
        if (current == 0) Die();
    }

    public void Die()
    {
        animator.SetTrigger(EAnimationParameter.death.ToString());
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
