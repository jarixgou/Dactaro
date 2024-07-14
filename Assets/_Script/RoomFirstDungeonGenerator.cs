using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0,10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    public TypeOfRooms _TypeOfRooms;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, 
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        Debug.LogWarning(roomsList.Count);
        
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        HashSet<Vector2Int> corridorFloor = new HashSet<Vector2Int>();
        HashSet<Vector2Int> floorSafePlace = new HashSet<Vector2Int>();
        
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }
        
        floorSafePlace = CreateRoomsSafePlace(roomsList);
        roomsList.RemoveAt(0);
        Debug.LogWarning(roomsList.Count);

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList); 
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }
        

        List<List<Vector2Int>> corridors = ConnectRooms(roomCenters);
        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorBrush(corridors[i]);
            corridorFloor.UnionWith(corridors[i]);
        }
        

        _tilemapVisualizer.PaintCorridorPath(corridorFloor);
        _tilemapVisualizer.PaintFloorTiles(floor);
        _tilemapVisualizer.PaintSafePlaceTiles(floorSafePlace);
        _spawnManager.SpwanEssantials(floor);
        _spawnManager.SpawnSafePlace(floorSafePlace);
        floor.UnionWith(floorSafePlace);
        foreach (var vector2Ints in corridors) floor.UnionWith(vector2Ints);
        WallGenerator.CreateWalls(floor, _tilemapVisualizer);
    }

    private HashSet<Vector2Int> CreateRoomsSafePlace(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        var roomBounds = roomsList[0];
        var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), 
            Mathf.RoundToInt(roomBounds.center.y));
        var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
        
        foreach (var position in roomFloor)
        {
            if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && 
                position.y >= (roomBounds.yMin + offset) && position.y <= (roomBounds.yMax - offset))
            {
                floor.Add(position);
            }
        }
        
        return floor;
    }

    private List<Vector2Int> IncreaseCorridorBrush(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }

        return newCorridor;
    }
    

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomsBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomsBounds.center.x),
                Mathf.RoundToInt(roomsBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomsBounds.xMin + offset) && position.x <= (roomsBounds.xMax - offset) && 
                    position.y >= (roomsBounds.yMin + offset) && position.y <= (roomsBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    private List<List<Vector2Int>> ConnectRooms(List<Vector2Int> roomCenters)
    {
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            List<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.Add(newCorridor);
        }

        return corridors;
    }

    private List<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }

            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }

            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2Int.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
