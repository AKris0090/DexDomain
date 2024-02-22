using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public readonly int x, y, xSize, ySize;
    public readonly int doorSize;
    public readonly bool North, East, South, West;
    public Room NorthRoom = null, EastRoom = null, SouthRoom = null, WestRoom = null;
    private List<RoomData.Dir> availableDirections = new();
    public bool visited;
    private bool isStart;
    private bool isEnd;
    private Dictionary<string, Collider2D> colliders = new();
    public bool Started => isStart;
    public bool Ended => isEnd;
    private enum ColliderType
    {
        NorthWest,
        NorthEast,
        SouthWest,
        SouthEast,
        WestNorth,
        WestSouth,
        EastNorth,
        EastSouth,
        NorthDoor,
        EastDoor,
        SouthDoor,
        WestDoor
    }

    public Room(RoomData roomData)
    {
        x = roomData.X;
        y = roomData.Y;
        xSize = roomData.Width;
        ySize = roomData.Height;
        doorSize = roomData.DoorSize;
        North = roomData.connectDirs[RoomData.Dir.North];
        East = roomData.connectDirs[RoomData.Dir.East]; 
        South = roomData.connectDirs[RoomData.Dir.South];
        West = roomData.connectDirs[RoomData.Dir.West];
        if (North) availableDirections.Add(RoomData.Dir.North);
        if (East) availableDirections.Add(RoomData.Dir.East);
        if (South) availableDirections.Add(RoomData.Dir.South);
        if (West) availableDirections.Add(RoomData.Dir.West);
        visited = false;
        isStart = false;
        isEnd = false;
        // Create Colliders
        colliders.Add("NorthWest", CreateCollider(ColliderType.NorthWest));
    }

    /// <summary>
    /// Get a random direction from the available directions.
    /// </summary>
    /// <returns>A direction that this room can connect to or North if room has no available connections.</returns>
    public RoomData.Dir GetRandomDirection()
    {
        if (availableDirections.Count == 0) return RoomData.Dir.North;
        return availableDirections[Random.Range(0, availableDirections.Count)];
    }

    /// <summary>
    /// Creates a collider for the room in the specified slot.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private Collider2D CreateCollider(ColliderType type)
    {
        GameObject go = new GameObject();
        go.transform.position = new Vector3(x, y, 0);
        go.transform.parent = transform;
        BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
        switch (type)
        {
            case ColliderType.NorthDoor:
                collider.size = new Vector2(doorSize, doorSize);
                collider.offset = new Vector2(-xSize / 2, ySize / 2);
                break;
        }
        return collider;
    }   
}
