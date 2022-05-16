using System;
using UnityEngine;

public enum EAxis { Horizontal, Vertical }
public enum EButtonDown { Dash, Melee, Ranged, Special, Interact }
public enum EButtonUp { Melee, Ranged, Special }

public static class InputExtensions
{
    public static bool GetInput<T>(this T interaction)
    {
        if (typeof(T).Equals(typeof(EButtonDown))) return Input.GetButtonDown(interaction.ToString());
        else if (typeof(T).Equals(typeof(EButtonUp))) return Input.GetButtonUp(interaction.ToString());
        else return false;
    }

    public static EAttackCommand ToEAttackCommand<T>(this T interaction)
    {
        if (typeof(T).Equals(typeof(EButtonDown))) return EAttackCommand.Begin;
        else if (typeof(T).Equals(typeof(EButtonUp))) return EAttackCommand.End;
        else return EAttackCommand.NDEF;
    }

    public static EAttackType ToEAttackType<T>(this T interaction)
    {
        if (typeof(T).Equals(typeof(EButtonDown)) || typeof(T).Equals(typeof(EButtonUp)))
        {
            foreach (EAttackType attackButton in Enum.GetValues(typeof(EAttackType)))
            {
                if (attackButton.ToString().Equals(interaction.ToString())) return attackButton;
            }
        }
        return EAttackType.NDEF;        
    }
}
