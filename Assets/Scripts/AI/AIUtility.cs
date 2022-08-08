using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public static bool ShouldFilter(this EObstacleFilter filter)
    {
        return filter != EObstacleFilter.None;
    }

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