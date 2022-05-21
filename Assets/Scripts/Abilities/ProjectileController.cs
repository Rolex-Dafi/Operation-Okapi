using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    private int damage;

    private Rigidbody2D rb;

    public void Init(int damage)
    {
        this.damage = damage;

        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 force)
    {
        rb.AddForce(force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO do damage if relevant + destroy this
    }
}
