using System;
using System.Collections.Generic;


[Serializable]
public class LevelDialogue
{
    public List<RoomDialogue> rooms;

    public LevelDialogue()
    {
        rooms = new List<RoomDialogue>();
    }
}
