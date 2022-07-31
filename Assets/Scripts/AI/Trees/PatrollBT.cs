using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class PatrollBT : CharacterTreeBase
{
    public Transform[] patrollPointsOld;
    public float waitBetweenWalks = 2f;

    protected override void Init()
    {
        Debug.Log("PatrollBT initiated");
        List<Node> patrollTasks = new List<Node>();
        for (int i = 0; i < patrollPointsOld.Length; i++)
        {
            patrollTasks.Add(new WalkToTarget(rootTree, patrollPointsOld[i].position));

            if (i < patrollPointsOld.Length - 1) patrollTasks.Add(new WaitFor(rootTree, waitBetweenWalks));
        }

        Root = new SequenceWithCachedLastChild(patrollTasks);
    }

}
