using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes this game object follow a specified target.
/// </summary>
public class FollowTarget : MonoBehaviour
{
    private Transform target;

    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private Vector2 scale = Vector2.one;

    private bool initialized = false;

    /// <summary>
    /// Initializes the follower.
    /// </summary>
    /// <param name="target">The target to follow</param>
    public void Init(Transform target)
    {
        this.target = target;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        transform.position = offset + target.position * scale;
    }
}
