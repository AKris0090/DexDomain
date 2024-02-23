using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Room NorthRoom { get; set; } = null;
    public Room SouthRoom { get; set; } = null;
    public Room EastRoom { get; set; } = null;
    public Room WestRoom { get; set; } = null;
    private RoomData roomData;
    private readonly List<RoomData.Dir> availableDirections = new();
    public bool visited;
    private bool isStart;
    private bool isEnd;
    private readonly Dictionary<ColliderType, Collider2D> colliders = new();
    // Getters for the room state flags
    public bool Started => isStart;
    public bool Ended => isEnd;
    // Specifies the colliders for the room
    // First direction in name is the direction to the wall
    // Second direction in name is relative to the doorway
    // Example: NorthWest is a collider on the north wall to the west of the North door
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

    public void Init(RoomData roomDataObject, Vector3 position)
    {
        roomData = roomDataObject;
        if (roomData.North) availableDirections.Add(RoomData.Dir.North);
        if (roomData.East) availableDirections.Add(RoomData.Dir.East);
        if (roomData.South) availableDirections.Add(RoomData.Dir.South);
        if (roomData.West) availableDirections.Add(RoomData.Dir.West);
        visited = false;
        isStart = false;
        isEnd = false;

        // Create the room game object
        gameObject.transform.position = position;
        gameObject.transform.parent = transform;
        gameObject.name = roomData.RoomName;
        // Create Colliders
        colliders.Add(ColliderType.NorthWest, CreateCollider(ColliderType.NorthWest));
        colliders.Add(ColliderType.NorthEast, CreateCollider(ColliderType.NorthEast));
        colliders.Add(ColliderType.SouthWest, CreateCollider(ColliderType.SouthWest));
        colliders.Add(ColliderType.SouthEast, CreateCollider(ColliderType.SouthEast));
        colliders.Add(ColliderType.WestNorth, CreateCollider(ColliderType.WestNorth));
        colliders.Add(ColliderType.WestSouth, CreateCollider(ColliderType.WestSouth));
        colliders.Add(ColliderType.EastNorth, CreateCollider(ColliderType.EastNorth));
        colliders.Add(ColliderType.EastSouth, CreateCollider(ColliderType.EastSouth));
        // Doors
        colliders.Add(ColliderType.NorthDoor, CreateCollider(ColliderType.NorthDoor));
        colliders.Add(ColliderType.EastDoor, CreateCollider(ColliderType.EastDoor));
        colliders.Add(ColliderType.SouthDoor, CreateCollider(ColliderType.SouthDoor));
        colliders.Add(ColliderType.WestDoor, CreateCollider(ColliderType.WestDoor));
        // Add rigidbody to the room game object
        gameObject.AddComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }

    /// <summary>
    /// Get a random direction from the available directions.
    /// </summary>
    /// <returns>The Room corresponding to the random direction.</returns>
    public Room GetRandomRoomConnection()
    {
        if (availableDirections.Count == 0) return null;
        return availableDirections[Random.Range(0, availableDirections.Count)] switch
        {
            RoomData.Dir.North => NorthRoom,
            RoomData.Dir.East => EastRoom,
            RoomData.Dir.South => SouthRoom,
            RoomData.Dir.West => WestRoom,
            _ => null,
        };
    }

    public RoomData.Dir GetRandomDirection()
    {
        if (availableDirections.Count == 0) return RoomData.Dir.North;
        return availableDirections[Random.Range(0, availableDirections.Count)];
    }

    public Room GetRoomFromDirection(RoomData.Dir dir)
    {
        return dir switch
        {
            RoomData.Dir.North => NorthRoom,
            RoomData.Dir.East => EastRoom,
            RoomData.Dir.South => SouthRoom,
            RoomData.Dir.West => WestRoom,
            _ => null,
        };
    }

    /// <summary>
    /// Creates a collider for the room in the specified slot.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private Collider2D CreateCollider(ColliderType type)
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        float wallLength = roomData.Width / 2 - roomData.DoorSize.x / 2;
        switch (type)
        {
            // Doors
            case ColliderType.NorthDoor:
                collider.size = new Vector2(roomData.DoorSize.x, roomData.DoorSize.y);
                collider.offset = new Vector2(0, roomData.Height / 2);
                break;
            case ColliderType.EastDoor:
                collider.size = new Vector2(roomData.DoorSize.y, roomData.DoorSize.x);
                collider.offset = new Vector2(roomData.Width / 2, 0);
                break;
            case ColliderType.SouthDoor:
                collider.size = new Vector2(roomData.DoorSize.x, roomData.DoorSize.y);
                collider.offset = new Vector2(0, -roomData.Height / 2);
                break;
            case ColliderType.WestDoor:
                collider.size = new Vector2(roomData.DoorSize.y, roomData.DoorSize.x);
                collider.offset = new Vector2(-roomData.Width / 2, 0);
                break;
            // Directions
            case ColliderType.NorthWest:
                collider.size = new Vector2(roomData.Width / 2 - roomData.DoorSize.x / 2, roomData.WallThickness);
                collider.offset = new Vector2(-((roomData.Width / 2 - roomData.DoorSize.x / 2) / 2 + roomData.DoorSize.x / 2), roomData.Height / 2);
                break;
            case ColliderType.NorthEast:
                collider.size = new Vector2(roomData.Width / 2 - roomData.DoorSize.x, roomData.WallThickness);
                collider.offset = new Vector2(roomData.Width / 2, roomData.Height / 2);
                break;
            case ColliderType.SouthWest:
                collider.size = new Vector2(roomData.Width / 2 - roomData.DoorSize.x, roomData.WallThickness);
                collider.offset = new Vector2(-roomData.DoorSize.x, -roomData.Height / 2);
                break;
            case ColliderType.SouthEast:
                collider.size = new Vector2(roomData.Width / 2 - roomData.DoorSize.x, roomData.WallThickness);
                collider.offset = new Vector2(roomData.DoorSize.x, -roomData.Height / 2);
                break;
            case ColliderType.WestNorth:
                collider.size = new Vector2(roomData.WallThickness, roomData.Height / 2 - roomData.DoorSize.x);
                collider.offset = new Vector2(-roomData.Width / 2, roomData.DoorSize.x);
                break;
            case ColliderType.WestSouth:
                collider.size = new Vector2(roomData.WallThickness, roomData.Height / 2 - roomData.DoorSize.x);
                collider.offset = new Vector2(-roomData.Width / 2, -roomData.DoorSize.x);
                break;
            case ColliderType.EastNorth:
                collider.size = new Vector2(roomData.WallThickness, roomData.Height / 2 - roomData.DoorSize.x);
                collider.offset = new Vector2(roomData.Width / 2, roomData.DoorSize.x / 2);
                break;
            case ColliderType.EastSouth:
                collider.size = new Vector2(roomData.WallThickness, roomData.Height / 2 - roomData.DoorSize.x);
                collider.offset = new Vector2(roomData.Width / 2, -roomData.DoorSize.x / 2);
                break;
        }
        return collider;
    }   
}
