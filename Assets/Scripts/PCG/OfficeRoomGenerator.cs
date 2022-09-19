using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Pathfinding.Examples;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = System.Random;

public class OfficeRoomGenerator : MapGenerator
{
    [Header("Objects")] public List<GameObject> horTables;
    public List<GameObject> verTables;
    public List<GameObject> extraItems;

    [Range(0, 100)] public int tableDensity = 85;
    [Range(0,100)]public int extraDensity = 15;

    private static readonly string _tableDir = "Sprites/Objects/Level_1/Table-blue/";
    private static readonly string _horDir = "hor/";
    private static readonly string _verDir = "ver/";
    private static readonly string _upDir = "up";
    private static readonly string _downDir = "down";
    private static readonly string _leftDir = "left/";
    private static readonly string _rightDir = "right/";

    [Header("sprites")]
    private Object[][] _sprites;

    private int _minTableGap = 2;

    private Room _leftRoom;
    private Room _rightRoom;
    private int _bigRoomTreshold = 4;
    
    private Wall _overlap;

    [Header("Tile sets to use for generation.")]
    public List<Tile> tileLst;

    public List<Tile> horWallLst;
    public List<Tile> verWallLst;
    
    public override void Generate()
    {
        base.Generate();
        SetUpRoomGen();
        GenerateOfficeFloor();
        GenerateWalls();
        GenerateColliders();

        GenerateGrid();
        GenerateDoors();
        GenerateObjects();
    }

    protected override void SetUpParameters()
    {
        if(maxWidth<= 0 || maxHeight <= 0) maxWidth = maxHeight = 25;
        _minWidth = _minHeight = 4;

        _roomMin = 10;
        _hallTreshold = 5;
    }

    private void SetUpRoomGen()
    {
        if (_sprites != null && _sprites.Length > 0) return;
        
        _sprites = new[]
        {
            Resources.LoadAll(_tableDir + _horDir + _leftDir + _upDir, typeof(Sprite)), // hor left up - 0
            Resources.LoadAll(_tableDir + _horDir + _leftDir + _downDir, typeof(Sprite)), // hor left down - 1
            Resources.LoadAll(_tableDir + _horDir + _rightDir + _upDir, typeof(Sprite)), //hor right up - 2
            Resources.LoadAll(_tableDir + _horDir + _rightDir + _downDir, typeof(Sprite)), //hor right down - 3

            Resources.LoadAll(_tableDir + _verDir + _leftDir + _upDir, typeof(Sprite)), // ver left up - 4
            Resources.LoadAll(_tableDir + _verDir + _leftDir + _downDir, typeof(Sprite)), // ver left down - 5
            Resources.LoadAll(_tableDir + _verDir + _rightDir + _upDir, typeof(Sprite)), // ver right up - 6
            Resources.LoadAll(_tableDir + _verDir + _rightDir + _downDir, typeof(Sprite)) // ver right down - 7
        };
        Debug.Log("Assets loaded.");
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

    private void HandleRoomsStartY(ref Room fixedRoom, ref Room movedRoom, Random rand)
    {
        movedRoom.StartY = rand.Next(fixedRoom.StartY - movedRoom.Height + _minHeight, 
            Math.Min(0, fixedRoom.StartY + fixedRoom.Height - movedRoom.Height));
    }

    internal override void GenerateWalls()
    {
        _walls = new List<Wall>();
        _overlap = new Wall();
        if (_leftRoom.Type == RoomType.LONG_HALL_VER)
        {
            // long vertical wall
            Debug.Log("Here.");
            AddWallToLst(_walls, HallType.HORIZONTAL, _leftRoom.StartX, _leftRoom.StartX + _leftRoom.Width,
                _rightRoom.Height);
            AddWallToLst(_walls, HallType.VERTICAL, _leftRoom.StartY, _rightRoom.Height,
                _rightRoom.StartX + _rightRoom.Width);
        }
        else
        {
            // handle all other cases
            if (_leftRoom.StartY + _leftRoom.Height == _rightRoom.StartY + _rightRoom.Height)
            {
                // only two walls
                AddWallToLst(_walls, HallType.HORIZONTAL, _leftRoom.StartX, _rightRoom.Width,
                    _leftRoom.StartY + _leftRoom.Height);
                AddWallToLst(_walls, HallType.VERTICAL, _rightRoom.StartY, _rightRoom.StartY + _rightRoom.Height,
                    _rightRoom.StartX + _rightRoom.Width);
            }
            else
            {
                // four walls
                AddWallToLst(_walls, HallType.HORIZONTAL, _leftRoom.StartX, _leftRoom.StartX + _leftRoom.Width,
                    _leftRoom.Height);
                AddWallToLst(_walls, HallType.VERTICAL, _rightRoom.StartY + _rightRoom.Height,
                    _leftRoom.StartY + _leftRoom.Height,
                    _leftRoom.StartX + _leftRoom.Width);
                AddWallToLst(_walls, HallType.HORIZONTAL, _rightRoom.StartX, _rightRoom.StartX + _rightRoom.Width,
                    _rightRoom.StartY + _rightRoom.Height);
                AddWallToLst(_walls, HallType.VERTICAL, _rightRoom.StartY, _rightRoom.StartY + _rightRoom.Height,
                    _rightRoom.StartX + _rightRoom.Width);
            }
        }

        if (_leftRoom.Type == RoomType.BIG_ROOM || _leftRoom.Type == RoomType.LONG_HALL_HOR ||
            _leftRoom.Type == RoomType.LONG_HALL_VER)
            _overlap.SetValues(HallType.HORIZONTAL, 0, 0, 0);
        else
            _overlap.SetValues(HallType.VERTICAL, _leftRoom.StartY, _rightRoom.StartY + _rightRoom.Height, -1);
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
                if(wall.Orientation == HallType.HORIZONTAL)
                    horWalls.SetTile(new Vector3Int(wall.Start, wall.Height, 0), horWallLst[3]);
                else
                    verWalls.SetTile(new Vector3Int(wall.Height, wall.Start, 0), verWallLst[3]);
            }
            else
            {
                if (wall.Orientation == HallType.HORIZONTAL)
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

    private void PutDownRoomTiles(Tile tile, Tilemap tm, Room room)
    {
        SetTilesToMap(tile, tm, room.StartX, room.StartY, 
            room.StartX + room.Width, room.StartY + room.Height);
    }

    private void GenerateColliders()
    {
        _cols = new List<Wall>();

        if (_leftRoom.Type == RoomType.LONG_HALL_VER)
        {
            // special case
            AddWallToLst(_cols, HallType.VERTICAL, _leftRoom.StartY - 1, _rightRoom.StartY + _rightRoom.Height + 1,
                _rightRoom.StartX - 1);
            AddWallToLst(_cols, HallType.VERTICAL, _leftRoom.StartY - 1, _rightRoom.StartY + _rightRoom.Height + 1,
                _rightRoom.StartX + _rightRoom.Width);

            AddWallToLst(_cols, HallType.HORIZONTAL, _leftRoom.StartX - 1, _leftRoom.StartX + _leftRoom.Width + 1,
                _rightRoom.StartY + _rightRoom.Height);
            AddWallToLst(_cols, HallType.HORIZONTAL, _leftRoom.StartX - 1, _leftRoom.StartX + _leftRoom.Width + 1,
                _leftRoom.StartY - 1);
        }
        else
        {
            // first add collider walls, that are always present fully
            AddWallToLst(_cols, HallType.VERTICAL, _leftRoom.StartY - 1, _leftRoom.StartY + _leftRoom.Height + 1,
                _leftRoom.StartX - 1);
            AddWallToLst(_cols, HallType.HORIZONTAL, _leftRoom.StartX - 1, _leftRoom.StartX + _leftRoom.Width + 1,
                _leftRoom.StartY + _leftRoom.Height);

            AddWallToLst(_cols, HallType.VERTICAL, _rightRoom.StartY - 1, _rightRoom.StartY + _rightRoom.Height + 1,
                _rightRoom.StartX + _rightRoom.Width);
            AddWallToLst(_cols, HallType.HORIZONTAL, _rightRoom.StartX - 1, _rightRoom.StartX + _rightRoom.Width + 1,
                _rightRoom.StartY - 1);

            // now walls, that dont extend on inner side
            AddWallToLst(_cols, HallType.HORIZONTAL, _leftRoom.StartX - 1, _leftRoom.StartX + _leftRoom.Width,
                _leftRoom.StartY - 1);
            AddWallToLst(_cols, HallType.HORIZONTAL, _rightRoom.StartX, _rightRoom.StartX + _rightRoom.Width + 1,
                _rightRoom.StartY + _rightRoom.Height);

            // now walls, that may not be present at all
            if (_leftRoom.StartY + _leftRoom.Height > _rightRoom.StartY + _rightRoom.Height)
                AddWallToLst(_cols, HallType.VERTICAL, _rightRoom.StartY + _rightRoom.Height + 1,
                    _leftRoom.StartY + _leftRoom.Height + 1, _rightRoom.StartX);
            if (_leftRoom.StartY > _rightRoom.StartY)
                AddWallToLst(_cols, HallType.VERTICAL, _rightRoom.StartY - 1, _leftRoom.StartY - 1,
                    _rightRoom.StartX - 1);
        }

        PutDownColliders(_cols);
    }

    internal override void GenerateGrid()
    {
        gridStartX = _leftRoom.StartX*2;
        gridStartY = _rightRoom.StartY*2;

        int gridWidth = _leftRoom.Type == RoomType.LONG_HALL_VER ? _leftRoom.Width*2 : (_leftRoom.Width + _rightRoom.Width) * 2;
        //int gridHeight = (_leftRoom.Type == RoomType.BIG_ROOM || _leftRoom.Type == RoomType.LONG_HALL_HOR) ? _leftRoom.Height * 2 : ((_leftRoom.StartY+_leftRoom.Height)-_rightRoom.StartY) * 2;
        int gridHeight = (_leftRoom.Type == RoomType.BIG_ROOM || _leftRoom.Type == RoomType.LONG_HALL_HOR) ? _leftRoom.Height * 2 : ((_leftRoom.Height + _rightRoom.Height)-_overlap.End) * 2;
        //Debug.Log("Overlap: " + _overlap.Start + "x" + _overlap.End + " height> " + _overlap.Height);

        Debug.Log("Grid dimensions: " + gridWidth + "x" + gridHeight);

        _grid = new GridTile[gridHeight, gridWidth];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if((x < _leftRoom.Width*2 && gridStartY + y < _leftRoom.StartY*2) ||
                   (x >= _leftRoom.Width*2 && gridStartY+y >= _rightRoom.StartY*2 + _rightRoom.Height*2))
                    _grid[y, x] = new GridTile(gridStartX + x, gridStartY + y, false);
                else
                    _grid[y, x] = new GridTile(gridStartX + x, gridStartY + y);
            }
        }
    }

    private void GenerateObjects()
    {
        if (extraItems.Count <= 0)
        {
            Debug.LogError("No extra items to put in the room!!");
            return;
        }
        Wall leftOverlap = new Wall();
        Wall rightOverlap = new Wall();
        leftOverlap.SetValues(_overlap.Orientation, _overlap.Start * 2, _overlap.End * 2, -1);
        rightOverlap.SetValues(_overlap.Orientation, _overlap.Start * 2, _overlap.End * 2, 0);
        switch (_leftRoom.Type)
        {
            case RoomType.ROOM:
                GenerateOfficeRoom((_leftRoom.StartX+2)*2, (_leftRoom.StartY+2)*2, (_leftRoom.Width-4)*2, (_leftRoom.Height-4)*2);
                GenerateExtraObjects(_leftRoom.StartX*2, _leftRoom.StartY*2, _leftRoom.Width*2-1, _leftRoom.Height*2-1, leftOverlap, false);
                break;
            case RoomType.BIG_ROOM:
                GenerateOfficeRoom((_leftRoom.StartX+2)*2, (_leftRoom.StartY+2)*2, (_rightRoom.Width + _leftRoom.Width-4)*2, (_leftRoom.Height-4)*2);
                GenerateExtraObjects(_leftRoom.StartX*2, _leftRoom.StartY*2, (_leftRoom.Width + _rightRoom.Width)*2-1, _leftRoom.Height*2-1, _overlap, false);
                break;
            case RoomType.LONG_HALL_HOR:
                GenerateExtraObjects(_leftRoom.StartX*2, _leftRoom.StartY*2, (_leftRoom.Width + _rightRoom.Width)*2-1, _leftRoom.Height*2-1, leftOverlap);
                break;
            case RoomType.LONG_HALL_VER:
                GenerateExtraObjects(_leftRoom.StartX*2, _leftRoom.StartY*2, _leftRoom.Width*2-1, (_leftRoom.Height + _rightRoom.Height)*2-1, leftOverlap);
                break;
            default:
                GenerateExtraObjects(_leftRoom.StartX*2, _leftRoom.StartY*2, _leftRoom.Width*2-1, _leftRoom.Height*2-1, leftOverlap);
                break;
        }

        if (_rightRoom.Type == RoomType.ROOM)
        {
            GenerateExtraObjects(_rightRoom.StartX * 2, _rightRoom.StartY * 2, _rightRoom.Width * 2 - 1,
                _rightRoom.Height * 2 - 1, rightOverlap, false);
            GenerateOfficeRoom((_rightRoom.StartX+2)*2, (_rightRoom.StartY+2)*2, (_rightRoom.Width-4)*2, (_rightRoom.Height-4)*2);
        }
        else if(_rightRoom.Type == RoomType.HOR_HALL || _rightRoom.Type == RoomType.VER_HALL)
            GenerateExtraObjects(_rightRoom.StartX*2, _rightRoom.StartY*2, _rightRoom.Width*2-1, _rightRoom.Height*2-1, rightOverlap);
    }

    private void GenerateOfficeRoom(int startX, int startY, int width, int height)
    {
        int tableWidth = horTables[0].transform.GetComponent<TableHandler>().width;
        int tableHeight = horTables[0].transform.GetComponent<TableHandler>().height;
        int tableAllowance = tableWidth + _minTableGap;
        if (width <  tableAllowance || height < tableAllowance) // no room for tables
            return;
        Random rnd = new Random();

        // generate table type
        int tableType = rnd.Next(0, _sprites.Length);
        HallType horOrientation = tableType < 4 ? HallType.VERTICAL : HallType.HORIZONTAL;
        bool downOrientation = tableType%2 == 1;
        int tableSide = tableType%4 < 2 ? 0 : 1;


        if (horOrientation == HallType.VERTICAL) // horizontal tables in vert column
        {
            int numRows = (width+_minTableGap) / tableAllowance;

            for (int i = 0; i < numRows; i++)
            {
                PlaceTableRow(startX + 1 + tableAllowance*i, 
                    startY + tableHeight,
                    tableHeight*2 + _minTableGap,
                    startY + height,
                    horOrientation,
                    downOrientation,
                    horTables[tableSide],
                    tableType,
                    rnd
                    );
            }
        }
        else // vertical tables in hor row
        {
            int numRows = (height+_minTableGap) / tableAllowance;

            for (int i = 0; i < numRows; i++)
            {
                PlaceTableRow(startX + tableHeight, 
                    startY + 1 + tableAllowance*i,
                    tableHeight*2 + _minTableGap,
                    startX + width,
                    horOrientation,
                    downOrientation,
                    verTables[tableSide],
                    tableType,
                    rnd
                );
            }
        }
    }

    private void PlaceTableRow(int startX, int startY, int step, int finish, HallType orientation, bool down, 
                                GameObject table, int tableVars, Random rnd)
    {
        int coordStep = orientation == HallType.VERTICAL ? startY : startX;
        float tableOffsetY = _obstaclesHolder.GetComponent<Grid>().cellSize.y/2;

        while (coordStep <= finish)
        {
            Vector3Int coords = orientation == HallType.VERTICAL ? new Vector3Int(startX, coordStep, 0) : 
                                                                    new Vector3Int(coordStep, startY, 0);
            GameObject tableInst = InstantiateObjectInWorld(table, coords, 0.0f, tableOffsetY);
            if (tableInst == null) continue;
            
            // rnd gen
            int max = (_sprites[tableVars].Length)/2;
            int tableIdx = rnd.Next(0, max)*2;
            bool chairCoinFlip = rnd.Next() % 2 == 0;
            
            Sprite main = (Sprite)_sprites[tableVars][tableIdx];
            Sprite support = (Sprite)_sprites[tableVars][tableIdx + 1];
            
            tableInst.GetComponent<TableHandler>().SetTableVariant(down, main, support, chairCoinFlip);
            
            // fill up grid for AI
            int x = orientation == HallType.VERTICAL ? startX - 1 : coordStep;
            int y = orientation == HallType.VERTICAL ? coordStep : startY - 1;
            int h = tableInst.GetComponent<TableHandler>().height;
            int w = tableInst.GetComponent<TableHandler>().width;

            if (chairCoinFlip)
            {
                if (orientation == HallType.HORIZONTAL)
                {
                    x += down ? -1 : 0;
                    w *= 2;
                }
                else
                {
                    y += down ? -1 : 0;
                    h *= 2;
                }
            }

            FillObstaclesGrid(x, y, w, h);
            //Debug.Log("W: " + w + " H: " + h + " x: " + x + " y: " + y);
            
            // step
            coordStep += step;
        }
    }

    private void GenerateExtraObjects(int startX, int startY, int width, int height, Wall blocked, bool hall = true, bool longHall = false)
    {
        Wall[] longWalls = new Wall[2];
        Wall[] shortWalls = new Wall[2];
        longWalls[0] = new Wall();
        longWalls[1] = new Wall();
        shortWalls[0] = new Wall();
        shortWalls[1] = new Wall();

        Wall horWallDown = new Wall();
        Wall horWallUp = new Wall();
        Wall vertWallLeft = new Wall();
        Wall vertWallRight = new Wall();

        horWallDown.SetValues(HallType.HORIZONTAL,startX, startX + width+1, startY);
        horWallUp.SetValues(HallType.HORIZONTAL, startX, startX + width+1, startY+height);

        if (blocked.Height == startX && blocked.Orientation == HallType.VERTICAL) // left wall overlap
        {
            vertWallLeft.SetValues(HallType.VERTICAL, startY+1, blocked.Start, startX);
            vertWallRight.SetValues(HallType.VERTICAL, startY+1, startY + height, startX + width);
        }
        else if(blocked.Orientation == HallType.VERTICAL)
        {
            vertWallLeft.SetValues(HallType.VERTICAL, startY+1, startY + height, startX);
            vertWallRight.SetValues(HallType.VERTICAL, blocked.End, startY + height, startX + width);
        }
        else
        {
            vertWallLeft.SetValues(HallType.VERTICAL, startY+1, startY + height, startX);
            vertWallRight.SetValues(HallType.VERTICAL, startY+1, startY + height, startX + width);
        }
        
        if (height > width)
        {
            longWalls[0] = vertWallLeft;
            longWalls[1] = vertWallRight;
            shortWalls[0] = horWallDown;
            shortWalls[1] = horWallUp;
        }
        else
        {
            shortWalls[0] = vertWallLeft;
            shortWalls[1] = vertWallRight;
            longWalls[0] = horWallDown;
            longWalls[1] = horWallUp;
        }

        int density = extraDensity;
        if (hall) density = (density*3)/2;
        if (longHall) density *= 2;

        foreach (var wall in longWalls)
        {
            if (wall.Orientation == HallType.VERTICAL)
                PutDownObjects(wall.Height, wall.Start, 1, wall.End-wall.Start, extraItems,density);
            else
                PutDownObjects(wall.Start, wall.Height, wall.End-wall.Start, 1, extraItems, density);
        }
        foreach (var wall in shortWalls)
        {
            if (wall.Orientation == HallType.VERTICAL)
                PutDownObjects(wall.Height, wall.Start, 1, wall.End-wall.Start, extraItems, density/2);
            else
                PutDownObjects(wall.Start, wall.Height, wall.End-wall.Start, 1, extraItems, density/2);
        }
    }

    /*
    private void PutDownExtraObjects(int xCoord, int yCoord, int width, int height, int objDensity)
    {
        Random rnd = new Random();
        float offsetMax = _obstaclesHolder.GetComponent<Grid>().cellSize.x;
        //Debug.Log("putting down objects from " + xCoord + "x" + yCoord + " size of " + width + "x" + height);
        int den = objDensity;
        for (var y = yCoord; y < yCoord + height; y++)
        {
            for (var x = xCoord; x < xCoord + width; x++)
            {
                if (((x == _entrance.EntrancePos.x*2 || x - 1 == _entrance.EntrancePos.x*2) && y == _entrance.EntrancePos.y*2 - 1) ||
                    (x == _entrance.ExitPos.x*2 - 1 && (y == _entrance.ExitPos.y*2 || y - 1 == _entrance.ExitPos.y*2)))
                    continue; // dont block doors
                if (rnd.Next(0, 100) <= den)
                {
                    int objIdx = GetNumFromRange(rnd, 0, extraItems.Count - 1);
                    InstantiateObjectInWorld(extraItems[objIdx], new Vector3Int(x, y, 0), 
                        GetNumFromRangeFloat(rnd, -(int)((offsetMax*100)/3), 
                            (int)((offsetMax*100)/3)) /100,
                        GetNumFromRangeFloat(rnd, -(int)((offsetMax*100)/3), 
                            (int)((offsetMax*100)/3)) /100);
                    den = den / 4;
                } else if (den < objDensity) den+=(objDensity-den)/4;
                else if (x % 4 == 0) den++; // to try and avoid completely empty rooms
            }
        }
    }*/

    protected override Vector2Int UnityToScriptCoord(int x, int y)
    {
        return new Vector2Int(x + Math.Abs(_leftRoom.StartX*2), y + Math.Abs(_leftRoom.Type == RoomType.LONG_HALL_VER? _leftRoom.StartY*2 : _rightRoom.StartY*2));
    }
}
