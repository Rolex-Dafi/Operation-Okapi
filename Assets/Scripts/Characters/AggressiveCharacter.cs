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
    }


    public void Move(Vector2 move)
    {
        // move rb
        rb.MovePosition(rb.position + move * Time.deltaTime * movementSpeed);

        // set anim vars
        animator.SetFloat("Speed", move.sqrMagnitude);

        move = Utility.SetTo01(move);
        animator.SetFloat("DirectionX", move.x);
        animator.SetFloat("DirectionY", move.y);

        if (move != Vector2.zero) facing = Utility.Vector2ToDirection(move);
    }

    public void Dash()
    {
        Vector2 direction = Utility.DirectionToVector2(facing).normalized;
        Debug.Log("dashing in: " + direction);
        rb.AddForce(direction * 2000);
    }

    public void Attack(Attack attack)
    {

    }

    public void TakeDamage(int amount)
    {
        int current = health.ChangeCurrent(-amount);
        if (current == 0) Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
