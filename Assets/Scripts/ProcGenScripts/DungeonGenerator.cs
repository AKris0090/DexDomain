using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public List<RoomData> SpawnableRooms = new();
    private readonly List<Room> _generatedRooms = new();
    [SerializeField]
    private int _minRooms = 1;
    [SerializeField]
    private int _maxRooms = 1;
    [SerializeField]
    private float _branchingFactor = 1f;
    [SerializeField]
    private float _centeringFactor = 1f;
    public float CenteringFactor { get => _centeringFactor; set => _centeringFactor = value; }
    public float BranchingFactor { get => _branchingFactor; set => _branchingFactor = value; }
    public int MinRooms { get => _minRooms; set => _minRooms = Mathf.Max(value, 0); }
    public int MaxRooms { get => _maxRooms; set => _maxRooms = Mathf.Max(value, _minRooms); }
    public void StartGeneration()
    {
        StartCoroutine(GenerateDungeon());
    }
    /// <summary>
    /// Generates a dungeon with a random number of rooms between the min and max room count.
    /// It chooses rooms from the spawnableRooms list and connects them together.
    /// </summary>
    /// <returns>This coroutine.</returns>
    public IEnumerator GenerateDungeon()
    {
        int numRooms = Random.Range(_minRooms, _maxRooms);        
        Room currentRoom = new GameObject().AddComponent<Room>();
        currentRoom.Init(GetRandomRoomType(), Vector3.zero);
        int roomGenCount = 1;
        Room[,] roomAdjMatrix = new Room[numRooms, numRooms];
        roomAdjMatrix[0, 0] = currentRoom;
        _generatedRooms.Add(currentRoom);
        int loopGuard = 0;
        bool isCentered = false;
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
            roomAdjMatrix[roomGenCount, roomGenCount] = newRoom;
            _generatedRooms.Add(newRoom);

            foreach (Room adjRoom in newRoom.FindAdjRooms())
            {
                roomAdjMatrix[_generatedRooms.IndexOf(adjRoom), roomGenCount] = newRoom;
                roomAdjMatrix[roomGenCount, _generatedRooms.IndexOf(adjRoom)] = adjRoom;
                ConnectRooms(adjRoom, newRoom, adjRoom.GetDirectionToRoom(newRoom));
            }

            currentRoom = newRoom;
            roomGenCount++;
            isCentered = false;
            yield return new WaitForSecondsRealtime(.25f);
        }
    }

    private RoomData GetRandomRoomType()
    {
        return SpawnableRooms[Random.Range(0, SpawnableRooms.Count)];
    }

    private List<Room> GetAdjRooms(int roomIndex, Room[,] adjMatrix)
    {
        List<Room> adjRooms = new(adjMatrix.Length);
        for (int i = 0; i < adjMatrix.GetLength(0); i++)
        {
            if (adjMatrix[roomIndex, i] != null && i != roomIndex)
                adjRooms.Add(adjMatrix[roomIndex, i]);
        }
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
            foreach (BoxCollider2D collider in room.GetComponents<BoxCollider2D>())
            {
                RaycastHit2D[] raycastHits = new RaycastHit2D[1];
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
                if (collider.Raycast(Vector2.up, raycastHits, 1f) > 0)
                {
                    GameObject hitObject = raycastHits[0].transform.gameObject;
                    if (hitObject == collider.gameObject)
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
    private void OnDrawGizmos()
    {
        DebugDraw();
    }
}
