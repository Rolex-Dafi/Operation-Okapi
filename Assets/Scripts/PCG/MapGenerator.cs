using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = System.Random;

/// <summary>
/// Base class for map generation
/// </summary>
public abstract class MapGenerator : MonoBehaviour
{
    protected Transform _roomHolder;
    protected Transform _gridHolder;
    protected Transform _obstaclesHolder;

    [Header("Collider objects")]
    public Tile collTile;
    public GameObject entranceCollider;

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
    };
    public struct GridTile
    {
        internal int XCoord;
        internal int YCoord;
        internal bool Empty;

        public GridTile(int x, int y, bool empty = true)
        {
            XCoord = x;
            YCoord = y;
            this.Empty = empty;
        }
    };
    
    internal GridTile[,] _grid;
    
    internal struct Entrance
    {
        internal Vector3Int EntrancePos;
        internal GameObject EntranceObj;
        internal Vector3Int ExitPos;
        internal GameObject ExitObj;
    }
    
    internal Entrance _entrance;

    [Header("Empty prefab of grid setup.")] public GameObject mapPrefab;
    
    public virtual void Generate()
    {
        Restart();
        SetUp();
    }

    internal abstract void GenerateGrid();
    protected abstract Vector2Int UnityToScriptCoord(int x, int y);

    internal Vector2Int ScriptToUnityCoord(int x, int y)
    {
        return new Vector2Int(_grid[y, x].XCoord, _grid[y, x].YCoord);
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
    
    public bool IsTileEmpty(int x, int y)
    {
        Vector2Int coords = UnityToScriptCoord(x, y);

        return _grid[coords.y, coords.x].Empty;
    }
    
    public GridTile[,] GetGrid()
    {
        return _grid;
    }

    public Vector3 GetSmallGridTileWorldCoordinates(int x, int y)
    {
        return _obstaclesHolder.GetComponent<Tilemap>()
            .GetCellCenterWorld(new Vector3Int(x, y, 0));
    }

    public Vector3 GetGridTileWorldCoordinates(int x, int y)
    {
        return _gridHolder.GetComponentInChildren<Tilemap>()
            .GetCellCenterWorld(new Vector3Int(x, y, 0));
    }

    internal void CleanUp()
    {
        Destroy(_obstaclesHolder.GetComponent<Tilemap>());
        Destroy(_obstaclesHolder.GetComponent<Grid>());
    }
    
    /// <summary>
    /// Flips a coin.
    /// </summary>
    /// <param name="rnd">Random generator required for the coin flip.</param>
    /// <returns>True if heads, false if tails.</returns>
    internal static bool Heads(Random rnd)
    {
        return rnd.Next() % 2 == 0;
    }

    public Vector3Int GetEntranceGridCoords()
    {
        return _entrance.EntrancePos;
    }
    public Vector3Int GetExitGridCoords()
    {
        return _entrance.ExitPos;
    }

    public Transform GetEntranceCollider()
    {
        return _entrance.EntranceObj.transform;
    }

    public Vector3 GetEntranceMiddlePoint()
    {
        return GetGridTileWorldCoordinates(_entrance.EntrancePos.x, _entrance.EntrancePos.y);
    }
    
    public Transform GetExitCollider()
    {
        return _entrance.ExitObj.transform;
    }
    
    public Vector3 GetExitMiddlePoint()
    {
        return GetGridTileWorldCoordinates(_entrance.ExitPos.x, _entrance.ExitPos.y);
    }
}
