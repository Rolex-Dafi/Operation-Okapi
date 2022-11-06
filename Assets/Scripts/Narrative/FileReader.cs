using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileReader : MonoBehaviour
{
    private Level currentLevel;
    private int currentRoom;
    private int currentPassage; // when there's more passages of dialogue in one room
    
    public Dictionary<Level, LevelDialogue> ReadFile(string path)
    {
        var fileInfo = new FileInfo(path);
        var streamReader = fileInfo.OpenText();

        var dict = new Dictionary<Level, LevelDialogue>
        {
            { Level.Office, new LevelDialogue() },
            { Level.Street, new LevelDialogue() },
            { Level.Mall, new LevelDialogue() },
            { Level.Roof, new LevelDialogue() }
        };
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();
            if (line == null) continue;
            
            // remove comments
            var validLine = line.Split('%')[0];

            // skip empty lines
            if (string.IsNullOrWhiteSpace(validLine)) continue;
            
            if (validLine.Contains("Level"))
            {
                currentRoom = -1;
                currentLevel = currentLevel == Level.Office ? Level.Office : currentLevel + 1;
                dict[currentLevel].rooms = new List<RoomDialogue>();
            }
            else if (validLine.Contains("Room"))
            {
                currentPassage = -1;
                ++currentRoom;
                dict[currentLevel].rooms[currentRoom].passages = new List<Passage>();
            }
            else if (validLine.Contains("Passage"))
            {
                ++currentPassage;
                var psgName = validLine.Contains(":") ? validLine.Split(':')[1] : "Outro";
                dict[currentLevel].rooms[currentRoom].passages.Add(new Passage(psgName));
            }
            else // actual dialogue line
            {
                dict[currentLevel].rooms[currentRoom].passages[currentPassage].lines.Add(validLine);
            }
        }
        
        streamReader.Close();

        return dict;
    }
}
