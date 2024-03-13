using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NavMeshPlus.Components;
using NavMeshPlus.Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DungeonGenerator : MonoBehaviour
{
    public float CenteringFactor { get => _centeringFactor; set => _centeringFactor = value; }
    public float BranchingFactor { get => _branchingFactor; set => _branchingFactor = value; }
    public int MinRooms { get => _minRooms; set => _minRooms = Mathf.Max(value, 0); }
    public int MaxRooms { get => _maxRooms; set => _maxRooms = Mathf.Max(value, _minRooms); }

    public List<RoomData> SpawnableRooms = new();
    public GameObject BossRoomPrefab;

    private GameObject _root;
    private readonly List<Room> _generatedRooms = new();
    [SerializeField]
    private int _minRooms = 1;
    [SerializeField]
    private int _maxRooms = 1;
    [SerializeField]
    private int _minEnemies = 2;
    [SerializeField]
    private int _maxEnemies = 4;
    [SerializeField]
    private float _branchingFactor = 1f;
    [SerializeField]
    private float _centeringFactor = 1f;
    public void StartGeneration() => StartCoroutine(GenerateDungeon());

    public void DestroyDungeon()
    {
        foreach (Room room in _generatedRooms)
        {
            Destroy(room.gameObject);
        }
        _generatedRooms.Clear();
        if (_root)
            Destroy(_root);
    }

    /// <summary>
    /// Generates a dungeon with a random number of rooms between the min and max room count.
    /// It chooses rooms from the spawnableRooms list and connects them together.
    /// </summary>
    /// <returns>This coroutine.</returns>
    public IEnumerator GenerateDungeon()
    {
        int numRooms = UnityEngine.Random.Range(_minRooms, _maxRooms);        
        Room currentRoom = new GameObject().AddComponent<Room>();
        currentRoom.Init(GetRandomRoomType(), Vector3.zero);
        int roomGenCount = 1;
        Dictionary<(int, int), Room> roomMatrix = new()
        {
            [(0, 0)] = currentRoom
        };
        _generatedRooms.Add(currentRoom);
        int loopGuard = 0;
        bool isCentered = false;
        // First pass to create rooms
        while (roomGenCount < numRooms)
        {
            if (loopGuard++ > 1000)
            {
                Debug.LogError("Infinite loop detected in dungeon generation.");
                break;
            }
            if (roomGenCount % _centeringFactor == 0 && !isCentered)
            {
                currentRoom = _generatedRooms[0];
                isCentered = true;
            }
            RoomData.Dir dir = currentRoom.GetRandomDirection(
                WeightFromDistance(currentRoom.DistanceFromStart, _branchingFactor));

            // If the room already has a room in the direction, move to that room
            if (currentRoom.GetRoomFromDirection(dir) != null)
            {
                currentRoom = currentRoom.GetRoomFromDirection(dir);
                continue;
            }

            Room newRoom = new GameObject().AddComponent<Room>();
            newRoom.Init(GetRandomRoomType(), currentRoom.transform.position);
            ConnectRooms(currentRoom, newRoom, dir);
            SnapRoomsTogether(currentRoom, newRoom, dir);
            roomMatrix[XY(newRoom)] = newRoom;
            _generatedRooms.Add(newRoom);

            foreach (Room adjRoom in GetAdjRooms(XY(newRoom), roomMatrix))
            {
                ConnectRooms(adjRoom, newRoom, adjRoom.GetDirectionToRoom(newRoom));
            }

            currentRoom = newRoom;
            roomGenCount++;
            isCentered = false;

            yield return new WaitForEndOfFrame();
        }
        // Second pass to create Boss room
        Debug.Log("Generating boss room");
        var bossRoom = Instantiate(BossRoomPrefab, new Vector3(50000f, 50000f), Quaternion.identity);
        var pair = FindFurthestRoom();
        Room furthestRoom = pair.Item1;
        RoomData.Dir dirToBoss = pair.Item2;
        
        // Third pass to create doors
        Debug.Log("Creating doors and nav meshes");
        StartCoroutine(CreateDoorsAndMeshes());

        // Fourth pass to spawn enemies
        Debug.Log("Spawning enemies");
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator CreateDoorsAndMeshes()
    {
        if (!_root)
            _root = new GameObject("NavMesh");
        var navMesh = _root.AddComponent<NavMeshSurface>();
        navMesh.collectObjects = CollectObjects.Children;
        navMesh.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        navMesh.agentTypeID = 0;
        navMesh.defaultArea = 0;
        //navMesh.size = new Vector2(_roomData.Width, _roomData.Height);
        var navCollector = _root.AddComponent<CollectSources2d>();
        _root.transform.rotation = Quaternion.Euler(-90, 0, 0);
        var settings = navMesh.GetBuildSettings();
        settings.agentRadius = 2f;
        settings.agentHeight = 2f;
        settings.agentClimb = 0.75f;
        settings.minRegionArea = 1f;
        settings.tileSize = 8;

        foreach (Room room in _generatedRooms)
        {
            room.transform.parent = _root.transform;
            room.CreateDoors();
            room.CreateNavMeshes();
            yield return new WaitForEndOfFrame();
        }

        navMesh.BuildNavMesh();
    }

    private IEnumerator SpawnEnemies()
    {
        foreach (Room room in _generatedRooms)
        {
            room.GenerateSpawnPoints(UnityEngine.Random.Range(_minEnemies, _maxEnemies));
            EnemyManager.Instance.GetRandomEnemies(
                UnityEngine.Random.Range(_minEnemies, _maxEnemies), 
                out room.Enemies
            );
            foreach (var (spawn, enemy) in room.SpawnPoints.Zip(room.Enemies, 
                (s, e) => new Tuple<GameObject, Enemy>(s, e)))
            {
                GameObject newEnemy = Instantiate(enemy.gameObject);
                newEnemy.transform.position = spawn.transform.position;
                newEnemy.SetActive(false);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private (int, int) XY(Room room) => (room.DistanceFromStart.x, room.DistanceFromStart.y);

    Tuple<Room, RoomData.Dir> FindFurthestRoom()
    {
        Room furthestRoom = _generatedRooms[0];
        int maxDistance = 0;
        foreach (var room in _generatedRooms)
        {
            if (room.GetConnectedRooms().Count == 4)
                continue;

            if (room.DistanceFromStart.sqrMagnitude > maxDistance)
            {
                maxDistance = room.DistanceFromStart.sqrMagnitude;
                furthestRoom = room;
            }
        }
        List<RoomData.Dir> dirs = new() { RoomData.Dir.North, RoomData.Dir.East, RoomData.Dir.South, RoomData.Dir.West };
        foreach (RoomData.Dir dir in dirs)
        {
            if (furthestRoom.GetRoomFromDirection(dir) == null)
                return new Tuple<Room, RoomData.Dir>(furthestRoom, dir);
        }
        return null;
    }

    private RoomData GetRandomRoomType() => SpawnableRooms[UnityEngine.Random.Range(0, SpawnableRooms.Count)];

    private List<Room> GetAdjRooms((int, int) coord, Dictionary<(int, int), Room> matrix)
    {
        List<Room> adjRooms = new(4);

        if (matrix.ContainsKey((coord.Item1, coord.Item2 + 1)))
            adjRooms.Add(matrix[(coord.Item1, coord.Item2 + 1)]);

        if (matrix.ContainsKey((coord.Item1, coord.Item2 - 1)))
            adjRooms.Add(matrix[(coord.Item1, coord.Item2 - 1)]);

        if (matrix.ContainsKey((coord.Item1 + 1, coord.Item2)))
            adjRooms.Add(matrix[(coord.Item1 + 1, coord.Item2)]);

        if (matrix.ContainsKey((coord.Item1 - 1, coord.Item2)))
            adjRooms.Add(matrix[(coord.Item1 - 1, coord.Item2)]);

        return adjRooms;
    }

    /// <summary>
    /// Stiches room connections together.
    /// </summary>
    /// <param name="dir">Direction from the fromRoom to the toRoom</param>
    private void ConnectRooms(Room fromRoom, Room toRoom, RoomData.Dir dir)
    {
        switch (dir)
        {
            case RoomData.Dir.North:
                fromRoom.NorthRoom = toRoom;
                toRoom.SouthRoom = fromRoom;
                toRoom.DistanceFromStart = fromRoom.DistanceFromStart + Vector2Int.up;
                break;
            case RoomData.Dir.East:
                fromRoom.EastRoom = toRoom;
                toRoom.WestRoom = fromRoom;
                toRoom.DistanceFromStart = fromRoom.DistanceFromStart + Vector2Int.right;
                break;
            case RoomData.Dir.South:
                fromRoom.SouthRoom = toRoom;
                toRoom.NorthRoom = fromRoom;
                toRoom.DistanceFromStart = fromRoom.DistanceFromStart + Vector2Int.down;
                break;
            case RoomData.Dir.West:
                fromRoom.WestRoom = toRoom;
                toRoom.EastRoom = fromRoom;
                toRoom.DistanceFromStart = fromRoom.DistanceFromStart + Vector2Int.left;
                break;
        }
    }

    /// <summary>
    /// Moves the room to the correct grid position based on the direction
    /// from the stationary room and its position.
    /// </summary>
    private void SnapRoomsTogether(Room staticRoom, Room roomToMove, RoomData.Dir dir)
    {
        // Calculate the position offset of the new room based on the direction
        Vector3 position = staticRoom.transform.position;
        position.x += dir switch
        {
            RoomData.Dir.East => roomToMove.GetRoomData().Width,
            RoomData.Dir.West => -roomToMove.GetRoomData().Width,
            _ => 0,
        };
        position.y += dir switch
        {
            RoomData.Dir.North => roomToMove.GetRoomData().Height,
            RoomData.Dir.South => -roomToMove.GetRoomData().Height,
            _ => 0,
        };
        roomToMove.transform.position = position;
    }

    private Vector2 WeightFromDistance(Vector2Int dist, float factor)
    {
        float x = dist.x, y = dist.y;
        Vector2 weight = new()
        {
            x = Mathf.Clamp(x / factor, -1, 1),
            y = Mathf.Clamp(y / factor, -1, 1)
        };
        return weight;
    }

    /// <summary>
    /// Draw the colliders of the generated rooms in the editor.
    /// </summary>
    private void DebugDraw()
    {
        foreach (Room room in _generatedRooms)
        {
            foreach (BoxCollider2D collider in room.GetComponentsInChildren<BoxCollider2D>())
            {
                RaycastHit2D[] raycastHits = new RaycastHit2D[1];
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
                if (collider.Raycast(Vector2.up, raycastHits, 1f) > 0)
                {
                    GameObject hitObject = raycastHits[0].transform.gameObject;
                    if (hitObject == collider.gameObject ||hitObject == collider.transform.parent.gameObject)
                        continue;
                    
                    Room roomHit = hitObject.GetComponent<Room>();
                    Gizmos.color = Color.red;
                    if (roomHit.GetConnectedRooms().Contains(room))
                    {
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawWireCube(raycastHits[0].collider.bounds.center,
                        raycastHits[0].collider.bounds.size);
                }
            }
        }
    }

    /// <summary>
    /// Draw the colliders of the generated rooms in the editor every unity message callback.
    /// </summary>
    private void OnDrawGizmos() => DebugDraw();
}
