using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Used for general sprite organization.
/// </summary>
public class SpriteOrganizer
{
    /// <summary>
    /// Deletes all files with the specified parameters.
    /// </summary>
    /// <param name="dir">The directory with the files</param>
    /// <param name="nameContains">The substring the names of the files to be deleted have to contain</param>
    public void Delete(string dir, string nameContains)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
            return;
        }

        DeleteRec(dirInfo, nameContains);

        AssetDatabase.Refresh();
    }

    private void DeleteRec(DirectoryInfo dirInfo, string nameContains)
    {
        foreach (DirectoryInfo subdir in dirInfo.GetDirectories())
        {
            FileInfo[] files = subdir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name.Contains(nameContains)) File.Delete(file.FullName);
            }
            DeleteRec(subdir, nameContains);
        }
    }

    /// <summary>
    /// Renames all files with the specified parameters.
    /// </summary>
    /// <param name="dir">The director with the files</param>
    /// <param name="renameFrom">Substring to be replaced</param>
    /// <param name="renameTo">The replacement substring</param>
    public void Rename(string dir, string renameFrom, string renameTo)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
            return;
        }

        RenameRec(dirInfo, renameFrom, renameTo);

        AssetDatabase.Refresh();
    }

    private void RenameRec(DirectoryInfo dirInfo, string renameFrom, string renameTo)
    {
        foreach (DirectoryInfo subdir in dirInfo.GetDirectories())
        {
            FileInfo[] files = subdir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!file.Name.Contains(renameFrom)) continue;

                string newName = file.FullName.Replace(renameFrom, renameTo);
                File.Move(file.FullName, newName);
            }
            RenameRec(subdir, renameFrom, renameTo);
        }
    }


}
