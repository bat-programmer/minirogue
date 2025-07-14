using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    // Lifetime game statistics
    private Dictionary<string, int> lifetimeStats = new Dictionary<string, int>()
    {
        {"totalEnemiesKilled", 0},
        {"totalCoinsCollected", 0},
        {"potionsUsed", 0},
        {"heartsSacrificed", 0},
        {"damageTaken", 0},
        {"perfectRoomClears", 0}
    };

    public event System.Action<Dictionary<string, int>> OnStatsUpdated;

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
}
