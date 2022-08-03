using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    private int damage;

    public void Init(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check for friendly fire
        if (!collision.CompareTag(tag))
        {
            // if object with which we collided is damageable, damage it
            IDamagable other = collision.gameObject.GetComponent<IDamagable>();
            if (other != null)
            {
                other.TakeDamage(damage);
            }
        }

    }
}
