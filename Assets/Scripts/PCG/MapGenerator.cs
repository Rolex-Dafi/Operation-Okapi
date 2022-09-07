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

    [Header("Maximum width and height of the generated rooms.")]
    public int maxWidth;
    public int maxHeight;
    
    internal int _minWidth;
    internal int _minHeight;
    
    internal int _roomMin;
    internal int _hallTreshold;
    
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
    
    internal enum RoomType
    {
        HOR_HALL, VER_HALL, ROOM, BIG_ROOM, LONG_HALL_VER, LONG_HALL_HOR
    }
    
    internal struct Room
    {
        internal int Width;
        internal int Height;
        internal int StartX;
        internal int StartY;
        internal RoomType Type;
    };
    
    protected List<Wall> _walls;
    protected List<Wall> _cols;

    [Header("Empty prefab of grid setup.")] public GameObject mapPrefab;
    
    /// <summary>
    /// Generate a room for the game.
    /// </summary>
    public virtual void Generate()
    {
        Restart();
        SetUp();
    }

    protected abstract void SetUpParameters();

    internal abstract void GenerateGrid();
    protected abstract Vector2Int UnityToScriptCoord(int x, int y);

    internal Vector2Int ScriptToUnityCoord(int x, int y)
    {
        return new Vector2Int(_grid[y, x].XCoord, _grid[y, x].YCoord);
    }
    
    private void SetUp()
    {
        Restart();
        
        SetUpParameters();
        
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
    
    internal void SetRoom(ref Room room, Random rand)
    {
        room = new Room();
        
        room.Height = rand.Next(_minHeight, maxHeight);
        room.Width = rand.Next(_minWidth, maxWidth);
        var diff = room.Height - room.Width;
        
        if (diff < 0 && room.Height < _hallTreshold) {
            room.Type = RoomType.HOR_HALL;
            room.Width += _hallTreshold;
        }
        else if (diff > 0 && room.Width < _hallTreshold)
        {
            room.Type = RoomType.VER_HALL;
            room.Height += _hallTreshold;
        }
        else
        {
            room.Type = RoomType.ROOM;
            if (room.Height < _roomMin) room.Height = rand.Next(_roomMin, maxHeight);
            if (room.Width < _roomMin) room.Width = rand.Next(_roomMin, maxWidth);
        }
    }
    
    public bool IsTileEmpty(int x, int y)
    {
        Vector2Int coords = UnityToScriptCoord(x, y);

        return _grid[coords.y, coords.x].Empty;
    }
    
    /// <summary>
    /// Get the grid used for obstacles.
    /// </summary>
    /// <returns>GridTile[,] grid</returns>
    public GridTile[,] GetGrid()
    {
        return _grid;
    }
    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Converts tile at x y coordinates in the smaller tilemap to world coordinates.
    /// </summary>
    /// <param name="x">Coordinate x of the tile in the tilemap.</param>
    /// <param name="y">Coordinate y of the tile in the tilemap.</param>
    /// <returns>Vector3 world coordinates of the tile.</returns>
    public Vector3 GetSmallGridTileWorldCoordinates(int x, int y)
    {
        return _obstaclesHolder.GetComponent<Tilemap>()
            .GetCellCenterWorld(new Vector3Int(x, y, 0));
    }

    /// <summary>
    /// Converts tile at x y coordinates in the tilemap to world coordinates.
    /// </summary>
    /// <param name="x">Coordinate x of the tile in the tilemap.</param>
    /// <param name="y">Coordinate y of the tile in the tilemap.</param>
    /// <returns>Vector3 world coordinates of the tile.</returns>
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

    /// <summary>
    /// Converts tile at x y coordinates to the middle point of the tile
    /// in world coordinates.
    /// </summary>
    /// <param name="x">Coordinate x of the tile in the tilemap.</param>
    /// <param name="y">Coordinate y of the tile in the tilemap.</param>
    /// <returns>Vector3 world coordinates of the middle point of the tile.</returns>
    public Vector3 GetGridTileWorldCoordinatesMiddle(int x, int y)
    {
        Vector3 vec = GetGridTileWorldCoordinates(x, y);
        vec.y -= _gridHolder.GetComponentInChildren<Grid>().cellSize.y / 2;
        return vec;
    }
    
    public Vector3 GetEntranceMiddlePoint()
    {
        return GetGridTileWorldCoordinatesMiddle(_entrance.EntrancePos.x, _entrance.EntrancePos.y);
    }
    
    public Transform GetExitCollider()
    {
        return _entrance.ExitObj.transform;
    }
    
    public Vector3 GetExitMiddlePoint()
    {
        return GetGridTileWorldCoordinatesMiddle(_entrance.ExitPos.x, _entrance.ExitPos.y);
    }

    protected GameObject GetExitObject()
    {
        var exitObj = Instantiate(entranceCollider, _roomHolder);
        exitObj.AddComponent<Interactable>();
        return exitObj;
    }

    public Interactable GetExitTrigger()
    {
        return _entrance.ExitObj.GetComponent<Interactable>();
    }

    public void DestroyCurrentRoom()
    {
        Restart();
    }
}
