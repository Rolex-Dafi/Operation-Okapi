using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles player input which pertains to the controlling of the player character.
/// </summary>
public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public Vector2 movement;
    [HideInInspector] public Vector2 mousePosition;

    public Dictionary<EButtonDown, UnityEvent<EButtonDown>> buttonDownEvents;
    public Dictionary<EButtonUp, UnityEvent<EButtonUp>> buttonUpEvents;

    public bool readInput;
    
    /// <summary>
    /// Initializes the input events relevant to player input.
    /// </summary>
    public void Init()
    {
        InputUtility.InitEvents(ref buttonDownEvents);
        InputUtility.InitEvents(ref buttonUpEvents);
    }
    
    private void Update()
    {
        if (!readInput) return;
        
        movement = new Vector2(Input.GetAxis(EAxis.Horizontal.ToString()), Input.GetAxis(EAxis.Vertical.ToString()));

        mousePosition = Input.mousePosition;

        InputUtility.GetInput(ref buttonDownEvents);
        InputUtility.GetInput(ref buttonUpEvents);        
    }
}
