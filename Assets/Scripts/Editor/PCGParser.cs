using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PCGParser : EditorWindow
{
    [MenuItem("Custom/PCG Parser")]
    static void Init()
    {
        PCGParser window = (PCGParser)GetWindow(typeof(PCGParser));
        window.Show();
    }

    private void OnGUI()
    {

        if (GUILayout.Button("Parse"))
        {
            Debug.Log("Editor scripting je very fun");
        }
    }
}
