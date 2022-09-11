using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An editor window for setting up the in-game items.
/// </summary>
public class ItemSetUpWindow : EditorWindow
{
    private string defaultItemFolder = "Sprites/Items/";
    private string uiFolder = "UI";
    private string worldFolder = "World";

    private string itemSOFolder = "Assets/ScriptableObjects/Items/";

    [MenuItem("Custom/Item Set-up")]
    static void Init()
    {
        ItemSetUpWindow window = (ItemSetUpWindow)GetWindow(typeof(ItemSetUpWindow));
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Get Items"))
        {
            UpdateItems();
            Debug.Log("Items updated.");
        }
    }

    private void UpdateItems()
    {
        var uiSprites = Resources.LoadAll<Sprite>(defaultItemFolder + uiFolder);
        if (uiSprites.Length == 0)
        {
            Debug.LogError("No sprites at " + defaultItemFolder + uiFolder);
        }
        var worldSprites = Resources.LoadAll<Sprite>(defaultItemFolder + worldFolder);
        if (worldSprites.Length == 0)
        {
            Debug.LogError("No sprites at " + defaultItemFolder + worldFolder);
        }

        var i = 0;
        foreach (var sprite in uiSprites)
        {
            Debug.Log("processing item " + sprite.name);
            var world = worldSprites.ToList().Find(x => x.name.Contains(sprite.name));
            if (world == null)
            {
                Debug.LogWarning("Couldnt find world sprite for item " + sprite.name);
                continue;
            }
            Debug.Log("adding item " + sprite.name);
            AddItem(i, sprite, world);
            ++i;
        }

        AssetDatabase.Refresh();
    }

    private void AddItem(int id, Sprite ui, Sprite world)
    {
        ItemSO newItem = CreateInstance<ItemSO>();
        newItem.UISprite = ui;
        newItem.WorldSprite = world;

        newItem.ID = id;
        newItem.ItemName = ui.name;

        if (!Directory.Exists(itemSOFolder))
        {
            Directory.CreateDirectory(itemSOFolder);
        }

        AssetDatabase.CreateAsset(newItem, itemSOFolder + ui.name + "Data.asset");
        AssetDatabase.SaveAssets();
    }
}
