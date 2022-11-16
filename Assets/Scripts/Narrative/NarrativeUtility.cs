using System;
using System.Collections.Generic;

[Serializable]
public class Passage
{
    public string passageName;
    public List<string> lines;

    public Passage(string passageName)
    {
        this.passageName = passageName;
        lines = new List<string>();
    }
}

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

[Serializable]
public class LevelDialogue
{
    public List<RoomDialogue> rooms;

    public LevelDialogue()
    {
        rooms = new List<RoomDialogue>();
    }
}