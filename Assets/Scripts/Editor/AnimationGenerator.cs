using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Generator for animation clips. The clips are placed in a hierarchy according to file names. 
/// Expects .png files on input, with the following naming convention:
/// {character-name}_{animation-name}_{direction}_{frame-number}.png
/// </summary>
public class AnimationGenerator
{
    private string characterName;

    /// <summary>
    /// Generates animation clips from sprites for the given character.
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public int GenerateAnimations(string characterName)
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

        this.characterName = characterName;

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
            if (GFXUtility.IsFolderRelevant(subInfo.Name, typeof(EAnimation)))
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
    /// <param name="frameRate"></param>
    /// <param name="loop"></param>
    /// <returns></returns>
    public bool GenerateAnimationClip(string loadFrom, string saveTo, string saveAs, int frameRate = 6, bool loop = true)
    {
        Debug.Log("Processing sprites at path " + loadFrom);

        if (File.Exists(saveTo + "/" + saveAs + ".anim")) return false;

        // set up the clip
        AnimationClip clip = new AnimationClip
        {
            frameRate = frameRate
        };
        AnimationClipSettings settings = new AnimationClipSettings
        {
            loopTime = loop
        };
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        // create the animation curve
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
        ObjectReferenceKeyframe[] frames = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            frames[i] = new ObjectReferenceKeyframe
            {
                time = i / clip.frameRate,
                value = sprites[i]
            };
        }
        AnimationUtility.SetObjectReferenceCurve(clip, binding, frames);

        // save the clip
        Debug.Log("Saving clip of name " + saveAs + " to folder " + saveTo);
        Directory.CreateDirectory(saveTo);
        AssetDatabase.CreateAsset(clip, saveTo + "/" + saveAs + ".anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return true;
    }



}
