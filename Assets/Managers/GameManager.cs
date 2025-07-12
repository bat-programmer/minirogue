using Assets.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform playerTransform;

    public int playerHealth = 100;

    private DefeatScreenManager defeatScreenManager;
    private WinScreenManager winScreenManager;
    public PotionManager potionManager; // Reference to the PotionManager
    public EconomyManager economyManager; // Reference to the EconomyManager
    public EnemyCombatManager enemyCombatManager; // Reference to the EnemyCombatManager

    public event System.Action OnAllEnemiesDefeated; // This event will now be invoked by EnemyCombatManager
    // Stats now handled by StatsManager
    public event System.Action<Dictionary<string, int>> OnStatsUpdated
    {
        add => StatsManager.Instance.OnStatsUpdated += value;
        remove => StatsManager.Instance.OnStatsUpdated -= value;
    }

    private void Start()
    {
        // EnemyCombatManager now handles enemy initialization and death events
    }
    public void RegisterEnemy()
    {
        enemyCombatManager.RegisterEnemy();
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

        defeatScreenManager = gameObject.AddComponent<DefeatScreenManager>();
        winScreenManager = gameObject.AddComponent<WinScreenManager>();
       //gameObject.AddComponent<StatsManager>(); // Initialize StatsManager
    }

    public void EquipPotion(IPotionEffect effect, string label, Sprite sprite, Color color)
    {
        // Delegate to PotionManager
        potionManager.EquipPotion(effect, label, sprite, color, playerTransform);
    }

    public void UsePotion(Jugador player)
    {
        // Delegate to PotionManager
        potionManager.UsePotion(player);
    }

    public void AddCoins(int amount)
    {
        economyManager.AddCoins(amount);
    }

    public bool RemoveCoins(int amount)
    {
        return economyManager.RemoveCoins(amount);
    }

    public void IncrementStat(string statName, int amount = 1)
    {
        StatsManager.Instance.IncrementStat(statName, amount);
    }

    public Dictionary<string, int> GetStats()
    {
        return StatsManager.Instance.GetStats();
    }

    private void OnDestroy()
    {
        // EnemyCombatManager now handles enemy death event subscription
    }
    public void HandleEnemyDeath()
    {
        enemyCombatManager.HandleEnemyDeath();
        // Delegate coin spawning to EconomyManager
        economyManager.TrySpawnCoin(enemyCombatManager.deadEnemies, playerTransform); // Need to expose deadEnemies from EnemyCombatManager
    }

    public void ShowDefeatScreen()
    {
        defeatScreenManager.ShowDefeatScreen(GetStats());
    }

    public void ShowWinScreen()
    {
        winScreenManager.ShowWinScreen(GetStats());
    }

}
