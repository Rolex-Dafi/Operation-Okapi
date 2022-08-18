using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class OfficeRoomGenerator : MapGenerator
{
    [Header("Maximum width and height of the generated rooms.")]
    public int maxWidth = 25;
    public int maxHeight = 25;

    [Header("Objects")] public GameObject test;

    private int _minWidth = 4;
    private int _minHeight = 4;

    private struct Room
    {
        internal int Width;
        internal int Height;
        internal int StartX;
        internal int StartY;
        internal RoomType Type;
    };

    private struct GridTile
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

    private Room _leftRoom;
    private Room _rightRoom;
    private int _hallTreshold = 5;
    private int _roomMin = 8;
    private int _bigRoomTreshold = 4;

    private GridTile[,] _grid;

    private enum RoomType
    {
        HOR_HALL, VER_HALL, ROOM, BIG_ROOM, LONG_HALL_VER, LONG_HALL_HOR
    }

    private List<Wall> _walls;
    private List<Wall> _cols;

    [Header("Tile sets to use for generation.")]
    public List<Tile> tileLst;

    public List<Tile> horWallLst;
    public List<Tile> verWallLst;
    
    public override void Generate()
    {
        base.Generate();
        GenerateOfficeFloor();
        GenerateOfficeWalls();
        GenerateColliders();

        GenerateGrid();
        GenerateObjects();
    }

    private void GenerateOfficeFloor()
    {
        if (tileLst.Count <= 0) return;
        
        var rand = new Random();
        var tm = _gridHolder.transform.GetChild(0).GetComponent<Tilemap>();

        // Set up the room
        SetRoom(ref _leftRoom, rand);
        SetRoom(ref _rightRoom, rand);

        _rightRoom.StartX = 0;
        _leftRoom.StartX = 0 - _leftRoom.Width;
        _leftRoom.StartY = 0;

        // handle special case rooms
        if (Math.Abs(_leftRoom.Height - _rightRoom.Height) < _bigRoomTreshold && _leftRoom.Type == RoomType.ROOM && _rightRoom.Type == RoomType.ROOM) // special case - big room
        {
            Debug.Log("Big room.");
            _leftRoom.Type = _rightRoom.Type = RoomType.BIG_ROOM;
            _leftRoom.Height = _rightRoom.Height = (_leftRoom.Height + _rightRoom.Height) / 2;

            _leftRoom.StartY = _rightRoom.StartY = 0 - _leftRoom.Height / 2;
        }
        else if (_leftRoom.Type == RoomType.HOR_HALL && _rightRoom.Type == RoomType.HOR_HALL) // special case - long horizontal hall
        {
            Debug.Log("Long horizontal hall.");
            _leftRoom.Type = _rightRoom.Type = RoomType.LONG_HALL_HOR;
            _leftRoom.Height = _rightRoom.Height = (_leftRoom.Height + _rightRoom.Height) / 2;
            
            _leftRoom.StartY = _rightRoom.StartY = 0 - _leftRoom.Height / 2;
        }
        else if (_leftRoom.Type == RoomType.VER_HALL && _rightRoom.Type == RoomType.VER_HALL) // special case - long vertical hall
        {
            Debug.Log("Long vertical hall.");
            _leftRoom.Type = _rightRoom.Type = RoomType.LONG_HALL_VER;
            _leftRoom.Width = _rightRoom.Width = (_leftRoom.Width + _rightRoom.Width) / 2;

            _leftRoom.StartX = _rightRoom.StartX = 0 - _leftRoom.Width / 2;
            _leftRoom.StartY = 0 - _leftRoom.Height;
            _rightRoom.StartY = 0;
        }
        else if (_leftRoom.Type == RoomType.HOR_HALL)
        {
            _rightRoom.StartY = 0 - _rightRoom.Height + _leftRoom.Height;
            Debug.Log("_| shape hall");
        } else if (_leftRoom.Type == RoomType.VER_HALL || _rightRoom.Type == RoomType.VER_HALL)
        {
            _rightRoom.StartY = 0 - _rightRoom.Height + _minHeight;
        }
        else // handle all other rooms
        {
            Debug.Log("Normal room.");
            // move right room for good wall rendering
            HandleRoomsStartY(ref _leftRoom, ref _rightRoom, rand);
        }
        
        // Set tiles
        PutDownRoomTiles(tileLst[rand.Next(0, tileLst.Count)], tm, _leftRoom);
        PutDownRoomTiles(tileLst[rand.Next(0, tileLst.Count)], tm, _rightRoom);
        
        Debug.Log("Left room starts at " + _leftRoom.StartX + "x" + _leftRoom.StartY +
                  " and is " + _leftRoom.Width + "x" + _leftRoom.Height + " big.");
        Debug.Log("Right room starts at " + _rightRoom.StartX + "x" + _rightRoom.StartY +
                  " and is " + _rightRoom.Width + "x" + _rightRoom.Height + " big.");
    }

    private void SetRoom(ref Room room, Random rand)
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

    private void HandleRoomsStartY(ref Room fixedRoom, ref Room movedRoom, Random rand)
    {
        movedRoom.StartY = rand.Next(fixedRoom.StartY - movedRoom.Height + _minHeight, 
            Math.Min(0, fixedRoom.StartY + fixedRoom.Height - movedRoom.Height));
    }

    private void GenerateOfficeWalls()
    {
        _walls = new List<Wall>();
        switch (_leftRoom.Type)
        {
            /*case RoomType.LONG_HALL_HOR:
                // long horizontal hall, add two walls
                AddWallToLst(true, _leftRoom.StartX, _rightRoom.StartX + _rightRoom.Width, 
                    _leftRoom.StartY + _leftRoom.Height);
                AddWallToLst(false, _leftRoom.StartY, _leftRoom.StartY + _leftRoom.Height, 
                    _rightRoom.StartX + _rightRoom.Width);
                break;*/
            case RoomType.LONG_HALL_VER:
                // long vertical wall
                AddWallToLst(_walls, true, _leftRoom.StartX, _leftRoom.StartX + _leftRoom.Width, 
                    _rightRoom.Height);
                AddWallToLst(_walls, false, _leftRoom.StartY, _rightRoom.Height, 
                    _rightRoom.StartX + _rightRoom.Width );
                break;
            /*case RoomType.BIG_ROOM:
                // big room
                AddWallToLst(true, _leftRoom.StartX, _rightRoom.StartX + _rightRoom.Width, 
                    _leftRoom.StartY + _leftRoom.Height);
                AddWallToLst(false, _leftRoom.StartY, _leftRoom.StartY + _leftRoom.Height ,
                    _rightRoom.StartX + _rightRoom.Width);
                break;*/
            default:
                // handle all other cases
                if (_leftRoom.StartY + _leftRoom.Height == _rightRoom.StartY + _rightRoom.Height)
                {
                    // only two walls
                    AddWallToLst(_walls, true, _leftRoom.StartX, _rightRoom.Width, 
                        _leftRoom.StartY + _leftRoom.Height);
                    AddWallToLst(_walls, false, _rightRoom.StartY, _rightRoom.StartY + _rightRoom.Height,
                        _rightRoom.StartX + _rightRoom.Width);
                }
                else
                {
                    // four walls
                    AddWallToLst(_walls, true, _leftRoom.StartX, _leftRoom.StartX + _leftRoom.Width,
                        _leftRoom.Height);
                    AddWallToLst(_walls, false, _rightRoom.StartY + _rightRoom.Height, _leftRoom.StartY + _leftRoom.Height,
                        _leftRoom.StartX + _leftRoom.Width);
                    AddWallToLst(_walls, true, _rightRoom.StartX, _rightRoom.StartX + _rightRoom.Width,
                        _rightRoom.StartY + _rightRoom.Height);
                    AddWallToLst(_walls, false, _rightRoom.StartY, _rightRoom.StartY + _rightRoom.Height,
                        _rightRoom.StartX + _rightRoom.Width);
                }
                break;
        }

        // finally set office walls to map
        PutDownWallTiles();
    }

    private void PutDownWallTiles()
    {
        if (horWallLst.Count <= 0 || verWallLst.Count <= 0 || _walls.Count <= 1)
        {
            Debug.Log("No walls to build.");
            return;
        }

        var rand = new Random();
        Tilemap horWalls = _gridHolder.transform.GetChild(1).GetComponent<Tilemap>();
        Tilemap verWalls = _gridHolder.transform.GetChild(2).GetComponent<Tilemap>();

        int idx = 0;
        
        foreach (var wall in _walls)
        {
            if (wall.End - wall.Start < 2)
            {
                if(wall.Horizontal)
                    horWalls.SetTile(new Vector3Int(wall.Start, wall.Height, 0), horWallLst[3]);
                else
                    verWalls.SetTile(new Vector3Int(wall.Height, wall.Start, 0), verWallLst[3]);
            }
            else
            {
                if (wall.Horizontal)
                {
                    SetTilesToMap(horWallLst[0], horWalls, wall.Start, wall.Height, wall.End, wall.Height + 1);
                    horWalls.SetTile(new Vector3Int(wall.End - 1, wall.Height, 0), horWallLst[1]);
                }
                else
                {
                    SetTilesToMap(verWallLst[0], verWalls, wall.Height, wall.Start, wall.Height + 1, wall.End);
                    verWalls.SetTile(new Vector3Int(wall.Height, wall.End - 1, 0), verWallLst[1]);
                }

                switch (idx)
                {
                    case 1 when _walls.Count > 2:
                        verWalls.SetTile(new Vector3Int(wall.Height, wall.Start, 0),
                            wall.End == wall.Start ? verWallLst[3] : verWallLst[2]);
                        break;
                    case 2:
                        horWalls.SetTile(new Vector3Int(wall.Start, wall.Height, 0), horWallLst[2]);
                        break;
                }
            }
            idx++;
        }
        
        
    }

    private void AddWallToLst(List<Wall> walls, bool horizontal, int start, int end, int height)
    {
        Wall newWall = new Wall();
        newWall.setValues(horizontal, start, end, height);
        walls.Add(newWall);
    }

    private void PutDownRoomTiles(Tile tile, Tilemap tm, Room room)
    {
        SetTilesToMap(tile, tm, room.StartX, room.StartY, 
            room.StartX + room.Width, room.StartY + room.Height);
    }

    private void GenerateColliders()
    {
        _cols = new List<Wall>();
        
        if (_leftRoom.Type == RoomType.LONG_HALL_HOR)
        {
            // special case
            AddWallToLst(_cols, true, _leftRoom.StartY-1, _rightRoom.StartY + _rightRoom.Height + 1, _leftRoom.StartX - 1);
            AddWallToLst(_cols, true, _leftRoom.StartY-1, _rightRoom.StartY + _rightRoom.Height + 1, _leftRoom.StartX + _leftRoom.Width);
            
            AddWallToLst(_cols, false, _leftRoom.StartX - 1, _leftRoom.StartX + _leftRoom.Width + 1, _leftRoom.StartY - 1);
            AddWallToLst(_cols, false, _leftRoom.StartX - 1, _leftRoom.StartX + _leftRoom.Width + 1, _rightRoom.StartY + _rightRoom.Height);
        }
        else
        {
            // first add collider walls, that are always present fully
            AddWallToLst(_cols, false, _leftRoom.StartY - 1, _leftRoom.StartY + _leftRoom.Height + 1, _leftRoom.StartX - 1);
            AddWallToLst(_cols, true, _leftRoom.StartX - 1, _leftRoom.StartX + _leftRoom.Width + 1, _leftRoom.StartY + _leftRoom.Height);
            
            AddWallToLst(_cols, false, _rightRoom.StartY - 1, _rightRoom.StartY + _rightRoom.Height + 1, _rightRoom.StartX + _rightRoom.Width);
            AddWallToLst(_cols, true, _rightRoom.StartX - 1, _rightRoom.StartX + _rightRoom.Width + 1, _rightRoom.StartY - 1);
            
            // now walls, that dont extend on inner side
            AddWallToLst(_cols, true, _leftRoom.StartX - 1, _leftRoom.StartX + _leftRoom.Width, _leftRoom.StartY - 1);
            AddWallToLst(_cols, true, _rightRoom.StartX, _rightRoom.StartX + _rightRoom.Width + 1, _rightRoom.StartY + _rightRoom.Height);
            
            // now walls, that may not be present at all
            if (_leftRoom.StartY + _leftRoom.Height > _rightRoom.StartY + _rightRoom.Height)
                AddWallToLst(_cols, false, _rightRoom.StartY + _rightRoom.Height + 1, _leftRoom.StartY + _leftRoom.Height + 1, _rightRoom.StartX);
            if (_leftRoom.StartY > _rightRoom.StartY)
                AddWallToLst(_cols, false, _rightRoom.StartY-1, _leftRoom.StartY-1, _rightRoom.StartX - 1);
        }
        PutDownColliders(_cols);
    }

    private void GenerateGrid()
    {
        int gridStartX = _leftRoom.StartX*2;
        int gridStartY = _rightRoom.StartY*2;

        int gridWidth = _leftRoom.Type == RoomType.LONG_HALL_VER ? _leftRoom.Width*2 : (_leftRoom.Width + _rightRoom.Width) * 2;
        int gridHeight = (_leftRoom.Type == RoomType.BIG_ROOM || _leftRoom.Type == RoomType.LONG_HALL_HOR) ? _leftRoom.Height * 2 : ((_leftRoom.StartY+_leftRoom.Height)-_rightRoom.StartY) * 2;
        
        Debug.Log("Grid dimensions: " + gridWidth + "x" + gridHeight);

        _grid = new GridTile[gridHeight, gridWidth];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                _grid[y, x] = new GridTile(gridStartX + x, gridStartY + y);
            }
        }
    }

    private void GenerateObjects()
    {
        for (int y = 0; y < _grid.GetLength(0); y++)
        {
            for (int x = 0; x < _grid.GetLength(1); x++)
            {
                Vector3 pos = _obstaclesHolder.GetComponent<Tilemap>()
                    .GetCellCenterWorld(new Vector3Int(_grid[y, x].XCoord, _grid[y, x].YCoord, 0));
                Instantiate(test, pos, Quaternion.identity, _obstaclesHolder);
            }
        }
    }
}
