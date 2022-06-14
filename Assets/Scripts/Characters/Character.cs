using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all in-game characters.
/// </summary>
public class Character : MonoBehaviour
{
    // resources
    protected Money money;

    // inventory/drops

    // components
    [HideInInspector] protected Animator animator;

    public Animator Animator { get => animator; set => animator = value; }

    protected void Init(int startingMoney = 0)
    {
        money = new Money(startingMoney);
        Animator = GetComponent<Animator>();
    }

}
