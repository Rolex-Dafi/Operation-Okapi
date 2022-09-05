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

    [HideInInspector] public bool gamepadConnected;

    public bool readInput;
    
    public void Init()
    {
        InputUtility.InitEvents(ref buttonDownEvents);
        InputUtility.InitEvents(ref buttonUpEvents);

        StartCoroutine(DetectGamepadInput());
    }
    
    private void Update()
    {
        if (!readInput) return;
        
        movement = new Vector2(Input.GetAxis(EAxis.Horizontal.ToString()), Input.GetAxis(EAxis.Vertical.ToString()));

        mousePosition = Input.mousePosition;

        InputUtility.GetInput(ref buttonDownEvents);
        InputUtility.GetInput(ref buttonUpEvents);        
    }

    private IEnumerator DetectGamepadInput()
    {
        while(true)
        {
            // check if any joystick is connected
            bool connected = false;
            foreach (string name in Input.GetJoystickNames())
            {
                //if any of the joystick names are not empty -> a gamepad is connected
                connected |= name != "";
            }
            gamepadConnected = connected;

            // check every 2 seconds
            yield return new WaitForSeconds(2);
        }
    }

    
}
