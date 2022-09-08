using System;
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
    
    internal enum HallType
    {
        HORIZONTAL, VERTICAL
    }
    
    internal struct Wall
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
    internal int gridStartX;
    internal int gridStartY;
    
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
    
    internal List<Wall> _walls;
    internal List<Wall> _cols;

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
    protected virtual Vector2Int UnityToScriptCoord(int x, int y)
    {
        return new Vector2Int(x + Math.Abs(gridStartX), y + Math.Abs(gridStartY));
    }

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

    internal void PutDownColliders(List<Wall> cols)
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
    
    internal void AddSectionedLine(List<Wall> line, Room room, Room cuttingRoom, int coord, HallType orientation)
    {
        if (orientation == HallType.VERTICAL)
        {
            AddWallToLst(line, orientation, room.StartY-1, 
                cuttingRoom.StartY, coord);
            AddWallToLst(line, orientation, cuttingRoom.StartY + cuttingRoom.Height, 
                room.StartY + room.Height+1, coord);
        }
        else
        {
            AddWallToLst(line, orientation, room.StartX-1, 
                cuttingRoom.StartX, coord);
            AddWallToLst(line, orientation, cuttingRoom.StartX + cuttingRoom.Width, 
                room.StartX + room.Width+1, coord);
        }
    }

    internal void AddAllSectionedLines(List<Wall> line, Room horRoom, Room verRoom)
    {
        AddSectionedLine(line, horRoom, verRoom, horRoom.StartY-1, HallType.HORIZONTAL);
        AddSectionedLine(line, horRoom, verRoom, horRoom.StartY + horRoom.Height, HallType.HORIZONTAL);
        
        AddSectionedLine(line, verRoom, horRoom, verRoom.StartX-1, HallType.VERTICAL);
        AddSectionedLine(line, verRoom, horRoom, verRoom.StartX + verRoom.Width, HallType.VERTICAL);
    }

    internal void AddParallelLines(List<Wall> line, Room room, HallType orientation)
    {
        if (orientation == HallType.HORIZONTAL)
        {
            AddWallToLst(line, orientation, room.StartX, room.StartX + room.Width,
                room.StartY + room.Height);
            AddWallToLst(line, orientation, room.StartX, room.StartX + room.Width,
                room.StartY - 1);
        }
        else
        {
            AddWallToLst(line, orientation, room.StartY - 1, room.StartY + room.Height + 1,
                room.StartX + room.Width);
            AddWallToLst(line, orientation, room.StartY - 1, room.StartY + room.Height + 1,
                room.StartX - 1);
        }
    }

    internal void AddWallToLst(List<Wall> walls, HallType orientation, int start, int end, int height)
    {
        Wall newWall = new Wall();
        newWall.SetValues(orientation, start, end, height);
        walls.Add(newWall);
    }

    internal abstract void GenerateWalls();
    
    internal void GenerateDoors()
    {
        Random rand = new Random();
        
        // generate entrance position
        _entrance.EntrancePos = new Vector3Int(rand.Next(_walls[0].Start + 1, _walls[0].End - 1), _walls[0].Height, 0);
        _entrance.ExitPos = new Vector3Int(_walls[_walls.Count - 1].Height, rand.Next(_walls[_walls.Count - 1].Start + 1, _walls[_walls.Count - 1].End - 1), 0);

        // generate entrance triggers
        _entrance.EntranceObj = Instantiate(entranceCollider, _roomHolder);
        _entrance.ExitObj = GetExitObject();

        _entrance.EntranceObj.transform.position = GetGridTileWorldCoordinates(_entrance.EntrancePos.x, _entrance.EntrancePos.y);
        _entrance.ExitObj.transform.position = GetGridTileWorldCoordinates(_entrance.ExitPos.x, _entrance.ExitPos.y);
        
        
        // erase wall tiles
        var horWalls = _gridHolder.transform.GetChild(1).GetComponent<Tilemap>();
        var verWalls = _gridHolder.transform.GetChild(2).GetComponent<Tilemap>();
        horWalls.SetTile(_entrance.EntrancePos, null);
        verWalls.SetTile(_entrance.ExitPos, null);
        
        // erase collider and replace it with new collider
        var colMap = _gridHolder.transform.GetChild(4).GetComponent<Tilemap>();
        colMap.SetTile(_entrance.EntrancePos, null);
        colMap.SetTile(_entrance.ExitPos, null);
        List<Wall> newCols = new List<Wall>();
        AddWallToLst(newCols, HallType.HORIZONTAL, _entrance.EntrancePos.x - 1, _entrance.EntrancePos.x + 2, _entrance.EntrancePos.y + 1);
        AddWallToLst(newCols, HallType.VERTICAL, _entrance.ExitPos.y - 1, _entrance.ExitPos.y + 2, _entrance.ExitPos.x + 1);
        PutDownColliders(newCols);
        // TODO: collider for entrance and exit

        //set down tiles
        var tileMap = _gridHolder.transform.GetChild(0).GetComponent<Tilemap>();
        tileMap.SetTile(_entrance.EntrancePos, tileMap.GetTile(new Vector3Int(_entrance.EntrancePos.x, _entrance.EntrancePos.y-1, _entrance.EntrancePos.z)));
        tileMap.SetTile(_entrance.ExitPos, tileMap.GetTile(new Vector3Int(_entrance.ExitPos.x-1, _entrance.ExitPos.y, _entrance.ExitPos.z)));
    }
    
    internal virtual void SetRoom(ref Room room, Random rand)
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
    
    internal void FillObstaclesGrid(int x, int y, int w, int h)
    {
        for (var j = 0; j < h; j++)
        {
            for (var i = 0; i < w; i++)
            {
                Vector2Int gridCoors = UnityToScriptCoord(x + i, y + j);
                _grid[gridCoors.y, gridCoors.x].Empty = false;
                //Debug.Log("Tile at " + (x+i) + "x" + (y+j) + " has been obstructed for AI.");
            }
        }
    }

    internal void PutDownObjects(int xCoord, int yCoord, int w, int h, List<GameObject> objects, int objDensity)
    {
        Random rnd = new Random();
        float offsetMax = _obstaclesHolder.GetComponent<Grid>().cellSize.x;
        
        int den = objDensity;
        for (var y = yCoord; y < yCoord + h; y++)
        {
            for (var x = xCoord; x < xCoord + w; x++)
            {
                if (((x == _entrance.EntrancePos.x*2 || x - 1 == _entrance.EntrancePos.x*2) && y == _entrance.EntrancePos.y*2 - 1) ||
                    (x == _entrance.ExitPos.x*2 - 1 && (y == _entrance.ExitPos.y*2 || y - 1 == _entrance.ExitPos.y*2)))
                    continue; // dont block doors
                if (rnd.Next(0, 100) <= den)
                {
                    int objIdx = GetNumFromRange(rnd, 0, objects.Count - 1);
                    InstantiateObjectInWorld(objects[objIdx], new Vector3Int(x, y, 0), 
                        GetNumFromRangeFloat(rnd, -(int)((offsetMax*100)/3), 
                            (int)((offsetMax*100)/3)) /100,
                        GetNumFromRangeFloat(rnd, -(int)((offsetMax*100)/3), 
                            (int)((offsetMax*100)/3)) /100);
                    den = den / 4;
                } else if (den < objDensity) den+=(objDensity-den)/4;
                else if (x % 4 == 0) den++; // to try and avoid completely empty rooms
            }
        }
    }
    
    internal GameObject InstantiateObjectInWorld(GameObject obj, Vector3Int coords, float offsetX = 0.0f, float offsetY = 0.0f)
    {
        Vector2Int gridCoords = UnityToScriptCoord(coords.x, coords.y);
        if (!_grid[gridCoords.y, gridCoords.x].Empty) return null;
        
        _grid[gridCoords.y, gridCoords.x].Empty = false;

        Vector3 pos = _obstaclesHolder.GetComponent<Tilemap>()
            .GetCellCenterWorld(coords);
        
        pos.x += offsetX;
        pos.y += offsetY;
        
        return Instantiate(obj, pos, Quaternion.identity, _obstaclesHolder);
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
    
    internal int GetNumFromRange(Random rnd, int start, int end)
    {
        if (start == end) return start;
        int bracket = 100 / (end - start);
        int genNum = rnd.Next(0, 100);
        return (int)Math.Ceiling((double)(genNum/bracket));
    }
    
    private float GetNumFromRangeFloat(Random rnd, int start, int end)
    {
        if (start == end) return start;
        int bracket = 100 / (end - start);
        float genNum = rnd.Next(start, end);
        return genNum / bracket;
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
