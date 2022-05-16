using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private int damage;

    public void Init(int damage)
    {
        this.damage = damage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // do damage if relevant + destroy this
    }
}
