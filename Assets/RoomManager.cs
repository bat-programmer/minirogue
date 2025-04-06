using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject[] normalRoomPrefabs;
    public GameObject exitRoomPrefab;

    public int numberOfRooms = 5;

    private Vector2 currentPos = Vector2.zero;
    private List<Vector2> usedPositions = new List<Vector2>();

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        SpawnRoom(startRoomPrefab, currentPos);

        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector2 newPos = GetNextRoomPosition(currentPos);
            if (!usedPositions.Contains(newPos))
            {
                GameObject roomPrefab = normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)];
                SpawnRoom(roomPrefab, newPos);
                currentPos = newPos;
            }
        }

        Vector2 exitPos = GetNextRoomPosition(currentPos);
        SpawnRoom(exitRoomPrefab, exitPos);
    }

    void SpawnRoom(GameObject prefab, Vector2 position)
    {
        GameObject room = Instantiate(prefab, position, Quaternion.identity);
        usedPositions.Add(position);
    }

    Vector2 GetNextRoomPosition(Vector2 current)
    {
        // For now, just move right – later we can randomize direction
        return current + new Vector2(20, 0); // 20 should be the width of your room in world units
    }
}
