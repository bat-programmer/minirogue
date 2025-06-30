using UnityEngine;

public class RoomClearRewardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private Vector2 spawnOffset;

    public void SpawnTreasure()
    {
        if (treasurePrefab == null)
        {
            Debug.LogWarning("Treasure prefab is not assigned.");
            return;
        }

        Instantiate(treasurePrefab, (Vector2)transform.position + spawnOffset, Quaternion.identity);
        Debug.Log("Treasure spawned!");
    }
}