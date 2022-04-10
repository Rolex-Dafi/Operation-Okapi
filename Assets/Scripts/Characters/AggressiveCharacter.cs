using UnityEngine;

/// <summary>
/// Base class for all aggressive characters - i.e. characters that can engage in combat.
/// Handles character movement and combat - both animation and physics.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class AggressiveCharacter : Character, IDamagable
{
    // combat
    protected Health health;
    protected float movementSpeed;

    // components
    protected Rigidbody2D rb;
    protected Animator animator;

    protected void Init(int startingHealth, int startingMoney, float movementSpeed)
    {
        Init(startingMoney);
        health = new Health(startingHealth);
        this.movementSpeed = movementSpeed;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    public void Move(Vector2 move)
    {
        // move rb
        rb.MovePosition(rb.position + move * Time.deltaTime * movementSpeed);

        // set anim vars
    }

    public void Dash()
    {

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
