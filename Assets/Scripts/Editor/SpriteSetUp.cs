using System.IO;
using UnityEditor;
using UnityEngine;

public class SpriteSetUp
{
    // remake this into a struct -sprite_info- later:
    private int ppu;

    public SpriteSetUp(int ppu)
    {
        this.ppu = ppu;
    }

    /// <summary>
    /// Sets PPU of all sprites in directory at <param name="dir"> to the 
    /// value provided in <param name="ppu">.
    /// </summary>
    /// <param name="dir">directory of sprites to set up</param>
    /// <param name="ppu">pixels per unit</param>
    public void SetSpriteImportSettings(string dir)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
            return;
        }

        GetDirPathRec(dirInfo, dir);
    }

    /// <summary>
    /// Recursivelly goes through all sub-directories.
    /// </summary>
    /// <param name="dirInfo"></param>
    /// <param name="dir"></param>
    private void GetDirPathRec(DirectoryInfo dirInfo, string dir)
    {
        foreach (DirectoryInfo subdir in dirInfo.GetDirectories())
        {
            FileInfo[] files = subdir.GetFiles();
            foreach (FileInfo file in files)
            {
                string path = dir + "/" + subdir.Name + "/" + file.Name;

                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (textureImporter != null)
                {
                    textureImporter.spritePixelsPerUnit = ppu;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
            GetDirPathRec(subdir, dir + "/" + subdir.Name);
        }
    }

}
