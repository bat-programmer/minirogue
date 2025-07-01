using UnityEngine;

public class RoomClearRewardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private Vector2 spawnOffset;

    [Header("Optional")]
    [SerializeField] private GameObject teleporter;
    public void SpawnTreasure()
    {
        if (treasurePrefab == null)
        {
            Debug.LogWarning("Treasure prefab is not assigned.");
            return;
        }

        Instantiate(treasurePrefab, (Vector2)transform.position + spawnOffset, Quaternion.identity);
        Debug.Log("Treasure spawned!");

        if (teleporter != null)
        {
            teleporter.SetActive(true);
            Debug.Log("Teleporter activated!");
        }
    }

}