using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySO : ScriptableObject
{
    public float coolDown;

    [Range(0, 10)] public int damage = 0;
    
    public float enemyPushbackDistance = 0;
    public float enemyPushbackSpeed = 40;   // if speed is negative -> pull instead of push
    
    // sound
    public FMODUnity.EventReference onBeginSound;
    public FMODUnity.EventReference onEndSound;
}
