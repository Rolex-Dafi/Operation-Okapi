using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    private int damage;
    private float lifetime;

    private Rigidbody2D rb;

    public void Init(int damage, float lifetime = Utility.defaultProjectileLifetime)
    {
        this.damage = damage;
        this.lifetime = lifetime;

        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 force)
    {
        rb.AddForce(force);
        StartCoroutine(LifetimeCountdown());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsFriendlyFire(collision.transform))
        {
            AggressiveCharacter other = collision.gameObject.GetComponent<AggressiveCharacter>();
            if (other != null)
            {
                other.TakeDamage(damage);
            }

            // TODO add animation/particle effect
            Destroy(gameObject);
        }

    }

    private bool IsFriendlyFire(Transform other)
    {
        // TODO make this more robust maybe - what if collider isn't on root?
        return transform.root == other;
    }

    private IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(lifetime);
        // TODO make this fancier ?
        Destroy(gameObject);
    }
}
