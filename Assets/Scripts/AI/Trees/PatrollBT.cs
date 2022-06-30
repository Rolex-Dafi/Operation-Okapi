using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class PatrollBT : CharacterTreeBase
{
    public Transform[] patrollPoints;
    public float waitBetweenWalks = 2f;

    protected override void Init()
    {
        List<Node> patrollTasks = new List<Node>();
        for (int i = 0; i < patrollPoints.Length; i++)
        {
            patrollTasks.Add(new WalkToTarget(rootTree, patrollPoints[i].position));

            if (i < patrollPoints.Length - 1) patrollTasks.Add(new WaitFor(rootTree, waitBetweenWalks));
        }

        Root = new Sequence(patrollTasks);
    }

}
