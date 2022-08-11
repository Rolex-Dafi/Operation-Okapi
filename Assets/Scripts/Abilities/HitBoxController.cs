using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    private AttackSO data;

    public void Init(AttackSO data)
    {
        this.data = data;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check for friendly fire
        if (!collision.CompareTag(tag))
        {
            // if object with which we collided is damageable, damage it
            if (collision.gameObject.TryGetComponent<IDamagable>(out var damagable))
            {
                damagable.TakeDamage(data.damage);
            }

            // if attack has pushaback
            if (data.enemyPushbackDistance > 0)
            {
                // and object with which we collided is pushable, push it away from this
                if (collision.gameObject.TryGetComponent<IPushable>(out var pushable))
                {
                    Vector2 direction = collision.transform.position - transform.position;
                    pushable.Push(direction, data.enemyPushbackDistance, data.enemyPushbackSpeed);
                }

            }
        }

    }
}
