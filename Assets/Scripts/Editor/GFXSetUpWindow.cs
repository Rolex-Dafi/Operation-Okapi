using UnityEditor;
using UnityEngine;

public class GFXSetUpWindow : EditorWindow
{
    // Sprite organization
    private bool showSpriteOrganization = false;
    private string nameRenameFrom;
    private string nameRenameTo;
    private string toRemove;

    // Sprite set up
    private string folderName = "";
    private string nameContains = "";
    private SpriteImportSettings settings = GFXUtility.defaultSpriteImportSettings;

    // Animations creation
    private string characterName = "";
    private EAbility animationType = EAbility.NDEF;
    private AnimationClipProperties animationClipProperties = GFXUtility.defaultAnimationClipProperties;

    [MenuItem("Custom/GFX Set-up")]
    static void Init()
    {
        GFXSetUpWindow window = (GFXSetUpWindow)GetWindow(typeof(GFXSetUpWindow));
        window.Show();
    }

    private void OnGUI()
    {
        #region Sprite organization
        GUILayout.Label("Sprite organization", EditorStyles.boldLabel);
        showSpriteOrganization = EditorGUILayout.Foldout(showSpriteOrganization, "Show/Hide");
        if (showSpriteOrganization)
        {
            folderName = EditorGUILayout.TextField(new GUIContent(
                "Directory",
                "Directory containing the sprites, relative to \"" + GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory +
                "\". If not specified, defaults to \"" + GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory + "\"."
                ), folderName);

            GUILayout.Label("Sprite Renaming", EditorStyles.miniBoldLabel);
            nameRenameFrom = EditorGUILayout.TextField(new GUIContent(
                "Rename from",
                "Rename this string for \"Rename to\" for all sprites in the given directory."
                ), nameRenameFrom);

            nameRenameTo = EditorGUILayout.TextField(new GUIContent(
                "Rename to",
                "Rename \"Rename from\" to this string for all sprites in the given directory."
                ), nameRenameTo);

            if (GUILayout.Button("Rename sprites"))
            {
                new SpriteOrganizer().Rename(
                    GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory + (folderName == "" ? "" : "/" + folderName),
                    nameRenameFrom,
                    nameRenameTo
                    );
                Debug.Log("Renaming finished");
            }

            GUILayout.Label("Sprite DELETION", EditorStyles.miniBoldLabel);

            toRemove = EditorGUILayout.TextField(new GUIContent(
                "Files to delete",
                "DELETES all files containing this string in their file name in the given directory."
                ), toRemove);

            if (GUILayout.Button("DELETE sprites"))
            {
                ConfirmPopup popup = new ConfirmPopup();
                PopupWindow.Show(new Rect(), popup);
                if (popup.Confirmed)
                {
                    new SpriteOrganizer().Delete(
                        GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory + (folderName == "" ? "" : "/" + folderName),
                        toRemove
                        );
                    Debug.Log("DELETING finished");
                }
            }
        }
        #endregion Sprite organization

        GUILayout.Space(20);

        #region Sprite set up
        GUILayout.Label("Sprite Set-up", EditorStyles.boldLabel);
        GUILayout.Label("Sprite Specification", EditorStyles.miniBoldLabel);
        folderName = EditorGUILayout.TextField(new GUIContent(
            "Directory",
            "Directory containing the sprites, relative to \"" + GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory +
            "\". If not specified, defaults to \"" + GFXUtility.resourcesDirectory + "/" + GFXUtility.spritesDirectory + "\"."
            ), folderName);
        nameContains = EditorGUILayout.TextField(new GUIContent(
            "Name contains",
            "Only modify sprites with this in their file name. If not specified, modifies all sprites in the given directory."
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
        #endregion Sprite set up

        GUILayout.Space(20);

        #region Animation clip generation
        GUILayout.Label("Animation clip generation", EditorStyles.boldLabel);
        characterName = EditorGUILayout.TextField(new GUIContent(
            "Character directory",
            "Looks for this character's directory in \"Assets/Resources/Sprites/Characters\"."
            ), characterName);
        GUILayout.Label("Animation clip settings", EditorStyles.miniBoldLabel);
        animationType = (EAbility)EditorGUILayout.EnumPopup(new GUIContent(
            "Animation type",
            "If not defined generates all possible animation clips given the contents of the character's directory."
            ), animationType);
        animationClipProperties.frameRate = EditorGUILayout.FloatField("Frame rate", animationClipProperties.frameRate);
        animationClipProperties.loop = EditorGUILayout.Toggle("Loop", animationClipProperties.loop);
        animationClipProperties.spriteColor = EditorGUILayout.ColorField("Sprite tint", animationClipProperties.spriteColor);
        animationClipProperties.duplicateSingleFrame = EditorGUILayout.Toggle(new GUIContent(
            "Duplicate single",
            "If there is only a single sprite for the given clip duplicate it into two frames?"
            ), animationClipProperties.duplicateSingleFrame);

        if (GUILayout.Button("Generate animation clips"))
        {
            int numClips = 0;
            if (animationType == EAbility.NDEF) numClips = new AnimationClipGenerator(characterName).GenerateAllAnimations();
            else numClips = new AnimationClipGenerator(characterName, animationClipProperties).GenerateAnimations(animationType);

            Debug.Log("Number of animation clips generated = " + numClips);
        }
        #endregion Animation clip generation
    }
}

public class ConfirmPopup : PopupWindowContent
{
    private bool confirmed;
    private string message;

    public bool Confirmed { get => confirmed; set => confirmed = value; }

    public ConfirmPopup(string message = "Are you sure you want to proceed?")
    {
        confirmed = false;
        this.message = message;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(250, 150);
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Label(message, EditorStyles.boldLabel); 
        
        if (GUILayout.Button("Yes"))
        {
            confirmed = true;
        }
        if (GUILayout.Button("Cancel"))
        {
            confirmed = false;
        }
    }

    public override void OnOpen()
    {
    }

    public override void OnClose()
    {
    }
}
