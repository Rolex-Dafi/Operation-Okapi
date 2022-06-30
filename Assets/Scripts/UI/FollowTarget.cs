using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform target;

    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private Vector2 scale = Vector2.one;

    private bool initialized = false;

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
