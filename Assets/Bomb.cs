using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Vector2 searchBoxSize = new Vector2(10f, 10f);
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Call this from the end of the explosion animation via Animation Event
    public void OnExplosionAnimationEnd()
    {
        // Find the nearest RoomEnabler
        RoomEnabler room = FindClosestRoomEnabler();
        if (room == null) return;

        // Use the room's area and layer mask to find enemies
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            room.transform.position,
            room.GetSearchBoxSize(),
            0f,
            room.GetSpawnerLayer()
        );

        foreach (var col in colliders)
        {
            NavEnemyBase enemy = col.GetComponent<NavEnemyBase>();
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                // Replace with your actual kill logic
                Destroy(enemy.gameObject);
            }
        }
    }

    private RoomEnabler FindClosestRoomEnabler()
    {
        RoomEnabler[] rooms = FindObjectsOfType<RoomEnabler>();
        RoomEnabler closest = null;
        float minDist = float.MaxValue;
        foreach (var room in rooms)
        {
            float dist = Vector2.Distance(transform.position, room.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = room;
            }
        }
        return closest;
    }
}
