using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    private int damage;
    private float range;
    private float speed;

    private Rigidbody2D rb;
    private Vector2 force;

    private string friendlyTag;

    private float distanceTravelled = 0;

    public void Init(int damage, float speed, float range, string friendlyTag)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;

        this.friendlyTag = friendlyTag;
        gameObject.tag = friendlyTag;

        rb = GetComponent<Rigidbody2D>();

        force = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (distanceTravelled > range) Destroy(gameObject);

        Vector2 step = force.normalized * Time.fixedDeltaTime * speed;
        rb.MovePosition(rb.position + step);

        distanceTravelled += step.magnitude; 
    }

    public void Shoot(Vector2 force)
    {
        this.force = force;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check for friendly fire
        if (!collision.CompareTag(friendlyTag))
        {
            // if object with which we collided is damageable, damage it
            IDamagable other = collision.gameObject.GetComponent<IDamagable>();
            if (other != null)
            {
                other.TakeDamage(damage);
            }

            // TODO add animation/particle effect
            Destroy(gameObject);
        }

    }
}
