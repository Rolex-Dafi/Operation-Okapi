using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepDistanceFromTarget : TaskBase
{
    private float _safeDistance;

    public KeepDistanceFromTarget(CharacterTreeBase characterBT, float safeDistance, string debugName = "keep target at a distance") : base(characterBT, debugName)
    {
        _safeDistance = safeDistance;
    }

    protected override void OnBegin()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnContinue()
    {
        throw new System.NotImplementedException();
    }
}
