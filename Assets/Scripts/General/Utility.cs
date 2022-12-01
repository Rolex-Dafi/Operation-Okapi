using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Utility
{
    // we're using 2:1 isometry, computed from dot product, in degrees
    public static float isometricAngle = Mathf.Acos(2 / Mathf.Sqrt(5)) * Mathf.Rad2Deg;

    public const string playerTagAndLayer = "Player";
    public const string enemyTagAndLayer = "Enemy";
    public const string obstacleLayer = "Obstacle";
    public const string wallLayer = "Wall";
    public const string ignoreObstaclesLayer = "IgnoreObstacles";

    public const string activateTrigger = "Activate";
    
    // TODO actually look this size up
    public const float minObstacleSize = .6f;

    // Diplomka veci
    public static bool gayVersion;
    public static bool secondSciat = false;

    public const float maxPlayTime = 30 * 60; // 30 minutes
    
    // analytics
    public static float gameStartTime;
    public static bool gameWon;

    public const int maxHealthBarSlots = 5;
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

/// <summary>
/// 
/// </summary>
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
    private int startup;
    private int active;
    private int recovery;

    public AttackFrames(EAttackEffect attackEffect = EAttackEffect.Click, int startup = 0, int active = 0, int recovery = 0)
    {
        AttackEffect = attackEffect;
        this.startup = startup;
        this.active = active;
        this.recovery = recovery;
    }

    public EAttackEffect AttackEffect { get; }

    private int Startup { get => startup; }
    private int Active { get => active; }
    private int Recovery { get => recovery; }

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
    // Tweening
    /// <summary>
    /// A tweening function which punches an image color towards the given value and then back to the starting one.
    /// </summary>
    /// <param name="image">Image to punch</param>
    /// <param name="color">The color value to punch towards</param>
    /// <param name="time">The duration of the tween</param>
    public static void PunchColor(this Image image, Color color, float time = .1f)
    {
        DOTween.Punch(
            ()=> image.color.ToVector3(), 
            x=> image.color = new Color(x.x, x.y, x.z, image.color.a), 
            color.ToVector3(), 
            time, 
            1, 
            0
        );
    }
    
    /// <summary>
    /// A tweening function which punches an image alpha towards the given value and then back to the starting one.
    /// </summary>
    /// <param name="image">Image to punch</param>
    /// <param name="punch">The alpha value to punch towards</param>
    /// <param name="time">The duration of the tween</param>
    public static void PunchAlpha(this Image image, float punch, float time = .1f)
    {
        DOTween.Punch(
            ()=> image.color.a * Vector3.one, 
            x=> image.color = new Color(image.color.r, image.color.g, image.color.b, x.x), 
            punch * Vector3.one, 
            time, 
            1, 
            0
        );
    }

    /// <summary>
    /// Converts a color to a Vector3. (Simply removes the alpha component.)
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private static Vector3 ToVector3(this Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }
    
    // Physics
    /// <summary>
    /// Moves a rigidbody in a given direction.
    /// </summary>
    /// <param name="rb">Rigidbody to add the force to</param>
    /// <param name="direction">Direction of the force</param>
    /// <param name="distance">Distance which the rigidbody should travel in the given direction</param>
    /// <param name="speed">The speed at which the rigidbody should travel</param>
    /// <param name="onEnd">Callback when the movement ends</param>
    /// <returns></returns>
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

    // Animation
    // TODO relocate these two to the gfx utility
    /// <summary>
    /// An editor helper function. Used when creating an animator controller.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
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

    /// <summary>
    /// An editor helper function. Used when creating an animator controller.
    /// Returns what state-machine a given ability should be created in.
    /// </summary>
    /// <param name="ability">The ability</param>
    /// <returns>The state-machine the ability should be in</returns>
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

    // General Utility
    /// <summary>
    /// Coverts a vector from cartesian to isometric coordinates.
    /// </summary>
    /// <param name="vector">Vector to convert</param>
    /// <returns>Vector in isometric coordinates</returns>
    public static Vector2 CartesianToIsometric(this Vector2 vector)
    {
        return new Vector2(vector.x, vector.y / 2);
    }

    /// <summary>
    /// Converts a Vector3 to a Vector2. (Simply removes the last coordinate.)
    /// </summary>
    /// <param name="vector">Vector to convert</param>
    /// <returns>Converted vector</returns>
    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }
    
    /// <summary>
    /// Converts a Vector2 to Vector3. Sets the last coordinate to 0.
    /// </summary>
    /// <param name="vector">Vector to convert</param>
    /// <returns>Converted vector</returns>
    public static Vector3 ToVector3(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    /// <summary>
    /// Converts a direction to a vector.
    /// </summary>
    /// <param name="direction">Direction to convert</param>
    /// <returns>Converted vector</returns>
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
}
