using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EAxis { Horizontal, Vertical }
public enum EButtonDown { Dash, Melee, Ranged, Special, Interact }
public enum EButtonUp { Melee, Ranged, Special }
public enum EAttackButton { Melee, Ranged, Special, NDEF }

public enum EUIButton { Escape, Enter }

public static class InputUtility
{
    public static void InitEvents<T>(ref Dictionary<T, UnityEvent<T>> eventDictionary)
    {
        eventDictionary = new Dictionary<T, UnityEvent<T>>();
        foreach (T interaction in Enum.GetValues(typeof(T)))
        {
            eventDictionary.Add(interaction, new UnityEvent<T>());
        }
    }

    public static void GetInput<T>(ref Dictionary<T, UnityEvent<T>> eventDictionary)
    {
        foreach (T interaction in Enum.GetValues(typeof(T)))
        {
            if (interaction.GetInput()) eventDictionary[interaction].Invoke(interaction);
        }
    }
}

public static class InputExtensions
{
    /// <summary>
    /// Tries to read input.
    /// </summary>
    /// <param name="interaction">The interaction we're trying to read (ex. a specific button)</param>
    /// <typeparam name="T">The type of interaction (ex. button down or up)</typeparam>
    /// <returns>Whether the input interaction occured</returns>
    public static bool GetInput<T>(this T interaction)
    {
        // player input
        if (typeof(T) == typeof(EButtonDown)) return Input.GetButtonDown(interaction.ToString());
        else if (typeof(T) == typeof(EButtonUp)) return Input.GetButtonUp(interaction.ToString());
        // ui input
        else if (typeof(T) == typeof(EUIButton)) return Input.GetButtonDown(interaction.ToString());
        else return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static EAttackCommand ToEAttackCommand<T>()
    {
        if (typeof(T) == typeof(EButtonDown)) return EAttackCommand.Begin;
        else if (typeof(T) == typeof(EButtonUp)) return EAttackCommand.End;
        else return EAttackCommand.NDEF;
    }

    public static EAttackButton ToEAttackButton<T>(this T interaction)
    {
        if (typeof(T) == typeof(EButtonDown) || typeof(T) == typeof(EButtonUp))
        {
            foreach (EAttackButton attackButton in Enum.GetValues(typeof(EAttackButton)))
            {
                if (attackButton.ToString().Equals(interaction.ToString())) return attackButton;
            }
        }
        return EAttackButton.NDEF;        
    }
    
    /// <summary>
    /// Returns the button name for any given interaction - context dependent, i.e. returns different names
    /// for keyboard and for gamepad input.
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static string GetButtonName(this EButtonDown button)
    {
        switch (button)
        {
            case EButtonDown.Dash:
                return GeneralInput.GamepadConnected ? "A" : "Space";
            case EButtonDown.Melee:
                return GeneralInput.GamepadConnected ? "X" : "RMB";
            case EButtonDown.Ranged:
                return GeneralInput.GamepadConnected ? "Y" : "LMB";
            case EButtonDown.Special:
                return GeneralInput.GamepadConnected ? "B" : "MMB";
            case EButtonDown.Interact:
                return GeneralInput.GamepadConnected ? "RB" : "E";
            default:
                return "";
        }
    }
}
