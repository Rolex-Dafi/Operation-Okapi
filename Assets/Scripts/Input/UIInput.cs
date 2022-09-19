using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Class for input which should be independent of the player character.
/// </summary>
public class UIInput : MonoBehaviour
{
    public Dictionary<EUIButton, UnityEvent<EUIButton>> buttonEvents;

    public static bool GamepadConnected;
    
    /// <summary>
    /// Initializes UI input events and starts detecting gamepad input.
    /// </summary>
    public void Init()
    {
        InputUtility.InitEvents(ref buttonEvents);
        
        StartCoroutine(DetectGamepadInput());
    }

    private void Update()
    {
        InputUtility.GetInput(ref buttonEvents);
    }

    private static IEnumerator DetectGamepadInput()
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
            GamepadConnected = connected;

            // check every 2 seconds
            yield return new WaitForSeconds(2);
        }
    }

    /// <summary>
    /// If a gamepad is connected, tries to select the first button it can find.
    /// </summary>
    public static void TrySelectFirstButton()
    {
        if (GamepadConnected)
        {
            FindObjectOfType<Button>()?.Select();
        }
    }
}
