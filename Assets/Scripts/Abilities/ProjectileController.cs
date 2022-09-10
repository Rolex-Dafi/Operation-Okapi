using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the shooting of projectiles from ranged attacks.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    private AttackSO data;

    private Rigidbody2D rb;

    private Animator animator;

    private string friendlyTag;

    /// <summary>
    /// Initializes the projectile.
    /// </summary>
    /// <param name="data">The data of the attack which spawned this projectile</param>
    /// <param name="friendlyTag">The tag of the game object which spawned this projectile</param>
    public void Init(AttackSO data, string friendlyTag)
    {
        this.data = data;

        this.friendlyTag = friendlyTag;
        gameObject.tag = friendlyTag;
        gameObject.layer = LayerMask.NameToLayer(friendlyTag);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Adds force to the projectile according to the given vector and the attack data.
    /// </summary>
    /// <param name="force">The force to add to the projectile</param>
    public void Shoot(Vector2 force)
    {
        StartCoroutine(rb.AddForceCustom(force, data.attackRange, data.projectileSpeed, OnEnd));
        
        // rotate in the direction of travel
        var rotation = Quaternion.FromToRotation(transform.up, force);
        
        // make it follow isometry rules
        rotation = Quaternion.Euler(
            Utility.isometricAngle,
            rotation.eulerAngles.y,
            rotation.eulerAngles.z
        );
        transform.rotation = rotation;
    }

    /// <summary>
    /// For projectiles shot from the sky - spawns the projectile at the given position. Expects an Animator
    /// component to handle enabling the hitbox.
    /// </summary>
    /// <param name="position">The position to spawn at</param>
    public void ShootAt(Vector2 position)
    {
        if (animator == null)
        {
            Debug.LogError("The projectile is missing an Animator component.");
            return;
        }

        transform.position = position;
        animator.SetTrigger(Utility.activateTrigger);
    }

    // TODO add animation/particle effect
    private void OnEnd()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitBoxController.HandleCollision(transform, collision, data, friendlyTag);
    }
}
