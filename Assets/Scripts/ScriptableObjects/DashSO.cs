using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DashData", menuName = "ScriptableObjects/Dash")]
public class DashSO : AbilitySO
{
    // maximum distance of the dash - in Unity units
    public float distance;

    public float speed;

    // wait time between dashes - before maxInSuccession number reached, in seconds
    public float deltaBeforeMax;
    // wait time between dashes - after maxInSuccession number reached, in seconds
    public float deltaAfterMax;

    // how many times can the character dash quickly in succesion
    // 1 -> can only dash once
    // 2 -> two times
    // etc.
    [Range(1, 7)] public int maxNumChained;

    public Dash GetDash(CombatCharacter character)
    {
        coolDown = deltaAfterMax;
        return new Dash(character, this);
    }
}
