using UnityEngine;

/// <summary>
/// Any pushable entity should implement this.
/// </summary>
public interface IPushable
{
    /// <summary>
    /// Push this pushable in the given direction.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <param name="speed"></param>
    public void Push(Vector2 direction, float distance, float speed);
}
