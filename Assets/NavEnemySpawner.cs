using UnityEngine;

public class NavEnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public int maxEnemiesToSpawn = 3;

    private int spawnedCount = 0;
    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && spawnedCount < maxEnemiesToSpawn)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }

        if (spawnedCount >= maxEnemiesToSpawn)
        {
            Destroy(gameObject); // Self destruct
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        spawnedCount++;

        GameManager.Instance?.RegisterEnemy();
    }
}
