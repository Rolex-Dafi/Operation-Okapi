using System;
using UnityEngine;

public static class GFXUtility
{
    public static readonly string resourcesDirectory = "Assets/Resources";
    public static readonly string spritesDirectory = "Sprites";
    public static readonly string characterSritesDirectory = "Sprites/Characters";
    public static readonly string characterAnimationsDirectory = "Assets/Animation/Characters";

    public static readonly float defaultAttackHitboxRange = .3f;
    public static readonly string hitboxObjectName = "MeleeHitBox";

    public static readonly SpriteImportSettings defaultSpriteImportSettings =
        new SpriteImportSettings
        {
            ppu = 540,
            pivot =
            new Pivot { type = SpriteAlignment.Center, vector = new Vector2(.5f, .25f) }
        };

    public static readonly float defaultAnimationFrameRate = 9;
    public static readonly AnimationClipProperties defaultAnimationClipProperties = 
        new AnimationClipProperties{
            frameRate = defaultAnimationFrameRate, 
            loop = false, 
            spriteColor = Color.white, 
            duplicateSingleFrame = false,
            characterName = "Ms-Cups",
            meleeHitBoxOnFrame = -1
        };

    /// <summary>
    /// Checks whether a folder could contain any sprites used in generating the animator controller.
    /// </summary>
    /// <param name="folderName">Name of the folder</param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsFolderRelevant(string folderName, Type t)
    {
        if (t.Equals(typeof(EAbilityType)) || t.Equals(typeof(EDirection)))
        {
            foreach (var value in Enum.GetValues(t))
            {
                if (folderName.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase)) return true;
            }
        }
        return false;
    }
}

public static class GFXExtensions
{
    /// <summary>
    /// Tries to convert a string to a Direction.
    /// </summary>
    /// <param name="direction">The string to convert</param>
    /// <returns>A direction</returns>
    public static EDirection GetDirection(this string direction)
    {
        foreach (var value in Enum.GetValues(typeof(EDirection)))
        {
            if (direction.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase)) return (EDirection)value;
        }

        return EDirection.NDEF;
    }
}



