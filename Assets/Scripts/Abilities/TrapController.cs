using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class TrapController : MonoBehaviour
{
    [SerializeField] private TrapSO trapData;
    
    private Animator animator;
    private readonly string activate = "Activate";

    public int trapID => trapData.ID;
    
    public void Init()
    {
        animator = GetComponent<Animator>();
    }

    public void ActivateTrap()
    {
        // play animation - the animation should enable the hitbox
        animator.SetTrigger(activate);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
