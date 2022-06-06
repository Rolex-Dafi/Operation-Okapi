using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDamagable
{
    public void TakeDamage(int amount);

    public void Die();
}
