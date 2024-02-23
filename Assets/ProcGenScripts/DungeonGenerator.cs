using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public readonly List<RoomData> spawnableRooms = new();
    private readonly List<Room> _generatedRooms = new();
    [SerializeField]
    private int _minRooms = 1;
    [SerializeField]
    private int _maxRooms = 1;
    public int MinRooms { get => _minRooms; set => _minRooms = Math.Max(value, 0); }
    public int MaxRooms { get => _maxRooms; set => _maxRooms = Math.Max(value, _minRooms); }

    /// <summary>
    /// Generates a dungeon with a random number of rooms between the min and max room count.
    /// It chooses rooms from the spawnableRooms list and connects them together.
    /// </summary>
    /// <returns>This coroutine.</returns>
    public IEnumerator GenerateDungeon()
    {
        int numRooms = UnityEngine.Random.Range(_minRooms, _maxRooms);
        RoomData baseRoom = spawnableRooms[UnityEngine.Random.Range(0, spawnableRooms.Count)];
        
        Room currentRoom = new GameObject().AddComponent<Room>();
        currentRoom.Init(baseRoom, Vector3.zero);
        int roomGenCount = 1;
        while (roomGenCount < numRooms)
        {
            RoomData.Dir dir = currentRoom.GetRandomDirection();
            // If the room already has a room in the direction, move to that room
            if (currentRoom.GetRoomFromDirection(dir) != null)
            {
                currentRoom = currentRoom.GetRoomFromDirection(dir);
                continue;
            }
            RoomData roomType = spawnableRooms[UnityEngine.Random.Range(0, spawnableRooms.Count)];
            Room newRoom = new GameObject().AddComponent<Room>();
            Vector3 position = currentRoom.transform.position;
            // Calculate the position offset of the new room based on the direction
            position.x += dir switch
            { 
                RoomData.Dir.East => roomType.Width,
                RoomData.Dir.West => -roomType.Width,
                _ => 0,
            };
            position.y += dir switch
            {
                RoomData.Dir.North => roomType.Height,
                RoomData.Dir.South => -roomType.Height,
                _ => 0,
            };
            newRoom.Init(roomType, position);
            switch (dir)
            {
                case RoomData.Dir.North:
                    currentRoom.NorthRoom = newRoom;
                    newRoom.SouthRoom = currentRoom;
                    break;
                case RoomData.Dir.East:
                    currentRoom.EastRoom = newRoom;
                    newRoom.WestRoom = currentRoom;
                    break;
                case RoomData.Dir.South:
                    currentRoom.SouthRoom = newRoom;
                    newRoom.NorthRoom = currentRoom;
                    break;
                case RoomData.Dir.West:
                    currentRoom.WestRoom = newRoom;
                    newRoom.EastRoom = currentRoom;
                    break;
            }
            _generatedRooms.Add(newRoom);
            currentRoom = newRoom;
            roomGenCount++;
            yield return new WaitForSecondsRealtime(.25f);
        }
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
