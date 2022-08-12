using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class Utility
{
    // we're using 2:1 isometry, computed from dot product, in degrees
    public static float isometricAngle = Mathf.Acos(2 / Mathf.Sqrt(5)) * Mathf.Rad2Deg;

    public const string playerTagAndLayer = "Player";
    public const string enemyTagAndLayer = "Enemy";
    public const string obstacleLayer = "Obstacle";
    public const string wallLayer = "Wall";

    // TODO actually look this size up
    public const float minObstacleSize = .6f;

    // scenes

    public const int mainMenuIndex = 1;
    public const int firstLevelIndex = 2;
}

// Enums

public enum EAbilityType
{
    idle, walk, dash, melee, ranged, special, hit, death, NDEF  
}

public enum EAttackCommand 
{ 
    Begin, End, NDEF 
}

public enum EAttackEffect // equivalent to button down, hold, up - for player
{ 
    Click, // 0 startup, x active, 0 recovery frames
    Spray, // x startup, y active, 0 recovery frames 
    Aim    // x startup, y active, z recovery frames
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
    directionX, directionY, speed, dashing, hit, death, attack, attackReleased, attackID
}

/// <summary>
/// Specifies the frame count per each attack phase - startup, active, recovery.
/// (So far) only utilized for animation clip generation - depending on the type of attack, some of the 
/// counts will always be zero.
/// </summary>
public class AttackFrames
{
    private EAttackEffect attackEffect;

    private int startup;
    private int active;
    private int recovery;

    public AttackFrames(EAttackEffect attackEffect = EAttackEffect.Click, int startup = 0, int active = 0, int recovery = 0)
    {
        this.attackEffect = attackEffect;
        this.startup = startup;
        this.active = active;
        this.recovery = recovery;
    }

    public EAttackEffect AttackEffect { get => attackEffect; }

    public int Startup { get => startup; }
    public int Active { get => active; }
    public int Recovery { get => recovery; }

    public Tuple<int, int> GetStartupIndexes()
    {
        return new Tuple<int, int>
        (
            0,
            Startup
        );
    }

    public Tuple<int, int> GetActiveIndexes()
    {
        return new Tuple<int, int>
        (
            Startup,
            Startup + Active
        );
    }

    public Tuple<int, int> GetRecoveryIndexes()
    {
        return new Tuple<int, int>
        (
            Startup + Active,
            Startup + Active + Recovery
        );
    }
}

public static class Extentions
{
    public static IEnumerator AddForceCustom(this Rigidbody2D rb, Vector2 direction, float distance, float speed, UnityAction onEnd = null)
    {
        float distanceTravelled = 0;
        while (true)
        {
            if (distanceTravelled > distance)
            {
                onEnd?.Invoke();
                break;
            }

            Vector2 step = speed * Time.fixedDeltaTime * direction;
            rb.MovePosition(rb.position + step);

            distanceTravelled += step.magnitude;

            yield return new WaitForFixedUpdate();
        }
    }


    public static AnimatorControllerParameterType ToAnimatorParameterType(this EAnimationParameter parameter)
    {
        switch (parameter)
        {
            case EAnimationParameter.directionX:
            case EAnimationParameter.directionY:
            case EAnimationParameter.speed:
                return AnimatorControllerParameterType.Float;
            case EAnimationParameter.attackID:
                return AnimatorControllerParameterType.Int;
            case EAnimationParameter.dashing:
                return AnimatorControllerParameterType.Bool;
            default:
                return AnimatorControllerParameterType.Trigger;
        }
    }

    public static EStateMachine ToEStateMachine(this EAbilityType ability)
    {
        switch (ability)
        {
            case EAbilityType.idle:
            case EAbilityType.walk:
                return EStateMachine.move;
            case EAbilityType.melee:
            case EAbilityType.ranged:
            case EAbilityType.special:
                return EStateMachine.attack;
            default:
                return EStateMachine.root;
        }
    }

    public static Vector2 CartesianToIsometric(this Vector2 vector)
    {
        return new Vector2(vector.x, vector.y / 2);
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
