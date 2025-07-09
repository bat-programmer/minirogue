using UnityEngine;

public class RoomClearRewardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private Vector2 spawnOffset;
    [SerializeField] private GameObject slotMachinePrefab;
    [SerializeField] private int coinThreshold = 3;

    [Header("Optional")]
    [SerializeField] private GameObject teleporter;
    public void SpawnTreasure(bool perfectClear)
    {
        Debug.LogWarning($"Spawning rewards for room at {transform.position}. Perfect clear: {perfectClear}");
        // Spawn chest if perfect clear
        if (perfectClear && treasurePrefab != null)
        {
            Instantiate(treasurePrefab, (Vector2)transform.position + spawnOffset, Quaternion.identity);
            Debug.LogWarning($"Treasure spawned! Perfect clear: {perfectClear}");
        }
        Debug.LogWarning($"Player coins: {GameManager.Instance.PlayerCoins}, Coin threshold: {coinThreshold}");
        Debug.LogWarning($"Slot machine prefab: {slotMachinePrefab != null}");
        // Spawn slot machine if player has enough coins
        if (GameManager.Instance != null && GameManager.Instance.PlayerCoins >= coinThreshold && slotMachinePrefab != null)
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
    }

}
