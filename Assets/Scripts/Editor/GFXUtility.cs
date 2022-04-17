using System;

public static class GFXUtility
{
    public static readonly string resourcesDirectory = "Assets/Resources";
    public static readonly string spritesDirectory = "Sprites";
    public static readonly string characterSritesDirectory = "Sprites/Characters";
    public static readonly string characterAnimationsDirectory = "Assets/Animation/Characters";

    public static bool IsFolderRelevant(string folderName, Type t)
    {
        if (t.Equals(typeof(EAnimation)) || t.Equals(typeof(EDirection))) 
        {
            foreach (var value in Enum.GetValues(t))
            {
                if (folderName.Equals(value.ToString())) return true;
            }
        }
        return false;
    }
}

public enum EAnimation
{
    idle, movement, dash, melee, ranged, hit, death
}

public enum EDirection
{
    n, e, s, w, ne, nw, se, sw
}
