using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteOrganizer
{
    public void Delete(string dir, string nameContains)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
            return;
        }

        DeleteRec(dirInfo, nameContains);
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

    public void Rename(string dir, string renameFrom, string renameTo)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
            return;
        }

        RenameRec(dirInfo, renameFrom, renameTo);
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
