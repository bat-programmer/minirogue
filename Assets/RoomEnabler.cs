using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnabler : MonoBehaviour, IPostTeleportAction
{
    [SerializeField] private Vector2 searchBoxSize = new Vector2(10f, 10f);
    [SerializeField] private LayerMask spawnerLayer;

    private bool activated = false;

    public void OnPostTeleport()
    {
        if (activated) return;

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
            EnemyBase enemy = col.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                
                enemy.enabled = true;
                
            }

        }

        activated = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, searchBoxSize);
    }
}
