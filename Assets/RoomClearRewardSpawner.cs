using UnityEngine;

public class RoomClearRewardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private Vector2 spawnOffset;

    [Header("Optional")]
    [SerializeField] private GameObject teleporter;
    public void SpawnTreasure(bool perfectClear)
    {
        GameObject prefabToSpawn = treasurePrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Treasure prefab is not assigned.");
            return;
        }

        Instantiate(prefabToSpawn, (Vector2)transform.position + spawnOffset, Quaternion.identity);
        if ( perfectClear)
        {
            Debug.Log($"Treasure spawned! Perfect clear: {perfectClear}");
        }
        

        if (teleporter != null)
        {
            teleporter.SetActive(true);
            Debug.Log("Teleporter activated!");
        }
    }

}