using UnityEditor;
using UnityEngine;

public class GFXSetUpWindow : EditorWindow
{
    // Dummy spriten generation
    private string dummyFolder;
    private string nameRenameFrom;
    private string nameRenameTo;
    private string toRemove;

    // Sprite set up
    private string folderName = "";
    private string nameContains = "";
    private SpriteImportSettings settings = 
        new SpriteImportSettings{ ppu = 540, pivot = 
            new Pivot { type = SpriteAlignment.Center, vector = new Vector2(.5f, .25f) } };

    // Animations creation
    private string characterName = "Player";


    [MenuItem("Custom/GFX Set-up")]
    static void Init()
    {
        GFXSetUpWindow window = (GFXSetUpWindow)GetWindow(typeof(GFXSetUpWindow));
        window.Show();
    }

    private void OnGUI()
    {
        // Dummy sprite generation
        GUILayout.Label("Dummy sprite generation", EditorStyles.boldLabel);
        dummyFolder = EditorGUILayout.TextField(new GUIContent(
            "Dummy folder",
            "Folder containing the dummy sprites, relative to \"" + GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory +
            "\". If not specified, defaults to \"" + GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory + "\"."
            ), dummyFolder);

        nameRenameFrom = EditorGUILayout.TextField(new GUIContent(
            "Rename from",
            "Rename this string for \"Rename to\" for all sprites in the given folder."
            ), nameRenameFrom);

        nameRenameTo = EditorGUILayout.TextField(new GUIContent(
            "Rename to",
            "Rename \"Rename from\" to this string for all sprites in the given folder."
            ), nameRenameTo);

        toRemove = EditorGUILayout.TextField(new GUIContent(
            "Files to remove",
            "Removes all files containing this string in the given folder."
            ), toRemove);

        if (GUILayout.Button("Rename dummy sprites"))
        {
            new DummySpriteGenerator().Rename(
                GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory + (dummyFolder == "" ? "" : "/" + dummyFolder),
                nameRenameFrom,
                nameRenameTo
                );
            Debug.Log("Renaming finished");
        }

        if (GUILayout.Button("Remove dummy sprites"))
        {
            new DummySpriteGenerator().Remove(
                GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory + (dummyFolder == "" ? "" : "/" + dummyFolder),
                toRemove
                );
            Debug.Log("Removing finished");
        }

        GUILayout.Space(20);


        // Sprite set up
        GUILayout.Label("Sprite Set-up", EditorStyles.boldLabel);
        GUILayout.Label("Sprite Specification", EditorStyles.miniBoldLabel);
        folderName = EditorGUILayout.TextField(new GUIContent(
            "Folder", 
            "Folder containing the sprites, relative to \"" + GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory +
            "\". If not specified, defaults to \"" + GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory + "\"."
            ), folderName);
        nameContains = EditorGUILayout.TextField(new GUIContent(
            "Name contains", 
            "Only modify sprites with this in their file name. If not specified, modifies all sprites in the given folder."
            ), nameContains);

        GUILayout.Label("Sprite Import Settings", EditorStyles.miniBoldLabel);
        settings.ppu = EditorGUILayout.IntField("Pixels Per Unit", settings.ppu);
        settings.pivot.type = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot", settings.pivot.type);
        if (settings.pivot.type == SpriteAlignment.Custom) settings.pivot.vector = EditorGUILayout.Vector2Field("", settings.pivot.vector);

        if (GUILayout.Button("Set up sprites"))
        {
            new SpriteSetUp(settings).SetSpriteImportSettings(
                GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory + (folderName == "" ? "" : "/" + folderName),
                nameContains
                );
            Debug.Log("Sprite set-up finished");
        }
        
        GUILayout.Space(20);

        // Animation clips generation
        GUILayout.Label("Animation creation", EditorStyles.boldLabel);
        characterName = EditorGUILayout.TextField("Character to generate", characterName);
        if (GUILayout.Button("Generate animation clips"))
        {
            int numClips = new AnimationGenerator().GenerateAnimations(characterName);

            Debug.Log("Number of animation clips generated = " + numClips);
        }
    }
}
