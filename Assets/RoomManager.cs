using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject normalRoomPrefab;
    public GameObject exitRoomPrefab;

    [Header("Debug")]
    public bool showDebugGizmos = true;

    private List<Room> placedRooms = new List<Room>();

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Clear any existing rooms
        foreach (var room in placedRooms)
        {
            if (room != null && room.gameObject != null)
                Destroy(room.gameObject);
        }
        placedRooms.Clear();

        // Place start room
        Room startRoom = SpawnRoom(startRoomPrefab, Vector2.zero);
        startRoom.roomType = RoomType.Start;
        startRoom.hasEnemies = false;  // Ensure no enemies in start room

        // Place normal room
        Vector2 normalRoomPos = new Vector2(startRoom.roomSize.x, 0);
        Room normalRoom = SpawnRoom(normalRoomPrefab, normalRoomPos);
        normalRoom.roomType = RoomType.Normal;

        // Place exit room
        Vector2 exitRoomPos = new Vector2(startRoom.roomSize.x * 2, 0);
        Room exitRoom = SpawnRoom(exitRoomPrefab, exitRoomPos);
        exitRoom.roomType = RoomType.Exit;

        // Connect rooms
        ConnectRooms(startRoom, normalRoom, Vector2.right);
        ConnectRooms(normalRoom, exitRoom, Vector2.right);
    }

    Room SpawnRoom(GameObject prefab, Vector2 position)
    {
        // Instantiate room at integer coordinates to avoid floating point issues
        Vector2 snapPos = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
        GameObject roomObj = Instantiate(prefab, snapPos, Quaternion.identity, transform);
        Room room = roomObj.GetComponent<Room>();

        if (room == null)
        {
            Debug.LogError("Spawned room prefab does not have a Room component!");
            return null;
        }

        placedRooms.Add(room);

        return room;
    }

    void ConnectRooms(Room roomA, Room roomB, Vector2 direction)
    {
        // Set doors based on connection direction
        if (direction == Vector2.right)
        {
            roomA.doorRight = true;
            roomB.doorLeft = true;
        }

        // Update door visuals
        UpdateDoorVisuals(roomA);
        UpdateDoorVisuals(roomB);
    }

    void UpdateDoorVisuals(Room room)
    {
        // Update door visuals based on the current door configuration
        DoorManager doorManager = room.GetComponent<DoorManager>();
        if (doorManager != null)
        {
            doorManager.UpdateDoors(room.doorTop, room.doorRight, room.doorBottom, room.doorLeft);
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || placedRooms == null)
            return;

        // Draw room boundaries and connections
        foreach (Room room in placedRooms)
        {
            if (room == null)
                continue;

            // Set color based on room type
            switch (room.roomType)
            {
                case RoomType.Start:
                    Gizmos.color = Color.green;
                    break;
                case RoomType.Exit:
                    Gizmos.color = Color.red;
                    break;
                default:
                    Gizmos.color = Color.blue;
                    break;
            }

            // Draw room boundary
            Vector3 roomCenter = room.transform.position;
            Vector3 roomSize = new Vector3(room.roomSize.x, room.roomSize.y, 1);
            Gizmos.DrawWireCube(roomCenter, roomSize);

            // Draw doors
            Gizmos.color = Color.yellow;
            float doorWidth = 2f;

            if (room.doorTop)
                Gizmos.DrawLine(roomCenter + new Vector3(-doorWidth / 2, room.roomSize.y / 2, 0),
                                roomCenter + new Vector3(doorWidth / 2, room.roomSize.y / 2, 0));

            if (room.doorBottom)
                Gizmos.DrawLine(roomCenter + new Vector3(-doorWidth / 2, -room.roomSize.y / 2, 0),
                                roomCenter + new Vector3(doorWidth / 2, -room.roomSize.y / 2, 0));

            if (room.doorLeft)
                Gizmos.DrawLine(roomCenter + new Vector3(-room.roomSize.x / 2, -doorWidth / 2, 0),
                                roomCenter + new Vector3(-room.roomSize.x / 2, doorWidth / 2, 0));

            if (room.doorRight)
                Gizmos.DrawLine(roomCenter + new Vector3(room.roomSize.x / 2, -doorWidth / 2, 0),
                                roomCenter + new Vector3(room.roomSize.x / 2, doorWidth / 2, 0));
        }
    }
}
