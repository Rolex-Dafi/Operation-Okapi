using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class OfficeRoomGenerator : MapGenerator
{
    [Header("Maximum width and height of the generated rooms.")]
    public int maxWidth = 20;
    public int maxHeight = 20;

    private int _minWidth = 4;
    private int _minHeight = 4;

    private struct RoomSize
    {
        internal int RightWidth;
        internal int RightHeight;
        internal int LeftWidth;
        internal int LeftHeight;
    };

    private RoomSize _roomSize;

    private struct Wall
    {
        internal bool Horizontal;
        internal int Start;
        internal int End;
        internal int Height;

        internal void setValues(bool b, int s, int e, int h)
        {
            Horizontal = b;
            Start = s;
            End = e;
            Height = h;
        }
    }

    private List<Wall> walls;

    [Header("Tile sets to use for generation.")]
    public List<Tile> tileLst;

    public List<Tile> horWallLst;
    public List<Tile> verWallLst;
    public override void Generate()
    {
        base.Generate();
        GenerateOfficeFloor();
        GenerateOfficeWalls();
    }

    private void GenerateOfficeFloor()
    {
        if (tileLst.Count <= 0) return;
        
        var rand = new Random();
        Tilemap tm = _gridHolder.transform.GetChild(0).GetComponent<Tilemap>();
        walls = new List<Wall>();

        _roomSize.LeftHeight = rand.Next(_minHeight, maxHeight);
        _roomSize.LeftWidth = rand.Next(_minWidth, maxWidth);

        SetTilesToMap(tileLst[rand.Next(0, tileLst.Count)], tm, -_roomSize.LeftWidth, 0, 0, _roomSize.LeftHeight);

        _roomSize.RightHeight = rand.Next(_minHeight, maxHeight);
        _roomSize.RightWidth = rand.Next(_minWidth, maxWidth);
        
        
        if (_roomSize.RightHeight < _roomSize.LeftHeight)
        {
            // cup
            SetTilesToMap(tileLst[rand.Next(0, tileLst.Count)], tm, 1, 0, _roomSize.RightWidth, _roomSize.RightHeight);
            
            AddWallToLst(true, -_roomSize.LeftWidth, 0, _roomSize.LeftHeight + 1);
            AddWallToLst(false, _roomSize.RightHeight + 1, _roomSize.LeftHeight, 1);
            AddWallToLst(true, 1, _roomSize.RightWidth, _roomSize.RightHeight + 1);
            AddWallToLst(false, 0, _roomSize.RightHeight, _roomSize.RightWidth + 1);
        }
        else
        {
            // roof
            SetTilesToMap(tileLst[rand.Next(0, tileLst.Count)], tm, 1, -(_roomSize.RightHeight - _roomSize.LeftHeight),
                _roomSize.RightWidth, _roomSize.LeftHeight);
            
            AddWallToLst(true, -_roomSize.LeftWidth, _roomSize.RightWidth, _roomSize.LeftHeight + 1);
            AddWallToLst(false, -(_roomSize.RightHeight - _roomSize.LeftHeight), _roomSize.LeftHeight, _roomSize.RightWidth + 1);
        }
    }

    private void GenerateOfficeWalls()
    {
        if (horWallLst.Count <= 0 || verWallLst.Count <= 0 || walls.Count <= 1)
        {
            Debug.Log("No walls to build.");
            return;
        }

        var rand = new Random();
        Tilemap horWalls = _gridHolder.transform.GetChild(1).GetComponent<Tilemap>();
        Tilemap verWalls = _gridHolder.transform.GetChild(2).GetComponent<Tilemap>();

        int idx = 0;
        
        foreach (var wall in walls)
        {
            if (wall.Horizontal)
            {
                SetTilesToMap(horWallLst[0], horWalls, wall.Start, wall.Height, wall.End, wall.Height);
                horWalls.SetTile(new Vector3Int(wall.End, wall.Height, 0), horWallLst[1]);
            }
            else
            {
                SetTilesToMap(verWallLst[0], verWalls, wall.Height, wall.Start, wall.Height, wall.End);
                verWalls.SetTile(new Vector3Int(wall.Height, wall.End, 0), verWallLst[1]);
            }
            
            switch (idx)
            {
                case 1 when walls.Count > 2:
                    verWalls.SetTile(new Vector3Int(wall.Height, wall.Start, 0),
                        wall.End == wall.Start ? verWallLst[3] : verWallLst[2]);
                    break;
                case 2:
                    horWalls.SetTile(new Vector3Int(wall.Start, wall.Height, 0), horWallLst[2]);
                    break;
            }

            idx++;
        }
        
        
    }

    private void AddWallToLst(bool horizontal, int start, int end, int height)
    {
        Wall newWall = new Wall();
        newWall.setValues(horizontal, start, end, height);
        walls.Add(newWall);
    }

    private void SetTilesToMap(Tile tile, Tilemap tileMap, int startX, int startY, int finX, int finY)
    {
        for (int x = startX; x <= finX; x++)
        {
            for (int y = startY; y <= finY; y++)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
