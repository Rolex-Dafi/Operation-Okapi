using System;
using System.Collections.Generic;

/// <summary>
/// Wrapper for the dialogue for one level. Contains a list of room dialogues.
/// </summary>
[Serializable]
public class LevelDialogue
{
    public List<RoomDialogue> rooms;

    /// <summary>
    /// Construct a level dialogue with a list of room dialogues.
    /// </summary>
    public LevelDialogue()
    {
        rooms = new List<RoomDialogue>();
    }
}
