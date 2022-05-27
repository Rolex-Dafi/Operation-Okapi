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
    [HideInInspector] public Animator animator;


    protected void Init(int startingMoney = 0)
    {
        Debug.Log("Init in Character");
        money = new Money(startingMoney);
        animator = GetComponent<Animator>();
    }

}
