using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileReaderWindow : EditorWindow
{
    private TextAsset dialogueFile;
    
    private LevelSO officeLevel;
    private LevelSO streetLevel;
    private LevelSO mallLevel;
    private LevelSO roofLevel;
    
    [MenuItem("Window/Dialogue file reader")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (FileReaderWindow)GetWindow(typeof(FileReaderWindow));
        window.Show();
    }
    
    void OnGUI()
    {
        GUILayout.Label("Dialogue reader", EditorStyles.boldLabel);
        dialogueFile = (TextAsset)EditorGUILayout.ObjectField("Dialogue file", dialogueFile, typeof(TextAsset), false);
        
        officeLevel = (LevelSO)EditorGUILayout.ObjectField("Office level", officeLevel, typeof(LevelSO), false);
        streetLevel = (LevelSO)EditorGUILayout.ObjectField("Street level", streetLevel, typeof(LevelSO), false);
        mallLevel = (LevelSO)EditorGUILayout.ObjectField("Mall level", mallLevel, typeof(LevelSO), false);
        roofLevel = (LevelSO)EditorGUILayout.ObjectField("Roof level", roofLevel, typeof(LevelSO), false);
        
        if (GUILayout.Button("Load from file"))
        {
            var dict = ReadFile();
            officeLevel.levelDialogue = dict[Level.Office];
            streetLevel.levelDialogue = dict[Level.Street];
            mallLevel.levelDialogue = dict[Level.Mall];
            roofLevel.levelDialogue = dict[Level.Roof];
        }
    }
    
    private Dictionary<Level, LevelDialogue> ReadFile()
    {
        var currentLevel = -1;
        var currentRoom = -1;
        var currentPassage = -1; // when there's more passages of dialogue in one room

        var dict = new Dictionary<Level, LevelDialogue>
        {
            { Level.Office, new LevelDialogue() },
            { Level.Street, new LevelDialogue() },
            { Level.Mall, new LevelDialogue() },
            { Level.Roof, new LevelDialogue() }
        };

        var lines = dialogueFile.text.Split('\n');
        
        foreach(var line in lines)
        {
            Debug.Log("line: " + line + ", level " + currentLevel + ", room " + currentRoom + ", passage " + currentPassage);
            
            // remove comments
            var validLine = line.Split('%')[0];
            
            // skip empty lines
            if (string.IsNullOrWhiteSpace(validLine)) continue;
            
            if (validLine.Contains("Level"))
            {
                currentRoom = -1;
                ++currentLevel;
                dict[(Level)currentLevel].rooms = new List<RoomDialogue>();
            }
            else if (validLine.Contains("Room"))
            {
                currentPassage = -1;
                ++currentRoom;
                dict[(Level)currentLevel].rooms.Add(new RoomDialogue());
                dict[(Level)currentLevel].rooms[currentRoom].passages = new List<Passage>();
            }
            else if (validLine.Contains("Passage"))
            {
                ++currentPassage;
                var psgName = validLine.Contains(":") ? validLine.Split(':')[1] : "Outro";
                dict[(Level)currentLevel].rooms[currentRoom].passages.Add(new Passage(psgName));
            }
            else // actual dialogue line
            {
                dict[(Level)currentLevel].rooms[currentRoom].passages[currentPassage].lines.Add(validLine);
            }
        }

        return dict;
    }
}
