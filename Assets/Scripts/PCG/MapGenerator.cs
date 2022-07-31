using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Base class for map generation
/// </summary>
public class MapGenerator : MonoBehaviour
{
    protected Transform _roomHolder;
    protected Transform _gridHolder;
    protected Transform _obstaclesHolder;

    [Header("Empty prefab of grid setup.")]
    public GameObject mapPrefab;

    public virtual void Generate()
    {
        Restart();
        SetUp();
    }

    private void SetUp()
    {
        Restart();
        
        _roomHolder = Instantiate(mapPrefab).transform;
        _gridHolder = _roomHolder.GetChild(0);
        _obstaclesHolder = _roomHolder.GetChild(1);
    }

    private void Restart()
    {
        if (_roomHolder == null) return;
        
        Destroy(_roomHolder.gameObject);
        _roomHolder = null;
        _gridHolder = null;
        _obstaclesHolder = null;
    }
}
