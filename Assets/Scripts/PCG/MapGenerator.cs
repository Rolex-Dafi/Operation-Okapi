using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for map generation
/// </summary>
public class MapGenerator : MonoBehaviour
{
    private Transform roomHolder;
    private Transform gridHolder;
    private Transform obstaclesHolder;

    public void Generate()
    {
        SetUp();
    }

    private void SetUp()
    {
        roomHolder = new GameObject("Room").transform;
        gridHolder = new GameObject("Grid").transform;
        obstaclesHolder = new GameObject("Obstacles").transform;

        gridHolder.parent = roomHolder;
        obstaclesHolder.parent = roomHolder;

        Grid g = gridHolder.gameObject.AddComponent(typeof(Grid)) as Grid;
        g.cellSize = new Vector3(1.0f, 0.5f, 1);
        g.cellLayout = GridLayout.CellLayout.IsometricZAsY;

        obstaclesHolder.gameObject.layer = 6;
    }
}
