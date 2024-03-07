using System.Collections.Generic;
using ProcGenScripts;
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
    public Vector2Int DistanceFromStart;
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

        gameObject.transform.position = position;
        gameObject.transform.parent = transform;
        gameObject.name = _roomData.RoomName;

        // Create Wall Colliders
        _colliders.Add(ColliderType.NorthWest, CreateCollider(ColliderType.NorthWest));
        _colliders.Add(ColliderType.NorthEast, CreateCollider(ColliderType.NorthEast));
        _colliders.Add(ColliderType.SouthWest, CreateCollider(ColliderType.SouthWest));
        _colliders.Add(ColliderType.SouthEast, CreateCollider(ColliderType.SouthEast));
        _colliders.Add(ColliderType.WestNorth, CreateCollider(ColliderType.WestNorth));
        _colliders.Add(ColliderType.WestSouth, CreateCollider(ColliderType.WestSouth));
        _colliders.Add(ColliderType.EastNorth, CreateCollider(ColliderType.EastNorth));
        _colliders.Add(ColliderType.EastSouth, CreateCollider(ColliderType.EastSouth));
        // Create Door Colliders
        _colliders.Add(ColliderType.NorthDoor, CreateCollider(ColliderType.NorthDoor));
        _colliders.Add(ColliderType.EastDoor, CreateCollider(ColliderType.EastDoor));
        _colliders.Add(ColliderType.SouthDoor, CreateCollider(ColliderType.SouthDoor));
        _colliders.Add(ColliderType.WestDoor, CreateCollider(ColliderType.WestDoor));

        if (!gameObject.GetComponent<Rigidbody2D>())
            gameObject.AddComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        CreateRoomSprite();
    }

    private void CreateRoomSprite()
    {
        // Create the room sprite
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _roomData.RoomSprite;
        spriteRenderer.sortingLayerName = "Rooms";
        spriteRenderer.sortingOrder = 0;
        // Tester code
        spriteRenderer.drawMode = SpriteDrawMode.Sliced;
        spriteRenderer.color = new Color(0.9698113f, 0.9696587f, 0.9570024f, 1f);
        spriteRenderer.size = new Vector2(_roomData.Width - _roomData.WallThickness, _roomData.Height - _roomData.WallThickness);
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
    /// Gets a weighted randoim direction from the available directions.
    /// The weight is used to determine the likelihood of a direction being chosen
    /// from [-1, 1] in the X and Y. 1, 1 would be a 50/50 chance of N or E. And
    /// 0, 0 would be an equal chance of all directions.
    /// </summary>
    /// <returns>A random direction this room can connect to and North if none.</returns>
    public RoomData.Dir GetRandomDirection(Vector2 weight)
    {
        if (availableDirections.Count == 0) return RoomData.Dir.North;

        WeightedRandom<RoomData.Dir> weightedRandom = new();

        if (availableDirections.Contains(RoomData.Dir.North))
        weightedRandom.Add(RoomData.Dir.North, 1 + weight.y);

        if (availableDirections.Contains(RoomData.Dir.South))
            weightedRandom.Add(RoomData.Dir.South, 1 - weight.y);

        if (availableDirections.Contains(RoomData.Dir.East))
            weightedRandom.Add(RoomData.Dir.East, 1 + weight.x);

        if (availableDirections.Contains(RoomData.Dir.West))
            weightedRandom.Add(RoomData.Dir.West, 1 - weight.x);

        RoomData.Dir dir = weightedRandom.GetRandom();
        Debug.Log($"Weight: {weight}");
        Debug.Log(dir);
        return dir;
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

    public ref readonly RoomData GetRoomData()
    {
        return ref _roomData;
    }

    public void StartRoom(bool lockDoors=true)
    {
        _isStart = true;
        if (lockDoors) LockDoors();
    }

    public void EndRoom(bool unlockDoors=true)
    {
        _isEnd = true;
        if (unlockDoors) UnlockDoors();
    }

    public void LockDoors()
    {
        if (_roomData.North) _colliders[ColliderType.NorthDoor].enabled = true;
        if (_roomData.East) _colliders[ColliderType.EastDoor].enabled = true;
        if (_roomData.South) _colliders[ColliderType.SouthDoor].enabled = true;
        if (_roomData.West) _colliders[ColliderType.WestDoor].enabled = true;
    }

    public void UnlockDoors()
    {
        if (_roomData.North) _colliders[ColliderType.NorthDoor].enabled = false;
        if (_roomData.East) _colliders[ColliderType.EastDoor].enabled = false;
        if (_roomData.South) _colliders[ColliderType.SouthDoor].enabled = false;
        if (_roomData.West) _colliders[ColliderType.WestDoor].enabled = false;
    }

    public void LockDoor(RoomData.Dir dir)
    {
        switch (dir)
        {
            case RoomData.Dir.North:
                _colliders[ColliderType.NorthDoor].enabled = true;
                break;
            case RoomData.Dir.East:
                _colliders[ColliderType.EastDoor].enabled = true;
                break;
            case RoomData.Dir.South:
                _colliders[ColliderType.SouthDoor].enabled = true;
                break;
            case RoomData.Dir.West:
                _colliders[ColliderType.WestDoor].enabled = true;
                break;
        }
    }

    public void UnlockDoor(RoomData.Dir dir)
    {
        switch (dir)
        {
            case RoomData.Dir.North:
                _colliders[ColliderType.NorthDoor].enabled = false;
                break;
            case RoomData.Dir.East:
                _colliders[ColliderType.EastDoor].enabled = false;
                break;
            case RoomData.Dir.South:
                _colliders[ColliderType.SouthDoor].enabled = false;
                break;
            case RoomData.Dir.West:
                _colliders[ColliderType.WestDoor].enabled = false;
                break;
        }
    }
}
