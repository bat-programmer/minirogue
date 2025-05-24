#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum RoomType
{
    Start,
    Normal,
    Exit
}

public class Room : MonoBehaviour
{
    public RoomType roomType;
    public Vector2Int roomSize = new Vector2Int(16, 16); // Room size in world units

    [Header("Doors")]
    public bool doorTop;
    public bool doorBottom;
    public bool doorLeft;
    public bool doorRight;

    [Header("Room Settings")]
    public bool hasEnemies = true;
    public bool hasItems = true;

    [Header("Prefabs")]
    public GameObject[] enemyPrefabs;
    public GameObject[] itemPrefabs;

    private DoorManager doorManager;

    private void Awake()
    {
        doorManager = GetComponent<DoorManager>();
    }

    private void Start()
    {
        if (doorManager != null)
        {
            doorManager.UpdateDoors(doorTop, doorRight, doorBottom, doorLeft);
        }

        if (roomType == RoomType.Start)
        {
            // No enemies in the start room
            hasEnemies = false;
        }

        if (hasEnemies && enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            SpawnEnemies();
        }

        if (hasItems && itemPrefabs != null && itemPrefabs.Length > 0)
        {
            SpawnItems();
        }
    }

    private void SpawnEnemies()
    {
        // Spawn 1-3 random enemies
        int enemyCount = Random.Range(1, 4);

        for (int i = 0; i < enemyCount; i++)
        {
            // Get random enemy prefab
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Get random position inside room (keeping some margin from walls)
            float xMargin = roomSize.x * 0.2f;
            float yMargin = roomSize.y * 0.2f;

            float xPos = Random.Range(-roomSize.x / 2 + xMargin, roomSize.x / 2 - xMargin);
            float yPos = Random.Range(-roomSize.y / 2 + yMargin, roomSize.y / 2 - yMargin);

            Vector3 spawnPos = transform.position + new Vector3(xPos, yPos, 0);

            // Spawn enemy
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
        }
    }

    private void SpawnItems()
    {
        // 50% chance to spawn an item
        if (Random.value > 0.5f)
            return;

        // Get random item prefab
        GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        // Get random position inside room (keeping some margin from walls)
        float xMargin = roomSize.x * 0.2f;
        float yMargin = roomSize.y * 0.2f;

        float xPos = Random.Range(-roomSize.x / 2 + xMargin, roomSize.x / 2 - xMargin);
        float yPos = Random.Range(-roomSize.y / 2 + yMargin, roomSize.y / 2 - yMargin);

        Vector3 spawnPos = transform.position + new Vector3(xPos, yPos, 0);

        // Spawn item
        Instantiate(itemPrefab, spawnPos, Quaternion.identity, transform);
    }

    void OnDrawGizmos()
    {
        // Draw room boundary in Scene view
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, roomSize.y, 1));

        // Show room type
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position, roomType.ToString());
#endif

    }
}