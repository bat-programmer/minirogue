using UnityEngine;
using TMPro; // For TextMeshProUGUI

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Coin Drop Settings")]
    public GameObject coinPrefab;
    [SerializeField] private int coinDropEveryXDeaths = 3;
    [SerializeField][Range(0f, 1f)] private float coinDropChance = 0.5f;

    [Header("Player Money")]
    [SerializeField] private int playerCoins = 0;
    public int PlayerCoins => playerCoins;

    public TextMeshProUGUI coinText; // Reference to the UI TextMeshPro element

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
        // Assuming StatsManager is still globally accessible
        StatsManager.Instance.IncrementStat("totalCoinsCollected", amount);
        Debug.Log($"[EconomyManager] Coins: {playerCoins}");
        UpdateCoinUI();
    }

    public bool RemoveCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            Debug.Log($"[EconomyManager] Coins: {playerCoins}");
            UpdateCoinUI();
            return true;
        }

        Debug.Log("[EconomyManager] Not enough coins!");
        return false;
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"x {playerCoins}";
        }
    }

    public void TrySpawnCoin(int deadEnemies, Transform playerTransform)
    {
        // Try to spawn coin every X deaths
        if (deadEnemies % coinDropEveryXDeaths == 0)
        {
            float roll = Random.value; // Random between 0 and 1
            if (roll < coinDropChance)
            {
                SpawnCoin(playerTransform);
            }
        }
    }

    private void SpawnCoin(Transform playerTransform)
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning("[EconomyManager] Coin prefab not assigned.");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("[EconomyManager] Player transform not assigned.");
            return;
        }

        // Spawn near the player with slight random offset
        Vector2 spawnPos = (Vector2)playerTransform.position + Random.insideUnitCircle * 1.5f;
        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }
}
