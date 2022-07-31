using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    private int damage;

    public void Init(int damage)
    {
        Debug.LogWarning("In hitbox init");
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogWarning("Hitbox collided with something, tag = " + collision.tag);

        // check for friendly fire
        if (!collision.CompareTag(tag))
        {
            Debug.LogWarning("Hitbox collided with something which isn't me");

            // if object with which we collided is damageable, damage it
            IDamagable other = collision.gameObject.GetComponent<IDamagable>();
            if (other != null)
            {
                Debug.LogWarning("Hitbox collided with something damageable");
                other.TakeDamage(damage);
            }
        }

    }
}
