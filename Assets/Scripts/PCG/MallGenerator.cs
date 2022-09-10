using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class MallGenerator : MapGenerator
{
    
    private Room _room;
    private Room _extraRoom;

    [Header("Floor tiles")] public List<Tile> floorTiles;

    private bool _crossroads = false;

    public List<GameObject> props;
    public List<Tile> storefrontsSW;
    public List<Tile> storefrontsSE;
    
    public override void Generate()
    {
        base.Generate();
        CleanUpMall();
        
        GenerateFloor();
        GenerateColliders();
        GenerateWalls();
        GenerateGrid();
        GenerateDoors(); 
        PutDownWalls();
        //PutDownObjects(gridStartX, gridStartY, _grid.GetLength(1), _grid.GetLength(0), props, 100);
    }

    protected override void SetUpParameters()
    {
        if (maxHeight <= 0) maxHeight = 20;
        if (maxWidth <= 0) maxWidth = 20;
        
        _minHeight = _minWidth = 2;
        
    }
    
    private void CleanUpMall()
    {
        _room = new Room();
        _extraRoom = new Room();
        _crossroads = false;
    }

    internal override void SetRoom(ref Room room, Random rand)
    {
        room = new Room();
        
        room.Height = rand.Next(_minHeight, maxHeight);
        room.Width = rand.Next(_minWidth, maxWidth);

        if (room.Width > room.Height)
        {
            room.Type = RoomType.HOR_HALL;
            room.Width *= 2;
            room.Height = rand.Next(_minHeight, _minHeight * 2);
        }
        else
        {
            room.Type = RoomType.VER_HALL;
            room.Height *= 2;
            room.Width = rand.Next(_minWidth, _minWidth * 2);
        }
    }

    private void FinishRoomSetUp(Random rnd)
    {
        _room.StartY = 0 - _room.Height / 2;
        _room.StartX = 0 - _room.Width / 2;

        if (_extraRoom.Height <= 0) return;
        
        if (Heads(rnd))
        {
            // crossroads
            _extraRoom.StartX = 0 - _extraRoom.Width / 2;
            _extraRoom.StartY = 0 - _extraRoom.Height / 2;
            _crossroads = true;
            if (_room.Type == RoomType.VER_HALL)
                (_extraRoom, _room) = (_room, _extraRoom);
        }
        else
        {
            _extraRoom.StartX = 0 + _room.Width / 2;
            _extraRoom.StartX += _room.Width % 2 == 0 ? 0 : 1;
            int n = GetNumFromRange(rnd, 0, 3);
            switch (n)
            {
                case 0: // bottom alignment
                    _extraRoom.StartY = _room.StartY;
                    //Debug.Log("bottom alignment " + _extraRoom.StartY);
                    break;
                case 1: // middle alignment
                    _extraRoom.StartY = 0 - _extraRoom.Height / 2;
                    //Debug.Log("middle alignment " + _extraRoom.StartY);
                    break;
                case 2: // top alignment
                    _extraRoom.StartY = _room.StartY + _room.Height - _extraRoom.Height;
                    //Debug.Log("top alignment " + _extraRoom.StartY);
                    break;
                default:
                    Debug.Log("Weird. " + n);
                    break;
            }
        }
    }

    private void GenerateFloor()
    {
        Random rnd = new Random();
        var tm = _gridHolder.transform.GetChild(0).GetComponent<Tilemap>();
        
        SetRoom(ref _room, rnd);
        SetRoom(ref _extraRoom, rnd);

        if (_room.Type == _extraRoom.Type)
        {
            switch (_room.Type)
            {
                case RoomType.HOR_HALL:
                    _room.Width += _extraRoom.Width;
                    _room.Height = _room.Height < _extraRoom.Height ? _extraRoom.Height : _room.Height;
                    break;
                case RoomType.VER_HALL:
                    _room.Height += _extraRoom.Height;
                    _room.Width = _room.Width < _extraRoom.Width ? _extraRoom.Width : _room.Width;
                    break;
                default:
                    break;
            }

            _extraRoom.Width = _extraRoom.Height = 0;
        }
        
        FinishRoomSetUp(rnd);
        SetTilesToMap(floorTiles[rnd.Next(0, floorTiles.Count-1)], tm, _room.StartX, _room.StartY,
            _room.StartX + _room.Width, _room.StartY + _room.Height);
        SetTilesToMap(floorTiles[rnd.Next(0, floorTiles.Count-1)], tm, _extraRoom.StartX, _extraRoom.StartY,
            _extraRoom.StartX + _extraRoom.Width, _extraRoom.StartY + _extraRoom.Height);
        
        Debug.Log("Room starts at: " + _room.StartX + "x" + _room.StartY + " and its size is: " + 
                  _room.Width + "x" + _room.Height);
        Debug.Log("Extra room starts at: " + _extraRoom.StartX + "x" + _extraRoom.StartY + " and its size is: " + 
                  _extraRoom.Width + "x" + _extraRoom.Height);
    }
    
    private void GenerateColliders()
    {
        Debug.Log("Colliders.");
        _cols = new List<Wall>();

        if (_crossroads)
        {
            // add parallel cols from both
            AddParallelLines(_cols, _room, HallType.VERTICAL);
            AddParallelLines(_cols, _extraRoom, HallType.HORIZONTAL);
            
            AddAllSectionedLines(_cols, _room, _extraRoom);

        } else if (_extraRoom.Height <= 0)
        {
            AddParallelLines(_cols, _room, HallType.VERTICAL);
            AddParallelLines(_cols, _room, HallType.HORIZONTAL);
        }
        else
        {
            // left wall intact
            AddWallToLst(_cols, HallType.VERTICAL, _room.StartY - 1, _room.StartY + _room.Height + 1,
                _room.StartX - 1);
            // hor colliders of both rooms
            AddParallelLines(_cols, _room, HallType.HORIZONTAL);
            AddParallelLines(_cols, _extraRoom, HallType.HORIZONTAL);
            
            // right wall of second room
            AddWallToLst(_cols, HallType.VERTICAL, _extraRoom.StartY - 1, 
                _extraRoom.StartY + _extraRoom.Height + 1, _extraRoom.StartX + _extraRoom.Width);
            
            // touching walls
            if (_room.Height > _extraRoom.Height)
                AddSectionedLine(_cols, _room, _extraRoom, 
                    _room.StartX + _room.Width, HallType.VERTICAL);
            else
                AddSectionedLine(_cols, _extraRoom, _room, 
                    _extraRoom.StartX - 1, HallType.VERTICAL);
        }
        
        PutDownColliders(_cols);
    }
    internal override void GenerateGrid()
    {
        gridStartX = _room.StartX*2;
        gridStartY = _room.StartY*2;
        int gridWidth = _room.Width * 2;
        int gridHeight = _room.Height * 2;

        if (_extraRoom.Height > 0)// accomodate for second room
        {      
            if (_crossroads || _room.Type == RoomType.HOR_HALL)
            {
                gridStartY = _extraRoom.StartY * 2;
                gridHeight = _extraRoom.Height * 2;
            }

            if (!_crossroads)
                gridWidth += _extraRoom.Width * 2;
        }
        
        Debug.Log("Grid dimensions: " + gridWidth + "x" + gridHeight);
        Debug.Log("Grid starts at " + gridStartX + "x" + gridStartY);

        _grid = new GridTile[gridHeight, gridWidth];
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if((x < _room.Width*2 && gridStartY + y >= _room.StartY*2 && gridStartY + y < _room.StartY*2 + _room.Height*2) ||
                   (_extraRoom.Height > 0 && 
                    (gridStartX + x >= _extraRoom.StartX*2 && gridStartX + x < _extraRoom.StartX*2 + _extraRoom.Width*2 && 
                     gridStartY + y >= _extraRoom.StartY*2 && gridStartY + y < _extraRoom.StartY*2 + _extraRoom.Height*2)))
                    _grid[y, x] = new GridTile(gridStartX + x, gridStartY + y);
                else
                    _grid[y, x] = new GridTile(gridStartX + x, gridStartY + y, false);
            }
        }
    }
    
    internal override void GenerateWalls()
    {
        _walls = new List<Wall>();
        
        if (_crossroads)
        {
            // add parallel top walls from both
            AddWallToLst(_walls, HallType.HORIZONTAL, _extraRoom.StartX, _extraRoom.StartX + _extraRoom.Width,
                _extraRoom.StartY + _extraRoom.Height);
            AddWallToLst(_walls, HallType.VERTICAL, _extraRoom.StartY, _room.StartY,
                _extraRoom.StartX + _extraRoom.Width);
            // no other walls at crossroads
            //AddAllSectionedLines(_walls, _room, _extraRoom);
        } else if (_extraRoom.Height <= 0)
        {
            AddWallToLst(_walls, HallType.HORIZONTAL, _room.StartX, _room.StartX + _room.Width,
                _room.StartY + _room.Height);
            AddWallToLst(_walls, HallType.VERTICAL, _room.StartY, _room.StartY + _room.Height,
                _room.StartX + _room.Width);
        }
        else
        {
            // hor colliders of both rooms
            AddWallToLst(_walls, HallType.HORIZONTAL, _room.StartX, _room.StartX + _room.Width,
                _room.StartY + _room.Height);
            
            // touching walls
            if (_room.Height > _extraRoom.Height && _extraRoom.StartY + _extraRoom.Height < _room.StartY + _room.Height)
                AddWallToLst(_walls, HallType.VERTICAL, _extraRoom.StartY + _extraRoom.Height, 
                    _room.StartY + _room.Height, _room.StartX + _room.Width);
            
            AddWallToLst(_walls, HallType.HORIZONTAL, _extraRoom.StartX, _extraRoom.StartX + _extraRoom.Width,
                _extraRoom.StartY + _extraRoom.Height);
            
            // right wall of second room
            AddWallToLst(_walls, HallType.VERTICAL, _extraRoom.StartY, 
                _extraRoom.StartY + _extraRoom.Height, _extraRoom.StartX + _extraRoom.Width);
        }
        
        Debug.Log("no walls: " + _walls.Count);
        //PutDownColliders(_walls);
        
        //PutDownWalls();
    }

    private void PutDownWalls()
    {
        Random rnd = new Random();
        var extraWall = _gridHolder.transform.GetChild(5).GetComponent<Tilemap>();
        
        foreach (var wall in _walls)
        {
            int width = wall.End - wall.Start;
            if (width <= 3) continue;
            
            //enough space for a store
            for (int i = 0; i < width-3; i++)
            {
                if (i % 4 != 0) continue;
                Vector3Int tilePos;
                Tile newTile;
                if (wall.Orientation == HallType.HORIZONTAL)
                {
                    tilePos = new Vector3Int(wall.Start + i, wall.Height, 0);
                    newTile = storefrontsSE[rnd.Next(0, storefrontsSE.Count )];
                }
                else
                {
                    tilePos = new Vector3Int(wall.Height, wall.Start + i, 0);
                    newTile = storefrontsSW[rnd.Next(0, storefrontsSW.Count )];
                }
                
                if(CheckWallTiles(extraWall, wall.Start+i-1, wall.Start+i+4, wall.Height, wall.Orientation))
                    extraWall.SetTile(tilePos, newTile);
            }
        }
    }
}
