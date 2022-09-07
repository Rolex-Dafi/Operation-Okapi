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

    private int _maxBorder = 4;
    private int _minBorder = 2;
    private int _borderTiles = 2;

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
        //GenerateWalls();
        GenerateColliders();

        //GenerateGrid();
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

    protected override void SetUpParameters()
    {
        if(maxHeight <= 0) maxHeight = 20;
        if (maxWidth <= 0) maxWidth = 20;
        _minHeight = _minWidth = 7;

        _roomMin = 10;
        _hallTreshold = 17;

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
    
    private void GenerateWalls()
    {
        
    }
    
    private void GenerateColliders()
    {
        Debug.Log("Colliders.");
        _cols = new List<Wall>();

        if (_streetType == StreetType.CROSSROADS)
        {
            // add parallel cols from both
            AddParallelColliders(_room, HallType.VERTICAL);
            AddParallelColliders(_extraRoom, HallType.HORIZONTAL);
            
            AddAllSectionedColliders(_room, _extraRoom);

        } else if (_streetType == StreetType.PLAZA || _extraRoom.Width <= 0)
        {
            AddParallelColliders(_room, HallType.VERTICAL);
            AddParallelColliders(_room, HallType.HORIZONTAL);
        }
        else
        {
            // left wall intact
            AddWallToLst(_cols, HallType.VERTICAL, _room.StartY - 1, _room.StartY + _room.Height + 1,
                _room.StartX - 1);
            // hor colliders of both rooms
            AddParallelColliders(_room, HallType.HORIZONTAL);
            AddParallelColliders(_extraRoom, HallType.HORIZONTAL);
            
            // right wall of second room
            AddWallToLst(_cols, HallType.VERTICAL, _extraRoom.StartY - 1, 
                _extraRoom.StartY + _extraRoom.Height + 1, _extraRoom.StartX + _extraRoom.Width);
            
            // touching walls
            if (_room.Height > _extraRoom.Height)
                AddSectionedCollider(_room, _extraRoom, 
                    _room.StartX + _room.Width, HallType.VERTICAL);
            else
                AddSectionedCollider(_extraRoom, _room, 
                    _extraRoom.StartX - 1, HallType.VERTICAL);
        }
        
        PutDownColliders(_cols);
    }

    private void AddSectionedCollider(Room room, Room cuttingRoom, int coord, HallType orientation)
    {
        if (orientation == HallType.VERTICAL)
        {
            AddWallToLst(_cols, orientation, room.StartY-1, 
                cuttingRoom.StartY, coord);
            AddWallToLst(_cols, orientation, cuttingRoom.StartY + cuttingRoom.Height, 
                room.StartY + room.Height+1, coord);
        }
        else
        {
            AddWallToLst(_cols, orientation, room.StartX-1, 
                cuttingRoom.StartX, coord);
            AddWallToLst(_cols, orientation, cuttingRoom.StartX + cuttingRoom.Width, 
                room.StartX + room.Width+1, coord);
        }
    }

    private void AddAllSectionedColliders(Room horRoom, Room verRoom)
    {
        AddSectionedCollider(horRoom, verRoom, horRoom.StartY-1, HallType.HORIZONTAL);
        AddSectionedCollider(horRoom, verRoom, horRoom.StartY + horRoom.Height, HallType.HORIZONTAL);
        
        AddSectionedCollider(verRoom, horRoom, verRoom.StartX-1, HallType.VERTICAL);
        AddSectionedCollider(verRoom, horRoom, verRoom.StartX + verRoom.Width, HallType.VERTICAL);
    }

    private void AddParallelColliders(Room room, HallType orientation)
    {
        if (orientation == HallType.HORIZONTAL)
        {
            AddWallToLst(_cols, orientation, room.StartX, room.StartX + room.Width,
                room.StartY + room.Height);
            AddWallToLst(_cols, orientation, room.StartX, room.StartX + room.Width,
                room.StartY - 1);
        }
        else
        {
            AddWallToLst(_cols, orientation, room.StartY - 1, room.StartY + room.Height + 1,
                room.StartX + room.Width);
            AddWallToLst(_cols, orientation, room.StartY - 1, room.StartY + room.Height + 1,
                room.StartX - 1);
        }
    }

    internal override void GenerateGrid()
    {
        throw new System.NotImplementedException();
    }

    protected override Vector2Int UnityToScriptCoord(int x, int y)
    {
        throw new System.NotImplementedException();
    }
}
