using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyCombatManager : MonoBehaviour
{
    public static EnemyCombatManager Instance { get; private set; }

    public int deadEnemies { get; private set; } = 0; // Exposed for external access
    private int totalEnemies = 0;

    public event Action OnAllEnemiesDefeated;

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

    private void Start()
    {
        // This might need to be called from GameManager or another central point
        // if enemies are spawned dynamically after Start.
        // For now, assuming all initial enemies are present at Start.
        totalEnemies = FindObjectsOfType<NavEnemyBase>().Length;
        Debug.Log($"[EnemyCombatManager] Initial total enemies: {totalEnemies}");

        NavEnemyBase.OnAnyEnemyDied += HandleEnemyDeath;
    }

    public void RegisterEnemy()
    {
        totalEnemies++;
        Debug.Log($"[EnemyCombatManager] Enemy registered. Total: {totalEnemies}");
    }

    public void HandleEnemyDeath()
    {
        deadEnemies++;
        // Assuming StatsManager is still globally accessible
        StatsManager.Instance.IncrementStat("totalEnemiesKilled");
        Debug.Log($"[EnemyCombatManager] Dead enemies: {deadEnemies}/{totalEnemies}");

        if (deadEnemies >= totalEnemies)
        {
            OnAllEnemiesDefeated?.Invoke();
            Debug.Log("[EnemyCombatManager] All enemies defeated!");
        }
    }

    private void OnDestroy()
    {
        NavEnemyBase.OnAnyEnemyDied -= HandleEnemyDeath;
    }
}
