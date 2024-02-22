using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "ProcGen/Room", order = 1)]
public class RoomData : ScriptableObject
{
    public enum Dir
    {
        North,
        East,
        South,
        West
    }
    // Editor exposed setter variables
    [Header("Room Position")]
    [SerializeField]
    private int x;
    [SerializeField]
    private int y;
    [Header("Room Size")]
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [Header("Room Connections")]
    [SerializeField]
    private bool North;
    [SerializeField]
    private bool East;
    [SerializeField]
    private bool South;
    [SerializeField]
    private bool West;
    [Header("Door Size")]
    [SerializeField]
    private int doorSize;

    // Public getters for the room position
    public int X => x;
    public int Y => y;
    // Public getters for the room size
    public int Width => width;
    public int Height => height;
    // Public getter for the door size
    public int DoorSize => doorSize;

    // Dictionary to store the connections
    public readonly Dictionary<Dir, bool> connectDirs = new()
    {
        { Dir.North, false },
        { Dir.East, false },
        { Dir.South, false },
        { Dir.West, false }
    };
    // On editor validate, update the dictionary
    // This is needed as Dictionary is not serializable
    private void OnValidate()
    {
        connectDirs[Dir.North] = North;
        connectDirs[Dir.East] = East;
        connectDirs[Dir.South] = South;
        connectDirs[Dir.West] = West;
    }
}
