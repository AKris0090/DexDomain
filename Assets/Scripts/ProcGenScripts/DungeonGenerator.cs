using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public List<RoomData> spawnableRooms = new();
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
        RoomData baseRoom = spawnableRooms[Random.Range(0, spawnableRooms.Count)];
        
        Room currentRoom = new GameObject().AddComponent<Room>();
        currentRoom.Init(baseRoom, Vector3.zero);
        int roomGenCount = 1;
        while (roomGenCount < numRooms)
        {

            RoomData.Dir dir = currentRoom.GetRandomDirection(
                WeightFromDistance(currentRoom.DistanceFromStart, _branchingFactor));

            // If the room already has a room in the direction, move to that room
            if (currentRoom.GetRoomFromDirection(dir) != null)
            {
                currentRoom = currentRoom.GetRoomFromDirection(dir);
                continue;
            }
            else if (roomGenCount % _centeringFactor == 0 && currentRoom != _generatedRooms[0])
            {
                currentRoom = _generatedRooms[0];
            }

            RoomData roomType = GetRandomRoomType();
            Room newRoom = new GameObject().AddComponent<Room>();
            newRoom.Init(roomType, currentRoom.transform.position);
            ConnectRooms(currentRoom, newRoom, dir);
            _generatedRooms.Add(newRoom);

            currentRoom = newRoom;
            roomGenCount++;
            yield return new WaitForSecondsRealtime(.25f);
        }
    }

    private RoomData GetRandomRoomType()
    {
        return spawnableRooms[Random.Range(0, spawnableRooms.Count)];
    }

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
        // Calculate the position offset of the new room based on the direction
        Vector3 position = fromRoom.transform.position;
        position.x += dir switch
        {
            RoomData.Dir.East => toRoom.GetRoomData().Width,
            RoomData.Dir.West => -toRoom.GetRoomData().Width,
            _ => 0,
        };
        position.y += dir switch
        {
            RoomData.Dir.North => toRoom.GetRoomData().Height,
            RoomData.Dir.South => -toRoom.GetRoomData().Height,
            _ => 0,
        };
        toRoom.transform.position = position;
    }

    private Vector2 WeightFromDistance(Vector2Int dist, float factor)
    {
        float x = dist.x, y = dist.y;
        Vector2 weight = new()
        {
            x = Mathf.Clamp(-x / factor, -1, 1),
            y = Mathf.Clamp(-y / factor, -1, 1)
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
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
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
