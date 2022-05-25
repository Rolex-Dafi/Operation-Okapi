using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformVisualizer : GizmoVisualizer
{
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawSphere(gizmoTransform.position, gizmoSize);
    }
}
