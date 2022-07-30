using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIInput : MonoBehaviour
{
    public Dictionary<EUIButton, UnityEvent<EUIButton>> buttonEvents;

    public void Init()
    {
        InputUtility.InitEvents(ref buttonEvents);
    }

    private void Update()
    {
        InputUtility.GetInput(ref buttonEvents);
    }
}
