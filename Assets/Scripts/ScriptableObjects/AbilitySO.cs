using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySO : ScriptableObject
{
    public float coolDown;

    // sound
    public FMODUnity.EventReference onBeginSound;
    public FMODUnity.EventReference onEndSound;
}
