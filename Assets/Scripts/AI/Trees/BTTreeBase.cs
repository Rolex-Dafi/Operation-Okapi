using BehaviourTree;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker), typeof(AggressiveCharacter))]
public abstract class BTTreeBase : TreeBase
{
    // required components
    private AggressiveCharacter character;
    private Seeker seeker; // for pathfinding

    public AggressiveCharacter Character { get => character; private set => character = value; }
    public Seeker Seeker { get => seeker; private set => seeker = value; }

    private void Start()
    {
        Init();
        Character = GetComponent<AggressiveCharacter>();
        Seeker = GetComponent<Seeker>();
    }
}
