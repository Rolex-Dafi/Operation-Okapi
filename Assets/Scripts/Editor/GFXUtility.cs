using System;
using UnityEngine;

public static class GFXUtility
{
    public static readonly string resourcesDirectory = "Assets/Resources";
    public static readonly string spritesDirectory = "Sprites";
    public static readonly string characterSritesDirectory = "Sprites/Characters";
    public static readonly string characterAnimationsDirectory = "Assets/Animation/Characters";

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
            duplicateSingleFrame = false 
        };


    public static bool IsFolderRelevant(string folderName, Type t)
    {
        if (t.Equals(typeof(EAbility)) || t.Equals(typeof(EDirection)))
        {
            foreach (var value in Enum.GetValues(t))
            {
                if (folderName.Equals(value.ToString())) return true;
            }
        }
        return false;
    }
}



