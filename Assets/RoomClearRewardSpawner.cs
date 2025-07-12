using UnityEngine;

public class RoomClearRewardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private Vector2 spawnOffset;
    [SerializeField] private GameObject slotMachinePrefab;
    [SerializeField] private int coinThreshold = 3;
    [SerializeField] private GameObject healthTraderPrefab;
    [SerializeField] private float healthTraderSpawnChance = 0.5f;

    [Header("Optional")]
    [SerializeField] private GameObject teleporter;
    public void SpawnTreasure(bool perfectClear)
    {
        // Spawn chest if perfect clear
        if (perfectClear && treasurePrefab != null)
        {
            Instantiate(treasurePrefab, (Vector2)transform.position + spawnOffset, Quaternion.identity);
            Debug.LogWarning($"Treasure spawned! Perfect clear: {perfectClear}");
        }

        // Spawn slot machine if player has enough coins
        if (GameManager.Instance != null && GameManager.Instance.economyManager.PlayerCoins >= coinThreshold && slotMachinePrefab != null)
        {
            Instantiate(slotMachinePrefab, 
                (Vector2)transform.position + spawnOffset + Vector2.right * 2f, 
                Quaternion.identity);
            Debug.LogWarning("Slot machine spawned!");
        }

        // Activate teleporter if exists
        if (teleporter != null)
        {
            teleporter.SetActive(true);
            Debug.Log("Teleporter activated!");
        }

        // Spawn health trader with chance
        if (healthTraderPrefab != null && Random.value < healthTraderSpawnChance)
        {
            Instantiate(healthTraderPrefab, 
                (Vector2)transform.position + spawnOffset + Vector2.left * 2f, 
                Quaternion.identity);
            Debug.Log("Health trader spawned!");
        }
    }

}
