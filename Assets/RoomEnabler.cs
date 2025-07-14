using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomEnabler : MonoBehaviour, IPostTeleportAction
{
    [SerializeField] private Vector2 searchBoxSize = new Vector2(10f, 10f);
    [SerializeField] private LayerMask spawnerLayer;
    [SerializeField] private float enemyCheckInterval = 1f;
    private RoomClearRewardSpawner rewardSpawner;

    [Header("Room Cleared Event")]    
    public UnityEvent<bool> onRoomClearedWithDamageStatus; // New event with damage status

    [Header("Room Activation")]
    [SerializeField] private BoxCollider2D triggerCollider;

    private bool activated = false;
    private bool allEnemiesDead = false;
    private bool playerTookDamageInRoom = false;
    private Jugador player;

    public Vector2 GetSearchBoxSize() => searchBoxSize;
    public LayerMask GetSpawnerLayer() => spawnerLayer;

    private void Awake()
    {
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<BoxCollider2D>();
            triggerCollider.size = searchBoxSize;
            triggerCollider.isTrigger = true;
        }
           rewardSpawner = GetComponentInChildren<RoomClearRewardSpawner>();

        if (rewardSpawner != null)
        {
            onRoomClearedWithDamageStatus.AddListener(rewardSpawner.SpawnTreasure);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            ActivateRoom();
        }
    }

    public void OnPostTeleport()
    {
        ActivateRoom();
    }

    public void ActivateRoom()
    {
        if (activated) return;

        // Find the player and subscribe to damage events
        if (player == null)
        {
            player = FindObjectOfType<Jugador>();
        }

        if (player != null)
        {
            // Subscribe to damage events when room activates
            player.OnDamageTaken += OnPlayerDamageTaken;
        }

        // Reset damage tracking for this room
        playerTookDamageInRoom = false;

        Debug.Log($"Searching for layer mask: {spawnerLayer.value}");
        // Find all colliders in the defined box area
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, searchBoxSize, 0f, spawnerLayer);
        Debug.Log($"Found {colliders.Length} colliders in the area.");

        foreach (var col in colliders)
        {
            NavEnemySpawner spawner = col.GetComponent<NavEnemySpawner>();
            if (spawner != null)
            {
                spawner.enabled = true;
            }
            NavEnemyBase navEnemy = col.GetComponent<NavEnemyBase>();
            if (navEnemy != null)
            {
                navEnemy.enabled = true;
                Debug.Log($"Enabled NavEnemyBase on {col.name}");
            }
        }

        activated = true;
        StartCoroutine(CheckIfAllEnemiesDeadCoroutine());
    }

    private void OnPlayerDamageTaken()
    {
        playerTookDamageInRoom = true;
        Debug.Log("Player took damage in room - perfect clear no longer possible");
    }

    private IEnumerator CheckIfAllEnemiesDeadCoroutine()
    {
        while (!allEnemiesDead)
        {
            yield return new WaitForSeconds(enemyCheckInterval);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, searchBoxSize, 0f, spawnerLayer);
            bool anyAlive = false;

            foreach (var col in colliders)
            {
                NavEnemyBase navEnemy = col.GetComponent<NavEnemyBase>();
                if (navEnemy != null && navEnemy.gameObject.activeInHierarchy)
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive)
            {
                allEnemiesDead = true;
                bool perfectClear = !playerTookDamageInRoom;

                Debug.Log($"All enemies in room '{name}' are dead! Perfect clear: {perfectClear}");

                // Track perfect room clears
                if (perfectClear)
                {
                    StatsManager.Instance?.IncrementStat("perfectRoomClears");
                }

                // Invoke both events
                //onRoomCleared?.Invoke();
                onRoomClearedWithDamageStatus?.Invoke(perfectClear);

                // Unsubscribe from damage events
                if (player != null)
                {
                    player.OnDamageTaken -= OnPlayerDamageTaken;
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up event subscription
        if (player != null)
        {
            player.OnDamageTaken -= OnPlayerDamageTaken;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, searchBoxSize);
    }
}
