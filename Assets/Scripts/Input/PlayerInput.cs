using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles player input which pertains to the controlling of the player character.
/// </summary>
public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Vector2> moveEvent;
    public Dictionary<EButtonDown, UnityEvent<EButtonDown>> buttonDownEvents;
    public Dictionary<EButtonUp, UnityEvent<EButtonUp>> buttonUpEvents;

    public void Init()
    {
        moveEvent = new UnityEvent<Vector2>();
        InitEvents(ref buttonDownEvents);
        InitEvents(ref buttonUpEvents);
    }

    private void InitEvents<T>(ref Dictionary<T, UnityEvent<T>> eventDictionary)
    {
        eventDictionary = new Dictionary<T, UnityEvent<T>>();
        foreach (T interaction in Enum.GetValues(typeof(T)))
        {
            eventDictionary.Add(interaction, new UnityEvent<T>());
        }
    }

    private void Update()
    {
        moveEvent.Invoke(new Vector2(
            Input.GetAxis(EAxis.Horizontal.ToString()), 
            Input.GetAxis(EAxis.Vertical.ToString())
        ));

        GetInput(ref buttonDownEvents);
        GetInput(ref buttonUpEvents);
    }

    private void GetInput<T>(ref Dictionary<T, UnityEvent<T>> eventDictionary)
    {
        foreach (T interaction in Enum.GetValues(typeof(T)))
        {
            if (interaction.GetInput()) eventDictionary[interaction].Invoke(interaction);
        }
    }
}
