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
    [SerializeField]
    private string roomName;
    [SerializeField] private Sprite roomSprite;
    [Header("Room Size")]
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private float wallThickness = 2f;
    [Header("Room Connections")]
    [SerializeField]
    private bool north;
    [SerializeField]
    private bool east;
    [SerializeField]
    private bool south;
    [SerializeField]
    private bool west;
    [Header("Door Size")]
    [SerializeField]
    private int doorSize;

    // Dictionary to store the connections
    private readonly Dictionary<Dir, bool> connectDirs = new()
    {
        { Dir.North, false },
        { Dir.East, false },
        { Dir.South, false },
        { Dir.West, false }
    };

    // Public getter properties for the room data

    public string RoomName => roomName;
    public Sprite RoomSprite => roomSprite;
    public int Width => width;
    public int Height => height;
    public int DoorSize => doorSize;
    // Public getters for the room connections
    public Dictionary<Dir, bool> ConnectDirs => connectDirs;
    public bool North => north;
    public bool East => east;
    public bool South => south;
    public bool West => west;
    public float WallThickness => wallThickness;

    // On editor validate, update the dictionary
    // This is needed as Dictionary is not serializable
    private void OnValidate()
    {
        connectDirs[Dir.North] = north;
        connectDirs[Dir.East] = east;
        connectDirs[Dir.South] = south;
        connectDirs[Dir.West] = west;
    }
}
