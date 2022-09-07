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
        GenerateWalls();
        GenerateColliders();

        GenerateGrid();
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
        maxHeight = maxWidth = 20;
        _minHeight = _minWidth = 10;

        _roomMin = 10;
        _hallTreshold = 15;

        _streetType = StreetType.PLAZA;
    }

    private void GenerateFloor()
    {
        Random rnd = new Random();
        var tm = _gridHolder.transform.GetChild(0).GetComponent<Tilemap>();
        
        SetRoom(ref _room, rnd);

        if (_room.Type != RoomType.ROOM)
        {
            _streetType = StreetType.STREET;
            if (Heads(rnd))
            {
                // coinflip for extra hall
                SetRoom(ref _extraRoom, rnd);
                if (_extraRoom.Type == RoomType.ROOM) MakeHall(_extraRoom, rnd);

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

    private void MakeHall(Room room, Random rnd)
    {
        if (room.Width > room.Height)
        {
            room.Type = RoomType.HOR_HALL;
            room.Width *= 2;
            room.Height = rnd.Next(_roomMin, _hallTreshold);
        }
        else
        {
            room.Type = RoomType.VER_HALL;
            room.Height *= 2;
            room.Width = rnd.Next(_roomMin, _hallTreshold);
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
            }
            else
            {
                _extraRoom.StartX = 0 + _room.Width / 2;
                if (Heads(rnd)) // bottom alignment
                    _extraRoom.StartY = _room.StartX;
                else // top alignment
                    _extraRoom.StartY = _room.StartX + _room.Height - _extraRoom.Height;
            }
        }

        _parkType = Heads(rnd);
    }

    private void SetUpFloorTiles(Tilemap tm, Random rnd)
    {
        if (_room.Type == RoomType.ROOM)
        {
            Tile tl = _parkType? grassTile[rnd.Next(0, grassTile.Count-1)] : 
                                    pavementTile[rnd.Next(0, pavementTile.Count-1)];
            
            SetTilesToMap(tl, tm, _room.StartX, _room.StartY,
                _room.StartX + _room.Width, _room.StartY + _room.Height);
        } else
        {
            Tile borderTile = _parkType? grassTile[rnd.Next(0, grassTile.Count-1)] : 
                asphaltTile[rnd.Next(0, asphaltTile.Count-1)];
            Tile walkwayTile = pavementTile[rnd.Next(0, pavementTile.Count - 1)];
            
            // single street
            if (_extraRoom.Height <= 0 && _room.Type == RoomType.HOR_HALL)
            {
                SetTilesToMap(walkwayTile, tm, _room.StartX, _room.StartY + _borderTiles,
                    _room.StartX + _room.Width, _room.StartY + _room.Height);
                SetTilesToMap(borderTile, tm, _room.StartX, _room.StartY,
                    _room.StartX + _room.Width, _room.StartY + _borderTiles);
            } else if (_extraRoom.Height <= 0 && _room.Type == RoomType.VER_HALL)
            {
                SetTilesToMap(walkwayTile, tm, _room.StartX + _borderTiles, _room.StartY,
                    _room.StartX + _room.Width, _room.StartY + _room.Height);
                SetTilesToMap(borderTile, tm, _room.StartX, _room.StartY,
                    _room.StartX + _borderTiles, _room.StartY + _room.Height);
            }
        }
        
        Debug.Log("Room starts at: " + _room.StartX + "x" + _room.StartY + " and its size is: " + 
                  _room.Width + "x" + _room.Height);
    }
    
    private void GenerateWalls()
    {
        
    }
    
    private void GenerateColliders()
    {
        
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
