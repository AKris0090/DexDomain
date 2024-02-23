using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public List<RoomData> spawnableRooms = new();
    private List<Room> generatedRooms = new();
    public int minRooms = 1;
    public int maxRooms = 2;

    public void Start()
    {
        StartCoroutine(GenerateDungeon());
    }

    //IEnumerator Start()
    //{
    //    yield return StartCoroutine(GenerateDungeon());
    //}

    IEnumerator GenerateDungeon()
    {
        int numRooms = Random.Range(minRooms, maxRooms);
        RoomData baseRoom = spawnableRooms[Random.Range(0, spawnableRooms.Count)];
        
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
            RoomData roomType = spawnableRooms[Random.Range(0, spawnableRooms.Count)];
            Room newRoom = new GameObject().AddComponent<Room>();
            Vector3 position = currentRoom.transform.position;
            // Calculate the position offset of the new room based on the direction
            position.x += dir switch
            { 
                RoomData.Dir.East => roomType.Width / 2,
                RoomData.Dir.West => -roomType.Width / 2,
                _ => 0,
            };
            position.y += dir switch
            {
                RoomData.Dir.North => roomType.Height / 2,
                RoomData.Dir.South => -roomType.Height / 2,
                _ => 0,
            };
            newRoom.Init(roomType, position);
            generatedRooms.Add(newRoom);
            roomGenCount++;
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private void DebugDraw()
    {
        foreach (var item in generatedRooms)
        {
            foreach (BoxCollider2D collider in item.GetComponents<BoxCollider2D>())
            {
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }

    private void OnDrawGizmos()
    {
        DebugDraw();
    }
}
