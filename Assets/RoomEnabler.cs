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


    [Header("Room Cleared Event")]
    public UnityEvent onRoomCleared;

    [Header("Room Activation Options")]
    [SerializeField] private bool activateOnStart = false;

    private bool activated = false;
    private bool allEnemiesDead = false;
    public Vector2 GetSearchBoxSize() => searchBoxSize;
    public LayerMask GetSpawnerLayer() => spawnerLayer;
    void Start()
    {
        if (activateOnStart)
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
            // Debug.Log($"Enabled spawner/enemy on {col.name}");
        }

        activated = true;
        StartCoroutine(CheckIfAllEnemiesDeadCoroutine());
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
                Debug.Log($"All enemies in room '{name}' are dead!");
                onRoomCleared?.Invoke();

                // You can do other things here, like:
                // - Open a door
                // - Spawn loot
                // - Signal LevelManager
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, searchBoxSize);
    }


}
