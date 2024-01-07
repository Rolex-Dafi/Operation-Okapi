using System;
using System.Collections.Generic;

/// <summary>
/// Wrapper for the dialogue for each room. Contains a list of passages for the room.
/// </summary>
[Serializable]
public class RoomDialogue
{
    public List<Passage> passages;

    /// <summary>
    /// Returns the passage with the "Outro" keyword, or the first passage if no "Outro" keyword is found.
    /// </summary>
    /// <returns></returns>
    public Passage GetOutroPassage()
    {
        foreach (var passage in passages)
        {
            if (passage.passageName.Contains("Outro")) return passage;
        }

        return passages[0];
    }
    
    /// <summary>
    /// Construct a room dialogue with a list of dialogue passages.
    /// </summary>
    public RoomDialogue()
    {
        passages = new List<Passage>();
    }
}
