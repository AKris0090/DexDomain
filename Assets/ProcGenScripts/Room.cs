using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Room NorthRoom { get; set; } = null;
    public Room SouthRoom { get; set; } = null;
    public Room EastRoom { get; set; } = null;
    public Room WestRoom { get; set; } = null;
    private RoomData _roomData;
    private readonly List<RoomData.Dir> availableDirections = new();
    public bool Visited;
    private bool _isStart;
    private bool _isEnd;
    private readonly Dictionary<ColliderType, Collider2D> _colliders = new();
    // Getters for the room state flags
    public bool Started => _isStart;
    public bool Ended => _isEnd;
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

    // <summary>
    /// Initializes the room object with the specified room data and position.
    /// Sets the available directions from the specifications of theRoomData object
    /// and creates colliders for the walls and doors. Adds a static rigidbody to
    /// the room gameObject and configures its name and position.
    /// </summary>
    /// <param name="roomDataObject">The RoomData to configure this room with.</param>
    /// <param name="position">The position of the center of the room relative to its parent.</param>
    public void Init(RoomData roomDataObject, Vector3 position)
    {
        _roomData = roomDataObject;
        if (_roomData.North) availableDirections.Add(RoomData.Dir.North);
        if (_roomData.East) availableDirections.Add(RoomData.Dir.East);
        if (_roomData.South) availableDirections.Add(RoomData.Dir.South);
        if (_roomData.West) availableDirections.Add(RoomData.Dir.West);
        Visited = false;
        _isStart = false;
        _isEnd = false;

        // Create the room game object
        gameObject.transform.position = position;
        gameObject.transform.parent = transform;
        gameObject.name = _roomData.RoomName;
        // Create Colliders
        _colliders.Add(ColliderType.NorthWest, CreateCollider(ColliderType.NorthWest));
        _colliders.Add(ColliderType.NorthEast, CreateCollider(ColliderType.NorthEast));
        _colliders.Add(ColliderType.SouthWest, CreateCollider(ColliderType.SouthWest));
        _colliders.Add(ColliderType.SouthEast, CreateCollider(ColliderType.SouthEast));
        _colliders.Add(ColliderType.WestNorth, CreateCollider(ColliderType.WestNorth));
        _colliders.Add(ColliderType.WestSouth, CreateCollider(ColliderType.WestSouth));
        _colliders.Add(ColliderType.EastNorth, CreateCollider(ColliderType.EastNorth));
        _colliders.Add(ColliderType.EastSouth, CreateCollider(ColliderType.EastSouth));
        // Doors
        _colliders.Add(ColliderType.NorthDoor, CreateCollider(ColliderType.NorthDoor));
        _colliders.Add(ColliderType.EastDoor, CreateCollider(ColliderType.EastDoor));
        _colliders.Add(ColliderType.SouthDoor, CreateCollider(ColliderType.SouthDoor));
        _colliders.Add(ColliderType.WestDoor, CreateCollider(ColliderType.WestDoor));
        // Add rigidbody to the room game object if it doesnt have one already
        if (!gameObject.GetComponent<Rigidbody2D>())
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

    /// <summary>
    /// Gets a randoim direction from the available directions.
    /// </summary>
    /// <returns>A random direction this room can connect to and North if none.</returns>
    public RoomData.Dir GetRandomDirection()
    {
        if (availableDirections.Count == 0) return RoomData.Dir.North;
        return availableDirections[Random.Range(0, availableDirections.Count)];
    }

    /// <summary>
    /// Returns the room in the specified direction.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns>The room Monobehavior for the specified direction. May be null.</returns>
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
    /// <returns>A new collider created for the specified type</returns>
    private Collider2D CreateCollider(ColliderType type)
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        float wallLengthH = _roomData.Width / 2 - _roomData.DoorSize.x / 2;
        float wallOffsetH = wallLengthH / 2 + _roomData.DoorSize.x / 2;
        float wallLengthV = _roomData.Height / 2 - _roomData.DoorSize.x / 2;
        float wallOffsetV = wallLengthV / 2 + _roomData.DoorSize.x / 2;
        switch (type)
        {
            // Doors
            case ColliderType.NorthDoor:
                collider.size = new Vector2(_roomData.DoorSize.x, _roomData.DoorSize.y);
                collider.offset = new Vector2(0, _roomData.Height / 2);
                break;
            case ColliderType.EastDoor:
                collider.size = new Vector2(_roomData.DoorSize.y, _roomData.DoorSize.x);
                collider.offset = new Vector2(_roomData.Width / 2, 0);
                break;
            case ColliderType.SouthDoor:
                collider.size = new Vector2(_roomData.DoorSize.x, _roomData.DoorSize.y);
                collider.offset = new Vector2(0, -_roomData.Height / 2);
                break;
            case ColliderType.WestDoor:
                collider.size = new Vector2(_roomData.DoorSize.y, _roomData.DoorSize.x);
                collider.offset = new Vector2(-_roomData.Width / 2, 0);
                break;
            // Directions
            case ColliderType.NorthWest:
                collider.size = new Vector2(wallLengthH, _roomData.WallThickness);
                collider.offset = new Vector2(-wallOffsetH, _roomData.Height / 2);
                break;
            case ColliderType.NorthEast:
                collider.size = new Vector2(wallLengthH, _roomData.WallThickness);
                collider.offset = new Vector2(wallOffsetH, _roomData.Height / 2);
                break;
            case ColliderType.SouthWest:
                collider.size = new Vector2(wallLengthH, _roomData.WallThickness);
                collider.offset = new Vector2(-wallOffsetH, -_roomData.Height / 2);
                break;
            case ColliderType.SouthEast:
                collider.size = new Vector2(wallLengthH, _roomData.WallThickness);
                collider.offset = new Vector2(wallOffsetH, -_roomData.Height / 2);
                break;
            case ColliderType.WestNorth:
                collider.size = new Vector2(_roomData.WallThickness, wallLengthV);
                collider.offset = new Vector2(-_roomData.Width / 2, wallOffsetV);
                break;
            case ColliderType.WestSouth:
                collider.size = new Vector2(_roomData.WallThickness, wallLengthV);
                collider.offset = new Vector2(-_roomData.Width / 2, -wallOffsetV);
                break;
            case ColliderType.EastNorth:
                collider.size = new Vector2(_roomData.WallThickness, wallLengthV);
                collider.offset = new Vector2(_roomData.Width / 2, wallOffsetV);
                break;
            case ColliderType.EastSouth:
                collider.size = new Vector2(_roomData.WallThickness, wallLengthV);
                collider.offset = new Vector2(_roomData.Width / 2, -wallOffsetV);
                break;
        }
        return collider;
    }   
}
