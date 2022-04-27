using System;
using System.IO;
using UnityEditor;
using UnityEngine;


public struct AnimationClipProperties
{
    public float frameRate;
    public bool loop;
    public Color spriteColor;            // for debug
    public bool duplicateSingleFrame;    // for debug

    public AnimationClipProperties(float frameRate, bool loop, Color spriteColor, bool duplicateSingleFrame)
    {
        this.frameRate = frameRate;
        this.loop = loop;
        this.spriteColor = spriteColor;
        this.duplicateSingleFrame = duplicateSingleFrame;
    }
}

/// <summary>
/// Generator for animation clips. The clips are placed in a hierarchy according to file names. 
/// Expects .png files on input, with the following naming convention:
/// {character-name}_{animation-name}_{direction}_{frame-number}.png
/// </summary>
public class AnimationClipGenerator
{
    private string characterName;
    private AnimationClipProperties animationProperties;

    public AnimationClipGenerator(string characterName)
    {
        this.characterName = characterName;
        animationProperties = GFXUtility.defaultAnimationClipProperties;
    }

    public AnimationClipGenerator(string characterName, AnimationClipProperties animationProperties)
    {
        this.characterName = characterName;
        this.animationProperties = animationProperties;
    }

    /// <summary>
    /// Generates animation clips from sprites for the given ability for the given character.
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns>Number of clips generated.</returns>
    public int GenerateAnimations(EAbility ability)
    {
        string dir = string.Join("/", new string[] {
            GFXUtility.resourcesDirectory,
            GFXUtility.characterSritesDirectory,
            characterName,
            ability.ToString()
        });

        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
            return 0;
        }

        return FindDirectionFolders(dirInfo);
    }

    /// <summary>
    /// Generates animation clips from sprites for the given character.
    /// </summary>
    /// <returns>Number of clips generated.</returns>
    public int GenerateAllAnimations()
    {
        string dir = string.Join("/", new string[] { 
            GFXUtility.resourcesDirectory, 
            GFXUtility.characterSritesDirectory,
            characterName
        });

        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
            return 0;
        }

        return FindAnimationFolders(dirInfo);
    }

    /// <summary>
    /// Attempt to find animation folders, specified by EAnimation enum in given directory.
    /// The directory should be at "Assets/Resources/Sprites/Characters/{character-name}".
    /// </summary>
    /// <param name="dirInfo">Directory to be searched.</param>
    private int FindAnimationFolders(DirectoryInfo dirInfo)
    {
        int numClips = 0;
        foreach (DirectoryInfo subInfo in dirInfo.GetDirectories())
        {
            if (GFXUtility.IsFolderRelevant(subInfo.Name, typeof(EAbility)))
            {
                numClips += FindDirectionFolders(subInfo);
            }
        }
        return numClips;
    }

    /// <summary>
    /// Attempt to find direction folders, specified by EDirection enum in given directory.
    /// The directory should be at "Assets/Resources/Sprites/Characters/{character-name}/{animation-name}".
    /// </summary>
    /// <param name="dirInfo">Directory to be searched.</param>
    private int FindDirectionFolders(DirectoryInfo dirInfo)
    {
        int numClips = 0;
        foreach (DirectoryInfo subInfo in dirInfo.GetDirectories())
        {
            if (GFXUtility.IsFolderRelevant(subInfo.Name, typeof(EDirection)))
            {
                string loadFrom = string.Join("/", new string[] { characterName, dirInfo.Name, subInfo.Name });
                string saveTo = string.Join("/", new string[] { GFXUtility.characterAnimationsDirectory, characterName, dirInfo.Name });
                string saveAs = string.Join("_", new string[] { characterName, dirInfo.Name, subInfo.Name });

                if (GenerateAnimationClip(loadFrom, saveTo, saveAs)) ++numClips;
            }
        }
        return numClips;
    }


    /// <summary>
    /// Generates an animation clip according to specified parameters and saves it.
    /// Expects the loadFrom path to be in the form "{character-name}/{animation-name}/{direction}".
    /// Expects the saveTo path to be in the form "Assets/Animation/Characters/{character-name}/{animation-name}".
    /// Expects the saveAs name to be in the form "{character-name}_{animation-name}_{direction}".
    /// </summary>
    /// <param name="loadFrom">Path to animation frame sprites.</param>
    /// <param name="saveTo">Path where the animation clip will be saved.</param>
    /// <param name="saveAs">The name of the clip.</param>
    /// <returns>Whether the clip was generated.</returns>
    private bool GenerateAnimationClip(string loadFrom, string saveTo, string saveAs)
    {
        Debug.Log("Processing sprites at path " + loadFrom);

        if (File.Exists(saveTo + "/" + saveAs + ".anim")) return false;

        // set up the clip
        AnimationClip clip = new AnimationClip
        {
            frameRate = animationProperties.frameRate
        };
        AnimationClipSettings settings = new AnimationClipSettings
        {
            loopTime = animationProperties.loop
        };
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        // add the sprites
        bool ret = AddSpriteAnimationCurve(clip, loadFrom);

        // tint the sprites if relevant
        if (animationProperties.spriteColor != Color.white)
        {
            ret = ret && AddColorAnimationCurve(clip, animationProperties.spriteColor);
        }

        // save the clip
        Debug.Log("Saving clip of name " + saveAs + " to folder " + saveTo);
        Directory.CreateDirectory(saveTo);
        AssetDatabase.CreateAsset(clip, saveTo + "/" + saveAs + ".anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return ret;
    }

    private bool AddSpriteAnimationCurve(AnimationClip clip, string loadFrom)
    {
        // create the binding
        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };

        // load the sprites
        Sprite[] sprites = Resources.LoadAll<Sprite>(GFXUtility.characterSritesDirectory + "/" + loadFrom);
        if (sprites.Length == 0)
        {
            Debug.LogError("No sprites at " + loadFrom);
            return false;
        }

        // add keyframes
        ObjectReferenceKeyframe[] frames;
        if (sprites.Length == 1 && animationProperties.duplicateSingleFrame)
        {
            frames = new ObjectReferenceKeyframe[2];
            for (int i = 0; i < 2; i++)
            {
                frames[i] = new ObjectReferenceKeyframe
                {
                    time = i / clip.frameRate,
                    value = sprites[0]
                };
            }
        }
        else
        {
            frames = new ObjectReferenceKeyframe[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                frames[i] = new ObjectReferenceKeyframe
                {
                    time = i / clip.frameRate,
                    value = sprites[i]
                };
            }
        }
        AnimationUtility.SetObjectReferenceCurve(clip, binding, frames);

        return true;
    }

    private bool AddColorAnimationCurve(AnimationClip clip, Color color)
    {
        string[] propertyNames = { "m_Color.r", "m_Color.g", "m_Color.b", "m_Color.a" };

        // create the bindings
        EditorCurveBinding[] bindings = new EditorCurveBinding[propertyNames.Length];
        for (int i = 0; i < propertyNames.Length; i++)
        {
            bindings[i] = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                propertyName = propertyNames[i]
            };
        }

        // add keyframes
        Keyframe[][] frames = new Keyframe[propertyNames.Length][];
        for (int i = 0; i < propertyNames.Length; i++)
        {
            frames[i] = new Keyframe[2];
            for (int j = 0; j < 2; j++)
            {
                frames[i][j] = new Keyframe
                {
                    time = j * (clip.length - 1f/clip.frameRate), // 0th frame and last frame
                    value = color[i]
                };
            }
        }        
        
        for (int i = 0; i < propertyNames.Length; i++)
        {
            AnimationUtility.SetEditorCurve(clip, bindings[i], new AnimationCurve(frames[i]));
        }

        return true;
    }


}
