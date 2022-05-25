using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoVisualizer : MonoBehaviour
{
    [Range(.05f, 1f)] [SerializeField] protected float gizmoSize = .1f;

    [SerializeField] protected Color gizmoColor;

    [SerializeField] protected Transform gizmoTransform;

}
