using BehaviourTree;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for behavior trees of all characters in the game.
/// </summary>
[RequireComponent(typeof(Seeker), typeof(CombatCharacter))]
public abstract class CharacterTreeBase : TreeBase
{
    [SerializeField] protected CharacterTreeBase rootTree = null;

    // required components
    private CombatCharacter character;
    private Seeker seeker; // for pathfinding

    // pathfinding
    public Vector3[] patrollPoints;

    // will be needed in subtrees (ex. see if player is in range)
    protected Transform playerCharacter;

    public CombatCharacter Character { get => character; private set => character = value; }
    public Seeker Seeker { get => seeker; private set => seeker = value; }

    public bool ShouldUpdate { get; set; }
    
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

        //StartCoroutine(BTLoop());
    }

    private void Update()
    {
        // only update the root tree, not any subtrees
        if (rootTree.Equals(this) && ShouldUpdate)
        {
            Root.Update();
        }
    }

    private IEnumerator BTLoop()
    {
        while (true)
        {
            // only update the root tree, not any subtrees
            if (rootTree.Equals(this) && ShouldUpdate)
            {
                Root.Update();
            }
            yield return new WaitForSeconds(.5f);
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

    // subtrees

    protected Node GetPatrollBT()
    {
        List<Node> patrollTasks = new List<Node>();
        for (int i = 0; i < patrollPoints.Length; i++)
        {
            patrollTasks.Add(new WalkToTarget(rootTree, patrollPoints[i]));

            if (i < patrollPoints.Length - 1) patrollTasks.Add(new WaitFor(rootTree, (Character.Data as EnemyCharacterSO).patrollWaitTime));
        }
        return new SequenceWithCachedLastChild(patrollTasks);
    }

    protected Node GetAttackBT(Attack attack, bool checkCD = false, bool precise = false)
    {
        // if the attack range is less than minimum obstacle size -> no need to check for obstacles
        EObstacleFilter obstacleFilter = attack.Data.attackRange < Utility.minObstacleSize ? EObstacleFilter.None : EObstacleFilter.Obstacles;

        // the attack task itself
        Node attackTask = new SequenceWithCachedLastChild(
            new List<Node>()
            {
                new AttackTarget(this, attack, AIUtility.PCPositionName, precise, debugName:"attack pc"),                            // attack pc THEN
                new WaitFor(this, attack.Data.recoveryTime, debugName:"attack recovery")                                    // attack recovery 
            }
        );

        // check if in attack range
        List<Node> attackSequence = new List<Node>();
        if (checkCD)
        {
            attackSequence.Add(new Inverter(new AbilityOnCD(this, attack, debugName: "attack on cd")));                                               // attack not on cd AND 
        }
        attackSequence.Add(new TargetInRange(this, AIUtility.PCPositionName, attack.Data.attackRange, obstacleFilter, debugName: "pc in atk range")); // pc in attack range AND
        attackSequence.Add(attackTask);                                                                                                               // perform atk task

        return new Sequence(attackSequence);
    }

    protected Node GetDashAttackBT(Attack attack, bool checkCD = true)
    {
        // dash attack
        Node dashTask = new SequenceWithCachedLastChild(
            new List<Node>()
            {
                new DashToTarget(this, AIUtility.PCPositionName, debugName:"dash to pc"),                                         // dash to pc THEN
                new AttackTarget(this, attack, AIUtility.PCPositionName, debugName:"attack pc"),                                  // dash attack pc THEN
                new WaitFor(this, attack.Data.recoveryTime, debugName:"attack recovery")                                          // attack recovery 
            }
        );

        // check if in dash range
        List<Node> dashSequence = new List<Node>();
        if (checkCD)
        {
            dashSequence.Add(new Inverter(new AbilityOnCD(this, Character.GetDash(), debugName: "dash on cd")));                                   // dash not on cd AND
        }
        dashSequence.Add(new TargetInRange(this, AIUtility.PCPositionName, Character.GetDash().Data.distance, EObstacleFilter.Obstacles));         // pc at dash range AND
        dashSequence.Add(dashTask);                                                                                                                // perform dash    

        return new Sequence(dashSequence);
    }

    /// <summary>
    /// Specifically for dashes that do damage.
    /// </summary>
    /// <param name="checkCD">Take cooldown into account</param>
    /// <returns></returns>
    protected Node GetDashBT(bool checkCD = true)
    {
        // dash attack
        Node dashTask = new SequenceWithCachedLastChild(
            new List<Node>()
            {
                new WaitFor(this, Character.GetDash().Data.deltaAfterMax, debugName:"dash startup"),    // dash startup
                new DashToTarget(this, AIUtility.PCPositionName, debugName:"dash to pc"),      // dash to pc
            }
        );

        // check if in dash range
        List<Node> dashSequence = new List<Node>();
        if (checkCD)
        {
            dashSequence.Add(new Inverter(new AbilityOnCD(this, Character.GetDash(), debugName: "dash on cd")));                                   // dash not on cd AND
        }
        dashSequence.Add(new TargetInRange(this, AIUtility.PCPositionName, Character.GetDash().Data.distance, EObstacleFilter.Obstacles));         // pc in dash range AND
        dashSequence.Add(dashTask);                                                                                                                // perform dash    

        return new Sequence(dashSequence);
    }

    protected Node GetTrapBT(Trap trap, bool checkCD = true)
    {
        Node trapTask = new SequenceWithCachedLastChild(
            new List<Node>()
            {
                new WaitFor(this, trap.Data.activationTime, debugName: "trap startup"), 
                new ActivateTrapTask(this, trap)
            }
        );

        List<Node> trapSequence = new List<Node>();
        if (checkCD)
        {
            trapSequence.Add(new Inverter(new AbilityOnCD(this, trap, debugName: "trap on cd")));
        }
        trapSequence.Add(trapTask);

        return new Sequence(trapSequence);
    }
}
