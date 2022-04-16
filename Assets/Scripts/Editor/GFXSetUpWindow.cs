using UnityEditor;
using UnityEngine;

public class GFXSetUpWindow : EditorWindow
{
    // Sprite set up
    string spritesDirectory = "Assets/Resources/Sprites";
    int spritePpu = 540;

    // Animations creation


    [MenuItem("Custom/GFX Set-up")]
    static void Init()
    {
        GFXSetUpWindow window = (GFXSetUpWindow)GetWindow(typeof(GFXSetUpWindow));
        window.Show();
    }

    private void OnGUI()
    {
        // Sprite set up
        GUILayout.Label("Sprite Set-up", EditorStyles.boldLabel);
        spritesDirectory = EditorGUILayout.TextField("Sprites' root directory", spritesDirectory);                
        GUILayout.Label("Sprite Properties", EditorStyles.miniBoldLabel);
        spritePpu = EditorGUILayout.IntField("Desired sprite PPU", spritePpu);

        if (GUILayout.Button("Set up sprites"))
        {
            new SpriteSetUp(spritePpu).SetSpriteImportSettings(spritesDirectory);
            Debug.Log("Sprite set-up finished");
        }

        // Animations creation
    }
}
