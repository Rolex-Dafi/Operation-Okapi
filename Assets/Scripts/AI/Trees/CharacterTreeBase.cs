using BehaviourTree;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker), typeof(CombatCharacter))]
public abstract class CharacterTreeBase : TreeBase
{
    // required components
    private CombatCharacter character;
    private Seeker seeker; // for pathfinding

    public CombatCharacter Character { get => character; private set => character = value; }
    public Seeker Seeker { get => seeker; private set => seeker = value; }

    private void Start()
    {
        Init();
        Character = GetComponent<CombatCharacter>();
        Seeker = GetComponent<Seeker>();
    }
}
