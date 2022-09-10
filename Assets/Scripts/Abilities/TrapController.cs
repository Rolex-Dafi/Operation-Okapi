using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TrapController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [FormerlySerializedAs("childrenTriggers")] [SerializeField] private TrapDamager[] childrenDamagers;
    
    public void Init(TrapSO data)
    {
        // the actual triggers are on child(ren)
        foreach (var child in childrenDamagers)
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
        animator.SetTrigger(Utility.activateTrigger);

        SpawnProjectiles();
    }

    private void SpawnProjectiles()
    {
        
    }

}
