using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class TrapController : MonoBehaviour
{
    private TrapSO data;
    
    private Animator animator;
    private readonly string activate = "Activate";
    
    public void Init(TrapSO data)
    {
        this.data = data;
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Plays an animation which activates the trap, so that it can damage the player.
    /// </summary>
    public void ActivateTrap()
    {
        // play animation - the animation should enable the hitbox
        animator.SetTrigger(activate);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitBoxController.HandleCollision(transform, collision, data, tag);
    }

}
