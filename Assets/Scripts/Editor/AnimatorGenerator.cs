using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Generates an animator controller for one character. Expects animation
/// clips on input.
/// </summary>
public class AnimatorGenerator
{
    private string characterName;

    /// <summary>
    /// Generates animation clips from sprites for the given character.
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public void GenerateAnimator(string characterName)
    {
        string dir = string.Join("/", new string[] {
            GFXUtility.characterAnimationsDirectory,
            characterName
        });

        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
        }

        this.characterName = characterName;

        AnimatorOverrideController animover = new AnimatorOverrideController();
        //animover.GetOverrides()
    }


}
