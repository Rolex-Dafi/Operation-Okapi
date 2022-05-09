using UnityEngine;

/// <summary>
/// Base class for all aggressive characters - i.e. characters that can engage in combat.
/// Handles character movement and combat - both animation and physics.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class AggressiveCharacter : Character, IDamagable
{
    // movement
    protected float movementSpeed;
    protected EDirection facing;

    // combat
    protected Health health;
    protected bool canMove;

    // components
    protected Rigidbody2D rb;
    protected Animator animator;

    protected void Init(int startingHealth, int startingMoney, float movementSpeed)
    {
        Init(startingMoney);
        this.movementSpeed = movementSpeed;
        facing = EDirection.s;

        health = new Health(startingHealth);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        canMove = true;
    }

    private void Update()
    {
        // set direction
        animator.SetFloat(EAnimationParameter.directionX.ToString(), facing.ToVector2().x);
        animator.SetFloat(EAnimationParameter.directionY.ToString(), facing.ToVector2().y);
    }


    public void Move(Vector2 move)
    {
        if (move != Vector2.zero) facing = move.ToDirection();

        if (!canMove) return;

        rb.MovePosition(rb.position + move * Time.deltaTime * movementSpeed);
        animator.SetFloat(EAnimationParameter.speed.ToString(), move.sqrMagnitude);
    }

    public void Dash()
    {
        if (!canMove) return;

        animator.SetTrigger(EAnimationParameter.dash.ToString());
        Vector2 direction = facing.ToVector2().normalized;
        Debug.Log("dashing in: " + direction);
        rb.AddForce(direction * 1500);
    }

    public void Attack(Attack attack)
    {
        canMove = false;
        animator.SetTrigger(EAnimationParameter.attack.ToString());
        animator.SetInteger(EAnimationParameter.attackNumber.ToString(), attack.attackNumber);
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
        Destroy(gameObject);
    }

    public void AllowToMove(bool canMove) => this.canMove = canMove;
}
