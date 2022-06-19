using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrollBT))]
public class DummyBT : BTTreeBase
{

    protected override void Init()
    {
        Root = GetComponent<PatrollBT>().Root;
    }

}
