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
    
    public override void Generate()
    {
        base.Generate();
        CleanUpMall();
        
        GenerateFloor();
        GenerateColliders();
        
        //GenerateGrid();
    }

    protected override void SetUpParameters()
    {
        if (maxHeight <= 0) maxHeight = 20;
        if (maxWidth <= 0) maxWidth = 20;
        
        _minHeight = _minWidth = 4;
        
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
            AddParallelColliders(_room, HallType.VERTICAL);
            AddParallelColliders(_extraRoom, HallType.HORIZONTAL);
            
            AddAllSectionedColliders(_room, _extraRoom);

        } else if (_extraRoom.Height <= 0)
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
