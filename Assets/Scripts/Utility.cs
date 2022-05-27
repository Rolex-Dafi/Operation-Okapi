using UnityEngine;

public static class Utility
{
    // we're using 2:1 isometry, computed from dot product, in degrees
    public static float isometricAngle = Mathf.Acos(2 / Mathf.Sqrt(5)) * Mathf.Rad2Deg;

    // in seconds
    public const float defaultProjectileLifetime = 5f;

    // in Unity units
    public const float defaultProjectileHeight = 3;
}

public enum EAbility
{
    idle, walk, dash, melee, ranged, hit, death, NDEF
}

public enum EDirection
{
    e, ne, n, nw, w, sw, s, se, NDEF
}

public enum EStateMachine
{
    move, attack, root
}

public enum EAnimationParameter
{
    directionX, directionY, speed, dash, hit, death, attack, attackID
}

public static class Extenstions
{   
    public static AnimatorControllerParameterType ToType(this EAnimationParameter parameter)
    {
        switch (parameter)
        {
            case EAnimationParameter.directionX:
            case EAnimationParameter.directionY:
            case EAnimationParameter.speed:
                return AnimatorControllerParameterType.Float;
            case EAnimationParameter.attackID:
                return AnimatorControllerParameterType.Int;
            default:
                return AnimatorControllerParameterType.Trigger;
        }
    }

    public static EStateMachine ToEStateMachine(this EAbility ability)
    {
        switch (ability)
        {
            case EAbility.idle:
            case EAbility.walk:
                return EStateMachine.move;
            case EAbility.melee:
            case EAbility.ranged:
                return EStateMachine.attack;
            default:
                return EStateMachine.root;
        }
    }

    public static Vector2 CartesianToIsometric(this Vector2 vector)
    {
        return new Vector2(vector.x * 2, vector.y);
    }

    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }
    public static Vector3 ToVector3(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static Vector2 ToVector2(this EDirection direction)
    {
        Vector2 ret = Vector2.zero;

        switch (direction)
        {
            case EDirection.n:
                ret =  new Vector2(0, 1);
                break;
            case EDirection.e:
                ret = new Vector2(1, 0);
                break;
            case EDirection.s:
                ret = new Vector2(0, -1);
                break;
            case EDirection.w:
                ret = new Vector2(-1, 0);
                break;
            case EDirection.ne:
                ret = new Vector2(1, 1);
                break;
            case EDirection.nw:
                ret = new Vector2(-1, 1);
                break;
            case EDirection.se:
                ret = new Vector2(1, -1);
                break;
            case EDirection.sw:
                ret = new Vector2(-1, -1);
                break;
        }

        return ret.normalized;
    }

    public static EDirection ToDirection(this Vector2 vector)
    {
        /*// size of the angle of one direction arc
        float directionIncrement = 45;

        // directions start from east:
        // angle between vector and pure east vector, in range (-180, 180)
        float angle = Vector2.SignedAngle(vector, new Vector2(1, 0));
        // convert to (0, 360)
        angle = angle < 0 ? 360 - angle : angle;

        // which direction arc does the input vector belong to?
        int direction = Mathf.FloorToInt((angle + directionIncrement / 2) / directionIncrement);
        // make sure the result will return a valid direction - loop around
        direction = direction > 7 ? 0 : direction;

        //return (EDirection)direction;*/

        if (vector.x == 0)
        {
            if (vector.y > 0)
            {
                return EDirection.n;
            }
            else if (vector.y < 0)
            {
                return EDirection.s;
            }
        }
        else if (vector.x > 0)
        {
            if (vector.y == 0)
            {
                return EDirection.e;
            }
            else if (vector.y > 0)
            {
                return EDirection.ne;
            }
            else
            {
                return EDirection.se;
            }
        }
        else
        {
            if (vector.y == 0)
            {
                return EDirection.w;
            }
            else if (vector.y > 0)
            {
                return EDirection.nw;
            }
            else
            {
                return EDirection.sw;
            }
        }

        return EDirection.NDEF;
    }

    public static Vector2 SetTo01(this Vector2 vector)
    {
        vector.x = vector.x == 0 ? 0 : vector.x < 0 ? -1 : 1;
        vector.y = vector.y == 0 ? 0 : vector.y < 0 ? -1 : 1;
        return vector;
    }
}
