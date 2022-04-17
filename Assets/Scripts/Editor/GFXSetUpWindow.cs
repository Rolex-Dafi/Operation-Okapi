using UnityEditor;
using UnityEngine;

public class GFXSetUpWindow : EditorWindow
{
    // Sprite set up
    int spritePpu = 540;

    // Animations creation
    string characterName = "Player";


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
        GUILayout.Label("Sprite Properties", EditorStyles.miniBoldLabel);
        spritePpu = EditorGUILayout.IntField("Desired sprite PPU", spritePpu);

        if (GUILayout.Button("Set up sprites"))
        {
            new SpriteSetUp(spritePpu).SetSpriteImportSettings(GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory);
            Debug.Log("Sprite set-up finished");
        }

        // Animations creation
        GUILayout.Label("Animation creation", EditorStyles.boldLabel);
        characterName = EditorGUILayout.TextField("Character to generate", characterName);
        if (GUILayout.Button("Generate animation clips"))
        {
            int numClips = new AnimationGenerator().GenerateAnimations(characterName);

            Debug.Log("Number of animation clips generated = " + numClips);
        }
    }
}
