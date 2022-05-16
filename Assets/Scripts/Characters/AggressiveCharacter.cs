using System.Collections.Generic;
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
    protected float currentSpeed;
    protected EDirection facing;

    // combat
    protected Health health;
    protected bool canMove;
    protected List<Attack> attacks;

    // components
    protected Rigidbody2D rb;

    protected void Init(int startingHealth, int startingMoney, float movementSpeed)
    {
        Init(startingMoney);
        this.movementSpeed = movementSpeed;
        currentSpeed = movementSpeed;
        facing = EDirection.s;

        health = new Health(startingHealth);
        attacks = new List<Attack>();

        rb = GetComponent<Rigidbody2D>();

        canMove = true;
    }

    private void Update()
    {
        Rotate(facing);
    }

    protected void Rotate(EDirection direction)
    {
        // set direction
        animator.SetFloat(EAnimationParameter.directionX.ToString(), direction.ToVector2().x);
        animator.SetFloat(EAnimationParameter.directionY.ToString(), direction.ToVector2().y);
    }

    public void Move(Vector2 move)
    {
        if (move != Vector2.zero) facing = move.ToDirection();

        if (!canMove) return;

        rb.MovePosition(rb.position + move * Time.deltaTime * currentSpeed);
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
}
