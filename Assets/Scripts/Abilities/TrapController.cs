using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private TrapTrigger[] childrenTriggers;
    
    private readonly string activate = "Activate";
    
    public void Init(TrapSO data)
    {
        // the actual triggers are on child(ren)
        foreach (var child in childrenTriggers)
        {
            child.Init(data);
        }
    }

    /// <summary>
    /// Plays an animation which activates the trap, so that it can damage the player.
    /// </summary>
    public void ActivateTrap()
    {
        // play animation - the animation should enable the hitbox
        animator.SetTrigger(activate);
    }

}
