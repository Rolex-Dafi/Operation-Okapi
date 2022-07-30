using BehaviourTree;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker), typeof(CombatCharacter))]
public abstract class CharacterTreeBase : TreeBase
{
    [SerializeField] protected CharacterTreeBase rootTree = null;

    // required components
    private CombatCharacter character;
    private Seeker seeker; // for pathfinding

    // pathfinding
    public Transform[] patrollPoints;

    // will be needed in subtrees (ex. see if player is in range)
    protected Transform playerCharacter;

    public CombatCharacter Character { get => character; private set => character = value; }
    public Seeker Seeker { get => seeker; private set => seeker = value; }

    private Dictionary<string, object> data;

    private void Start()
    {
        // if root tree is null, this is root
        rootTree = rootTree == null ? this : rootTree;

        // player always has to be spawned before the enemies !
        playerCharacter = FindObjectOfType<PlayerCharacter>().transform;

        Character = GetComponent<CombatCharacter>();
        Seeker = GetComponent<Seeker>();

        data = new Dictionary<string, object>();

        Init();
    }

    private void Update()
    {
        // only update the root tree, not any subtrees
        if (rootTree.Equals(this))
        {
            Root.Update();
        }
    }

    public void AddItem(string name, object item)
    {
        if (data.ContainsKey(name))
        {
            data[name] = item;
        }
        else
        {
            data.Add(name, item);
        }
    }

    public object GetItem(string name)
    {
        if (data.ContainsKey(name))
        {
            return data[name];
        }
        else
        {
            return null;
        }
    }

    public void RemoveItem(string name)
    {
        data.Remove(name);
    }
}
