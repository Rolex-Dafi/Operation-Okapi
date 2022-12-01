using System;
using System.Collections.Generic;


[Serializable]
public class RoomDialogue
{
    public List<Passage> passages;

    public Passage GetOutroPassage()
    {
        foreach (var passage in passages)
        {
            if (passage.passageName.Contains("Outro")) return passage;
        }

        return passages[0];
    }
    
    public RoomDialogue()
    {
        passages = new List<Passage>();
    }
}
