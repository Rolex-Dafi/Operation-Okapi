using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public Tile collTile;

    protected enum HallType
    {
        HORIZONTAL, VERTICAL
    }
    
    protected struct Wall
    {
        internal HallType Orientation;
        internal int Start;
        internal int End;
        internal int Height;

        internal void SetValues(HallType b, int s, int e, int h)
        {
            Orientation = b;
            Start = s;
            End = e;
            Height = h;
        }
    }

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

    protected void SetTilesToMap(Tile tile, Tilemap tileMap, int startX, int startY, int finX, int finY)
    {
        for (int x = startX; x < finX; x++)
        {
            for (int y = startY; y < finY; y++)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    protected void PutDownColliders(List<Wall> cols)
    {
        var tm = _gridHolder.transform.GetChild(4).GetComponent<Tilemap>();

        foreach (var col in  cols)
        {
            if (col.Orientation == HallType.HORIZONTAL)
                SetTilesToMap(collTile, tm, col.Start, col.Height, col.End, col.Height + 1);
            else
                SetTilesToMap(collTile, tm, col.Height, col.Start, col.Height + 1, col.End);
        }
    }
}
