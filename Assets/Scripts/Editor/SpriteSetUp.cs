using System;
using System.IO;
using UnityEditor;
using UnityEngine;


public struct SpriteImportSettings
{
    public int ppu;
    public Pivot pivot;
}

[Serializable]
public struct Pivot
{
    public SpriteAlignment type;
    public Vector2 vector;
}

/// <summary>
/// Class which handles setting up the sprites for use in the game.
/// </summary>
public class SpriteSetUp
{
    // remake this into a struct "sprite_info" later:
    private SpriteImportSettings settings;

    public SpriteSetUp(SpriteImportSettings settings)
    {
        this.settings = settings;
    }

    /// <summary>
    /// Sets import settings of all sprites in given directory, containing string specified in their file name.
    /// </summary>
    /// <param name="dir">Directory of sprites to modify</param>
    /// <param name="nameContains">Only modify sprites with this in their file name. Modifies all sprites
    /// if not specified. </param>
    public void SetSpriteImportSettings(string dir, string nameContains)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
            return;
        }

        GetDirPathRec(dirInfo, dir, nameContains);
    }

    /// <summary>
    /// Recursivelly goes through all sub-directories.
    /// </summary>
    /// <param name="dirInfo"></param>
    /// <param name="dir"></param>
    private void GetDirPathRec(DirectoryInfo dirInfo, string dir, string nameContains)
    {
        foreach (DirectoryInfo subdir in dirInfo.GetDirectories())
        {
            FileInfo[] files = subdir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (nameContains != "" && !file.Name.Contains(nameContains)) continue;

                string path = dir + "/" + subdir.Name + "/" + file.Name;

                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (textureImporter != null)
                {
                    TextureImporterSettings texSettings = new TextureImporterSettings();

                    textureImporter.ReadTextureSettings(texSettings);
                    texSettings.spriteAlignment = (int)settings.pivot.type;
                    textureImporter.SetTextureSettings(texSettings);

                    textureImporter.spritePixelsPerUnit = settings.ppu;
                    textureImporter.spritePivot = settings.pivot.vector;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
            GetDirPathRec(subdir, dir + "/" + subdir.Name, nameContains);
        }
    }

}
