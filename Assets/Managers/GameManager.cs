using Assets.Interfaces;
using TMPro;
using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public IPotionEffect currentPotion;
    public PotionUISlot potionUISlot; // Assign in the Inspector
    public static GameManager Instance;
    public Transform playerTransform;

    public int playerHealth = 100;

    // Track discovered potion types
    private Dictionary<System.Type, bool> discoveredPotions = new Dictionary<System.Type, bool>();

    // Track first potion assignment
    public bool HasAssignedFirstPotion { get; set; } = false;

    // Reference to the current potion the player is holding
    public GameObject heldPotion;
    public event System.Action OnAllEnemiesDefeated;
    private int deadEnemies = 0;
    private int totalEnemies = 0;
    public TextMeshProUGUI coinText;
    
    // Lifetime game statistics
    private Dictionary<string, int> lifetimeStats = new Dictionary<string, int>()
    {
        {"totalEnemiesKilled", 0},
        {"totalCoinsCollected", 0},
        {"potionsUsed", 0},
        {"heartsSacrificed", 0},
        {"damageTaken", 0}
    };
    public event System.Action<Dictionary<string, int>> OnStatsUpdated;

    [Header("Coin Drop Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinDropEveryXDeaths = 3;
    [SerializeField][Range(0f, 1f)] private float coinDropChance = 0.5f;

    [Header("Player Money")]
    [SerializeField] private int playerCoins = 0;
    public int PlayerCoins => playerCoins;

    private void Start()
    {
        totalEnemies = FindObjectsOfType<NavEnemyBase>().Length;
        deadEnemies = 0;

        NavEnemyBase.OnAnyEnemyDied += HandleEnemyDeath;
    }
    public void RegisterEnemy()
    {
        totalEnemies++;
        Debug.Log($"[GameManager] Enemy registered. Total: {totalEnemies}");
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public bool IsPotionDiscovered(System.Type potionType)
    {
        return discoveredPotions.ContainsKey(potionType);
    }

    public void DiscoverPotion(IPotionEffect effect)
    {
        discoveredPotions[effect.GetType()] = true;
    }

    public void EquipPotion(IPotionEffect effect, string label, Sprite sprite, Color color)
    {
        Vector3 textPosition = playerTransform != null ? playerTransform.position : transform.position;

        FloatingText.Create(textPosition, "Potion Grabbed!", Color.green);
        Debug.Log("Potion equipped: " + effect.GetType().Name);
        currentPotion = effect;
        Debug.Log("Potion color: " + color.ToString());
        potionUISlot.SetPotion(sprite, color, label);
        
        // Mark as discovered when equipped
        DiscoverPotion(effect);
    }

    public void UsePotion(Jugador player)
    {
        if (currentPotion != null)
        {
            string potionName = currentPotion.GetUILabel();
            currentPotion.ApplyEffect(player);
            currentPotion = null;
            potionUISlot.ClearPotion();
            FloatingText.Create(player.transform.position, $"{potionName} used", Color.yellow);
            IncrementStat("potionsUsed");
        }
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
        IncrementStat("totalCoinsCollected", amount);
        Debug.Log($"[GameManager] Coins: {playerCoins}");
        UpdateCoinUI();
    }

    public bool RemoveCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            Debug.Log($"[GameManager] Coins: {playerCoins}");
            UpdateCoinUI();
            return true;
        }

        Debug.Log("[GameManager] Not enough coins!");
        return false;
    }

    public void IncrementStat(string statName, int amount = 1)
    {
        if (lifetimeStats.ContainsKey(statName))
        {
            lifetimeStats[statName] += amount;
            OnStatsUpdated?.Invoke(lifetimeStats);
        }
    }

    public Dictionary<string, int> GetStats()
    {
        return new Dictionary<string, int>(lifetimeStats);
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"x {playerCoins}";
        }
    }

    private void OnDestroy()
    {
        NavEnemyBase.OnAnyEnemyDied -= HandleEnemyDeath;
    }
    private void HandleEnemyDeath()
    {
        deadEnemies++;
        IncrementStat("totalEnemiesKilled");

        if (deadEnemies >= totalEnemies)
        {
            OnAllEnemiesDefeated?.Invoke();
        }
        // Try to spawn coin every X deaths
        if (deadEnemies % coinDropEveryXDeaths == 0)
        {
            float roll = Random.value; // Random between 0 and 1
            if (roll < coinDropChance)
            {
                SpawnCoin();
            }
        }
    }
    private void SpawnCoin()
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning("[GameManager] Coin prefab not assigned.");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("[GameManager] Player transform not assigned.");
            return;
        }

        // Spawn near the player with slight random offset
        Vector2 spawnPos = (Vector2)playerTransform.position + Random.insideUnitCircle * 1.5f;
        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }

}
