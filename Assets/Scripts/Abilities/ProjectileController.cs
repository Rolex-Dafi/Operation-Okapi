using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    private AttackSO data;

    private Rigidbody2D rb;

    private string friendlyTag;

    public void Init(AttackSO data, string friendlyTag)
    {
        this.data = data;

        this.friendlyTag = friendlyTag;
        gameObject.tag = friendlyTag;

        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 force)
    {
        StartCoroutine(rb.AddForceCustom(force, data.attackRange, data.projectileSpeed, OnEnd));
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
