using UnityEngine;

public static class AIUtility
{
    public static readonly string PCPositionName = "PCPosition";
    public static readonly int MeleeAttackID = 0;
    public static readonly int RangedAttackID = 1;
}

public enum EObstacleFilter
{
    None,
    Obstacles,
    Walls
}

public static class AIExtensions
{
    /// <summary>
    /// Converts an obstacle filter into a bool.
    /// </summary>
    /// <param name="filter">The obstacle filter</param>
    /// <returns>Whether the filter is set</returns>
    public static bool ShouldFilter(this EObstacleFilter filter)
    {
        return filter != EObstacleFilter.None;
    }

    /// <summary>
    /// Converts an obstacle filter to a layer mask.
    /// </summary>
    /// <param name="filter">The obstacle filter</param>
    /// <returns>The layer mask corresponding to the filter</returns>
    public static LayerMask ToLayerMask(this EObstacleFilter filter)
    {
        switch (filter)
        {
            case EObstacleFilter.Obstacles:
                return LayerMask.GetMask(Utility.obstacleLayer);
            case EObstacleFilter.Walls:
                return LayerMask.GetMask(Utility.wallLayer);
        }
        return LayerMask.GetMask("Default");
    }
}