using System.Collections.Generic;
using ProcGenScripts;
using UnityEngine;
using NavMeshPlus.Components;
using System.Collections;

public class Room : MonoBehaviour
{
    public Room NorthRoom = null;
    public Room SouthRoom = null;
    public Room EastRoom = null;
    public Room WestRoom = null;
    public Vector2 CameraPosition => transform.position;

    public List<GameObject> Enemies = new();
    public bool Started => _isStart;
    public bool Ended => _isEnd;
    public List<GameObject> SpawnPoints => _spawnPoints;

    public Vector2Int DistanceFromStart;
    public bool Visited;
    public GameObject BossDoor = null;

    private RoomData _roomData;
    private readonly List<RoomData.Dir> availableDirections = new();
    private bool _isStart;
    private bool _isEnd;
    private readonly Dictionary<ColliderType, GameObject> _colliders = new();
    private Dictionary<RoomData.Dir, GameObject> _doors = new();
    private SpriteRenderer _roomSprite;
    private List<GameObject> _spawnPoints = new();

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
    /// and creates colliders for the walls. Adds a static rigidbody to
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
        DistanceFromStart = Vector2Int.zero;

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


        if (!gameObject.GetComponent<Rigidbody2D>())
            gameObject.AddComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        _roomSprite = CreateRoomSprite();
    }


    public void CreateDoors()
    {
        Dictionary<RoomData.Dir, GameObject> doors = new();
        if (NorthRoom != null) doors.Add(RoomData.Dir.North, CreateDoor(RoomData.Dir.North));
        else _colliders.Add(ColliderType.NorthDoor, CreateCollider(ColliderType.NorthDoor));

        if (SouthRoom != null) doors.Add(RoomData.Dir.South, CreateDoor(RoomData.Dir.South));
        else _colliders.Add(ColliderType.SouthDoor, CreateCollider(ColliderType.SouthDoor));

        if (EastRoom != null) doors.Add(RoomData.Dir.East, CreateDoor(RoomData.Dir.East));
        else _colliders.Add(ColliderType.EastDoor, CreateCollider(ColliderType.EastDoor));

        if (WestRoom != null) doors.Add(RoomData.Dir.West, CreateDoor(RoomData.Dir.West));
        else _colliders.Add(ColliderType.WestDoor, CreateCollider(ColliderType.WestDoor));

        foreach (var door in doors)
        {
            door.Value.tag = "Door";
            _colliders.Add(door.Key switch
            {
                RoomData.Dir.North => ColliderType.NorthDoor,
                RoomData.Dir.East => ColliderType.EastDoor,
                RoomData.Dir.South => ColliderType.SouthDoor,
                RoomData.Dir.West => ColliderType.WestDoor,
                _ => ColliderType.NorthDoor,
            },
                door.Value
            );
        }

        _doors = doors;
    }

    public List<Room> GetConnectedRooms()
    {
        List<Room> connectedRooms = new();
        if (NorthRoom != null) connectedRooms.Add(NorthRoom);
        if (EastRoom != null) connectedRooms.Add(EastRoom);
        if (SouthRoom != null) connectedRooms.Add(SouthRoom);
        if (WestRoom != null) connectedRooms.Add(WestRoom);
        return connectedRooms;
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

    public RoomData.Dir GetDirectionToRoom(Room toRoom)
    {
        // If the room is connected to the specified room, return the direction
        switch (toRoom)
        {
            case { } when toRoom == NorthRoom:
                return RoomData.Dir.North;
            case { } when toRoom == EastRoom:
                return RoomData.Dir.East;
            case { } when toRoom == SouthRoom:
                return RoomData.Dir.South;
            case { } when toRoom == WestRoom:
                return RoomData.Dir.West;
        }
        // Else calculate direction based on position
        Vector3 toRoomPos = toRoom.transform.position;
        Vector3 thisRoomPos = transform.position;
        Vector3 diff = toRoomPos - thisRoomPos;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            return diff.x > 0 ? RoomData.Dir.East : RoomData.Dir.West;
        }
        return diff.y > 0 ? RoomData.Dir.North : RoomData.Dir.South;
    }

    public ref readonly RoomData GetRoomData()
    {
        return ref _roomData;
    }

    public void StartRoom(bool lockDoors = true)
    {
        if (_isEnd)
        {
            Debug.LogError("Room is already ended");
            return;
        }

        if (_isStart)
        {
            Debug.LogError("Room is already started");
            return;
        }

        _isStart = true;
        SetVisibility(true);

        if (Enemies != null)
            foreach (var enemy in Enemies)
            {
                if (enemy)
                    enemy.gameObject.SetActive(true);
            }

        if (lockDoors) LockDoors();
        else UnlockDoors();
        StartCoroutine(CheckRoomEnd());
    }

    public void EndRoom(bool unlockDoors = true)
    {
        if (_isEnd)
        {
            Debug.LogError("Room is already ended");
            return;
        }

        _isEnd = true;

        if (Enemies != null)
            foreach (var enemy in Enemies)
            {
                if (enemy)
                {
                    if (EnemyManager.Instance)
                        EnemyManager.Instance.DamageEnemy(enemy, 1000);
                    else
                        Destroy(enemy);
                }
            }

        if (unlockDoors) UnlockDoors();
        if (BossDoor)
        {
            FindFirstObjectByType<DungeonGenerator>().GetComponent<DungeonGenerator>().OpenBossDoor();
        }
    }

    public void SetVisibility(bool visible)
    {
        _roomSprite.enabled = visible;
        foreach (var door in _doors)
        {
            door.Value.GetComponent<SpriteRenderer>().enabled = visible;
            door.Value.GetComponent<BoxCollider2D>().enabled = visible;
        }
    }

    public void LockDoors()
    {
        foreach (var door in _doors)
        {
            LockDoor(door.Key);
        }
    }

    public void UnlockDoors()
    {
        foreach (var door in _doors)
        {
            UnlockDoor(door.Key);
        }
    }

    public void LockDoor(RoomData.Dir dir)
    {
        if (!_doors.ContainsKey(dir))
        {
            Debug.LogError("Door not found");
            return;
        }

        _doors[dir].TryGetComponent(out SpriteRenderer spriteRenderer);

        if (!spriteRenderer)
        {
            Debug.LogError("Door sprite renderer not found");
            return;
        }

        spriteRenderer.sprite = _roomData.DoorClosedSprite;
    }

    public void UnlockDoor(RoomData.Dir dir)
    {
        if (!_doors.ContainsKey(dir))
        {
            Debug.LogError("Door not found");
            return;
        }

        _doors[dir].TryGetComponent(out SpriteRenderer spriteRenderer);

        if (!spriteRenderer)
        {
            Debug.LogError("Door sprite renderer not found");
            return;
        }

        spriteRenderer.sprite = _roomData.DoorOpenSprite;
    }

    public bool IsDoorLocked(RoomData.Dir dir)
    {
        if (!_doors.ContainsKey(dir))
        {
            Debug.LogError("Door not found");
            return false;
        }

        return _doors[dir].GetComponent<SpriteRenderer>().sprite == _roomData.DoorClosedSprite;
    }

    public RoomData.Dir GetDoorDirection(GameObject door)
    {
        return door switch
        {
            { } when _doors.ContainsKey(RoomData.Dir.North) && door == _doors[RoomData.Dir.North]
                => RoomData.Dir.North,
            { } when _doors.ContainsKey(RoomData.Dir.South) && door == _doors[RoomData.Dir.South]
                => RoomData.Dir.South,
            { } when _doors.ContainsKey(RoomData.Dir.East) && door == _doors[RoomData.Dir.East]
                => RoomData.Dir.East,
            { } when _doors.ContainsKey(RoomData.Dir.West) && door == _doors[RoomData.Dir.West]
                => RoomData.Dir.West,
            _ => RoomData.Dir.North,
        };
    }

    public override string ToString()
    {
        string str = $"Room: {_roomData.RoomName} ";
        str += $"Position: {DistanceFromStart} ";
        str += $"Connections: ";
        if (NorthRoom) str += "North ";
        if (SouthRoom) str += "South ";
        if (EastRoom) str += "East ";
        if (WestRoom) str += "West ";
        return str;
    }

    /** 
     * Enemy Functions 
    **/

    // Overwrites previous spawn points, if any, and adds them as children to the room
    public void GenerateSpawnPoints(int count)
    {
        List<GameObject> spawnPoints = new();

        for (int i = 0; i < count; i++)
        {
            GameObject spawnPoint = new("Spawn Point");
            float width = (_roomData.Width * 0.8f);
            float height = (_roomData.Height * 0.8f);
            spawnPoint.transform.parent = transform;
            spawnPoint.transform.localPosition = new(
                Random.Range(-width / 2f, width / 2f),
                Random.Range(-height / 2f, height / 2f),
                0f
            );
            spawnPoints.Add(spawnPoint);
        }

        _spawnPoints = spawnPoints;
    }
    public void CreateNavMeshes()
    {

        foreach (var collider in _colliders)
        {
            var meshMod = collider.Value.AddComponent<NavMeshModifier>();
            meshMod.overrideArea = true;
            meshMod.AffectsAgentType(-1);
            meshMod.area = 1;
        }
        var walkBoxObj = new GameObject("Walk Box");
        walkBoxObj.transform.parent = transform;
        walkBoxObj.transform.localPosition = Vector3.zero;
        var walkBox = walkBoxObj.AddComponent<BoxCollider2D>();
        walkBox.size = new(
            _roomData.Width - _roomData.WallThickness,
            _roomData.Height - _roomData.WallThickness
        );
        walkBox.isTrigger = true;
        var mod = walkBoxObj.AddComponent<NavMeshModifier>();
        mod.overrideArea = true;
        mod.AffectsAgentType(-1);
        mod.area = 0;
        Invoke(nameof(SetWalkBoxLayer), 0.75f);
    }

    private void SetWalkBoxLayer()
    {
        var walkBoxObj = transform.Find("Walk Box").gameObject;
        walkBoxObj.layer = LayerMask.NameToLayer("Ignore Raycast");
    }


    public Vector2 GetTeleportLocation(RoomData.Dir dir)
    {
        return dir switch
        {
            RoomData.Dir.North => new(
                transform.position.x,
                transform.position.y + _roomData.Height / 2 - _roomData.WallThickness
            ),
            RoomData.Dir.East => new(
                transform.position.x + _roomData.Width / 2 - _roomData.WallThickness,
                transform.position.y
            ),
            RoomData.Dir.South => new(
                transform.position.x,
                transform.position.y - _roomData.Height / 2 + _roomData.WallThickness
            ),
            RoomData.Dir.West => new(
                transform.position.x - _roomData.Width / 2 + _roomData.WallThickness,
                transform.position.y
            ),
            _ => Vector2.zero,
        };
    }
    public Vector2 TeleportLocationToNextRoom(GameObject door)
    {
        var nextRoom = GetRoomFromDirection(GetDoorDirection(door));

        if (!nextRoom)
        {
            Debug.LogError("Next room not found");
            Debug.Break();
            return Vector2.zero;
        }

        return nextRoom.GetTeleportLocation(nextRoom.GetDirectionToRoom(this));
    }

    // Private functions

    /// <summary>
    /// Creates a collider for the room in the specified slot.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>A new collider created for the specified type</returns>
    private GameObject CreateCollider(ColliderType type, GameObject go = null)
    {
        bool isDoor = true;
        if (go == null)
        {
            go = new($"{type} Wall");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            isDoor = false;
        }
        BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
        float wallLengthH = _roomData.Width / 2 - _roomData.DoorSize / 2;
        float wallOffsetH = wallLengthH / 2 + _roomData.DoorSize / 2;
        float wallLengthV = _roomData.Height / 2 - _roomData.DoorSize / 2;
        float wallOffsetV = wallLengthV / 2 + _roomData.DoorSize / 2;
        switch (type)
        {
            // Doors
            case ColliderType.NorthDoor:
                collider.size = new(_roomData.DoorSize, _roomData.WallThickness);
                if (!isDoor)
                    go.transform.localPosition = new(0, _roomData.Height / 2);
                break;
            case ColliderType.EastDoor:
                collider.size = new(_roomData.WallThickness, _roomData.DoorSize);
                if (!isDoor)
                    go.transform.localPosition = new(_roomData.Width / 2, 0);
                break;
            case ColliderType.SouthDoor:
                collider.size = new(_roomData.DoorSize, _roomData.WallThickness);
                if (!isDoor)
                    go.transform.localPosition = new(0, -_roomData.Height / 2);
                break;
            case ColliderType.WestDoor:
                collider.size = new(_roomData.WallThickness, _roomData.DoorSize);
                if (!isDoor)
                    go.transform.localPosition = new(-_roomData.Width / 2, 0);
                break;
            // Directions
            case ColliderType.NorthWest:
                collider.size = new(wallLengthH, _roomData.WallThickness);
                go.transform.localPosition = new(-wallOffsetH, _roomData.Height / 2);
                break;
            case ColliderType.NorthEast:
                collider.size = new(wallLengthH, _roomData.WallThickness);
                go.transform.localPosition = new(wallOffsetH, _roomData.Height / 2);
                break;
            case ColliderType.SouthWest:
                collider.size = new(wallLengthH, _roomData.WallThickness);
                go.transform.localPosition = new(-wallOffsetH, -_roomData.Height / 2);
                break;
            case ColliderType.SouthEast:
                collider.size = new(wallLengthH, _roomData.WallThickness);
                go.transform.localPosition = new(wallOffsetH, -_roomData.Height / 2);
                break;
            case ColliderType.WestNorth:
                collider.size = new(_roomData.WallThickness, wallLengthV);
                go.transform.localPosition = new(-_roomData.Width / 2, wallOffsetV);
                break;
            case ColliderType.WestSouth:
                collider.size = new(_roomData.WallThickness, wallLengthV);
                go.transform.localPosition = new(-_roomData.Width / 2, -wallOffsetV);
                break;
            case ColliderType.EastNorth:
                collider.size = new(_roomData.WallThickness, wallLengthV);
                go.transform.localPosition = new(_roomData.Width / 2, wallOffsetV);
                break;
            case ColliderType.EastSouth:
                collider.size = new(_roomData.WallThickness, wallLengthV);
                go.transform.localPosition = new(_roomData.Width / 2, -wallOffsetV);
                break;
        }
        return go;
    }

    private GameObject CreateDoor(RoomData.Dir dir)
    {
        string doorName = $"{_roomData.RoomName} Door {dir}";
        GameObject door = new(doorName);
        door.transform.parent = transform;

        SpriteRenderer doorRenderer = door.AddComponent<SpriteRenderer>();
        //doorRenderer.transform.localScale = doorScale;
        doorRenderer.sprite = _roomData.DoorClosedSprite;
        doorRenderer.sortingLayerName = "Rooms";
        doorRenderer.sortingOrder = 1;
        doorRenderer.size = new Vector2(_roomData.DoorSize, _roomData.WallThickness);

        CreateCollider(dir switch
        {
            RoomData.Dir.North => ColliderType.NorthDoor,
            RoomData.Dir.East => ColliderType.EastDoor,
            RoomData.Dir.South => ColliderType.SouthDoor,
            RoomData.Dir.West => ColliderType.WestDoor,
            _ => ColliderType.NorthDoor,
        }, door);

        var collider = door.GetComponent<BoxCollider2D>();
        if (dir == RoomData.Dir.West || dir == RoomData.Dir.East)
            collider.size = new(collider.size.y, collider.size.x);

        door.transform.SetLocalPositionAndRotation(dir switch
        {
            RoomData.Dir.North => new Vector3(0, _roomData.Height / 2),
            RoomData.Dir.East => new Vector3(_roomData.Width / 2, 0),
            RoomData.Dir.South => new Vector3(0, -_roomData.Height / 2),
            RoomData.Dir.West => new Vector3(-_roomData.Width / 2, 0),
            _ => Vector3.zero,
        }, dir switch
        {
            RoomData.Dir.North => Quaternion.Euler(0, 0, 0),
            RoomData.Dir.East => Quaternion.Euler(0, 0, -90),
            RoomData.Dir.South => Quaternion.Euler(0, 0, -180),
            RoomData.Dir.West => Quaternion.Euler(0, 0, 90),
            _ => Quaternion.identity,
        });

        door.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        return door;
    }

    private SpriteRenderer CreateRoomSprite()
    {
        SpriteRenderer roomRenderer = gameObject.AddComponent<SpriteRenderer>();
        roomRenderer.sprite = _roomData.RoomSprite;
        roomRenderer.sortingLayerName = "Rooms";
        roomRenderer.sortingOrder = -1;

        return roomRenderer;
    }

    public IEnumerator CheckRoomEnd()
    {
        while (Enemies != null && Enemies.Count != 0)
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                GameObject enemy = Enemies[i];
                if (enemy == null)
                {
                    Enemies.Remove(enemy);
                    Debug.Log("Enemies: " + Enemies.Count);
                    i--;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        EndRoom();
    }

}