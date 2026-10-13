using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A struct for custom animation clip properties.
/// </summary>
public struct AnimationClipProperties
{
    public float frameRate;
    public bool loop;
    public Color spriteColor;            // for debug
    public bool duplicateSingleFrame;    // for debug

    // for hitbox curves
    public string characterName;
    public int meleeHitBoxOnFrame;

    public AnimationClipProperties(float frameRate, bool loop, Color spriteColor, bool duplicateSingleFrame, string characterName, int meleeHitBoxOnFrame = -1)
    {
        this.frameRate = frameRate;
        this.loop = loop;
        this.spriteColor = spriteColor;
        this.duplicateSingleFrame = duplicateSingleFrame;
        this.characterName = characterName;
        this.meleeHitBoxOnFrame = meleeHitBoxOnFrame;
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
    public int GenerateAnimations(EAbilityType ability, AttackFrames attackFrames = null)
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

        AttackFrames frames = attackFrames != null && attackFrames.AttackEffect != EAttackEffect.Click ? attackFrames : null;

        return FindDirectionFolders(dirInfo, frames);
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
            if (GFXUtility.IsFolderRelevant(subInfo.Name, typeof(EAbilityType)))
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
    /// <param name="attackFrames">If the frames are to be split into several animation clips </param>
    private int FindDirectionFolders(DirectoryInfo dirInfo, AttackFrames attackFrames = null)
    {
        int numClips = 0;
        foreach (DirectoryInfo subInfo in dirInfo.GetDirectories())
        {
            if (GFXUtility.IsFolderRelevant(subInfo.Name, typeof(EDirection)))
            {
                string loadFrom = string.Join("/", new string[] { characterName, dirInfo.Name, subInfo.Name });
                string saveTo = string.Join("/", new string[] { GFXUtility.characterAnimationsDirectory, characterName, dirInfo.Name });
                string saveAs = string.Join("_", new string[] { characterName, dirInfo.Name, subInfo.Name });

                if (attackFrames != null)
                {
                    if (GenerateAnimationClip(loadFrom, saveTo + "/startup", saveAs + "_startup", attackFrames.GetStartupIndexes(), subInfo.Name.GetDirection())) ++numClips;
                    if (GenerateAnimationClip(loadFrom, saveTo + "/active", saveAs + "_active", attackFrames.GetActiveIndexes(), subInfo.Name.GetDirection())) ++numClips;
                    if (GenerateAnimationClip(loadFrom, saveTo + "/recovery", saveAs + "_recovery", attackFrames.GetRecoveryIndexes(), subInfo.Name.GetDirection())) ++numClips;
                }
                else
                {
                    if (GenerateAnimationClip(loadFrom, saveTo, saveAs, null, subInfo.Name.GetDirection())) ++numClips;
                }
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
    /// <param name="fromToFrames">Which frames to take from the loadFrom folder (only relevant for attack animations).</param>
    /// <returns>Whether the clip was generated.</returns>
    private bool GenerateAnimationClip(string loadFrom, string saveTo, string saveAs, Tuple<int, int> fromToFrames = null, EDirection direction = EDirection.NDEF)
    {
        Debug.Log("Processing sprites at path " + loadFrom);

        // num of frames to generate from is zero -> don't generate an animation clip
        if (fromToFrames != null && fromToFrames.Item1 == fromToFrames.Item2) return false;

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
        bool ret = AddSpriteAnimationCurve(clip, loadFrom, fromToFrames);

        // tint the sprites if relevant
        if (animationProperties.spriteColor != Color.white)
        {
            Debug.Log("coloring sprite with " + animationProperties.spriteColor + " color");
            ret = ret && AddColorAnimationCurve(clip, animationProperties.spriteColor);
        }

        // add hitbox anim if relevant
        if (animationProperties.meleeHitBoxOnFrame > -1)
        {
            Debug.Log("adding hitbox to anim");
            ret = ret && AddHitBoxAnimationCurves(clip, direction);
        }

        // save the clip
        Debug.Log("Saving clip of name " + saveAs + " to folder " + saveTo);
        Directory.CreateDirectory(saveTo);
        AssetDatabase.CreateAsset(clip, saveTo + "/" + saveAs + ".anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return ret;
    }

    private bool AddSpriteAnimationCurve(AnimationClip clip, string loadFrom, Tuple<int, int> fromToFrames = null)
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
        if (fromToFrames != null)
        {
            sprites = sprites.SubArray(fromToFrames.Item1, fromToFrames.Item2);
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

    private bool AddHitBoxAnimationCurves(AnimationClip clip, EDirection direction)
    {
        Vector2 offset = (direction.ToVector2() * GFXUtility.defaultAttackHitboxRange).CartesianToIsometric();

        string hitboxPath = GFXUtility.hitboxObjectName;

        // set hitbox active on given frame in clip settings
        EditorCurveBinding activeCurveBind = new EditorCurveBinding
        {
            type = typeof(CircleCollider2D),
            path = hitboxPath,
            propertyName = "m_Enabled"
        };
        int enabledLength = animationProperties.meleeHitBoxOnFrame > 0 ? 2 : 1;
        Keyframe[] enabledFrames = new Keyframe[enabledLength];
        for (int i = 0; i < enabledLength; i++)
        {
            enabledFrames[i] = new Keyframe
            {
                time = i * animationProperties.meleeHitBoxOnFrame / clip.frameRate,
                value = enabledLength > 1 && i == 0 ? 0 : 1 // true = 1, false = 0
            };
        }
        AnimationUtility.SetEditorCurve(clip, activeCurveBind, new AnimationCurve(enabledFrames));

        // set the offset for the hitbox according to the direction
        string[] propertyNames = { "m_Offset.x", "m_Offset.y" };
        // create the bindings
        EditorCurveBinding[] bindings = new EditorCurveBinding[propertyNames.Length];
        for (int i = 0; i < propertyNames.Length; i++)
        {
            bindings[i] = new EditorCurveBinding
            {
                type = typeof(CircleCollider2D),
                path = hitboxPath,
                propertyName = propertyNames[i]
            };
        }
        Keyframe[] frames = new Keyframe[propertyNames.Length];
        for (int i = 0; i < propertyNames.Length; i++)
        {
            frames[i] = new Keyframe
            {
                time = 0, 
                value = offset[i]
            };
        }
        for (int i = 0; i < propertyNames.Length; i++)
        {
            AnimationUtility.SetEditorCurve(clip, bindings[i], new AnimationCurve(frames[i]));
        }

        return true;
    }
}

public static class AnimationClipGeneratorExtensions
{
    /// <summary>
    /// Returns a sub-array of the provided array, specified by the given indexes (from - inclusive, to - exclusive).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static T[] SubArray<T>(this T[] array, int from, int to)
    {
        T[] subsetArray = new T[to - from];
        for (int i = 0; i < subsetArray.Length; i++)
        {
            subsetArray[i] = array[i + from];
        }
        return subsetArray;
    }
}