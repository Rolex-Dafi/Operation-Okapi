using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class StreetGenerator : MapGenerator
{
    [Header("Tiles")] public List<Tile> grassTile;
    public List<Tile> pavementTile;
    public List<Tile> asphaltTile;

    private Room _room;
    private Room _extraRoom;

    private int _maxBorder = 2;
    private int _minBorder = 1;
    private int _borderTiles = 2;
    
    public List<GameObject> props;

    public List<Tile> buildingsVert;
    public List<Tile> buildingsHor;

    internal enum StreetType
    {
        PLAZA, STREET, CROSSROADS
    }

    private StreetType _streetType;
    private bool _parkType;
    
    public override void Generate()
    {
        base.Generate();

        CleanUpStreet();

        if (!CheckSprites())
        {
            Debug.Log("Some sprites needed for generation are missing in the inspector.");
            return;
        }
        
        GenerateFloor();
        GenerateWalls();
        GenerateColliders();
        GenerateGrid();
        GenerateDoors();

        PutDownBuildings();
    }

    private void CleanUpStreet()
    {
        _room = new Room();
        _extraRoom = new Room();
    }

    private bool CheckSprites()
    {
        return grassTile != null && pavementTile != null && asphaltTile != null 
               && grassTile.Count > 0 && pavementTile.Count > 0 && asphaltTile.Count > 0;
    }

    internal override void Restart()
    {
        base.Restart();
        _streetType = StreetType.PLAZA;
        Random rnd = new Random();
        _borderTiles = rnd.Next(_minBorder, _maxBorder);
        
    }

    protected override void SetUpParameters()
    {
        if(maxHeight <= 0) maxHeight = 20;
        if (maxWidth <= 0) maxWidth = 20;
        _minHeight = _minWidth = 5;

        _roomMin = 8;
        _hallTreshold = 7;

        _streetType = StreetType.PLAZA;
        
        Random rnd = new Random();
        _borderTiles = rnd.Next(_minBorder, _maxBorder);
    }

    private void GenerateFloor()
    {
        Random rnd = new Random();
        var tm = _gridHolder.transform.GetChild(0).GetComponent<Tilemap>();
        
        SetRoom(ref _room, rnd);

        if (_room.Type != RoomType.ROOM)
        {
            _streetType = StreetType.STREET;
            if (_room.Type == RoomType.HOR_HALL) _room.Height = rnd.Next(_minHeight, _minHeight + _borderTiles);
            else _room.Width = rnd.Next(_minWidth, _minWidth + _borderTiles);
            
            if (Heads(rnd))
            {
                // coinflip for extra hall
                SetRoom(ref _extraRoom, rnd);
                if (_extraRoom.Type == RoomType.ROOM) MakeHall(ref _extraRoom, rnd);
                
                if (_room.Type == RoomType.HOR_HALL && _extraRoom.Type == RoomType.HOR_HALL)
                {
                    _room.Width += _extraRoom.Width;
                    _extraRoom.Width = _extraRoom.Height = 0;
                }
                else if (_room.Type == RoomType.VER_HALL && _extraRoom.Type == RoomType.VER_HALL)
                {
                    _room.Height += _extraRoom.Height;
                    _extraRoom.Width = _extraRoom.Height = 0;
                }
            }
        }
        
        FinishRoomSetUp(rnd);
        SetUpFloorTiles(tm, rnd);
    }

    private void MakeHall(ref Room room, Random rnd)
    {
        if (room.Width > room.Height)
        {
            room.Type = RoomType.HOR_HALL;
            room.Width *= 2;
            if (room.Width <= _hallTreshold) room.Width = _hallTreshold * 2;
            room.Height = rnd.Next(_minHeight, _minHeight + _borderTiles);
        }
        else
        {
            room.Type = RoomType.VER_HALL;
            room.Height *= 2;
            if (room.Height <= _hallTreshold) room.Height = _hallTreshold * 2;
            room.Width = rnd.Next(_minWidth, _minWidth + _borderTiles);
        }
    }

    private void FinishRoomSetUp(Random rnd)
    {
        _room.StartY = 0 - _room.Height / 2;
        _room.StartX = 0 - _room.Width / 2;
        
        if (_extraRoom.Height > 0)
        {
            if (Heads(rnd))
            {
                // crossroads
                _extraRoom.StartX = 0 - _extraRoom.Width / 2;
                _extraRoom.StartY = 0 - _extraRoom.Height / 2;
                _streetType = StreetType.CROSSROADS;
                if (_room.Type == RoomType.VER_HALL)
                    (_extraRoom, _room) = (_room, _extraRoom);
            }
            else
            {
                _extraRoom.StartX = 0 + _room.Width / 2;
                _extraRoom.StartX += _room.Width % 2 == 0 ? 0 : 1;
                if (Heads(rnd)) // bottom alignment
                    _extraRoom.StartY = _room.StartY;
                else // top alignment
                {
                    _extraRoom.StartY = _room.StartY + _room.Height - _extraRoom.Height;
                }
            }
        }

        _parkType = Heads(rnd);
    }

    private void SetUpFloorTiles(Tilemap tm, Random rnd)
    {
        if (_room.Type == RoomType.ROOM)
        {
            Tile tl = _parkType? grassTile[rnd.Next(0, grassTile.Count)] : 
                                    pavementTile[rnd.Next(0, pavementTile.Count)];
            
            SetTilesToMap(tl, tm, _room.StartX, _room.StartY,
                _room.StartX + _room.Width, _room.StartY + _room.Height);
        } else
        {
            Tile borderTile = _parkType? grassTile[rnd.Next(0, grassTile.Count)] : 
                asphaltTile[rnd.Next(0, asphaltTile.Count-1)];
            Tile walkwayTile = pavementTile[rnd.Next(0, pavementTile.Count)];
            
            if (_streetType == StreetType.CROSSROADS) // X crossroads
            {
                SetTilesToMap(walkwayTile, tm, _room.StartX, _room.StartY,
                    _room.StartX + _room.Width, _room.StartY + _room.Height);
                SetTilesToMap(walkwayTile, tm, _extraRoom.StartX, _extraRoom.StartY,
                    _extraRoom.StartX + _extraRoom.Width, _extraRoom.StartY + _extraRoom.Height);
            } 
            else if (_room.Type == RoomType.HOR_HALL) // single street
            {
                SetTilesToMap(walkwayTile, tm, _room.StartX, _room.StartY + _borderTiles,
                    _room.StartX + _room.Width, _room.StartY + _room.Height);
                SetTilesToMap(borderTile, tm, _room.StartX, _room.StartY,
                    _room.StartX + _room.Width, _room.StartY + _borderTiles);
                if (_extraRoom.Height > 0)
                {
                    SetTilesToMap(walkwayTile, tm, _extraRoom.StartX, _extraRoom.StartY,
                        _extraRoom.StartX + _extraRoom.Width, _extraRoom.StartY + _extraRoom.Height);
                    if (_room.StartY == _extraRoom.StartY)
                    {
                        SetTilesToMap(borderTile, tm, _extraRoom.StartX, _extraRoom.StartY,
                            _extraRoom.StartX + _extraRoom.Width, _extraRoom.StartY + _borderTiles);
                    }
                    else
                    {
                        SetTilesToMap(borderTile, tm, _extraRoom.StartX, _extraRoom.StartY,
                            _extraRoom.StartX + _borderTiles, _room.StartY + _borderTiles);
                    }
                }
            } 
            else if (_room.Type == RoomType.VER_HALL)
            {
                SetTilesToMap(walkwayTile, tm, _room.StartX + _borderTiles, _room.StartY,
                    _room.StartX + _room.Width, _room.StartY + _room.Height);
                SetTilesToMap(borderTile, tm, _room.StartX, _room.StartY,
                    _room.StartX + _borderTiles, _room.StartY + _room.Height);
                if(_extraRoom.Height > 0)
                {
                    SetTilesToMap(walkwayTile, tm, _extraRoom.StartX, _extraRoom.StartY,
                        _extraRoom.StartX + _extraRoom.Width, _extraRoom.StartY + _extraRoom.Height);
                    if (_room.StartY == _extraRoom.StartY)
                    {
                        SetTilesToMap(borderTile, tm, _room.StartX + _borderTiles, _extraRoom.StartY,
                            _extraRoom.StartX + _extraRoom.Width, _extraRoom.StartY + _borderTiles);
                    }
                }
            }
        }
        
        Debug.Log("Room starts at: " + _room.StartX + "x" + _room.StartY + " and its size is: " + 
                  _room.Width + "x" + _room.Height);
        Debug.Log("Extra room starts at: " + _extraRoom.StartX + "x" + _extraRoom.StartY + " and its size is: " + 
                  _extraRoom.Width + "x" + _extraRoom.Height);
    }
    
    internal override void GenerateWalls()
    {
        _walls = new List<Wall>();
        
        if (_streetType == StreetType.CROSSROADS)
        {
            // add parallel top walls from both
            AddWallToLst(_walls, HallType.HORIZONTAL, _extraRoom.StartX, _extraRoom.StartX + _extraRoom.Width,
                _extraRoom.StartY + _extraRoom.Height);
            AddWallToLst(_walls, HallType.VERTICAL, _extraRoom.StartY, _room.StartY,
                _extraRoom.StartX + _extraRoom.Width);
            // no other walls at crossroads
            //AddAllSectionedLines(_walls, _room, _extraRoom);
        } else if (_streetType == StreetType.PLAZA)
        {
            AddWallToLst(_walls, HallType.HORIZONTAL, _room.StartX, _room.StartX + _room.Width,
                _room.StartY + _room.Height);
            AddWallToLst(_walls, HallType.VERTICAL, _room.StartY, _room.StartY + _room.Height,
                _room.StartX + _room.Width);
        } else if (_extraRoom.Height <= 0)
        {
            AddWallToLst(_walls, HallType.HORIZONTAL, _room.Type == RoomType.VER_HALL ? _room.StartX + _borderTiles : _room.StartX, _room.StartX + _room.Width,
                _room.StartY + _room.Height);
            AddWallToLst(_walls, HallType.VERTICAL, _room.Type == RoomType.HOR_HALL ? _room.StartY + _borderTiles : _room.StartY, _room.StartY + _room.Height,
                _room.StartX + _room.Width);
        }
        else
        {
            // hor colliders of both rooms
            AddWallToLst(_walls, HallType.HORIZONTAL, 
                _room.Type == RoomType.HOR_HALL ? _room.StartX : _room.StartX + _borderTiles,
                _room.StartX + _room.Width,
                _room.StartY + _room.Height);
            
            // touching walls
            if (_room.Height > _extraRoom.Height && _extraRoom.StartY + _extraRoom.Height < _room.StartY + _room.Height)
                AddWallToLst(_walls, HallType.VERTICAL, _extraRoom.StartY + _extraRoom.Height, 
                    _room.StartY + _room.Height, _room.StartX + _room.Width);
            
            AddWallToLst(_walls, HallType.HORIZONTAL, _extraRoom.StartX, _extraRoom.StartX + _extraRoom.Width,
                _extraRoom.StartY + _extraRoom.Height);
            
            // right wall of second room
            AddWallToLst(_walls, HallType.VERTICAL, 
                (_extraRoom.Type == RoomType.HOR_HALL && _room.StartY == _extraRoom.StartY) ? _extraRoom.StartY + _borderTiles : _extraRoom.StartY, 
                _extraRoom.StartY + _extraRoom.Height, _extraRoom.StartX + _extraRoom.Width);
        }
        
        Debug.Log("no walls: " + _walls.Count);
        //PutDownColliders(_walls);
    }
    
    private void GenerateColliders()
    {
        Debug.Log("Colliders.");
        _cols = new List<Wall>();

        if (_streetType == StreetType.CROSSROADS)
        {
            // add parallel cols from both
            AddParallelLines(_cols, _room, HallType.VERTICAL);
            AddParallelLines(_cols, _extraRoom, HallType.HORIZONTAL);
            
            AddAllSectionedLines(_cols, _room, _extraRoom);

        } else if (_streetType == StreetType.PLAZA || _extraRoom.Width <= 0)
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
            if (_streetType == StreetType.CROSSROADS || _room.Type == RoomType.HOR_HALL)
            {
                gridStartY = _extraRoom.StartY * 2;
                gridHeight = _extraRoom.Height * 2;
            }

            if (_streetType == StreetType.STREET)
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
    
    private void PutDownBuildings()
    {
        Random rnd = new Random();
        var extraWall = _gridHolder.transform.GetChild(5).GetComponent<Tilemap>();

        foreach (var wall in _walls)
        {
            int reserve = wall.Orientation == HallType.VERTICAL ? 10 : 6;
            int width = wall.End - wall.Start;
            if (width <= reserve) continue;
            
            //enough space for a store
            for (int i = 0; i < width-reserve; i++)
            {
                if (i % (reserve) != 0) continue;
                Vector3Int tilePos;
                Tile newTile;
                if (wall.Orientation == HallType.HORIZONTAL)
                {
                    tilePos = new Vector3Int(wall.Start + i, wall.Height, 0);
                    newTile = buildingsHor[rnd.Next(0, buildingsHor.Count )];
                }
                else
                {
                    tilePos = new Vector3Int(wall.Height, wall.Start + i, 0);
                    newTile = buildingsVert[rnd.Next(0, buildingsVert.Count )];
                }
                
                if(CheckWallTiles(extraWall, wall.Start+i, wall.Start+i+reserve, wall.Height, wall.Orientation))
                    extraWall.SetTile(tilePos, newTile);
            }
        }
    }
}
